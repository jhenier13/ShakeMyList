using System;
using System.Collections.Generic;
using ShakeMyList.Mobile.Views;

namespace ShakeMyList.Mobile.Presenters
{
    public class ShakeListViewerPresenter
    {
        private IShakeListViewerView __view;
        private ShakeList __list;

        public ShakeListViewerPresenter(IShakeListViewerView view, ShakeList list)
        {
            if (view == null)
                throw new ArgumentNullException("The view can't be null");

            if (list == null)
                throw new ArgumentNullException("The list can't be null");

            __view = view;
            __list = list;
        }

        public void LoadData()
        {
            __list.LoadItems();
            __view.ListName = __list.Name;
            __view.Items = new List<ShakeItem>(__list.Items);
        }

        public void RenameShakeListName(string newName)
        {
            __list.Name = newName;
            __view.ListName = newName;
            __list.Save();
        }

        public void LockItem(int index)
        {
            __list.LockItem(index);
            __view.Items[index].IsLocked = true;
        }

        public void UnlockItem(int index)
        {
            __list.UnlockItem(index);
            __view.Items[index].IsLocked = false;
        }

        public void MarkItem(int index)
        {
            __list.MarkItem(index);
            __view.Items[index].IsMarked = true;
        }

        public void UnMarkItem(int index)
        {
            __list.UnmarkItem(index);
            __view.Items[index].IsMarked = false;
        }

        public void MoveItem(int sourceIndex, int destinyIndex)
        {
            __list.MoveItem(sourceIndex, destinyIndex);
            __view.Items.Move(sourceIndex, destinyIndex);
        }

        public void SaveList()
        {
            __list.Name = __view.ListName;
            __list.Save();
        }

        public void ShakeTheList()
        {
            List<MoveLog> changes = __list.Reshuffle();
            __view.RefreshItemsDraw(changes);
        }

        public ShakeList Clone()
        {
            ShakeList clonedList = __list.Clone();
            return clonedList;
        }

        public ShakeList GetModelList()
        {
            return __list;
        }
    }
}

