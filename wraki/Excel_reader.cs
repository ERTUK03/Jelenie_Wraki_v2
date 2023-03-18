using OfficeOpenXml;

namespace wraki
{
    internal static class Excel_reader
    {
        private static int serie_n;
        private static int serie_p;

        public static void Wczytaj_liste(string file)
        {
            try
            {
                byte[] bin = File.ReadAllBytes(file);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using MemoryStream stream = new(bin);
                using ExcelPackage excelPackage = new(stream);

                Database.Execute("DELETE FROM wyniki");

                foreach (ExcelWorksheet worksheet in excelPackage.Workbook.Worksheets)
                {
                    for (int i = worksheet.Dimension.Start.Row + 2; i <= worksheet.Dimension.End.Row; i++)
                    {
                        if (worksheet.Cells[i, worksheet.Dimension.Start.Column + 1].Value != null)
                        {
                            if (worksheet.Cells[i, worksheet.Dimension.Start.Column + 1].Value.ToString() != "PUSTE")
                            {
                                Database.Execute($"INSERT INTO wyniki(id, nazwa) VALUES({worksheet.Cells[i, worksheet.Dimension.Start.Column].Value},'{worksheet.Cells[i, worksheet.Dimension.Start.Column + 1].Value}')");
                            }
                        }
                    }
                }
            }
            catch
            {

            }
        }

        public static void Wczytaj_starty(string file)
        {
            try
            {
                List<List<Tuple<string,int>>> lista = new();
                List<List<Tuple<string,int>>> lista_pro = new();

                serie_n = 0;
                serie_p = 0;
                int maksy = 0;
                int maksx = 0;
                int curr = 0;
                bool isPro = false;

                int serie = 0;

                byte[] bin = File.ReadAllBytes(file);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using MemoryStream stream = new(bin);
                using ExcelPackage excelPackage = new(stream);
                foreach (ExcelWorksheet worksheet in excelPackage.Workbook.Worksheets)
                {
                    for (int i = worksheet.Dimension.Start.Row + 3; i <= worksheet.Dimension.End.Row; i++)
                    {
                        for (int j = worksheet.Dimension.Start.Column + 3; j <= worksheet.Dimension.End.Column; j++)
                        {
                            if (worksheet.Cells[i, j].Value != null)
                            {
                                List<Tuple<string,int>> temp = new();
                                if (worksheet.Cells[i, j].Value.ToString() == "1 Eliminacja PRO")
                                {
                                    i += 3;
                                    isPro = true;
                                }

                                for (int m = i; m <= worksheet.Dimension.End.Row; m++)
                                {
                                    for (int n = j; n <= worksheet.Dimension.End.Column; n++)
                                    {
                                        curr = n - j;
                                        if (worksheet.Cells[m, n].Value != null)
                                        {
                                            temp.Add(new Tuple<string,int>(worksheet.Cells[m, n].Value.ToString(),m-i+1));
                                            maksy = n - j;
                                            maksx = m - i;
                                        }
                                        else break;
                                    }
                                    if (curr == 0)
                                    {
                                        j += maksy + 1;
                                        maksy = 0;
                                        break;
                                    }
                                }
                                serie++;
                                if (!isPro) lista.Add(temp);
                                else lista_pro.Add(temp);
                            }
                        }
                        if (!isPro) serie_n = serie == 0 ? serie_n : serie;
                        else serie_p = serie == 0 ? serie_p : serie;
                        serie = 0;
                        i += maksx + 1;
                        maksx = -1;
                    }

                    Database.Execute("DELETE FROM starty");

                    int biegi = 1;
                    for (int i = 0; i < lista.Count; i++)
                    {
                        for (int j = 0; j < lista[i].Count; j++)
                        {
                            Database.Execute($"INSERT INTO starty(nazwa_startu, seria, id_zawodnika) VALUES('{"Seria: " + ((i % serie_n) + 1) + " Bieg: " + biegi}',{lista[i][j].Item2},{lista[i][j].Item1})");
                        }
                        if ((i + 1) % serie_n == 0) biegi++;
                    }
                    biegi = 1;
                    for (int i = 0; i < lista_pro.Count; i++)
                    {
                        for (int j = 0; j < lista_pro[i].Count; j++)
                        {
                            Database.Execute($"INSERT INTO starty(nazwa_startu, seria, id_zawodnika) VALUES('{"Seria PRO: " + ((i % serie_p) + 1) + " Bieg: " + biegi}',{lista_pro[i][j].Item2},{lista_pro[i][j].Item1})");
                        }
                        if ((i + 1) % serie_p == 0) biegi++;
                    }
                }
            }
            catch
            {

            }
        }
    }
}