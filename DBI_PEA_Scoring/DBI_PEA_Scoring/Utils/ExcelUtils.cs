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

        private static int LastRowOfResultSheet;
        public static void ExportResultsExcel(List<Result> results, double maxPoint, int numOfQuestion)
        {
            //Open Excel
            try
            {
                Application excelApp = new Application();
                object missing = Missing.Value;
                Workbook book = null;
                try
                {
                    excelApp.Visible = true;

                    //Open Workbook
                    book = excelApp.Workbooks.Add(missing);

                    //Add Detail Sheet
                    AddDetailSheet(book.Worksheets[1], results, maxPoint, numOfQuestion);

                    //Add Result Sheet
                    AddResultSheet(book.Worksheets.Add(missing, missing, missing, missing), results, maxPoint, numOfQuestion);

                    //Add Analyze Sheet
                    //AddResultSheet(book.Worksheets.Add(missing, missing, missing, missing), results, maxPoint, numOfQuestion);

                    //Saving file to location
                    
                }
                // Should get by type of exception but chua co thoi gian research and debug. -> 2.0
                catch (Exception e)
                {
                    throw new Exception("Export failed.\n " + e.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("You must install Office to export.\n" + ex.Message);
            }

        }

        private static void AddDetailSheet(Worksheet sheetResult, List<Result> results, double maxPoint, int numOfQuestion)
        {
            sheetResult.Name = "Detail";
            //Insert Title
            int lastRow = sheetResult.Cells.SpecialCells(XlCellType.xlCellTypeLastCell).Row;
            sheetResult.Cells[lastRow, 1] = "No";
            sheetResult.Cells[lastRow, 2] = "Paper No";
            sheetResult.Cells[lastRow, 3] = "Login";
            sheetResult.Cells[lastRow, 4] = "Details";
            //Insert Data
            foreach (var result in results)
            {
                lastRow++;
                sheetResult.Cells[lastRow, 1] = (lastRow - 1);
                sheetResult.Cells[lastRow, 2] = result.PaperNo;
                sheetResult.Cells[lastRow, 3] = result.StudentID;

                string note = "";
                for (int i = 0; i < result.Points.Length; i++)
                {
                    note = string.Concat(note, "[QN=", (i + 1), ", Mark=", result.Points[i], "] => ");

                    if (!string.IsNullOrEmpty(result.Logs[i]))
                    {
                        note = string.Concat(note, result.Logs[i], "");
                    }
                }
                sheetResult.Cells[lastRow, 4] = note.Substring(0, note.Length - 1);
            }
            //Fit columns
            sheetResult.Columns.AutoFit();
            LastRowOfResultSheet = lastRow;
        }

        private static void AddAnalyzeSheet(Worksheet sheetResult, List<Result> results, double maxPoint, int numOfQuestion)
        {
            
        }


        private static void AddResultSheet(Worksheet sheetResult, List<Result> results, double maxPoint, int numOfQuestion)
        {
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
            sheetResult.Cells[lastRow, numOfQuestion + 8] = "Details";

            //Insert Data
            foreach (var result in results)
            {
                lastRow++;
                sheetResult.Cells[lastRow, 1] = (lastRow - 1);
                sheetResult.Cells[lastRow, 2] = result.PaperNo;
                sheetResult.Cells[lastRow, 3] = result.StudentID;
                sheetResult.Cells[lastRow, 4] = result.ExamCode;
                double totalPoints = Math.Round(result.SumOfPoint(), 2);
                sheetResult.Cells[lastRow, 5] = totalPoints;
                sheetResult.Cells[lastRow, 6] = maxPoint;
                sheetResult.Cells[lastRow, 7].Formula = $"=E{lastRow}/F{lastRow}*10";
                for (int i = 8; i < 8 + numOfQuestion; i++)
                {
                    sheetResult.Cells[lastRow, i] = result.Points[i - 8];
                }
                string hyperlinkTargetAddress = "Detail!D" + lastRow;
                sheetResult.Cells[lastRow, numOfQuestion + 8] = "View Details";
                sheetResult.Hyperlinks.Add(sheetResult.Cells[lastRow, numOfQuestion + 8], "", hyperlinkTargetAddress, "View Details");
                //Fit columns
                sheetResult.Columns.AutoFit();
            }
        }
    }
}
