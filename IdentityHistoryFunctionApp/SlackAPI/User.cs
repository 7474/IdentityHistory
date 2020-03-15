using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityHistoryFunctionApp.SlackAPI
{
   public class User
    {
        public string id; //"W012A3CDE",
        public string team_id; //"T012AB3C4",
        public string name; //"spengler",
        public string deleted; //false,
        public string color; //"9f69e7",
        public string real_name; //"Egon Spengler",
        public string tz; //"America/New_York",
        public string tz_label; //"Eastern Daylight Time",
        public int tz_offset; //-14400,
        public UserProfile profile; //a,
        public bool is_admin; //true,
        public bool is_owner; //false,
        public bool is_primary_owner; //false,
        public bool is_restricted; //false,
        public bool is_ultra_restricted; //false,
        public bool is_bot; //false,
        public bool is_stranger; //false,
        public long updated; //1502138686,
        public bool is_app_user; //false,
        public bool is_invited_user; //false,
        public bool has_2fa; //false,
        public string locale; //"en-US"
    }
}
