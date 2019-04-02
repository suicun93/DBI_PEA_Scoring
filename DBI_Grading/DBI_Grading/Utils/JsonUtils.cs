using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using DBI_Grading.Model.Student;
using DBI_Grading.Model.Teacher;
using Newtonsoft.Json;

namespace DBI_Grading.Utils
{
    internal class JsonUtils
    {
        //Object to JsonString
        public static string SerializeJson(object obj)
        {
            var jsonString = JsonConvert.SerializeObject(obj);
            return jsonString;
        }

        public static bool WriteJson(object obj, string path)
        {
            try
            {
                //path with name of file, remember
                File.WriteAllText(path, SerializeJson(obj));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static object LoadQuestion(string localPath)
        {
            var stream = new FileStream(localPath, FileMode.Open, FileAccess.Read);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Binder = new PreMergeToMergedDeserializationBinder();
            return formatter.Deserialize(stream) as PaperSet;
        }

        //public static PaperSet PaperSetFromJson(string localPath)
        //{
        //    // read file into a string and deserialize JSON to a type
        //    return JsonConvert.DeserializeObject<PaperSet>(File.ReadAllText(localPath));
        //}

        public static Submission SubmissionFromJson(string str)
        {
            return JsonConvert.DeserializeObject<Submission>(File.ReadAllText(str));
        }
    }
}