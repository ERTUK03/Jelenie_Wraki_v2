using Microsoft.Maui.Controls.Internals;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace wraki
{
    public partial class Pop_up_select : ContentPage
    {
        private Label label;
        private static Grid grid;

        public Pop_up_select(int min, ref Button button, int count)
        {
            List<int> list = Database.conn.QueryScalars<int>($"SELECT id FROM wyniki WHERE punkty={min}");



            label = new()
            {
                Text = $"Wybierz {count} uczestników",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Margin = 35,
                WidthRequest = 150,
                TextColor = Colors.White
            };

            grid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = GridLength.Star }
                },
                RowSpacing = 5,
                Padding = new Thickness(10),
                BackgroundColor=Colors.Black
            };

            grid.Padding = new Thickness(10);

            for (int i = 0; i < list.Count; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                var label = new Label { Text = $"{list[i]}", VerticalOptions = LayoutOptions.Center };
                var checkBox = new CheckBox { VerticalOptions = LayoutOptions.Center };

                grid.Add(checkBox, 0, i);
                grid.Add(label, 1, i);
            }

            this.Content = new VerticalStackLayout
            {
                Children =
                {
                    label,grid,button
                }
            };

            this.Content.BackgroundColor = Colors.Black;
        }

        public static List<int> get_items()
        {
            List<int> list = new();

            for (int row = 0; row < grid.RowDefinitions.Count; row++)
            {
                var checkBox = grid.Children.FirstOrDefault(c => grid.GetRow(c) == row && grid.GetColumn(c) == 0) as CheckBox;
                var label = grid.Children.FirstOrDefault(c => grid.GetRow(c) == row && grid.GetColumn(c) == 1) as Label;

                if (checkBox.IsChecked)
                {
                    list.Add(int.Parse(label.Text));
                    Debug.WriteLine(int.Parse(label.Text));
                }
            }

            return list;
        }
    }
}