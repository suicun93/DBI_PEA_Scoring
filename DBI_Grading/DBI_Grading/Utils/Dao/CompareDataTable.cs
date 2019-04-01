using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

// ReSharper disable SpecifyACultureInStringConversionExplicitly

namespace DBI_Grading.Utils.Dao
{
    public partial class General
    {
        /// <summary>
        ///     Compare Columns Name of tables
        /// </summary>
        /// <param name="dataTableAnswer"></param>
        /// <param name="dataTableSolution"></param>
        /// <returns>"(Empty)" if true, "(comment)" if false</returns>
        internal static string CompareColumnsName(DataTable dataTableAnswer, DataTable dataTableSolution)
        {
            for (var i = 0; i < dataTableSolution.Columns.Count; i++)
                if (!dataTableSolution.Columns[i].ColumnName.ToLower()
                    .Equals(dataTableAnswer.Columns[i].ColumnName.ToLower()))
                    return "Column Name wrong - " + dataTableSolution.Columns[i].ColumnName;
            return "";
        }

        internal static DataTable SortColumnNameTable(DataTable dt)
        {
            var columnArray = new DataColumn[dt.Columns.Count];
            dt.Columns.CopyTo(columnArray, 0);
            var ordinal = -1;
            foreach (var orderedColumn in columnArray.OrderBy(c => c.ColumnName))
            {
                orderedColumn.SetOrdinal(++ordinal);
            }
            return dt;
        }

        public static DataTable SortDataTable(DataTable dt, string columnName)
        {
            dt.DefaultView.Sort = columnName;
            return dt.DefaultView.ToTable();
        }

        public static bool CompareColumnName(DataTable dtSchemaAnswer, DataTable dtSchemaTQ)
        {
            List<string> columnNameListAnswer = GetColumnsName(dtSchemaAnswer);
            List<string> columnNameListTQ = GetColumnsName(dtSchemaTQ);
            return !columnNameListTQ.Except(columnNameListAnswer).Any();
        }

        public static List<string> GetColumnsName(DataTable dt)
        {
            List<string> columnNameList = new List<string>();
            for (var i = 0; i < dt.Columns.Count; i++)
                columnNameList.Add(dt.Columns[i].ColumnName);
            return columnNameList;
        }

        public static DataTable DistinctTable(DataTable dt, List<string> columns)
        {
            DataTable dtUniqRecords = new DataTable();
            dtUniqRecords = dt.DefaultView.ToTable(true, columns.ToArray());
            return dtUniqRecords;
        }

        internal static bool CompareTwoDataTablesByExceptOneDirection(DataTable dataTableAnswer, DataTable dataTableSolution)
        {
            return !dataTableAnswer.AsEnumerable().Except(dataTableSolution.AsEnumerable(), DataRowComparer.Default)
                       .Any();
        }

        public static DataTable RotateTable(DataTable dt)
        {
            DataTable dt2 = new DataTable();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt2.Columns.Add();
            }
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                dt2.Rows.Add();
            }
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    dt2.Rows[i][j] = dt.Rows[j][i];
                }
            }
            return dt2;
        }

        internal static bool CheckDataTableSort(DataTable dataTableAnswer, DataTable dataTableTq)
        {
            string firstColumnName = dataTableTq.Columns[0].ColumnName;
            return CompareTwoDataTablesByRow(SortDataTable(DistinctTable(dataTableAnswer, GetColumnsName(dataTableAnswer)), firstColumnName),
                SortDataTable(DistinctTable(dataTableTq, GetColumnsName(dataTableTq)), firstColumnName));
        }

        public static DataTable LowerCaseColumnName(DataTable dt)
        {
            foreach (DataColumn dataColumn in dt.Columns)
            {
                dataColumn.ColumnName = dataColumn.ColumnName.ToLower();
            }
            return dt;
        }

        /// <summary>
        ///     Check data
        /// </summary>
        /// <param name="dataTableAnswer"></param>
        /// <param name="dataTableTq"></param>
        /// <returns>
        ///     true = same
        /// </returns>
        internal static bool CompareData(DataTable dataTableAnswer, DataTable dataTableTq, bool isRotate)
        {
            //Get First Column from TQ
            string firstColumnName = dataTableTq.Columns[0].ColumnName;

            //Sort Column Name
            SortColumnNameTable(dataTableAnswer);
            SortColumnNameTable(dataTableTq);
            DataTable sortedTableAnswer, sortedTableTq;

            try
            {
                //Sort Data by firstColumn
                sortedTableTq = SortDataTable(dataTableTq, firstColumnName);
                sortedTableAnswer = SortDataTable(dataTableAnswer, firstColumnName);
            }
            catch
            {
                throw new Exception("Answer missing column name " + firstColumnName);
            }

            //Distinct
            DataTable distinctTableTq = DistinctTable(sortedTableTq, GetColumnsName(dataTableTq));
            DataTable distinctTableAnswer = DistinctTable(sortedTableAnswer, GetColumnsName(dataTableAnswer));

            //Compare Data
            DataTable rotateTableTq = RotateTable(distinctTableTq);
            DataTable rotateTableAnswer = RotateTable(distinctTableAnswer);
            if (isRotate)
            {
                return CompareTwoDataTablesByExceptOneDirection(rotateTableAnswer, rotateTableTq);
            }
            return CompareTwoDataTablesByExceptOneDirection(rotateTableTq, rotateTableAnswer);
        }

        /// <summary>
        ///     Compare data line by row
        /// </summary>
        /// <param name="dataTableAnswer"></param>
        /// <param name="dataTableSolution"></param>
        /// <returns>
        ///     true = same
        /// </returns>
        internal static bool CompareTwoDataTablesByRow(DataTable dataTableAnswer, DataTable dataTableSolution)
        {
            //Distinct
            DistinctTable(dataTableAnswer, GetColumnsName(dataTableAnswer));
            DistinctTable(dataTableSolution, GetColumnsName(dataTableSolution));

            //Sort by column name
            SortColumnNameTable(dataTableAnswer);
            SortColumnNameTable(dataTableSolution);

            if (dataTableSolution.Rows.Count != dataTableAnswer.Rows.Count ||
                dataTableSolution.Columns.Count != dataTableAnswer.Columns.Count) return false;
            for (var i = 0; i < dataTableSolution.Rows.Count; i++)
            {
                var rowArraySolution = dataTableSolution.Rows[i].ItemArray;
                var rowArrayAnswer = dataTableAnswer.Rows[i].ItemArray;
                if (!rowArraySolution.SequenceEqual(rowArrayAnswer))
                    return false;
            }
            return true;
        }

        /// <summary>
        ///     Compare 2 DataSet
        /// </summary>
        /// <param name="dataSetAnswer"></param>
        /// <param name="dataSetSolution"></param>
        /// <param name="isRotate"></param>
        /// <returns></returns>
        internal static bool CompareTwoDataSets(DataSet dataSetAnswer, DataSet dataSetSolution, bool isRotate)
        {
            var countComparison = 0;
            foreach (DataTable dataTableSolution in dataSetSolution.Tables)
            {
                foreach (DataTable dataTableAnswer in dataSetAnswer.Tables)
                {
                    if (CompareData(dataTableAnswer, dataTableSolution, isRotate))
                        break;
                    countComparison++;
                }
                if (countComparison == dataSetAnswer.Tables.Count)
                    return false;
            }
            return true;
        }
    }
}