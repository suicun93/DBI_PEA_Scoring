using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DBI_PEA_Scoring.Model
{
    public class Input
    {

        public Input(DataGridView dataGridView, int row, Result result)
        {
            Row = row;
            DataGridView = dataGridView;
            Result = result;
        }
        public Result Result { get; set; }
        public int Row { get; set; }
        public DataGridView DataGridView { get; set; }
    }
}
