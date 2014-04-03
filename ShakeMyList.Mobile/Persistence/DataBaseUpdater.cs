using System;
using System.IO;
using Mono.Data.Sqlite;

namespace ShakeMyList.Mobile.Persistence
{
    public static class DataBaseUpdater
    {
        //LastOpenedDate    Format:YYYYMMDDHHMM
        private static string SHAKELIST_SCHEMA = @"CREATE TABLE ShakeLists (
                                                 ListID             INTEGER     PRIMARY KEY     NOT NULL,
                                                 Name               VARCHAR,
                                                 AutomaticSave      INTEGER,
                                                 LastOpenedDate     VARCHAR
                                                 )";

        private static string SHAKEITEM_SCHEMA = @"CREATE TABLE ShakeItems (
                                                 ItemID             INTEGER     PRIMARY KEY     NOT NULL,
                                                 ListID             INTEGER,
                                                 Name               VARCHAR,
                                                 ItemIndex              INTEGER,
                                                 IsLocked           INTEGER,
                                                 IsMarked           INTEGER
                                                 )";
        private static string APPLICATION_SCHEMA = @"CREATE TABLE Application (
                                                   Version      VARCHAR
                                                   )";

        public static void TryCreateDataBase()
        {
            bool databaseExists = File.Exists(DBEnviroment.DATABASE_FILE_PATH);

            if (!databaseExists)
            {
                Directory.CreateDirectory(DBEnviroment.DATABASE_DIRECTORY_PATH);
                SqliteConnection.CreateFile(DBEnviroment.DATABASE_FILE_PATH);
                CreateDataBaseSchema();
            }
        }

        private static void CreateDataBaseSchema()
        {
            SQLiteLinker.ExecuteQuery(SHAKELIST_SCHEMA);
            SQLiteLinker.ExecuteQuery(SHAKEITEM_SCHEMA);
            SQLiteLinker.ExecuteQuery(APPLICATION_SCHEMA);

            string insertVersionQuery = string.Format("INSERT INTO Application(Version) VALUES (\'{0}\')", "1.0.0");
            SQLiteLinker.ExecuteQuery(insertVersionQuery);
        }
    }
}

