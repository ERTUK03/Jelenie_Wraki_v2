using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using SQLite;

namespace wraki
{
    internal static class Database
    {
        public readonly static SQLiteConnection conn = new(Path.Combine("C:/Users/ertuk/Desktop", "database.db"));

        public static void Execute(string query)
        {
            conn.Execute(query);
        }

        public static void Prepare_database()
        {
            conn.Execute("CREATE TABLE IF NOT EXISTS wyniki(id INTEGER PRIMARY KEY, nazwa TEXT, punkty INT DEFAULT 0)");
            conn.Execute("CREATE TABLE IF NOT EXISTS starty(nazwa_startu  TEXT, seria INT, id_zawodnika INT)");
            conn.Execute("CREATE TABLE IF NOT EXISTS runda(id INT, punkty INT DEFAULT 0, seria INT, czas INT DEFAULT 0)");
        }
    }
}