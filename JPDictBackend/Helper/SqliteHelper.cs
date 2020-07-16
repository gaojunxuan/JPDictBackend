using JPDictBackend.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace JPDictBackend.Helper
{
    class SqliteHelper
    {
#if DEBUG
        static string currentPath = "dict.db";
#else
        static string currentPath = @"D:\home\site\wwwroot\dict.db";
#endif
        static SQLiteConnection _conn = new SQLiteConnection(currentPath);
        public static IEnumerable<Dict> Query(string keyword)
        {
            return _conn.Query<Dict>("SELECT * FROM Dict WHERE Keyword = ?", keyword);
        }
    }
}
