using System;
using System.Collections.Generic;
using ShakeMyList.Mobile.Views;

namespace ShakeMyList.Mobile.Presenters
{
    public class ShakeListEditorPresenter
    {
        private IShakeListEditorView __view;
        private ShakeList __list;

        public ShakeListEditorPresenter(IShakeListEditorView view)
        {
            if (view == null)
                throw new ArgumentNullException("The view can't be null");

            __view = view;
            __list = new ShakeList();
            __list.Name = string.Empty;
        }

        public ShakeListEditorPresenter(IShakeListEditorView view, ShakeList list)
        {
            if (view == null)
                throw new ArgumentNullException("The view can't be null");

            __view = view;
            __list = list;

            __list.LoadItems();
        }

        public void LoadData()
        {
            __view.Name = __list.Name;
            __view.Items = new List<ShakeItem>(__list.Items);
        }

        public void Save()
        {
            __list.Name = __view.Name;
            __list.Save();
        }

        public void AddNewItemFirst(string itemName)
        {

        }

        public bool AddNewItemLast(string itemName)
        {
            if (string.IsNullOrEmpty(itemName))
                return false;

            ShakeItem newItem = new ShakeItem(){ Name = itemName };
            __list.AddItem(newItem);
            __view.Items.Add(newItem);
            return true;
        }

        public void DeleteItem(int index)
        {
            __list.RemoveItem(index);
            __view.Items.RemoveAt(index);
        }

        public void MoveItem(int source, int destiny)
        {
            __list.MoveItem(source, destiny);

            ShakeItem movedItem = __view.Items[source];
            __view.Items.RemoveAt(source);
            __view.Items.Insert(destiny, movedItem);
        }

        public ShakeList GetModelList()
        {
            return __list;
        }
    }
}

