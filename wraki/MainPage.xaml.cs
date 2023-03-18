using SQLite;
using System.IO.Ports;
using System.Diagnostics;

namespace wraki;

public partial class MainPage : ContentPage
{
    private int min;

    private class Wyscigi
    {
        public string Nazwa_startu { get; set; }
    }

    public MainPage()
    {
        Database.Prepare_database();

        InitializeComponent();

        Load_main();
    }

    private void Load_main()
    {
        Views.Refresh();
        Views.button.Clicked += (sender, args) => Load_start();
        Views.wyjdz.Clicked += (sender, args) => Wyjdz();
        Refresh();

        this.Content = new VerticalStackLayout
        {
            Children =
            {
                Views.picker,Views.ports,Views.button
            }
        };
    }

    private void Load_start()
    {
        if (Views.picker.SelectedIndex == -1 || Views.ports.SelectedIndex == -1) return;

        Licznik.Set_licznik(Views.picker.SelectedItem.ToString(), Views.ports.SelectedItem.ToString());

        this.Content = new HorizontalStackLayout
        {
            Children =
            {
                Views.label, Views.start, Views.stop, Views.koniec, Views.zapisz, Views.wyjdz
            }
        };
    }

    private static void Refresh()
    {
        Views.picker.Items.Clear();
        Views.ports.Items.Clear();
        string[] prts = SerialPort.GetPortNames();

        foreach (string port in prts)
        {
            Views.ports.Items.Add(port);
        }
        List<Wyscigi> lista = Database.conn.Query<Wyscigi>("SELECT DISTINCT nazwa_startu FROM starty ORDER BY nazwa_startu");
        foreach (var row in lista)
        {
            Views.picker.Items.Add(row.Nazwa_startu);
        }
    }

    private static void Refresh_start()
    {
        Views.picker.Items.Clear();

        List<Wyscigi> lista = Database.conn.Query<Wyscigi>("SELECT DISTINCT nazwa_startu FROM starty ORDER BY nazwa_startu");
        foreach (var row in lista)
        {
            Views.picker.Items.Add(row.Nazwa_startu);
        }
    }

    private async void Wczytaj_zalogi(object sender, EventArgs e)
    {
        var result = await FilePicker.Default.PickAsync();
        if (result is not null) Excel_reader.Wczytaj_liste(result.FullPath);
    }

    private async void Wczytaj_starty(object sender, EventArgs e)
    {
        var result = await FilePicker.Default.PickAsync();
        if (result is not null) Excel_reader.Wczytaj_starty(result.FullPath);
        Load_main();
    }

    private void Wyjdz()
    {
        Load_main();
    }

    private void Zobacz_zalogi(object sender, EventArgs e)
    {

    }

    private void Zobacz_starty(object sender, EventArgs e)
    {

    }

    private void Zobacz_punktacje(object sender, EventArgs e)
    {

    }

    private async void Polfinal(object sender, EventArgs e)
    {
        int zawodnicy = Int32.Parse(await DisplayPromptAsync("Stwórz półfinał", "Wpisz ilość zawodników", keyboard: Keyboard.Numeric));
        List<int> id = Database.conn.QueryScalars<int>($"SELECT id FROM wyniki WHERE id<100 ORDER BY punkty DESC LIMIT {zawodnicy}");
        Database.Execute("DELETE FROM starty WHERE nazwa_startu LIKE 'Półfinał S%'");

        int biegi;
        int serie = 1;

        if (zawodnicy == 20)
        {
            biegi = 4;
            serie = 2;
        }
        else if (zawodnicy == 15) biegi = 3;
        else if (zawodnicy == 12) biegi = 3;
        else if (zawodnicy == 10) biegi = 2;
        else return;

        int idn = 0;

        if (serie == 2)
        {
            for (int i = biegi; i > 0; i--)
            {
                for (int j = biegi - i; j < id.Count; j += biegi)
                {
                    int n = (i > 2) ? 2 : 1;
                    int m = (i == 4) ? 2 : (i == 3) ? 1 : (i == 2) ? 2 : 1;
                    Database.Execute($"INSERT INTO starty(nazwa_startu, seria, id_zawodnika) VALUES('Półfinał Seria: 1 Bieg: {n}',{m},{id[j]})");
                    idn = id[j];
                }
            }
        }
        else
        {
            for (int i = biegi; i > 0; i--)
            {
                for (int j = biegi - i; j < id.Count; j += biegi)
                {
                    Database.Execute($"INSERT INTO starty(nazwa_startu, seria, id_zawodnika) VALUES('Półfinał Seria: 1 Bieg: 1',{i},{id[j]})");
                    idn = id[j];
                }
            }
        }

        min = Database.conn.ExecuteScalar<int>($"SELECT punkty FROM wyniki WHERE id={idn}");

        Refresh_start();
    }

    private void Polfinal_conf(object sender, EventArgs e)
    {
        Button button = new()
        {
            Text = "Zapisz",
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            Margin = 50,
            WidthRequest = 150
        };

        List<int> present = Database.conn.QueryScalars<int>("SELECT id_zawodnika FROM starty WHERE nazwa_startu LIKE 'Półfinał S%'");
        List<int> pkt = Database.conn.QueryScalars<int>($"SELECT id FROM wyniki WHERE punkty={min} AND id<100");

        IEnumerable<int> list = present.Intersect(pkt);

        Window sc = new(new Pop_up_select(min, ref button, list.Count()));
        Application.Current.OpenWindow(sc);

        button.Clicked += (sender, args) =>
        {
            List<int> next = Pop_up_select.get_items();

            for (int i = 0; i < list.Count(); i++)
            {
                Database.Execute($"UPDATE starty SET id_zawodnika = {next[i]} WHERE id_zawodnika={list.ElementAt(i)} AND nazwa_startu LIKE 'P%'");
            }

            Application.Current.CloseWindow(sc);
        };
    }

    private void Final(object sender, EventArgs e)
    {
        Database.Execute("DELETE FROM starty WHERE nazwa_startu='Finał'");

        List<string> list;

        int n = Database.conn.ExecuteScalar<int>("SELECT COUNT(seria) FROM starty WHERE nazwa_startu LIKE 'Półfinał S%'");

        int m;

        if (n == 20 || n == 15 || n == 12) m = 2;
        else m = 3;

        list = Database.conn.QueryScalars<string>($"SELECT name FROM sqlite_schema WHERE name LIKE('PółfinałS%') AND type='table'");

        foreach(var x in list)
        {
            List<int> nums = Database.conn.QueryScalars<int>($"SELECT numer FROM {x} WHERE miejsce < {m + 1}");
            foreach(var y in nums)
            {
                Database.Execute($"INSERT INTO starty(nazwa_startu, seria, id_zawodnika) VALUES('Finał',1,{y})");
            }
        }

        Refresh_start();
    }
}

