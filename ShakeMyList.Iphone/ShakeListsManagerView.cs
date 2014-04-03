using System;
using MonoTouch.UIKit;
using ShakeMyList.Mobile.Views;
using ShakeMyList.Mobile;
using System.Collections.Generic;
using ShakeMyList.Mobile.Presenters;
using UIComponents.Frames;
using System.Drawing;
using UIComponents.Events;

namespace ShakeMyList.Iphone
{
    public class ShakeListsManagerView : UIViewController, IShakeListsManagerView
    {
        private ShakeListsManagerPresenter __presenter;
        private GridView __innerFrame;
        private UITableView __table;
        private UIBarButtonItem __edit;
        private List<ShakeList> __allLists;
        private ShakeListsSource __listsSource;

        public List<ShakeList> AllLists
        {
            get
            {
                return __allLists;
            }
            set
            {
                __allLists = value;
                __listsSource = new ShakeListsSource(__allLists);
                __listsSource.RowHasBeenSelected = this.ListsSource_RowHasBeenSelected;
                __listsSource.RowDeleted = this.ListsSource_RowDeleted;
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

            __innerFrame.Frame = new RectangleF(0, 0, this.View.Frame.Width, this.View.Frame.Height);
            __innerFrame.UpdateChildrenLayout();

            __presenter.LoadData();
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
            __edit.Clicked += this.EditButton_Click;

            __table = new UITableView();
        }

        private void AddUIControls()
        {
            __innerFrame.AddChild(__table);

            this.NavigationItem.RightBarButtonItem = __edit;

            this.Add(__innerFrame);
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

        private void ListsSource_RowHasBeenSelected(object sender, SelectRowEventArgs e)
        {
            if (__table.Editing)
                return;

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

