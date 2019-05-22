using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Utility
{
    public class JsonHelper
    {
        public static string ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T JsonToT<T>(string jStr)
        {
            if (jStr == null)
                jStr = "";
            return JsonConvert.DeserializeObject<T>(jStr);
        }

        public static T JsonToTByKey<T>(string jStr,string key,bool isArr=false)
        {
            JObject jObj = (JObject)JsonConvert.DeserializeObject(jStr);
            JToken obj = null;
            if (!isArr)
                obj = jObj[key];
            else
            {
                obj=(JArray)jObj[key];
            }
            return obj.ToObject<T>();
        }
    }
}
