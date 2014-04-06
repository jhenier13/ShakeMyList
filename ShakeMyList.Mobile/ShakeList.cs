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
            _items.Move(source, destination);
            this.RefreshIndexes();
            return true;
        }

        public void LockItem(int index)
        {
            _items[index].IsLocked = true;
        }

        public void UnlockItem(int index)
        {
            _items[index].IsLocked = false;
        }

        public void MarkItem(int index)
        {
            _items[index].IsMarked = true;
        }

        public void UnmarkItem(int index)
        {
            _items[index].IsMarked = false;
        }

        public List<MoveLog> Reshuffle()
        {
            List<MoveLog> changesLog = new List<MoveLog>();

            ShakeItem[] newItems = new ShakeItem[_items.Count];
            List<int> unlockedIndexes = new List<int>();

            //Put the locked items in their places
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].IsLocked)
                    newItems[i] = _items[i];
                else
                    unlockedIndexes.Add(i);
            }

            //Put the un-locked items int new places
            for (int j = 0; j < _items.Count; j++)
            {
                if (newItems[j] != null)
                    continue;

                int randomIndex = RandomGenerator.GenerateInteger(0, unlockedIndexes.Count);
                int newItemIndex = unlockedIndexes[randomIndex];
                newItems[j] = _items[newItemIndex];

                unlockedIndexes.RemoveAt(randomIndex);
                changesLog.Add(new MoveLog(){ OriginalIndex = newItemIndex, ChangedIndex = j });
            }

            _items.Clear();
            _items.AddRange(newItems);

            return changesLog;
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

    public struct MoveLog
    {
        public int OriginalIndex{ get; set; }

        public int ChangedIndex{ get; set; }
    }
}

