using System;
using MonoTouch.UIKit;
using ShakeMyList.Mobile;
using System.Collections.Generic;
using MonoTouch.Foundation;
using LobaSoft.IOS.UIComponents.Events;

namespace ShakeMyList.Iphone
{
    public class ShakeListsSource : UITableViewSource
    {
        private IList<ShakeList> __lists;
        private string __cellIdentifier = "ShakeListCell";
        public event SelectRowEventHandler RowHasBeenSelected;
        public event DeleteRowEventHandler RowDeleted;

        public ShakeListsSource(IList<ShakeList> lists)
        {
            __lists = lists;
        }

        public override int RowsInSection(UITableView tableview, int section)
        {
            return __lists.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = tableView.DequeueReusableCell(__cellIdentifier);

            if (cell == null)
                cell = new UITableViewCell(UITableViewCellStyle.Default, __cellIdentifier);

            ShakeList list = __lists[indexPath.Row];
            cell.TextLabel.Text = list.Name;

            return cell;
        }

        public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
        {
            return true;
        }

        public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return UITableViewCellEditingStyle.Delete;
        }

        public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
        {
            switch (editingStyle)
            {
                case UITableViewCellEditingStyle.Delete:
                    var handler = this.RowDeleted;

                    if (handler != null)
                        handler(this, new DeleteRowEventArgs(){ DeleteIndex = indexPath.Row });

                    tableView.DeleteRows(new NSIndexPath[]{ indexPath }, UITableViewRowAnimation.Fade);
                    break;
                default:
                    break;
            }
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var handler = this.RowHasBeenSelected;

            if (handler != null)
                handler(this, new SelectRowEventArgs(){ SelectedIndex = indexPath.Row });
        }
    }
}

