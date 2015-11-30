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

        public List<UserData> GetChattingInfo(int chatNumber, string message)
        {
            List<UserData> result = new List<UserData>();

            JsonParser jsonParser = new JsonParser(message);

            string[] jsonUserNumber = jsonParser.GetStringArrayValue(JsonName.UserNumber);
            string[] jsonNickname = jsonParser.GetStringArrayValue(JsonName.Nickname);

            for(int i = 0; i < jsonUserNumber.Length; i++)
            {
                int userNumber = Convert.ToInt32(jsonUserNumber[i]);
                string nickname = jsonNickname[i];

                if (chatNumber == userNumber) // Teacher
                {
                    result.Insert(0, new UserData { Number = userNumber, Nickname = nickname, IsTeacher = true });
                }
                else // Student
                {
                    result.Add(new UserData { Number = userNumber, Nickname = nickname, IsTeacher = false });
                }

                
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
