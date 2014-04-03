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
    public class ShakeListEditorView :UIViewController, IShakeListEditorView
    {
        //Attributes
        private AppViewMode __mode;
        private List<ShakeItem> __items;
        private ShakeListEditorPresenter __presenter;
        //UIControls
        private GridView __innerFrame;
        private UITextField __newItem;
        private UIButton __addNewItem;
        private UIBarButtonItem __done;
        private UITableView __list;
        private NameInputView __nameInput;
        //UIControls extras
        private ShakeListItemsSource __listSource;

        public string Name
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
                __listSource = new ShakeListItemsSource(__items);
                __listSource.RowMoved = this.ListSource_RowMoved;
                __listSource.RowDeleted = this.ListSource_RowDeleted;
                __listSource.RowsScrolled += (object sender, EventArgs e) => this.DismissKeyboard();
                __list.Source = __listSource;
            }
        }

        public ShakeListEditorView()
        {
            __presenter = new ShakeListEditorPresenter(this);
            __mode = AppViewMode.New;

            //This is required so the parent UINavigationController doesn't resize this UIViewController like he wants
            this.EdgesForExtendedLayout = UIRectEdge.None;
            this.View.BackgroundColor = UIColor.White;
        }

        public ShakeListEditorView(ShakeList editList, bool isAClon = false)
        {
            __presenter = new ShakeListEditorPresenter(this, editList);
            __mode = (isAClon) ? AppViewMode.New : AppViewMode.Edit;

            this.EdgesForExtendedLayout = UIRectEdge.None;
            this.View.BackgroundColor = UIColor.White;
        }

        public void RefreshItemsDraw()
        {
        }

        public void ShowMessage(string message)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }

        public override void LoadView()
        {
            base.LoadView();

            this.CreateGrid();
            this.CreateUIControls();
            this.AddUIControls();

            this.CreateTapAndGesturesHandler();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            __innerFrame.Frame = new RectangleF(0, 0, this.View.Frame.Width, this.View.Frame.Height);
            __innerFrame.UpdateChildrenLayout();

            __list.SetEditing(true, true);
            __presenter.LoadData();
        }

        private void CreateTapAndGesturesHandler()
        {
            UITapGestureRecognizer tap = new UITapGestureRecognizer();
            tap.CancelsTouchesInView = false;
            tap.AddTarget(new Action<NSObject>(((NSObject obj) => this.DismissKeyboard())));

            __list.AddGestureRecognizer(tap);
        }

        private void DismissKeyboard()
        {
            __newItem.ResignFirstResponder();
        }

        private void CreateGrid()
        {
            __innerFrame = new GridView();
            __innerFrame.AddRowsAndColumns("40;1.0*", "");
            __innerFrame.Frame = new RectangleF(0, 0, this.View.Frame.Width, this.View.Frame.Height);

            UIView firstRowBackground = new UIView();
            firstRowBackground.BackgroundColor = UIColor.FromWhiteAlpha(0.8F, 0.5F);
            __innerFrame.AddChild(firstRowBackground);
        }

        private void CreateUIControls()
        {
            __newItem = new UITextField();
            __newItem.BorderStyle = UITextBorderStyle.RoundedRect;
            __newItem.Placeholder = "New Item";
            __newItem.AutocorrectionType = UITextAutocorrectionType.No;
            __newItem.AutocapitalizationType = UITextAutocapitalizationType.None;
            __newItem.ClearButtonMode = UITextFieldViewMode.Always;

            __addNewItem = new UIButton(UIButtonType.System);
            __addNewItem.Frame = new RectangleF(0, 0, 60, 0);
            __addNewItem.SetTitle("ADD", UIControlState.Normal);
            __addNewItem.Font = UIFont.BoldSystemFontOfSize(16);
            __addNewItem.TouchDown += this.AddNewItem_TouchDown;

            __done = new UIBarButtonItem();
            __done.Style = UIBarButtonItemStyle.Done;
            __done.Title = "Done";
            __done.Clicked += this.DoneButton_Click;

            __list = new UITableView();
        }

        private void AddUIControls()
        {
            __innerFrame.AddChild(__newItem, 0, 0, new SubViewThickness(5, 5, 60, 5));
            __innerFrame.AddChild(__addNewItem, 0, 0, 1, 1, true, false, GridHorizontalAlignment.Right);
            __innerFrame.AddChild(__list, 1, 0);

            this.NavigationItem.RightBarButtonItem = __done;

            this.Add(__innerFrame);
        }

        private void GoBackToList()
        {
            this.NavigationController.PopViewControllerAnimated(true);
        }

        private void BeReplacedWithViewer()
        {
            UIViewController[] navigationControllers = this.NavigationController.ViewControllers;
            ShakeList listToView = __presenter.GetModelList();
            ShakeListViewerView listViewer = new ShakeListViewerView(listToView);
            navigationControllers[navigationControllers.Length - 1] = listViewer;

            this.NavigationController.SetViewControllers(navigationControllers, true);
        }

        private void RequestName()
        {
            if (__nameInput == null)
            {
                __nameInput = new NameInputView();
                __nameInput.ModalInPopover = true;
                __nameInput.Ok += this.NameInput_Ok;
                __nameInput.Cancel += this.NameInput_Cancel;
            }

            __nameInput.Name = this.Title;
            this.PresentViewController(__nameInput, true, null);
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            this.DismissKeyboard();
        }

        #region UIControls Events

        private void AddNewItem_TouchDown(object sender, EventArgs e)
        {
            string newItemName = __newItem.Text;
            bool succeded = __presenter.AddNewItemLast(newItemName);

            if (!succeded)
                return;

            __newItem.Text = string.Empty;
            __newItem.ResignFirstResponder();

            NSIndexPath newIndex = NSIndexPath.FromRowSection(__items.Count - 1, 0);

            __list.BeginUpdates();
            __list.InsertRows(new NSIndexPath[]{ newIndex }, UITableViewRowAnimation.Top);
            __list.EndUpdates();
        }

        private void DoneButton_Click(object sender, EventArgs e)
        {
            __newItem.ResignFirstResponder();

            switch (__mode)
            {
                case AppViewMode.New:
                    this.RequestName();
                    break;
                case AppViewMode.Edit:
                    __presenter.Save();
                    this.GoBackToList();
                    break;
                default:
                    throw new InvalidOperationException("This mode hasn't Done Action");
            }
        }

        private void ListSource_RowMoved(object sender, MoveRowEventArgs e)
        {
            __presenter.MoveItem(e.SourceIndex, e.DestinationIndex);
        }

        private void ListSource_RowDeleted(object sender, DeleteRowEventArgs e)
        {
            __presenter.DeleteItem(e.DeleteIndex);
        }

        private void NameInput_Ok(object sender, EventArgs e)
        {
            this.Name = __nameInput.Name;
            __presenter.Save();
            this.DismissViewController(true, null);
            this.BeReplacedWithViewer();
        }

        private void NameInput_Cancel(object sender, EventArgs e)
        {
            this.DismissViewController(true, null);
        }

        #endregion
    }
}

