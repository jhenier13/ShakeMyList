using System;
using ShakeMyList.Mobile.Persistence;

namespace ShakeMyList.Mobile
{
    public partial class ShakeItem
    {
        private int _id;
        private int _listId;
        private string _name;
        private bool _isLocked;
        private bool _isMarked;
        private int _index;

        public int Id { get { return _id; } internal set { _id = value; } }

        public int ListId { get { return _listId; } internal set { _listId = value; } }

        public string Name { get { return _name; } set { _name = value; } }

        public bool IsLocked { get { return _isLocked; } set { _isLocked = value; } }

        public bool IsMarked { get { return _isMarked; } set { _isMarked = value; } }

        public int Index { get { return _index; } set { _index = (value < -1) ? -1 : value; } }

        public ShakeItem()
        {
            this.Id = PersistenceDefaultValues.NO_IDENTIFIED;
            this.Name = string.Empty;
            this.IsLocked = false;
            this.IsMarked = false;
            this.Index = -1;
        }

        public ShakeItem(int id)
        {
            this.LoadByID(id);
        }

        public ShakeItem Clone()
        {
            ShakeItem clonedItem = new ShakeItem();
            clonedItem.Name = this.Name;
            clonedItem.IsLocked = this.IsLocked;
            clonedItem.IsMarked = this.IsMarked;

            return clonedItem;
        }
    }
}

