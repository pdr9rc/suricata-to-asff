using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace SuricataToASFF.Types
{
    public class Configuration
    {
        public Dictionary<string, string> FieldTypes { get; set; }
        public Dictionary<string, DicType> DictTypes { get; set; }
        public Dictionary<string, JObject> ValueTypes { get; set; }
    }
}
