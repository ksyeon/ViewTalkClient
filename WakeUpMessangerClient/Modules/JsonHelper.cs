using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Json;

namespace WakeUpMessangerClient.Modules
{
    public class JsonName
    {
        public const string ID = "ID";
        public const string Password = "Password";
    }

    public class JsonHelper
    {
        public JsonHelper()
        {

        }

        public string GetLoginInfo(string id, string password)
        {
            JsonObjectCollection jsonObjectCollection = new JsonObjectCollection();

            jsonObjectCollection.Add(new JsonStringValue(JsonName.ID, id));
            jsonObjectCollection.Add(new JsonStringValue(JsonName.Password, password));

            return jsonObjectCollection.ToString();
        }
    }
}
