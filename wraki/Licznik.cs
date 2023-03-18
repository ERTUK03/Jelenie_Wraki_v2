using System.Diagnostics;

namespace wraki
{
    internal class Licznik
    {
        private static Stopwatch timer = new();

        private static List<List<Miejsce>> list;

        public class Miejsce
        {
            public int Id { get; set; }
            public int Punkty { get; set; }
            public int Seria { get; set; }
            public int Czas { get; set; }
        }

        private class Uczestnik
        {
            public int Seria { get; set; }
            public int Id_zawodnika { get; set; }
        }

        public static void Set_licznik(string wyscig, string port)
        {
            list = new();
            Excel_writer.Set_nazwa(wyscig);
            timer.Reset();
            Database.Execute("DELETE FROM runda");
            List<Uczestnik> lista = Database.conn.Query<Uczestnik>($"SELECT seria, id_zawodnika FROM starty WHERE nazwa_startu='{wyscig}'");

            foreach (var row in lista)
            {
                Database.Execute($"INSERT INTO runda(id, seria) VALUES({row.Id_zawodnika},{row.Seria})");
            }

            Views.start.Clicked += (sender, args) => Start();
            Views.stop.Clicked += (sender, args) => Stop();
            Views.koniec.Clicked += (sender, args) => Koniec();
            Views.zapisz.Clicked += (sender, args) => Zapisz();

            Serial_reader.Set_port(port, ref timer);

            _ = Synchro();
        }

        private static async Task Synchro()
        {
            while (true)
            {
                await Task.Delay(1);
                Views.label.Text = String.Format("{0:00}:{1:00}:{2:00}", timer.Elapsed.Minutes, timer.Elapsed.Seconds, timer.Elapsed.Milliseconds / 10);
            }
        }

        private static void Start()
        {
            timer.Start();
            Serial_reader.Start();
        }

        private static void Stop()
        {
            timer.Stop();
            Serial_reader.Stop();
        }

        private static void Koniec()
        {
            timer.Stop();
            Serial_reader.Stop();
            List<Tuple<int, int>> lista = Serial_reader.Get_list();

            foreach (var item in lista)
            {
                Database.Execute($"UPDATE runda SET czas={item.Item2}, punkty=punkty+1 WHERE id={item.Item1}");
            }

            int num = Database.conn.ExecuteScalar<int>("SELECT MAX(seria) FROM runda");

            for (int i = 1; i <= num; i++)
            {
                list.Add(Database.conn.Query<Miejsce>($"SELECT id, punkty, czas FROM runda WHERE seria={i} AND punkty>0 ORDER BY punkty DESC, czas ASC"));
            }
        }

        private static void Zapisz()
        {
            Excel_writer.Write(list);
        }
    }
}