using System;
using System.Collections.Generic;
using ShakeMyList.Mobile.Persistence;
using System.Data;

namespace ShakeMyList.Mobile
{
    public partial class ShakeItem
    {
        #region DB_FIELDS

        private static readonly string MAX_ID = "MaxID";
        public static readonly string TABLE_NAME = "ShakeItems";
        public static readonly string ID = "ItemID";
        public static readonly string LIST_ID = "ListID";
        public static readonly string NAME = "Name";
        public static readonly string INDEX = "ItemIndex";
        public static readonly string IS_LOCKED = "IsLocked";
        public static readonly string IS_MARKED = "IsMarked";

        #endregion

        public void Save()
        {
            if (this.Id == PersistenceDefaultValues.NO_IDENTIFIED)
                this.Insert();
            else
                this.Update();
        }

        public void Delete()
        {
            if (this.Id == PersistenceDefaultValues.NO_IDENTIFIED)
                return;

            string deleteQuery = string.Format("DELETE FROM {0} WHERE {1}={2}", TABLE_NAME, ID, this.Id);
            SQLiteLinker.ExecuteQuery(deleteQuery);

            this.Id = PersistenceDefaultValues.NO_IDENTIFIED;
        }

        internal static void DeleteItems(int listID)
        {
            string deleteQuery = string.Format("DELETE FROM {0} WHERE {1}={2}", TABLE_NAME, LIST_ID, listID);
            SQLiteLinker.ExecuteQuery(deleteQuery);
        }

        internal static List<ShakeItem> GetItems(int listID)
        {
            List<ShakeItem> items = new List<ShakeItem>();

            string getItemsQuery = string.Format("SELECT * FROM {0} WHERE {1}={2} ORDER BY {3}", TABLE_NAME, LIST_ID, listID, INDEX);
            DataTable table = SQLiteLinker.GetDataTable(getItemsQuery);

            foreach (DataRow singleRow in table.Rows)
            {
                ShakeItem newItem = new ShakeItem();
                newItem.LoadData(singleRow);

                items.Add(newItem);
            }

            return items;
        }

        private void LoadByID(int id)
        {
            if (id == PersistenceDefaultValues.NO_IDENTIFIED)
                throw new ArgumentException("This id is for new objects, created by the default constructor");

            if (id <= 0)
                throw new ArgumentException("The id has to be a value above zero");

            string loadQuery = string.Format("SELECT * FROM {0} WHERE {1}={2}", TABLE_NAME, ID, this.Id);
            DataTable table = SQLiteLinker.GetDataTable(loadQuery);

            if (table.Rows.Count == 0)
                throw new ArgumentException("The Id doesn't exists");

            this.LoadData(table.Rows[0]);
        }

        private void LoadData(DataRow row)
        {
            this.Id = Convert.ToInt32(row[ID]);
            this.ListId = Convert.ToInt32(row[LIST_ID]);
            this.Name = Convert.ToString(row[NAME]);
            this.Index = Convert.ToInt32(row[INDEX]);

            int isLockedValue = Convert.ToInt32(row[IS_LOCKED]);
            this.IsLocked = (isLockedValue == 0) ? false : true;

            int isMarkedValue = Convert.ToInt32(row[IS_MARKED]);
            this.IsMarked = (isMarkedValue == 0) ? false : true;
        }

        private void Insert()
        {
            this.Id = this.GetNewID();
            string insertQuery = string.Format("INSERT INTO {0} ({1},{3},{5},{7},{9},{11}) VALUES ({2},{4},\'{6}\',{8},{10},{12})",
                                     TABLE_NAME, ID, this.Id, LIST_ID, this.ListId, NAME, SQLiteLinker.FixSQLInjection(this.Name), INDEX, this.Index, IS_LOCKED, this.IsLocked ? "1" : "0", IS_MARKED, this.IsMarked ? "1" : "0");

            SQLiteLinker.ExecuteQuery(insertQuery);
        }

        private void Update()
        {
            string updateQuery = string.Format("UPDATE {0} SET {3}={4}, {5}=\'{6}\',{7}={8},{9}={10},{11}={12} WHERE {1}={2}",
                                     TABLE_NAME, ID, this.Id, LIST_ID, this.ListId, NAME, SQLiteLinker.FixSQLInjection(this.Name), INDEX, this.Index, IS_LOCKED, this.IsLocked ? "1" : "0", IS_MARKED, this.IsMarked ? "1" : "0");

            SQLiteLinker.ExecuteQuery(updateQuery);
        }

        private int GetNewID()
        {
            string selectMaxQuery = string.Format("SELECT MAX ({1}) AS {2} FROM {0}", TABLE_NAME, ID, MAX_ID);
            DataTable table = SQLiteLinker.GetDataTable(selectMaxQuery);

            if (table.Rows.Count == 0)
                throw new InvalidOperationException("This should return always 1 row!!");

            DataRow firstRow = table.Rows[0];

            if (firstRow[MAX_ID] == DBNull.Value)
                return PersistenceDefaultValues.FIRST_IDENTIFIER;

            int maxIdentifier = Convert.ToInt32(firstRow[MAX_ID]);
            maxIdentifier++;

            return maxIdentifier;
        }
    }
}

