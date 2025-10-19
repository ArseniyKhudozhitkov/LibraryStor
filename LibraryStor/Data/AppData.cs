using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Data
{
    public static class AppData
    {
        public static DatabaseContext Context { get; set; }

        static AppData()
        {
            Context = new DatabaseContext();
        }

        public static void SaveAllData()
        {
            Context?.SaveData();
        }
    }
}
