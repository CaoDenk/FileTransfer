using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace FileTransferWpf.Tools
{
    internal static class MyJson
    {
        /// <summary>
        /// 根据json对象key获取value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsn"></param>
        /// <param name="key"></param>
        /// <returns></returns>
       public static T? GetValue<T>(this JsonNode jsn,string key)
        {
            JsonObject jsonObject = jsn.AsObject();

            JsonNode? jsonNode = jsonObject[key];
        
            return jsonNode.GetValue<T>();
        }


    }
}
