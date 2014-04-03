using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace ShakeMyList.Iphone
{
    public class CustomNavigationController : UINavigationController
    {
        private MainMenuView __menuView;

        public CustomNavigationController()
        {
            this.View.AutosizesSubviews = false;
        }

        public CustomNavigationController(RectangleF frame)
        {
            this.View.AutosizesSubviews = false;
            this.View.Frame = frame;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.ModalPresentationStyle = UIModalPresentationStyle.CurrentContext;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        public override void LoadView()
        {
            base.LoadView();

            RectangleF menuFrame = new RectangleF(0, this.NavigationBar.Frame.Height, this.View.Frame.Width, this.View.Frame.Height - this.NavigationBar.Frame.Height);
            __menuView = new MainMenuView(menuFrame);
            this.PushViewController(__menuView, true);
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
        }
    }
}

