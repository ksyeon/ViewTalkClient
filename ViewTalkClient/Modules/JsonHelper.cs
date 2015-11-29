using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Json;

using ViewTalkClient.Models;

namespace ViewTalkClient.Modules
{
    public class JsonHelper
    {
        public JsonHelper()
        {

        }

        public string SetLoginInfo(string id, string password)
        {
            JsonObjectCollection result = new JsonObjectCollection();

            result.Add(new JsonStringValue(JsonName.ID, id));
            result.Add(new JsonStringValue(JsonName.Password, password));

            return result.ToString();
        }

        public List<UserData> GetChattingInfo(string data)
        {
            List<UserData> result = new List<UserData>();

            JsonParser jsonParser = new JsonParser(data);

            string[] userNumber = jsonParser.GetStringArrayValue(JsonName.UserNumber);
            string[] nickname = jsonParser.GetStringArrayValue(JsonName.Nickname);

            for(int i = 0; i < userNumber.Length; i++)
            {
                int number = Convert.ToInt32(userNumber[i]);
                bool isTeacher = false;

                if ( i == 0 ) isTeacher = true;

                result.Add(new UserData { Number = number, Nickname = nickname[i], IsTeacher = isTeacher });
            }

            return result;
        }
    }

    public class JsonName
    {
        public const string ID = "ID";
        public const string Password = "Password";
        public const string UserNumber = "UserNumber";
        public const string Nickname = "Nickname";
    }
}
