using System;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using ShakeMyList.Mobile;
using ShakeMyList.Mobile.Presenters;
using ShakeMyList.Mobile.Views;
using LobaSoft.IOS.UIComponents.Events;
using LobaSoft.IOS.UIComponents.Frames;
using LobaSoft.IOS.UIComponents;

namespace ShakeMyList.Iphone
{
    public class ShakeListViewerView : UIViewController, IShakeListViewerView, IDisposableView
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
                if (__items == value)
                    return;

                if (__listSource != null)
                    this.DetachListSourceEventHandlers();

                __items = value;
                __listSource = new NoEditShakeListItemsSource(__items);
                this.AttachListSourceEventHandlers();
                __listItems.Source = __listSource;
            }
        }

        public ShakeListViewerView(ShakeList list)
        {
            this.EdgesForExtendedLayout = UIRectEdge.None;
            this.View.BackgroundColor = UIColor.White;
            __presenter = new ShakeListViewerPresenter(this, list);
        }

        public void AttachEventHandlers()
        {
            __shake.TouchDown += this.Shake_TouchDown;
            __options.Clicked += this.Options_Click;

            if (__listSource != null)
                this.AttachListSourceEventHandlers();
        }

        public void DetachEventHandlers()
        {
            __shake.TouchDown -= this.Shake_TouchDown;
            __options.Clicked -= this.Options_Click;

            if (__listSource != null)
                this.DetachListSourceEventHandlers();
        }

        public void CleanSubViews()
        {
        }

        public void AddSubViews()
        {
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
            this.AttachEventHandlers();

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
            this.DetachEventHandlers();
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

                ShakeItem itemToMove = itemsStartState[log.OriginalIndex];
                int itemIndexInView = __items.IndexOf(itemToMove);

                NSIndexPath source = NSIndexPath.FromRowSection(itemIndexInView, 0);
                NSIndexPath destiny = NSIndexPath.FromRowSection(log.ChangedIndex, 0);

                __listItems.MoveRow(source, destiny);
                __items.Move(source.Row, destiny.Row);
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

            __options = new UIBarButtonItem();
            __options.Style = UIBarButtonItemStyle.Plain;
            __options.Title = "Options";

            __listItems = new UITableView();
        }

        private void AddUIControls()
        {
            __innerFrame.AddChild(__listItems, 0, 0);
            __innerFrame.AddChild(__shake, 1, 0);

            this.NavigationItem.RightBarButtonItem = __options;

            this.Add(__innerFrame);
        }

        private void AttachListSourceEventHandlers()
        {
            __listSource.RowMoved += this.ItemsSource_RowMoved;
            __listSource.RowLocked += this.ItemsSource_RowLocked;
            __listSource.RowUnlocked += this.ItemsSource_RowUnlocked;
            __listSource.RowMarked += this.ItemsSource_RowMarked;
            __listSource.RowUnmarked += this.ItemsSource_RowUnmarked;
        }

        private void DetachListSourceEventHandlers()
        {
            __listSource.RowMoved -= this.ItemsSource_RowMoved;
            __listSource.RowLocked -= this.ItemsSource_RowLocked;
            __listSource.RowUnlocked -= this.ItemsSource_RowUnlocked;
            __listSource.RowMarked -= this.ItemsSource_RowMarked;
            __listSource.RowUnmarked -= this.ItemsSource_RowUnmarked;
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
            NameInputView nameInput = new NameInputView();
            nameInput.Ok += this.NameInput_OK;
            nameInput.Cancel += this.NameInput_Cancel;

            nameInput.Name = this.Title;
            this.PresentViewController(nameInput, true, null);
        }

        private void CloneListAndGoToEditView()
        {
            ShakeList listClon = __presenter.Clone();

            ShakeListEditorView listEditor = new ShakeListEditorView(listClon, true);
            UIViewController[] controllers = this.NavigationController.ViewControllers;
            controllers[controllers.Length - 1] = listEditor;
            this.NavigationController.SetViewControllers(controllers, true);
        }

        private void Options_Click(object sender, EventArgs e)
        {
            UIActionSheet optionsSheet = new UIActionSheet(string.Empty);
            optionsSheet.AddButton("Save");
            optionsSheet.AddButton("Rename");
            optionsSheet.AddButton("Edit");
            optionsSheet.AddButton("Clone");
            optionsSheet.AddButton("Cancel");
            optionsSheet.CancelButtonIndex = 4;
            optionsSheet.Clicked += this.Option_Selected;

            optionsSheet.ShowInView(this.View);
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
            NameInputView nameInput = sender as NameInputView;
            string newName = nameInput.Name;
            __presenter.RenameShakeListName(newName);
            nameInput.Ok -= NameInput_OK;
            nameInput.Cancel -= NameInput_Cancel;
            this.DismissViewController(true, null);
        }

        private void NameInput_Cancel(object sender, EventArgs e)
        {
            NameInputView nameInput = sender as NameInputView;
            nameInput.Ok -= NameInput_OK;
            nameInput.Cancel -= NameInput_Cancel;
            this.DismissViewController(true, null);
        }
    }
}

