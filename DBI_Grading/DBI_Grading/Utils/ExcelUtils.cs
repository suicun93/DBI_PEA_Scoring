using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DBI_Grading.Model;
using DBI_Grading.Model.Student;
using Microsoft.Office.Interop.Excel;

namespace DBI_Grading.Utils
{
    internal class ExcelUtils
    {
        private static WorksheetFunction _wsf;
        public static int LastRowOfResultSheet { get; private set; }

        public static void ExportResultsExcel(List<Result> results, List<Submission> submissions, double maxPoint, int numOfQuestion)
        {
            //Open Excel
            try
            {
                var excelApp = new Application();
                object missing = Missing.Value;
                try
                {
                    excelApp.Visible = true;
                    excelApp.EnableAnimations = false;
                    //Open Workbook
                    var book = excelApp.Workbooks.Add(missing);
                    _wsf = excelApp.WorksheetFunction;

                    //Add Result Sheet
                    AddResultSheet(book.Worksheets[1], results, maxPoint, numOfQuestion);

                    //Add Detail Sheet
                    AddDetailSheet(book.Sheets.Add(After: book.Sheets[book.Sheets.Count]), results, numOfQuestion);

                    //Add Answer Sheet
                    AddAnswerPathSheet(book.Sheets.Add(After: book.Sheets[book.Sheets.Count]), submissions,
                        numOfQuestion);

                    //Add Analyze Sheet
                    AddDataAnalyzeSheet(book.Sheets.Add(After: book.Sheets[book.Sheets.Count]), book.Worksheets[1]);

                    //Saving file to location

                    book.Sheets[1].Select(missing);

                    excelApp.EnableAnimations = true;
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

        private static void AddAnswerPathSheet(Worksheet sheetAnswerPath, List<Submission> submissions, int numOfQuestion)
        {
            sheetAnswerPath.Name = "03_AnswerPath";
            //Insert Title
            var lastRow = sheetAnswerPath.Cells.SpecialCells(XlCellType.xlCellTypeLastCell).Row;
            sheetAnswerPath.Cells[lastRow, 1] = "No";
            sheetAnswerPath.Cells[lastRow, 2] = "Paper No";
            sheetAnswerPath.Cells[lastRow, 3] = "Login";
            for (var i = 4; i < 4 + numOfQuestion; i++)
                sheetAnswerPath.Cells[lastRow, i] = string.Concat("Q", i - 3);
            //Insert Data
            for (int i = 0; i < submissions.Count; i++)
            {
                Submission submission = submissions[i];
                lastRow++;
                sheetAnswerPath.Cells[lastRow, 1] = lastRow - 1;
                sheetAnswerPath.Cells[lastRow, 2] = submission.PaperNo;
                sheetAnswerPath.Cells[lastRow, 3] = submission.StudentID;
                for (var j = 0; j < submission.AnswerPaths.Count; j++)
                {
                    sheetAnswerPath.Cells[lastRow, j + 4] = submission.AnswerPaths[j];
                }
                int countEmpty = 0;
                while (countEmpty < submission.AnswerPaths.Count && submission.AnswerPaths[countEmpty].Equals("Cannot found"))
                {
                    countEmpty++;
                    if (countEmpty >= submission.AnswerPaths.Count)
                    {
                        sheetAnswerPath.Rows[lastRow].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Yellow);
                    }
                }
            }
            //Fit columns
            sheetAnswerPath.Columns.AutoFit();
            sheetAnswerPath.Range["A:X"].VerticalAlignment = XlVAlign.xlVAlignTop;
            sheetAnswerPath.Range["D:X"].ColumnWidth = 30;
            sheetAnswerPath.Range["A:X"].RowHeight = 20;
        }

        private static void AddDetailSheet(Worksheet sheetDetail, List<Result> results,
            int numOfQuestion)
        {
            sheetDetail.Name = "02_Detail";
            //Insert Title
            var lastRow = sheetDetail.Cells.SpecialCells(XlCellType.xlCellTypeLastCell).Row;
            sheetDetail.Cells[lastRow, 1] = "No";
            sheetDetail.Cells[lastRow, 2] = "Paper No";
            sheetDetail.Cells[lastRow, 3] = "Login";
            sheetDetail.Cells[lastRow, 4] = "Details";
            for (var i = 5; i < 5 + numOfQuestion; i++)
                sheetDetail.Cells[lastRow, i] = string.Concat("Q", i - 4);
            //Insert Data
            foreach (var result in results)
            {
                lastRow++;
                sheetDetail.Cells[lastRow, 1] = lastRow - 1;
                sheetDetail.Cells[lastRow, 2] = result.PaperNo;
                sheetDetail.Cells[lastRow, 3] = result.StudentID;

                var comment = "";
                for (var i = 0; i < result.ListCandidates.Count; i++)
                {
                    comment = string.Concat(comment, "[QN=", i + 1, ", Mark=", result.Points[i], "] => ");

                    if (!string.IsNullOrEmpty(result.Logs[i]))
                        comment = string.Concat(comment, result.Logs[i], "\n");
                    if (!string.IsNullOrEmpty(result.ListAnswers.ElementAt(i).Trim()))
                        sheetDetail.Cells[lastRow, i + 5] = string.Concat(
                            result.ListCandidates.ElementAt(i).QuestionRequirement,
                            "\nAnswer:\n", result.ListAnswers.ElementAt(i));
                    else
                        sheetDetail.Cells[lastRow, i + 5] = "Empty Answer";
                }
                sheetDetail.Cells[lastRow, 4] = comment.Trim();
            }
            //Fit columns
            sheetDetail.Columns.AutoFit();
            sheetDetail.Range["A:X"].VerticalAlignment = XlVAlign.xlVAlignTop;
            sheetDetail.Range["D:X"].ColumnWidth = 30;
            sheetDetail.Range["A:X"].RowHeight = 20;
        }

        private static void AddDataAnalyzeSheet(Worksheet sheetDataAnalyze, Worksheet sheetResult)
        {
            //Insert Title
            sheetDataAnalyze.Name = "04_DataAnalyze";

            //Add 

            var lastRow = sheetDataAnalyze.Cells.SpecialCells(XlCellType.xlCellTypeLastCell).Row;
            sheetDataAnalyze.Cells[lastRow, 1] = "Mark";
            sheetDataAnalyze.Cells[lastRow, 2] = "Amount";
            //Counting
            var scoreRange = sheetResult.Range["G:G"];
            for (var i = 0; i < 11; i++)
            {
                lastRow++;
                sheetDataAnalyze.Cells[lastRow, 1] = string.Concat("'>=", i);
                sheetDataAnalyze.Cells[lastRow, 2] = _wsf.CountIfs(scoreRange, string.Concat(">=", i), scoreRange,
                    string.Concat("<", i + 1));
            }
            //Add Score Line Chart
            AddChart(sheetDataAnalyze, sheetDataAnalyze.Range["A1", "B12"], 200, 15, "Score Line", "Amount", "Score");
            //Add Paper Score
        }

        private static void AddChart(Worksheet worksheet, Range range, double left, double top, string title,
            string categoryTitle, string valueTitle)
        {
            // Add chart.
            if (worksheet.ChartObjects() is ChartObjects charts)
            {
                var chartObject = charts.Add(left, top, 300, 300);
                var chart = chartObject.Chart;

                // Set chart range.
                chart.SetSourceData(range);

                // Set chart properties.
                chart.ChartType = XlChartType.xlLine;
                chart.ChartWizard(range,
                    Title: title,
                    CategoryTitle: categoryTitle,
                    ValueTitle: valueTitle);
            }
        }


        private static void AddResultSheet(Worksheet sheetResult, List<Result> results, double maxPoint,
            int numOfQuestion)
        {
            sheetResult.Name = "01_Result";
            //Insert Title
            var lastRow = sheetResult.Cells.SpecialCells(XlCellType.xlCellTypeLastCell).Row;
            sheetResult.Cells[lastRow, 1] = "No";
            sheetResult.Cells[lastRow, 2] = "Paper No";
            sheetResult.Cells[lastRow, 3] = "Login";
            sheetResult.Cells[lastRow, 4] = "Test Name  ";
            sheetResult.Cells[lastRow, 5] = "Mark";
            sheetResult.Cells[lastRow, 6] = "Paper Mark";
            sheetResult.Cells[lastRow, 7] = "Mark(10)";
            for (var i = 8; i < 8 + numOfQuestion; i++)
                sheetResult.Cells[lastRow, i] = string.Concat("Q", i - 7);
            sheetResult.Cells[lastRow, numOfQuestion + 8] = "Log Details";
            sheetResult.Cells[lastRow, numOfQuestion + 9] = "Answer Path";
            sheetResult.Cells[lastRow, numOfQuestion + 10] = "Note";
            //Insert Data
            foreach (var result in results)
            {
                lastRow++;
                sheetResult.Cells[lastRow, 1] = lastRow - 1;
                sheetResult.Cells[lastRow, 2] = result.PaperNo;
                sheetResult.Cells[lastRow, 3] = result.StudentID;
                sheetResult.Cells[lastRow, 4] = result.ExamCode;
                var totalPoints = Math.Round(result.SumOfPoint(), 2);
                sheetResult.Cells[lastRow, 5] = totalPoints;
                sheetResult.Cells[lastRow, 6] = maxPoint;
                sheetResult.Cells[lastRow, 7].Formula = $"=E{lastRow}/F{lastRow}*10";
                for (var i = 8; i < 8 + numOfQuestion; i++)
                {
                    var hyper = "02_Detail!" + (char)(61 + i) + lastRow + "";
                    sheetResult.Cells[lastRow, i] = result.Points[i - 8];
                    sheetResult.Hyperlinks.Add(sheetResult.Cells[lastRow, i], "", hyper);
                    if (result.ListAnswers[i - 8].ToLower().Contains("go\n") || result.ListAnswers[i - 8].ToLower().Contains("\ngo"))
                    {
                        sheetResult.Cells[lastRow, i].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.PaleVioletRed);
                    }
                }
                sheetResult.Cells[lastRow, numOfQuestion + 8] = "View Details";
                sheetResult.Hyperlinks.Add(sheetResult.Cells[lastRow, numOfQuestion + 8], "",
                    "02_Detail!D" + lastRow + "", "View Details");
                sheetResult.Cells[lastRow, numOfQuestion + 9] = "View AnswerPath";
                sheetResult.Hyperlinks.Add(sheetResult.Cells[lastRow, numOfQuestion + 9], "",
                    "03_AnswerPath!C" + lastRow + "", "View AnswerPath");
                int countEmpty = 0;
                while (countEmpty < result.ListAnswers.Count && result.ListAnswers[countEmpty].Equals(""))
                {
                    countEmpty++;
                    if (countEmpty >= result.ListAnswers.Count)
                    {
                        sheetResult.Rows[lastRow].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Yellow);
                        sheetResult.Cells[lastRow, numOfQuestion + 10] = "Cannot found any answer";
                    }
                }
            }
            //Fit columns
            sheetResult.Columns.AutoFit();
            //sheetResult.Range["A:G"].VerticalAlignment = XlVAlign.xlVAlignTop;
            LastRowOfResultSheet = lastRow;
        }
    }
}