using System;
using MonoTouch.UIKit;
using System.Drawing;
using UIComponents;

namespace ShakeMyList.Iphone
{
    public class CustomViewController : UIViewController
    {
        private CustomNavigationController __customNavigation;

        public CustomViewController()
        {
            this.View.AutosizesSubviews = false;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        public override void LoadView()
        {
            base.LoadView();

            RectangleF navigationFrame = new RectangleF(0, IphoneEnviroment.StatusBarHeight, this.View.Frame.Width, this.View.Frame.Height - IphoneEnviroment.StatusBarHeight);
            __customNavigation = new CustomNavigationController(navigationFrame);
            this.Add(__customNavigation.View);
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
        }
    }
}

