using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityHistoryFunctionApp.SlackAPI
{
   public class User
    {
        public string id { get; set; } //"W012A3CDE",
        public string team_id { get; set; } //"T012AB3C4",
        public string name { get; set; } //"spengler",
        public string deleted { get; set; } //false,
        public string color { get; set; } //"9f69e7",
        public string real_name { get; set; } //"Egon Spengler",
        public string tz { get; set; } //"America/New_York",
        public string tz_label { get; set; } //"Eastern Daylight Time",
        public int tz_offset { get; set; } //-14400,
        public UserProfile profile { get; set; } //a,
        public bool is_admin { get; set; } //true,
        public bool is_owner { get; set; } //false,
        public bool is_primary_owner { get; set; } //false,
        public bool is_restricted { get; set; } //false,
        public bool is_ultra_restricted { get; set; } //false,
        public bool is_bot { get; set; } //false,
        public bool is_stranger { get; set; } //false,
        public long updated { get; set; } //1502138686,
        public bool is_app_user { get; set; } //false,
        public bool is_invited_user { get; set; } //false,
        public bool has_2fa { get; set; } //false,
        public string locale { get; set; } //"en-US"
    }
}
