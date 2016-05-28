using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace Lampe
{
    class db
    {
        public static String[] Blacklist = {"chrome", "Hearthstone", "WoW", "game", "Battle.net", "firefox", "MicrosoftEdge", "Diablo III"};

        public static Boolean Lookup(String proc)
        {
            foreach (string x in Blacklist)
            {
                string Proc = proc;
                if (x.Contains(Proc))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
