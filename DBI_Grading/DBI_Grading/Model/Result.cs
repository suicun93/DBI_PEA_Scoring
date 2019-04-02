﻿using System;
using System.Collections.Generic;
using System.Linq;
using DBI_Grading.Common;
using DBI_Grading.Model.Teacher;
using DBI_Grading.Utils;

namespace DBI_Grading.Model
{
    public class Result
    {
        private double MaxPoint;

        public Result()
        {
            Points = new double[Constant.PaperSet.QuestionSet.QuestionList.Count];
            ListAnswers = new List<string>();
            ListCandidates = new List<Candidate>();
            Logs = new string[Constant.PaperSet.QuestionSet.QuestionList.Count];
        }

        public string StudentID { get; set; }
        public string PaperNo { get; set; }
        public string ExamCode { get; set; }
        public List<string> ListAnswers { get; set; }
        public List<Candidate> ListCandidates { get; set; }
        public double[] Points { get; set; }
        public string[] Logs { get; set; }

        /// <summary>
        ///     Get Sum of point
        /// </summary>
        /// <returns> Sum of point(double)</returns>
        public double SumOfPoint()
        {
            double sum = 0;
            foreach (var point in Points)
                sum += point;
            sum = Math.Round(sum, 2);
            if (sum >= MaxPoint) sum = MaxPoint;
            return sum;
        }

        /// <summary>
        ///     Count Student's point by question and answer
        /// </summary>
        /// <param name="candidate">Question</param>
        /// <param name="answer">Student's answer</param>
        /// <param name="questionOrder"></param>
        /// <returns>
        ///     True if correct
        ///     False if incorrect.
        /// </returns>
        /// <exception>
        ///     if exception was found, throw it for GetPoint function to handle
        ///     <cref>SQLException</cref>
        /// </exception>
        private Dictionary<string, string> GradeAnswer(Candidate candidate, string answer, int questionOrder)
        {
            // await TaskEx.Delay(100);
            if (string.IsNullOrEmpty(answer.Trim()))
                throw new Exception("Empty.");
            // Process by Question Type
            switch (candidate.QuestionType)
            {
                case Candidate.QuestionTypes.Schema:
                    // Schema Question
                    return PaperUtils.SchemaType(candidate, StudentID, answer, questionOrder);
                case Candidate.QuestionTypes.Select:
                    //Select Question
                    return PaperUtils.SelectType(candidate, StudentID, answer, questionOrder);
                case Candidate.QuestionTypes.DML:
                    // DML: Insert/Delete/Update Question
                    return PaperUtils.TriggerProcedureType(candidate, StudentID, answer, questionOrder);
                case Candidate.QuestionTypes.Procedure:
                    // Procedure Question
                    return PaperUtils.TriggerProcedureType(candidate, StudentID, answer, questionOrder);
                case Candidate.QuestionTypes.Trigger:
                    // Trigger Question
                    return PaperUtils.TriggerProcedureType(candidate, StudentID, answer, questionOrder);
                default:
                    // Not supported yet
                    throw new Exception("This question type has not been supported yet.");
            }
        }

        /// <summary>
        ///     Get GradeAnswer function
        /// </summary>
        public void GetPoint()
        {
            foreach (var candidate in ListCandidates)
                MaxPoint += candidate.Point;
            if (MaxPoint > 10)
                MaxPoint = Math.Floor(MaxPoint);
            else
                MaxPoint = Math.Ceiling(MaxPoint);
            // Count number of candidate
            var numberOfQuestion = ListCandidates.Count;
            // Wrong PaperNo
            if (numberOfQuestion == 0)
            {
                Logs[0] = "Wrong Paper No\n";
                for (var i = 0; i < Constant.PaperSet.QuestionSet.QuestionList.Count; i++)
                    Points[i] = 0;
                return;
            }
            // Get mark one by one
            for (var questionOrder = 0; questionOrder < ListCandidates.Count; questionOrder++)
                try
                {
                    if (numberOfQuestion > questionOrder)
                    {
                        var res = GradeAnswer(ListCandidates.ElementAt(questionOrder),
                            ListAnswers.ElementAt(questionOrder), questionOrder);
                        //Exactly -> Log true and return 0 point
                        if (res != null)
                        {
                            Points[questionOrder] = Math.Round(double.Parse(res["Point"]), 4);
                            Logs[questionOrder] = res["Comment"];
                        }
                        else
                        {
                            Points[questionOrder] = 0;
                            Logs[questionOrder] = "False\n";
                        }
                    }
                    else
                    {
                        // Not enough candidate 
                        // It rarely happens, it's this project's demos and faults.
                        throw new Exception("No questions found at question " + questionOrder + " paperNo = " +
                                            PaperNo + "\n");
                    }
                }
                catch (Exception e)
                {
                    // When something's wrong:
                    // Log error and return 0 point for student.
                    Points[questionOrder] = 0;
                    Logs[questionOrder] = e.Message + "\n";
                }
        }
    }
}