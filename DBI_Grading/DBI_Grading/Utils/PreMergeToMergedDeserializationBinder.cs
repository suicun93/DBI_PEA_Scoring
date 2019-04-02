using DBI_Grading.Model.Teacher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace DBI_Grading.Utils
{
    sealed class PreMergeToMergedDeserializationBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type returntype = null;
            // Paper Set
            if (typeName.StartsWith("DBI_Exam_Creator_Tool.Entities.PaperSet"))
            {
                returntype = typeof(PaperSet);
            }
            // List<Paper>
            else if (typeName.StartsWith("System.Collections.Generic.List`1[[DBI_Exam_Creator_Tool.Entities.Paper, DBI_Exam_Creator_Tool, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]"))
            {
                returntype = typeof(List<Paper>);
            }
            // List<String>
            else if (typeName.StartsWith("System.Collections.Generic.List`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]"))
            {
                returntype = typeof(List<String>);
            }
            // List<int>
            else if (typeName.StartsWith("System.Collections.Generic.List`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]"))
            {
                returntype = typeof(List<int>);
            }
            // QuestionSet
            else if (typeName.StartsWith("DBI_Exam_Creator_Tool.Entities.QuestionSet"))
            {
                returntype = typeof(QuestionSet);
            }
            // Paper
            else if (typeName.StartsWith("DBI_Exam_Creator_Tool.Entities.Paper"))
            {
                returntype = typeof(Paper);
            }
            // List<Question>
            else if (typeName.StartsWith("System.Collections.Generic.List`1[[DBI_Exam_Creator_Tool.Entities.Question, DBI_Exam_Creator_Tool, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]"))
            {
                returntype = typeof(List<Question>);
            }
            // Question
            else if (typeName.StartsWith("DBI_Exam_Creator_Tool.Entities.Question"))
            {
                returntype = typeof(Question);
            }
            // List<Candidate>
            else if (typeName.StartsWith("System.Collections.Generic.List`1[[DBI_Exam_Creator_Tool.Entities.Candidate, DBI_Exam_Creator_Tool, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]"))
            {
                returntype = typeof(List<Candidate>);
            }
            // Candidate.QuestionTypes
            else if (typeName.StartsWith("DBI_Exam_Creator_Tool.Entities.Candidate+QuestionTypes"))
            {
                returntype = typeof(Candidate.QuestionTypes);
            }
            // Candidate
            else if (typeName.StartsWith("DBI_Exam_Creator_Tool.Entities.Candidate"))
            {
                returntype = typeof(Candidate);
            }
            else
            {
                returntype =
                        Type.GetType(String.Format("{0}, {1}",
                        typeName, assemblyName));
            }
            return returntype;
        }
    }
}
