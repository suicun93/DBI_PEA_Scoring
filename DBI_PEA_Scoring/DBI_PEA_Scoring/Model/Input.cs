using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DBI_PEA_Scoring.Model
{
    public class Input
    {

        public Input(DataGridView dataGridView, int row)
        {
            this.row = row;
            this.dataGridView = dataGridView;
        }

        public int row { get; set; }
        public DataGridView dataGridView { get; set; }
    }
}
