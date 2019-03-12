using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DBI_PEA_Scoring.Model;
using Microsoft.Office.Interop.Excel;

namespace DBI_PEA_Scoring.Utils
{
    class ExcelUtils
    {
        public static void ExportResultsExcel(string path, List<Result> results, double maxPoint, int numOfQuestion)
        {
            //Open Excel
            try
            {
                Application excelApp = new Application();
                object missing = Missing.Value;
                try
                {
                    excelApp.Visible = false;

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
                    for (int i = 8; i < 8 + numOfQuestion; i++)
                    {
                        sheetResult.Cells[lastRow, i] = string.Concat("Q", i - 7);
                    }


                    //Insert Data
                    foreach (var result in results)
                    {
                        lastRow++;
                        sheetResult.Cells[lastRow, 1] = (lastRow - 1);
                        sheetResult.Cells[lastRow, 2] = result.PaperNo;
                        sheetResult.Cells[lastRow, 3] = result.StudentID;
                        sheetResult.Cells[lastRow, 4] = result.ExamCode;
                        double totalPoints = result.SumOfPoint();
                        sheetResult.Cells[lastRow, 5] = totalPoints;
                        sheetResult.Cells[lastRow, 6] = maxPoint;
                        sheetResult.Cells[lastRow, 7].Formula = "Sum(E2;f2)";
                        for (int i = 8; i < 8 + numOfQuestion; i++)
                        {
                            sheetResult.Cells[lastRow, i] = result.Points[i - 8];
                        }
                        ;
                        //string note = "";
                        //for (int i = 0; i < result.Points.Length; i++)
                        //{
                        //    if (!double.IsNaN(result.Points[i]))
                        //    {
                        //        note = string.Concat(note, "[QN=", (i + 1), ", Mark=", result.Points[i], "]");
                        //        if (!string.IsNullOrEmpty(result.Logs[i]))
                        //        {
                        //            note = string.Concat(note, "=>", result.Logs[i], "");
                        //        }
                        //    }
                        //    note = string.Concat(note, ";");
                        //}
                        //sheetResult.Cells[lastRow, 8] = note.Substring(0, note.Length - 1);

                        //Fit columns
                        sheetResult.Columns.AutoFit();
                    }
                    //Saving file to location
                    sheetResult.SaveAs(path, XlFileFormat.xlAddIn8);
                    excelApp.Visible = true;
                }
                // Should get by type of exception but chua co thoi gian research and debug. -> 2.0
                catch (Exception e)
                {
                    excelApp.Quit();
                    throw new Exception("Export failed.\n " + e.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("You must install Office to export.\n" + ex.Message);
            }

        }
    }
}
