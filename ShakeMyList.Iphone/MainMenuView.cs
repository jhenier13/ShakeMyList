using System;
using MonoTouch.UIKit;
using ShakeMyList.Mobile.Views;
using UIComponents.Frames;
using UIComponents.Factories;
using System.Collections.Generic;
using ShakeMyList.Mobile;
using System.Drawing;

namespace ShakeMyList.Iphone
{
    public class MainMenuView: UIViewController, IMenuView
    {
        private GridView __innerFrame;
        private UIButton __createListButton;
        private UIButton __viewListsButton;
        private UIButton __generatorButton;

        public List<ShakeList> RecentLists
        {
            get{ throw new NotImplementedException(); }
            set{ throw new NotImplementedException(); }
        }

        public MainMenuView(RectangleF frame)
        {
            this.View.Frame = frame;
            this.View.AutosizesSubviews = false;
            //This is required so the parent UINavigationController doesn't resize this UIViewController like he wants
            this.EdgesForExtendedLayout = UIRectEdge.None;
            this.View.BackgroundColor = UIColor.White;
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
        }

        public override void LoadView()
        {
            base.LoadView();

            this.CreateGrid();
            this.CreateUIControls();
            this.AddUIControls();     
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
        }

        private void CreateGrid()
        {
            __innerFrame = new GridView();
            __innerFrame.AddRowsAndColumns("0.5*;100;15;100;15;100;0.5*", "0.5*;100;0.5*");

            __innerFrame.Frame = new RectangleF(0, 0, this.View.Frame.Width, this.View.Frame.Height);
        }

        private void AddUIControls()
        {
            __innerFrame.AddChild(__createListButton, 1, 1);
            __innerFrame.AddChild(__viewListsButton, 3, 1);
            __innerFrame.AddChild(__generatorButton, 5, 1);

            this.Add(__innerFrame);
        }

        private void CreateUIControls()
        {
            __createListButton = UIButtonFactory.CreateImageButton("AddButton.png");
            __createListButton.TouchDown += this.CreateListButton_TouchDown;
            __viewListsButton = UIButtonFactory.CreateImageButton("ListButton.png");
            __viewListsButton.TouchDown += this.ViewListsButton_TouchDown;
            __generatorButton = UIButtonFactory.CreateImageButton("AddButton.png");
        }

        private void CreateListButton_TouchDown(object sender, EventArgs e)
        {
            ShakeListEditorView newListView = new ShakeListEditorView();
            this.NavigationController.PushViewController(newListView, true);
        }

        private void ViewListsButton_TouchDown(object sender, EventArgs e)
        {
            ShakeListsManagerView newManagerView = new ShakeListsManagerView();
            this.NavigationController.PushViewController(newManagerView, true);
        }
       
    }
}

