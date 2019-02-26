﻿using System;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using DBI_PEA_Scoring.Common;

namespace DBI_PEA_Scoring.Utils.DaoType
{
    public class SelectType
    {
        /// <summary>
        /// Marking Query Select type
        /// </summary>
        /// <param name="isSort">is this question need sorting</param>
        /// <param name="dbTeacherName"></param>
        /// <param name="dbStudentName"></param>
        /// <param name="queryTeacher"></param>
        /// <param name="queryStudent"></param>
        /// <returns></returns>
        public static bool MarkSelectType(bool isSort, string dbTeacherName, string dbStudentName, string queryTeacher,
            string queryStudent)
        {
            switch (isSort)
            {
                case true:
                    return General.CompareTableNoSort(dbTeacherName, dbStudentName, queryTeacher, queryStudent);
                case false:
                    return General.CompareTableSort(dbTeacherName, dbStudentName, queryTeacher, queryStudent);
                default:
                    return false;
            }
        }
    }
}
