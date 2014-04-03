using System;
using System.IO;

namespace ShakeMyList.Mobile
{
    public static class EnviromentDirectories
    {
        private static readonly string IOS_LIBRARY = "Library";
        public static readonly string IOS_LIBRARY_DIRECTORY;
        public static readonly string IOS_DOCUMENTS_DIRECTORY;

        static EnviromentDirectories()
        {
            IOS_DOCUMENTS_DIRECTORY = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            IOS_LIBRARY_DIRECTORY = Path.Combine(IOS_DOCUMENTS_DIRECTORY, "..", IOS_LIBRARY);
        }
    }
}

