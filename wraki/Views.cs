using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wraki
{
    internal static class Views
    {
        public static Label label;
        public static Button start;
        public static Button stop;
        public static Button koniec;
        public static Button zapisz;

        public static Picker picker;
        public static Picker ports;
        public static Button button;
        public static Button wyjdz;

        public static void Refresh()
        {
            picker = new()
            {
                Title = "Wybierz serię i bieg",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Margin = 50,
                WidthRequest = 150
            };

            ports = new()
            {
                Title = "Port",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Margin = 50,
                WidthRequest = 150
            };

            button = new()
            {
                Text = "Start",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Margin = 50,
                WidthRequest = 150
            };

            wyjdz = new()
            {
                Text = "Wyjdź",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Margin = 35,
                WidthRequest = 150
            };

            label = new()
            {
                Text = "Czas",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Margin = 35,
                WidthRequest = 150
            };

            start = new()
            {
                Text = "Start",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Margin = 35,
                WidthRequest = 150
            };

            stop = new()
            {
                Text = "Stop",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Margin = 35,
                WidthRequest = 150
            };

            koniec = new()
            {
                Text = "Koniec",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Margin = 35,
                WidthRequest = 150
            };

            zapisz = new()
            {
                Text = "Zapisz",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Margin = 35,
                WidthRequest = 150
            };
        }
    }
}
