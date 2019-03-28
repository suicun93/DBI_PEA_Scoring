﻿namespace DBI_Grading.Model
{
    public class Input
    {
        public Input(int row, Result result)
        {
            Row = row;
            Result = result;
        }

        public Result Result { get; set; }
        public int Row { get; set; }
    }
}