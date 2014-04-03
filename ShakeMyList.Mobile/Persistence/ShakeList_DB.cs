using System;
using System.Collections.Generic;
using System.Data;
using ShakeMyList.Mobile.Persistence;

namespace ShakeMyList.Mobile
{
    public partial class ShakeList
    {
        #region DB_FIELDS

        private static readonly string MAX_ID = "MaxID";
        public static readonly string TABLE_NAME = "ShakeLists";
        public static readonly string ID = "ListID";
        public static readonly string NAME = "Name";
        public static readonly string AUTOMATIC_SAVE = "AutomaticSave";
        public static readonly string LAST_OPENED_DATE = "LastOpenedDate";

        #endregion

        public void LoadItems()
        {
            if (this.Id == PersistenceDefaultValues.NO_IDENTIFIED)
                return;

            _items.Clear();
            List<ShakeItem> items = ShakeItem.GetItems(this.Id);

            _items.AddRange(items);
        }

        public void Save()
        {
            if (this.Id == PersistenceDefaultValues.NO_IDENTIFIED)
                this.Insert();
            else
                this.Update();

            this.SaveItems();
        }

        public void Delete()
        {
            if (this.Id == PersistenceDefaultValues.NO_IDENTIFIED)
                return;

            string deleteQuery = string.Format("DELETE FROM {0} WHERE {1}={2}", TABLE_NAME, ID, this.Id);
            SQLiteLinker.ExecuteQuery(deleteQuery);

            ShakeItem.DeleteItems(this.Id);
            this.Id = PersistenceDefaultValues.NO_IDENTIFIED;
        }

        public static List<ShakeList> GetAllLists()
        {
            List<ShakeList> allLists = new List<ShakeList>();

            string selectAllQuery = string.Format("SELECT * FROM {0}", TABLE_NAME);
            DataTable table = SQLiteLinker.GetDataTable(selectAllQuery);

            foreach (DataRow singleRow in table.Rows)
            {
                ShakeList newList = new ShakeList();
                newList.LoadData(singleRow);

                allLists.Add(newList);
            }

            return allLists;
        }

        public static List<ShakeList> GetRecentLists()
        {
            List<ShakeList> recentLists = new List<ShakeList>();
            string selectRecentsQuery = string.Format("SELECT * FROM {0} ORDER BY {1} DESC LIMIT {2}", TABLE_NAME, LAST_OPENED_DATE, 5);
            DataTable table = SQLiteLinker.GetDataTable(selectRecentsQuery);

            foreach (DataRow singleRow in table.Rows)
            {
                ShakeList newList = new ShakeList();
                newList.LoadData(singleRow);

                recentLists.Add(newList);
            }

            return recentLists;
        }

        private void LoadByID(int id)
        {
            if (id == PersistenceDefaultValues.NO_IDENTIFIED)
                throw new ArgumentException("This id is for new objects, created by the default constructor");

            if (id <= 0)
                throw new ArgumentException("The id has to be a value above zero");

            string selectQuery = string.Format("SELECT * FROM {0} WHERE {1}={2}", TABLE_NAME, ID, id);
            DataTable table = SQLiteLinker.GetDataTable(selectQuery);

            if (table.Rows.Count == 0)
                throw new ArgumentException("The id doesn't exists");

            this.LoadData(table.Rows[0]);
        }

        private void LoadData(DataRow row)
        {
            this.Id = Convert.ToInt32(row[ID]);
            this.Name = Convert.ToString(row[NAME]);

            int automaticSaveValue = Convert.ToInt32(row[AUTOMATIC_SAVE]);
            this.AutomaticSave = (automaticSaveValue == 0) ? false : true;
        }

        private void Insert()
        {
            this.Id = this.GetNewId();
            string insertQuery = string.Format("INSERT INTO {0} ({1},{3},{5}) VALUES ({2},\'{4}\',{6})",
                                     TABLE_NAME, ID, this.Id, NAME, SQLiteLinker.FixSQLInjection(this.Name), AUTOMATIC_SAVE, this.AutomaticSave ? "1" : "0");

            SQLiteLinker.ExecuteQuery(insertQuery);
        }

        private void Update()
        {
            string updateQuery = string.Format("UPDATE {0} SET {3}=\'{4}\',{5}={6} WHERE {1}={2}", 
                                     TABLE_NAME, ID, this.Id, NAME, SQLiteLinker.FixSQLInjection(this.Name), AUTOMATIC_SAVE, this.AutomaticSave ? "1" : "0");

            SQLiteLinker.ExecuteQuery(updateQuery);
        }

        private void SaveItems()
        {
            foreach (ShakeItem item in _items)
            {
                item.ListId = this.Id;
                item.Save();
            }
        }

        private void SetLastOpenedDate(DateTime date)
        {
            string dateStr = date.ToString(ApplicationFormats.DATE_FULL_FORMAT);
            string updateQuery = string.Format("UPDATE {0} SET {3}=\'{4}\' WHERE {1}={2}", TABLE_NAME, ID, this.Id, LAST_OPENED_DATE, dateStr);

            SQLiteLinker.ExecuteQuery(updateQuery);
        }

        private int GetNewId()
        {
            string selectMaxQuery = string.Format("SELECT MAX ({1}) AS {2}  FROM {0}", TABLE_NAME, ID, MAX_ID);
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

