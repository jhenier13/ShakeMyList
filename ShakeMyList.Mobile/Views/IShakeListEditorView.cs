using System;
using System.Collections.Generic;

namespace ShakeMyList.Mobile.Views
{
    public interface IShakeListEditorView
    {
        string Name { get; set; }

        List<ShakeItem> Items { get; set; }

        void RefreshItemsDraw();

        void ShowMessage(string message);
    }
}

