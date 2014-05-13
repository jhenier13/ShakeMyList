using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using ShakeMyList.Mobile;

namespace ShakeMyList.Iphone
{
    public class ShakeListCell : UITableViewCell
    {
        public ShakeListCell(string cellID) : base(UITableViewCellStyle.Default, cellID)
        {
        }
    }

    public class NoEditShakeItemCell : UITableViewCell
    {
        private const float LOCK_WIDTH = 30F;
        private const float LOCK_HEIGHT = 30F;
        //Static Attributes
        private static UIImage LOCK_ICON;
        private static UIImage UNLOCK_ICON;
        //UIControls
        private UIButton __lockButton;
        //Events
        public event EventHandler Locked;
        public event EventHandler Unlocked;

        public bool IsLocked
        {
            get{ return __lockButton.Selected; }
            set{ __lockButton.Selected = value; }
        }

        public ShakeItem Item
        {
            get;
            set;
        }

        public override bool Selected
        {
            get
            {
                return base.Selected;
            }
            set
            {
                base.Selected = value;

                if (value)
                    this.Accessory = UITableViewCellAccessory.Checkmark;
                else
                    this.Accessory = UITableViewCellAccessory.None;
            }
        }

        static NoEditShakeItemCell()
        {
            LOCK_ICON = UIImage.FromFile("LockIcon.png");
            UNLOCK_ICON = UIImage.FromFile("UnlockIcon.png");
        }

        public NoEditShakeItemCell(string cellID) : base(UITableViewCellStyle.Default, cellID)
        {
            __lockButton = new UIButton(UIButtonType.Custom);
            __lockButton.TouchDown += this.LockButton_TouchDown;
            __lockButton.SetImage(UNLOCK_ICON, UIControlState.Normal);
            __lockButton.SetImage(LOCK_ICON, UIControlState.Selected);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            float xPosition = 5;
            float yPosition = (this.Frame.Height - LOCK_HEIGHT) / 2;
            __lockButton.Frame = new RectangleF(xPosition, yPosition, LOCK_WIDTH, LOCK_HEIGHT);

            this.ContentView.Add(__lockButton);

            this.TextLabel.Frame = new RectangleF(this.TextLabel.Frame.X + LOCK_WIDTH, this.TextLabel.Frame.Y, this.TextLabel.Frame.Width, this.TextLabel.Frame.Height);
        }

        private void LockButton_TouchDown(object sender, EventArgs e)
        {
            EventHandler handler = null;
            this.IsLocked = !this.IsLocked;

            if (this.IsLocked)
                handler = this.Locked;
            else
                handler = this.Unlocked;

            if (handler != null)
                handler(this, new EventArgs());
        }
    }
}

