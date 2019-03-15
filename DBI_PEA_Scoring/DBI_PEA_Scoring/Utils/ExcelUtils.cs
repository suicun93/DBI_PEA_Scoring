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
                try
                {
                    excelApp.Visible = true;

                    //Open Workbook
                    var book = excelApp.Workbooks.Add(missing);

                    //Add Result Sheet
                    AddResultSheet(book.Worksheets[1], results, maxPoint, numOfQuestion);

                    //Add Detail Sheet
                    AddDetailSheet(book.Sheets.Add(After: book.Sheets[book.Sheets.Count]), results, maxPoint, numOfQuestion);

                    //Add Analyze Sheet
                    AddDataAnalyzeSheet(book.Sheets.Add(After: book.Sheets[book.Sheets.Count]), results, maxPoint, numOfQuestion);

                    //Saving file to location

                    book.Sheets[1].Select(missing);
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

        private static void AddDetailSheet(Worksheet sheetDetail, List<Result> results, double maxPoint, int numOfQuestion)
        {
            sheetDetail.Name = "02_Detail";
            //Insert Title
            int lastRow = sheetDetail.Cells.SpecialCells(XlCellType.xlCellTypeLastCell).Row;
            sheetDetail.Cells[lastRow, 1] = "No";
            sheetDetail.Cells[lastRow, 2] = "Paper No";
            sheetDetail.Cells[lastRow, 3] = "Login";
            sheetDetail.Cells[lastRow, 4] = "Details";
            for (int i = 5; i < 5 + numOfQuestion; i++)
            {
                sheetDetail.Cells[lastRow, i] = string.Concat("Q", i - 4);
            }
            //Insert Data
            foreach (var result in results)
            {
                lastRow++;
                sheetDetail.Cells[lastRow, 1] = (lastRow - 1);
                sheetDetail.Cells[lastRow, 2] = result.PaperNo;
                sheetDetail.Cells[lastRow, 3] = result.StudentID;

                string comment = "";
                for (int i = 0; i < result.ListCandidates.Count; i++)
                {
                    comment = string.Concat(comment, "[QN=", (i + 1), ", Mark=", result.Points[i], "] => ");

                    if (!string.IsNullOrEmpty(result.Logs[i]))
                    {
                        comment = string.Concat(comment, result.Logs[i], "\n");
                    }
                    sheetDetail.Cells[lastRow, (i + 5)] = string.Concat(result.ListCandidates.ElementAt(i).QuestionRequirement,
                        "\nAnswer:\n",
                        result.ListAnswers.ElementAt(i));
                }
                sheetDetail.Cells[lastRow, 4] = comment.Trim();
            }
            //Fit columns
            sheetDetail.Columns.AutoFit();
            sheetDetail.Range["A:G"].VerticalAlignment = XlVAlign.xlVAlignTop;
            sheetDetail.Range["D:X"].ColumnWidth = 60;
        }

        private static void AddDataAnalyzeSheet(Worksheet sheetDataAnalyze, List<Result> results, double maxPoint, int numOfQuestion)
        {
            sheetDataAnalyze.Name = "03_DataAnalyze";
            //Insert Title
            int lastRow = sheetDataAnalyze.Cells.SpecialCells(XlCellType.xlCellTypeLastCell).Row;
            sheetDataAnalyze.Cells[lastRow, 1] = "Mark";
            sheetDataAnalyze.Cells[lastRow, 2] = "Number";
            for (int i = 0; i < 10; i++)
            {
                sheetDataAnalyze.Cells[lastRow, 1] = string.Concat(i, "-", i + 1);
               // sheetDataAnalyze.Cells[lastRow, 2] = "=COUNTIFS(Result!G:G;\">" + i + "\";Result!G:G;\"<=" + (i + 1) + "\")";
            }


        }


        private static void AddResultSheet(Worksheet sheetResult, List<Result> results, double maxPoint, int numOfQuestion)
        {
            sheetResult.Name = "01_Result";
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
            sheetResult.Cells[lastRow, numOfQuestion + 8] = "Log Details";

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
                    string hyper = "02_Detail!" + (char)(61 + i) + lastRow + "";
                    sheetResult.Cells[lastRow, i] = result.Points[i - 8];
                    sheetResult.Hyperlinks.Add(sheetResult.Cells[lastRow, i], "", hyper);
                }
                sheetResult.Cells[lastRow, numOfQuestion + 8] = "View Details";
                sheetResult.Hyperlinks.Add(sheetResult.Cells[lastRow, numOfQuestion + 8], "", "02_Detail!D" + lastRow + "", "View Details");
            }
            //Fit columns
            sheetResult.Columns.AutoFit();
            sheetResult.Range["A:G"].VerticalAlignment = XlVAlign.xlVAlignTop;
            LastRowOfResultSheet = lastRow;
        }
    }
}
