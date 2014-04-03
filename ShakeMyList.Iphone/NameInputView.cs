using System;
using MonoTouch.UIKit;
using UIComponents.Frames;
using System.Drawing;

namespace ShakeMyList.Iphone
{
    public class NameInputView : UIViewController
    {
        //UIControls
        private GridView __innerFrame;
        private UITextField __name;
        private UIButton __ok;
        private UIButton __cancel;
        private UILabel __title;
        private UIView __innerWindow;
        //EventHandlers
        public event EventHandler Ok;
        public event EventHandler Cancel;

        public string Name
        {
            get { return (__name == null) ? string.Empty : __name.Text; }
            set
            {
                if (__name != null)
                    __name.Text = value;
            }
        }

        public NameInputView()
        {
            this.View.BackgroundColor = UIColor.FromWhiteAlpha(0.8F, 0.5F);
        }

        public override void LoadView()
        {
            base.LoadView();
            this.CreateGrid();
            this.CreateUIControls();
            this.AddUIControls();}

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

        private void CreateGrid()
        {
            __innerFrame = new GridView();
            __innerFrame.Frame = new RectangleF(0, 0, this.View.Frame.Width, this.View.Frame.Height);
            __innerFrame.AddRowsAndColumns("0.35*;25;40;30;0.65*", "0.5*;100;100;0.5*");
        }

        private void CreateUIControls()
        {
            __title = new UILabel();
            __title.Text = "Enter List Name";
            __title.TextAlignment = UITextAlignment.Center;
            __title.Font = UIFont.SystemFontOfSize(12);

            __name = new UITextField();
            __name.Placeholder = "List Name";
            __name.BorderStyle = UITextBorderStyle.RoundedRect;
            __name.ClearButtonMode = UITextFieldViewMode.Always;
            __name.AutocorrectionType = UITextAutocorrectionType.No;
            __name.AutocapitalizationType = UITextAutocapitalizationType.None;

            __ok = new UIButton(UIButtonType.System);
            __ok.SetTitle("Ok", UIControlState.Normal);
            __ok.TouchDown += this.OK_TouchDown;

            __cancel = new UIButton(UIButtonType.System);
            __cancel.SetTitle("Cancel", UIControlState.Normal);
            __cancel.TouchDown += this.Cancel_TouchDown;

            __innerWindow = new UIView();
            __innerWindow.BackgroundColor = UIColor.LightGray;
        }

        private void AddUIControls()
        {
            __innerFrame.AddChild(__innerWindow, 1, 1, 3, 2);
            __innerFrame.AddChild(__title, 1, 1, 1, 2);
            __innerFrame.AddChild(__name, 2, 1, 1, 2);
            __innerFrame.SetMargin(__name, new SubViewThickness(5, 3));
            __innerFrame.AddChild(__ok, 3, 1);
            __innerFrame.AddChild(__cancel, 3, 2);

            this.Add(__innerFrame);
        }

        private void OK_TouchDown(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(__name.Text))
                return;

            var handler = this.Ok;

            if (handler != null)
                handler(this, new EventArgs());
        }

        private void Cancel_TouchDown(object sender, EventArgs e)
        {
            var handler = this.Cancel;

            if (handler != null)
                handler(this, new EventArgs());
        }
    }
}

