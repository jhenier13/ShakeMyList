using System;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using ShakeMyList.Mobile;
using ShakeMyList.Mobile.Presenters;
using ShakeMyList.Mobile.Views;
using UIComponents.Events;
using UIComponents.Frames;

namespace ShakeMyList.Iphone
{
    public class ShakeListViewerView : UIViewController, IShakeListViewerView
    {
        //Attributes
        private ShakeListViewerPresenter __presenter;
        private List<ShakeItem> __items;
        private bool __isInternallyMoving;
        //UIControls
        private GridView __innerFrame;
        private UITableView __listItems;
        private UIButton __shake;
        private UIBarButtonItem __options;
        private UIActionSheet __optionsSheet;
        private NameInputView __nameInput;
        //UIControls Extras
        private NoEditShakeListItemsSource __listSource;

        public string ListName
        {
            get
            {
                return this.Title;
            }
            set
            {
                this.Title = value;
            }
        }

        public List<ShakeItem> Items
        {
            get
            {
                return __items;
            }
            set
            {
                __items = value;
                __listSource = new NoEditShakeListItemsSource(__items);
                __listSource.RowMoved += this.ItemsSource_RowMoved;
                __listSource.RowLocked += this.ItemsSource_RowLocked;
                __listSource.RowUnlocked += this.ItemsSource_RowUnlocked;
                __listSource.RowMarked += this.ItemsSource_RowMarked;
                __listSource.RowUnmarked += this.ItemsSource_RowUnmarked;
                __listItems.Source = __listSource;
            }
        }

        public ShakeListViewerView(ShakeList list)
        {
            this.EdgesForExtendedLayout = UIRectEdge.None;
            this.View.BackgroundColor = UIColor.White;
            __presenter = new ShakeListViewerPresenter(this, list);
        }

        public override void LoadView()
        {
            base.LoadView();

            this.CreateGrid();
            this.CreateUIControls();
            this.AddUIControls();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            __innerFrame.Frame = new RectangleF(0, 0, this.View.Frame.Width, this.View.Frame.Height);
            __innerFrame.UpdateChildrenLayout();

            __presenter.LoadData();
            __listItems.SetEditing(true, true);
        }

        #region Required overrides for Shake Event

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            this.BecomeFirstResponder();
        }

        public override void ViewDidDisappear(bool animated)
        {
            this.ResignFirstResponder();
            base.ViewDidDisappear(animated);
        }

        public override bool CanBecomeFirstResponder
        {
            get
            {
                return true;
            }
        }

        #endregion

        public override void MotionEnded(UIEventSubtype motion, UIEvent evt)
        {
            if (motion == UIEventSubtype.MotionShake)
                this.ShakeDevice();

            base.MotionEnded(motion, evt);
        }

        public void RefreshItemsDraw(IList<MoveLog> changes)
        {
            List<ShakeItem> itemsStartState = new List<ShakeItem>(__items);

            for (int i = 0; i < changes.Count; i++)
            {
                MoveLog log = changes[i];

                Console.WriteLine("MoveLog From: {0}  To: {1}", log.OriginalIndex, log.ChangedIndex);
                ShakeItem itemToMove = itemsStartState[log.OriginalIndex];
                int itemIndexInView = __items.IndexOf(itemToMove);
                Console.WriteLine("ItemIndexInView: {0}", itemIndexInView);

                NSIndexPath source = NSIndexPath.FromRowSection(itemIndexInView, 0);
                NSIndexPath destiny = NSIndexPath.FromRowSection(log.ChangedIndex, 0);

                __listItems.MoveRow(source, destiny);
                __items.Move(source.Row, destiny.Row);
                Console.WriteLine("MoveRow  From: {0}  To: {1}", source.Row, destiny.Row);
            }
        }

        public void ShowMessage(string message)
        {
        }

        private void CreateGrid()
        {
            __innerFrame = new GridView();
            __innerFrame.AddRowsAndColumns("1.0*;40", "");
            __innerFrame.Frame = new RectangleF(0, 0, this.View.Frame.Width, this.View.Frame.Height);
        }

        private void CreateUIControls()
        {
            __shake = new UIButton(UIButtonType.System);
            __shake.BackgroundColor = UIColor.FromWhiteAlpha(0.8F, 0.5F);
            __shake.SetTitle("Shake", UIControlState.Normal);
            __shake.Font = UIFont.BoldSystemFontOfSize(25F);
            __shake.TouchDown += this.Shake_TouchDown;

            __options = new UIBarButtonItem();
            __options.Style = UIBarButtonItemStyle.Plain;
            __options.Title = "Options";
            __options.Clicked += this.Options_Click;

            __listItems = new UITableView();
        }

        private void AddUIControls()
        {
            __innerFrame.AddChild(__listItems, 0, 0);
            __innerFrame.AddChild(__shake, 1, 0);

            this.NavigationItem.RightBarButtonItem = __options;

            this.Add(__innerFrame);
        }

        private void ShakeDevice()
        {
            try
            {
                __isInternallyMoving = true;
                __presenter.ShakeTheList();
            }
            finally
            {
                __isInternallyMoving = false;
            }
        }

        private void GoToEditView()
        {
            ShakeList currentList = __presenter.GetModelList();
            ShakeListEditorView editorView = new ShakeListEditorView(currentList);

            this.NavigationController.PushViewController(editorView, true);
        }

        private void ShowRenameWindow()
        {
            if (__nameInput == null)
            {
                __nameInput = new NameInputView();
                __nameInput.Ok += this.NameInput_OK;
                __nameInput.Cancel += this.NameInput_Cancel;
            }

            __nameInput.Name = this.Title;
            this.PresentViewController(__nameInput, true, null);
        }

        private void CloneListAndGoToEditView()
        {
            ShakeList listClon = __presenter.Clone();
            ShakeListEditorView listEditor = new ShakeListEditorView(listClon, true);
            UIViewController[] controllers = this.NavigationController.ViewControllers;
            controllers[controllers.Length - 1] = listEditor;

            this.NavigationController.SetViewControllers(controllers, true);
        }

        #region EventHandlers

        private void Options_Click(object sender, EventArgs e)
        {
            __optionsSheet = new UIActionSheet(string.Empty);
            __optionsSheet.AddButton("Save");
            __optionsSheet.AddButton("Rename");
            __optionsSheet.AddButton("Edit");
            __optionsSheet.AddButton("Clone");
            __optionsSheet.AddButton("Cancel");
            __optionsSheet.CancelButtonIndex = 4;
            __optionsSheet.Clicked += this.Option_Selected;

            __optionsSheet.ShowInView(this.View);
        }

        private void Shake_TouchDown(object sender, EventArgs e)
        {
            this.ShakeDevice();
        }

        private void ItemsSource_RowMoved(object sender, MoveRowEventArgs e)
        {
            if (__isInternallyMoving)
                return;

            __presenter.MoveItem(e.SourceIndex, e.DestinationIndex);
        }

        private void ItemsSource_RowLocked(object sender, RowLockEventArgs e)
        {
            __presenter.LockItem(e.Index);
        }

        private void ItemsSource_RowUnlocked(object sender, RowLockEventArgs e)
        {
            __presenter.UnlockItem(e.Index);
        }

        private void ItemsSource_RowMarked(object sender, RowMarkedEventArgs e)
        {
            __presenter.MarkItem(e.Index);
        }

        private void ItemsSource_RowUnmarked(object sender, RowMarkedEventArgs e)
        {
            __presenter.UnMarkItem(e.Index);
        }

        private void Option_Selected(object sender, UIButtonEventArgs e)
        {
            switch (e.ButtonIndex)
            {
                case 0:
                    __presenter.SaveList();
                    break;
                case 1:
                    this.ShowRenameWindow();
                    break;
                case 2:
                    this.GoToEditView();
                    break;
                case 3:
                    this.CloneListAndGoToEditView();
                    break;
            }
        }

        private void NameInput_OK(object sender, EventArgs e)
        {
            string newName = __nameInput.Name;
            __presenter.RenameShakeListName(newName);
            this.DismissViewController(true, null);
        }

        private void NameInput_Cancel(object sender, EventArgs e)
        {
            this.DismissViewController(true, null);
        }

        #endregion
    }
}

