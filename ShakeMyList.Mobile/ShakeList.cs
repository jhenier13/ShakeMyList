using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ShakeMyList.Mobile.Persistence;

namespace ShakeMyList.Mobile
{
    public partial class ShakeList
    {
        private int _id;
        private string _name;
        private bool _automaticSave = false;
        //All the operations in the list has to be done in this collection
        private List<ShakeItem> _items;

        public int Id { get { return _id; } internal set { _id = value; } }

        public string Name { get { return _name; } set { _name = value; } }

        public bool AutomaticSave { get { return _automaticSave; } set { _automaticSave = value; } }

        public ReadOnlyCollection<ShakeItem> Items { get; private set; }

        public ShakeList()
        {
            this.Name = string.Empty;
            this.Id = PersistenceDefaultValues.NO_IDENTIFIED;
            _items = new List<ShakeItem>();
            this.Items = _items.AsReadOnly();
        }

        public ShakeList(int id)
        {
            _items = new List<ShakeItem>();
            this.Items = _items.AsReadOnly();
            this.LoadByID(id);
        }

        public void OpenRecently()
        {
            this.SetLastOpenedDate(DateTime.Now);
        }

        public bool AddItem(ShakeItem newItem)
        {
            newItem.Index = _items.Count;
            _items.Add(newItem);
            return true;
        }

        public bool RemoveItem(int index)
        {
            ShakeItem itemToRemove = _items[index];
            _items.RemoveAt(index);
            itemToRemove.Delete();
            this.RefreshIndexes();
            return true;
        }

        public bool MoveItem(int source, int destination)
        {
            ShakeItem movedItem = _items[source];
            _items.RemoveAt(source);
            _items.Insert(destination, movedItem);
            this.RefreshIndexes();
            return true;
        }

        public void LockItem(int index)
        {
        }

        public void UnlockItem(int index)
        {
        }

        public void Reshuffle()
        {
        }

        public ShakeList Clone()
        {
            ShakeList clonedList = new ShakeList();
            clonedList.Name = this.Name;

            foreach (ShakeItem item in _items)
            {
                ShakeItem newClonedItem = item.Clone();
                clonedList.AddItem(newClonedItem);
            }

            return clonedList;
        }

        private void RefreshIndexes()
        {
            for (int i = 0; i < _items.Count; i++)
                _items[i].Index = i;
        }
    }
}

