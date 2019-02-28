using System;
using System.Collections.Generic;
using System.Reflection;
using DBI_PEA_Scoring.Model;
using Microsoft.Office.Interop.Excel;

namespace DBI_PEA_Scoring.Utils
{
    class ExcelUtils
    {
        public static void ExportResultsExcel(string path, List<Result> results)
        {
            //Open Excel
            Application excelApp = new Application();
            object missing = Missing.Value;
            try
            {
                excelApp.Visible = true;

                //Open Workbook
                Workbook book = excelApp.Workbooks.Add(missing);

                //Access Sheet
                //Worksheet sheetResult = book.Worksheets.Add(missing, missing, missing, missing);
                Worksheet sheetResult = book.Worksheets[1];
                sheetResult.Name = "Result";
                //Insert Title
                int lastRow = sheetResult.Cells.SpecialCells(XlCellType.xlCellTypeLastCell).Row;
                sheetResult.Cells[lastRow, 1] = "No";
                sheetResult.Cells[lastRow, 2] = "Paper No";
                sheetResult.Cells[lastRow, 3] = "Login";
                sheetResult.Cells[lastRow, 4] = "Test Name  ";
                sheetResult.Cells[lastRow, 5] = "Mark";
                sheetResult.Cells[lastRow, 6] = "Paper Mark";
                sheetResult.Cells[lastRow, 7] = "Mark(10)";
                sheetResult.Cells[lastRow, 8] = "Note";

                //Insert Data
                foreach (var result in results)
                {
                    lastRow++;
                    sheetResult.Cells[lastRow, 1] = (lastRow - 1);
                    sheetResult.Cells[lastRow, 2] = result.PaperNo;
                    sheetResult.Cells[lastRow, 3] = result.StudentID;
                    //sheetResult.Cells[lastRow, 4] = result.StudentID;
                    sheetResult.Cells[lastRow, 4] = "Bổ sung Exam Code sau";
                    double totalPoints = result.SumOfPoint();
                    sheetResult.Cells[lastRow, 5] = totalPoints;
                    //double maxPoint = 10.0;
                    sheetResult.Cells[lastRow, 6] = "Bổ sung Số điểm tối đa";
                    //sheetResult.Cells[lastRow, 7] = (totalPoints / maxPoint) * 10;
                    sheetResult.Cells[lastRow, 7] = "Bổ sung điểm chính xác";
                    string note = "";
                    for(int i = 0; i < result.Points.Length; i++)
                    {
                        if (!double.IsNaN(result.Points[i]))
                        {
                            note = string.Concat(note, "[QN=", (i + 1), ", Mark=", result.Points[i], "]");
                            if (!string.IsNullOrEmpty(result.Logs[i]))
                            {
                                note = string.Concat(note, "=>", result.Logs[i], "");
                            }
                        }
                        note = string.Concat(note, ";");
                    }
                    sheetResult.Cells[lastRow, 8] = note.Substring(0, note.Length - 1);

                    //Fit columns
                    sheetResult.Columns.AutoFit();
                    //sheetResult.Columns[1].ColumnWidth = excelApp.InchesToPoints(8.43);
                    //sheetResult.Columns[2].ColumnWidtth = excelApp.InchesToPoints(8.43);
                    //sheetResult.Columns[3].ColumnWidth = excelApp.InchesToPoints(27.86);
                    //sheetResult.Columns[4].ColumnWidth = excelApp.InchesToPoints(41.86);
                    //sheetResult.Columns[5].ColumnWidth = excelApp.InchesToPoints(8.43);
                    //sheetResult.Columns[6].ColumnWidth = excelApp.InchesToPoints(12.86);
                    //sheetResult.Columns[7].ColumnWidth = excelApp.InchesToPoints(8.43);
                    //sheetResult.Columns[8].ColumnWidth = excelApp.InchesToPoints(8.43);

                    //Saving file to location
                    sheetResult.SaveAs(path, XlFileFormat.xlAddIn8);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
