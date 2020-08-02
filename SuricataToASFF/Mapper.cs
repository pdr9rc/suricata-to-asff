using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SuricataToASFF.Types;

namespace SuricataToASFF
{
    public class Mapper
    {
        private string _path = "/etc";
        private string _file = "mapping.json";
        private static Configuration _configuration;

        public Mapper Init()
        {
            var json = File.ReadAllText($"{_path}/{_file}");
            _configuration = JsonConvert.DeserializeObject<Configuration>(json);
            return this;
        }

        public static List<Dictionary<string, JObject>> GetFlowAlerts(string path, string file, ref long position)
        {
            var alerts = new List<Dictionary<string, JObject>>();
            using (StreamReader reader = new StreamReader($"{path}/{file}"))
            {
                reader.BaseStream.Position = position;
                var json = reader.ReadLine();
                position = reader.BaseStream.Position;
                Dictionary<string, JObject> alert = JsonConvert.DeserializeObject<Dictionary<string, JObject>>(json);
                var flow_id = alert["flow_id"];
                alerts.Add(alert);
                while (alert["flow_id"] == flow_id)
                {
                    alert = JsonConvert.DeserializeObject<Dictionary<string, JObject>>(json);
                    if (alert["flow_id"] != flow_id) 
                        break;
                    position = reader.BaseStream.Position;
                }
            }

            return alerts;
        }
        //readonly static FieldInfo charPosField = typeof(StreamReader).GetField("charPos", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        //readonly static FieldInfo charLenField = typeof(StreamReader).GetField("charLen", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        //readonly static FieldInfo charBufferField = typeof(StreamReader).GetField("charBuffer", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        ////static long ActualPosition(StreamReader reader)
        //{
        //    var charBuffer = (char[])charBufferField.GetValue(reader);
        //    var charLen = (int)charLenField.GetValue(reader);
        //    var charPos = (int)charPosField.GetValue(reader);

        //    return reader.BaseStream.Position - reader.CurrentEncoding.GetByteCount(charBuffer, charPos, charLen - charPos);
        //}

        public static string Map(Dictionary<string, JObject> alert)
        {

            JObject finding = new JObject();

            foreach (var keyValuePair in _configuration.ValueTypes)
            {
                finding[keyValuePair.Key] = keyValuePair.Value;
            }

            foreach (var keyValuePair in _configuration.FieldTypes)
            {
                finding[keyValuePair.Key] = alert[keyValuePair.Value];
            }

            foreach (var keyValuePair in _configuration.DictTypes)
            {
                finding[keyValuePair.Key] = keyValuePair.Value.Dictionary[alert[keyValuePair.Value.Field].ToString()];
            }

            return finding.ToString();
        }

    }
}