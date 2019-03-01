using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using DBI_PEA_Scoring.Model;

namespace DBI_PEA_Scoring.Utils
{
    class JsonUtils
    {
        //Object to JsonString
        public static string SerializeJson(object obj)
        {
            string jsonString = JsonConvert.SerializeObject(obj);
            return jsonString;
        }

        public static bool WriteJson(object obj, string path)
        {
            try
            {
                File.WriteAllText(path + "\\ExamItems.dat", SerializeJson(obj));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static object LoadQuestion(string path)
        {
            string jsonQuestion = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<TestItem>>(jsonQuestion);
        }

        public static List<Question> DeserializeJson(string localPath)
        {
            // read file into a string and deserialize JSON to a type
            return JsonConvert.DeserializeObject<List<Question>>(File.ReadAllText(localPath));
        }
    }
}
