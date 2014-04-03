using System;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using ShakeMyList.Mobile;
using UIComponents.Events;

namespace ShakeMyList.Iphone
{
    public class ShakeListItemsSource : UITableViewSource
    {
        private IList<ShakeItem> __items;
        private string __cellIdentifier = "ShakeItemCell";
        public MoveRowEventHandler RowMoved;
        public DeleteRowEventHandler RowDeleted;
        public event EventHandler RowsScrolled;

        public ShakeListItemsSource(IList<ShakeItem> items)
        {
            if (items == null)
                throw new ArgumentNullException("Items can't be null");

            __items = items;
        }

        public override int RowsInSection(UITableView tableview, int section)
        {
            return __items.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
        {
            UITableViewCell cell = tableView.DequeueReusableCell(__cellIdentifier);

            if (cell == null)
                cell = new ShakeListCell(__cellIdentifier);

            ShakeItem item = __items[indexPath.Row];
            cell.TextLabel.Text = item.Name;

            return cell;
        }

        public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
        {
            return true;
        }

        public override bool CanMoveRow(UITableView tableView, NSIndexPath indexPath)
        {
            return true;
        }

        public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return UITableViewCellEditingStyle.Delete;
        }

        public override void MoveRow(UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
        {
            var handler = this.RowMoved;

            if (handler != null)
                handler(this, new MoveRowEventArgs(){ SourceIndex = sourceIndexPath.Row, DestinationIndex = destinationIndexPath.Row });
        }

        public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
        {
            switch (editingStyle)
            {
                case UITableViewCellEditingStyle.Delete:
                    var handler = this.RowDeleted;

                    if (handler != null)
                        handler(this, new DeleteRowEventArgs(){ DeleteIndex = indexPath.Row });

                    tableView.DeleteRows(new NSIndexPath[]{ indexPath }, UITableViewRowAnimation.Bottom);
                    break;
                default:
                    break;
            }
        }

        public override void Scrolled(UIScrollView scrollView)
        {
            var handler = this.RowsScrolled;

            if (handler != null)
                handler(this, new EventArgs());
        }
    }
}

