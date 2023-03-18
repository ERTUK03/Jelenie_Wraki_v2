using Microsoft.Maui.Controls.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace wraki
{
    public partial class Monitor : ContentPage
    {
        private static List<Tuple<int, int>> list;

        private static Label label;

        public Monitor(ref List<Tuple<int, int>> lista)
        {
            label = new()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Margin = 35,
                WidthRequest = 150,
                TextColor = Colors.White
            };

            list = lista;
            this.Content = new VerticalStackLayout
            {
                Children =
                {
                    label
                }
            };
            _ = set_label();
        }

        private static async Task set_label()
        {
            int n = 0;
            while (true)
            {
                await Task.Delay(1);
                while(n<list.Count)
                {
                    label.Text += list[n].Item1+" ";
                    n++;
                }
            }
        }
    }
}
