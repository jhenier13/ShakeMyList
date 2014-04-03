using System;
using System.IO;

namespace ShakeMyList.Mobile.Persistence
{
    public static class DBEnviroment
    {
        private static readonly string DATABASE_NAME = "ShakeMyListDB.db";
        private static readonly string DATABASE_FOLDER_NAME = "DataBase";
        public static readonly string DATABASE_DIRECTORY_PATH;
        public static readonly string DATABASE_FILE_PATH;

        static DBEnviroment()
        {
            DATABASE_DIRECTORY_PATH = Path.Combine(EnviromentDirectories.IOS_LIBRARY_DIRECTORY, DATABASE_FOLDER_NAME);
            DATABASE_FILE_PATH = Path.Combine(DATABASE_DIRECTORY_PATH, DATABASE_NAME);
        }
    }
}

