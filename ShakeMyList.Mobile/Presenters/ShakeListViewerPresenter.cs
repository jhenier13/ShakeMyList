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
        }

        public void UnMarkItem(int index)
        {
        }

        public void MoveItem(int sourceIndex, int destinyIndex)
        {
            __list.MoveItem(sourceIndex, destinyIndex);

            ShakeItem movedItem = __view.Items[sourceIndex];
            __view.Items.RemoveAt(sourceIndex);
            __view.Items.Insert(destinyIndex, movedItem);
        }

        public void SaveList()
        {
            __list.Name = __view.ListName;
            __list.Save();
        }

        public void ShakeTheList()
        {
            __list.Reshuffle();
            __view.RefreshItemsDraw();
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

