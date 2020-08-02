using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace SuricataToASFF.Types
{
    public class DicType
    {
        public string Field { get; set; }
        public Dictionary<string, JObject> Dictionary { get; set; }
    }
}
