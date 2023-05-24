using OfficeOpenXml;
using System.Diagnostics;

namespace wraki
{
    internal static class Excel_writer
    {
        private static string nazwa_startu;

        public static void Set_nazwa(string str)
        {
            nazwa_startu = str;
        }

        public static void Write(List<List<Licznik.Miejsce>> lista)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using ExcelPackage excelPackage = new();

            Database.Execute($"CREATE TABLE IF NOT EXISTS {nazwa_startu.Replace(" ", String.Empty).Replace(":", String.Empty)}(miejsce int,numer int)");
            Database.Execute($"DELETE FROM {nazwa_startu.Replace(" ", String.Empty).Replace(":", String.Empty)}");

            for (int i = 0; i < lista.Count; i++)
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Seria " + (i + 1));

                worksheet.Cells[1, 1].Value = "Miejsce";
                worksheet.Cells[1, 2].Value = "Numer załogi";
                worksheet.Cells[1, 3].Value = "Punkty";
                worksheet.Cells[1, 4].Value = "Ilość okrązeń";
                worksheet.Cells[1, 5].Value = "Czas";

                for (int j = 0; j < lista[i].Count; j++)
                {
                    Database.Execute($"INSERT INTO {nazwa_startu.Replace(" ", String.Empty).Replace(":", String.Empty)}(miejsce,numer) VALUES({j + 1},{lista[i][j].Id})");

                    worksheet.Cells[j + 2, 1].Value = $"{j + 1}.";
                    worksheet.Cells[j + 2, 2].Value = lista[i][j].Id;
                    worksheet.Cells[j + 2, 3].Value = 5 - j;
                    Database.Execute($"UPDATE wyniki SET punkty=punkty+{5-j} WHERE id={lista[i][j].Id}");
                    worksheet.Cells[j + 2, 4].Value = lista[i][j].Punkty-1;
                    worksheet.Cells[j + 2, 5].Value = Parse_time(lista[i][j].Czas);

                }
            }
            byte[] bin = excelPackage.GetAsByteArray();
            string FileName = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}/{nazwa_startu.Replace(":", String.Empty)} {DateTime.Today:d}.xlsx";

            File.WriteAllBytes(FileName, bin);
        }

        private static string Parse_time(int ms)
        {
            TimeSpan t = TimeSpan.FromMilliseconds(ms);
            return string.Format("{0:D2}m:{1:D2}s:{2:D3}ms",t.Minutes,t.Seconds,t.Milliseconds);
        }
    }
}
