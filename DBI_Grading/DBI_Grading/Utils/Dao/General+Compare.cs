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
        /// <param name="dataTableAnswerSchema"></param>
        /// <param name="dataTableSolutionSchema"></param>
        /// <returns>"(Empty)" if true, "(comment)" if false</returns>
        internal static string CompareColumnsNameOfTables(DataTable dataTableAnswerSchema,
            DataTable dataTableSolutionSchema)
        {
            for (var i = 0; i < dataTableSolutionSchema.Rows.Count; i++)
                if (!dataTableSolutionSchema.Rows[i]["ColumnName"].ToString().ToLower()
                    .Equals(dataTableAnswerSchema.Rows[i]["ColumnName"].ToString().ToLower()))
                    return "Column Name wrong - " + dataTableSolutionSchema.Rows[i]["ColumnName"];
            return "";
        }

        /// <summary>
        ///     Check data using except 2-way (Rows can be difference)
        /// </summary>
        /// <param name="dataTableAnswer"></param>
        /// <param name="dataTableSolution"></param>
        /// <returns>
        ///     true = same
        /// </returns>
        internal static bool CompareTwoDataTablesByExcept(DataTable dataTableAnswer, DataTable dataTableSolution)
        {
            return !dataTableAnswer.AsEnumerable().Except(dataTableSolution.AsEnumerable(), DataRowComparer.Default)
                       .Any() && !dataTableSolution.AsEnumerable()
                       .Except(dataTableAnswer.AsEnumerable(), DataRowComparer.Default).Any();
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
        /// <returns></returns>
        internal static bool CompareTwoDataSetsByRow(DataSet dataSetAnswer, DataSet dataSetSolution)
        {
            var countComparison = 0;
            foreach (DataTable dataTableSolution in dataSetSolution.Tables)
            {
                foreach (DataTable dataTableAnswer in dataSetAnswer.Tables)
                {
                    if (CompareTwoDataTablesByRow(dataTableAnswer, dataTableSolution))
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