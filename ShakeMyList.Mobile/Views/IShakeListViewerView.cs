using System;
using System.Collections.Generic;

namespace ShakeMyList.Mobile.Views
{
    public interface IShakeListViewerView
    {
        string ListName { get; set; }

        List<ShakeItem> Items { get; set; }

        void RefreshItemsDraw(IList<MoveLog> changes);

        void ShowMessage(string message);
    }
}

