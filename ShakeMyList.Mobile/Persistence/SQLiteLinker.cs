using System;
using Mono.Data.Sqlite;
using System.Data;

namespace ShakeMyList.Mobile.Persistence
{
    public static class SQLiteLinker
    {
        private static SqliteConnection __connection;
        private static readonly string CONNECTION_STRING;

        static SQLiteLinker()
        {
            SqliteConnectionStringBuilder connectionBuilder = new SqliteConnectionStringBuilder();
            connectionBuilder.JournalMode = SQLiteJournalModeEnum.Persist;
            connectionBuilder.DataSource = DBEnviroment.DATABASE_FILE_PATH;
            CONNECTION_STRING = connectionBuilder.ConnectionString;

            __connection = new SqliteConnection(CONNECTION_STRING);
        }

        public static int ExecuteQuery(string query)
        {
            int rowsAffected = 0;
            SqliteCommand command = null;

            try
            {
                __connection.Open();
                command = __connection.CreateCommand();
                command.CommandText = query;
                rowsAffected = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error in the DataBase", ex);
            }
            finally
            {
                if (command != null)
                    command.Dispose();

                __connection.Close();
            }

            return rowsAffected;
        }

        public static DataTable GetDataTable(string query)
        {
            DataSet tempDataSet = new DataSet();
            SqliteDataAdapter tempAdapter = null;

            try
            {
                __connection.Open();
                tempAdapter = new SqliteDataAdapter(query, __connection);
                tempDataSet.Reset();
                tempAdapter.Fill(tempDataSet);

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error in the DataBase", ex);
            }
            finally
            {
                if (tempAdapter != null)
                    tempAdapter.Dispose();

                __connection.Close();
            }

            return tempDataSet.Tables[0];
        }

        public static string FixSQLInjection(string data)
        {
            return data.Replace("'", "''");
        }
    }
}

