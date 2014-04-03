using System;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using ShakeMyList.Mobile;
using UIComponents.Events;

namespace ShakeMyList.Iphone
{
    public class NoEditShakeListItemsSource : UITableViewSource
    {
        private IList<ShakeItem> __items;
        private string __cellIdentifier = "NoEditShakeItemCell";

        public event MoveRowEventHandler RowMoved;
        public event RowLockedEventHandler RowLocked;
        public event RowLockedEventHandler RowUnlocked;

        public NoEditShakeListItemsSource(IList<ShakeItem> items)
        {
            if (items == null)
                throw new ArgumentNullException("Items can't be null");

            __items = items;
        }

        public override int RowsInSection(UITableView tableview, int section)
        {
            return __items.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            NoEditShakeItemCell cell = tableView.DequeueReusableCell(__cellIdentifier) as NoEditShakeItemCell;

            if (cell == null)
            {
                cell = new NoEditShakeItemCell(__cellIdentifier);
                cell.Locked += ShakeItemCell_Locked;
                cell.Unlocked += ShakeItemCell_Unlocked;
            }

            ShakeItem item = __items[indexPath.Row];
            cell.TextLabel.Text = item.Name;
            cell.IsLocked = item.IsLocked;
            cell.Item = item;

            return cell;
        }

        public override bool CanMoveRow(UITableView tableView, NSIndexPath indexPath)
        {
            return true;
        }

        public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
        {
            return true;
        }

        public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return UITableViewCellEditingStyle.None;
        }

        public override bool ShouldIndentWhileEditing(UITableView tableView, NSIndexPath indexPath)
        {
            return false;
        }

        public override void MoveRow(UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
        {
            var handler = this.RowMoved;

            if (handler != null)
                handler(this, new MoveRowEventArgs(){ SourceIndex = sourceIndexPath.Row, DestinationIndex = destinationIndexPath.Row });
        }

        private void ShakeItemCell_Locked(object sender, EventArgs e)
        {
            NoEditShakeItemCell cell = sender as NoEditShakeItemCell;
            int itemIndex = __items.IndexOf(cell.Item);

            var handler = this.RowLocked;

            if (handler != null)
                handler(this, new RowLockEventArgs(){ Index = itemIndex, IsLocked = true });
        }

        private void ShakeItemCell_Unlocked(object sender, EventArgs e)
        {
            NoEditShakeItemCell cell = sender as NoEditShakeItemCell;
            int itemIndex = __items.IndexOf(cell.Item);

            var handler = this.RowUnlocked;

            if (handler != null)
                handler(this, new RowLockEventArgs(){ Index = itemIndex, IsLocked = false });
        }
    }
    public delegate void RowLockedEventHandler(object sender,RowLockEventArgs e);
    public class RowLockEventArgs : EventArgs
    {
        public int Index { get; set; }

        public bool IsLocked { get; set; }
    }
}

