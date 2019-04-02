using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using DBI_Grading.Model.Teacher;

namespace DBI_Grading.Utils
{
    internal class SerializationUtils
    {
        public static object LoadQuestion(string localPath)
        {
            var stream = new FileStream(localPath, FileMode.Open, FileAccess.Read);
            var formatter = new BinaryFormatter();
            formatter.Binder = new PaperSetBinder();
            return formatter.Deserialize(stream) as PaperSet;
        }
    }

    internal sealed class PaperSetBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type returntype;
            // Paper Set
            if (typeName.StartsWith("DBI202_Creator.Entities.Paper.PaperSet"))
                returntype = typeof(PaperSet);
            // List<Paper>
            else if (typeName.StartsWith(
                "System.Collections.Generic.List`1[[DBI202_Creator.Entities.Paper.Paper, DBI202_Creator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]")
            )
                returntype = typeof(List<Paper>);
            // List<String>
            else if (typeName.StartsWith(
                "System.Collections.Generic.List`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]")
            )
                returntype = typeof(List<string>);
            // List<int>
            else if (typeName.StartsWith(
                "System.Collections.Generic.List`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]")
            )
                returntype = typeof(List<int>);
            // QuestionSet
            else if (typeName.StartsWith("DBI202_Creator.Entities.Question.QuestionSet"))
                returntype = typeof(QuestionSet);
            // Paper
            else if (typeName.StartsWith("DBI202_Creator.Entities.Paper.Paper"))
                returntype = typeof(Paper);
            // List<Question>
            else if (typeName.StartsWith(
                "System.Collections.Generic.List`1[[DBI202_Creator.Entities.Question.Question, DBI202_Creator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]")
            )
                returntype = typeof(List<Question>);
            // Question
            else if (typeName.StartsWith("DBI202_Creator.Entities.Question.Question"))
                returntype = typeof(Question);
            // List<Candidate>
            else if (typeName.StartsWith(
                "System.Collections.Generic.List`1[[DBI202_Creator.Entities.Candidate.Candidate, DBI202_Creator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]")
            )
                returntype = typeof(List<Candidate>);
            // Candidate.QuestionTypes
            else if (typeName.StartsWith("DBI202_Creator.Entities.Candidate.Candidate+QuestionTypes"))
                returntype = typeof(Candidate.QuestionTypes);
            // Candidate
            else if (typeName.StartsWith("DBI202_Creator.Entities.Candidate.Candidate"))
                returntype = typeof(Candidate);
            else
                returntype = Type.GetType(string.Format("{0}, {1}", typeName, assemblyName));
            return returntype;
        }
    }
}