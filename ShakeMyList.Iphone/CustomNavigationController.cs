using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace ShakeMyList.Iphone
{
    public class CustomNavigationController : UINavigationController
    {
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
            this.InteractivePopGestureRecognizer.Enabled = false;
        }

        public override void LoadView()
        {
            base.LoadView();

            ShakeListsManagerView listsManager = new ShakeListsManagerView();
            this.PushViewController(listsManager, true);
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
        }
    }
}

