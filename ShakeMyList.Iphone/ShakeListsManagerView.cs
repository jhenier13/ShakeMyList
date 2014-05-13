using System;
using System.Collections.Generic;
using System.Drawing;
using LobaSoft.IOS.UIComponents;
using LobaSoft.IOS.UIComponents.Events;
using LobaSoft.IOS.UIComponents.Frames;
using MonoTouch.UIKit;
using ShakeMyList.Mobile;
using ShakeMyList.Mobile.Presenters;
using ShakeMyList.Mobile.Views;

namespace ShakeMyList.Iphone
{
    public class ShakeListsManagerView : UIViewController, IShakeListsManagerView, IDisposableView
    {
        //Attributes
        private ShakeListsManagerPresenter __presenter;
        private List<ShakeList> __allLists;
        //UIControls
        private GridView __innerFrame;
        private UITableView __table;
        private UIBarButtonItem __edit;
        private UIBarButtonItem __add;
        private UIBarButtonItem __generator;
        //UIControls Extras
        private ShakeListsSource __listsSource;

        public List<ShakeList> AllLists
        {
            get
            {
                return __allLists;
            }
            set
            {
                if (__allLists == value)
                    return;

                if (__listsSource != null)
                    this.DetachListsEventHandlers();

                __allLists = value;
                __listsSource = new ShakeListsSource(__allLists);
                this.AttachListsEventHandlers();
                __table.Source = __listsSource;
            }
        }

        public ShakeListsManagerView()
        {
            //This is required so the parent UINavigationController doesn't resize this UIViewController like he wants
            this.EdgesForExtendedLayout = UIRectEdge.None;
            this.View.BackgroundColor = UIColor.White;
            __presenter = new ShakeListsManagerPresenter(this);
            this.Title = "Lists";
        }

        public void AttachEventHandlers()
        {
            __edit.Clicked += this.EditButton_Click;
            __add.Clicked += this.Add_Clicked;
            __generator.Clicked += this.Generator_Clicked;

            if (__listsSource != null)
                this.AttachListsEventHandlers();
        }

        public void DetachEventHandlers()
        {
            __edit.Clicked -= this.EditButton_Click;
            __add.Clicked -= this.Add_Clicked;
            __generator.Clicked -= this.Generator_Clicked;

            if (__listsSource != null)
                this.DetachListsEventHandlers();
        }

        public void CleanSubViews()
        {
        }

        public void AddSubViews()
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
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.NavigationController.ToolbarHidden = false;
            this.AttachEventHandlers();

            __innerFrame.Frame = new RectangleF(0, 0, this.View.Frame.Width, this.View.Frame.Height);
            __innerFrame.UpdateChildrenLayout();

            __presenter.LoadData();
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            this.DetachEventHandlers();
        }

        private void CreateGrid()
        {
            __innerFrame = new GridView();
            __innerFrame.Frame = new RectangleF(0, 0, this.View.Frame.Width, this.View.Frame.Height);
        }

        private void CreateUIControls()
        {
            __edit = new UIBarButtonItem();
            __edit.Style = UIBarButtonItemStyle.Plain;
            __edit.Title = "Edit";

            __add = new UIBarButtonItem(UIBarButtonSystemItem.Add);

            __generator = new UIBarButtonItem();
            __generator.Title = "Generator";
            __generator.Style = UIBarButtonItemStyle.Plain;

            __table = new UITableView();
        }

        private void AddUIControls()
        {
            __innerFrame.AddChild(__table);

            this.NavigationItem.RightBarButtonItem = __edit;
            this.NavigationItem.LeftBarButtonItem = __add;

            UIBarButtonItem dummy1 = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
            UIBarButtonItem dummy2 = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
            this.ToolbarItems = new UIBarButtonItem[]{ dummy1, __generator, dummy2 };

            this.Add(__innerFrame);
        }

        private void AttachListsEventHandlers()
        {
            __listsSource.RowHasBeenSelected += this.ListsSource_RowHasBeenSelected;
            __listsSource.RowDeleted += this.ListsSource_RowDeleted;
        }

        private void DetachListsEventHandlers()
        {
            __listsSource.RowHasBeenSelected -= this.ListsSource_RowHasBeenSelected;
            __listsSource.RowDeleted -= this.ListsSource_RowDeleted;
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            if (__table.Editing)
            {
                //Edit is done
                __table.SetEditing(false, true);
                __edit.Title = "Edit";
            }
            else
            {
                //Edit has begun
                __table.SetEditing(true, true);
                __edit.Title = "Done";
            }
        }

        private void Add_Clicked(object sender, EventArgs e)
        {
            this.NavigationController.ToolbarHidden = true;
            ShakeListEditorView newListView = new ShakeListEditorView();
            this.NavigationController.PushViewController(newListView, true);
        }

        private void Generator_Clicked(object sender, EventArgs e)
        {
//            this.NavigationController.ToolbarHidden = true;
            //Go the the generator view
        }

        private void ListsSource_RowHasBeenSelected(object sender, SelectRowEventArgs e)
        {
            if (__table.Editing)
                return;

            this.NavigationController.ToolbarHidden = true;
            ShakeList list = __presenter.ListFrom(e.SelectedIndex);
            ShakeListViewerView listView = new ShakeListViewerView(list);

            this.NavigationController.PushViewController(listView, true);
        }

        private void ListsSource_RowDeleted(object sender, DeleteRowEventArgs e)
        {
            __presenter.DeleteList(e.DeleteIndex);
        }
    }
}

