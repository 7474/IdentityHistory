using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityHistoryFunctionApp.SlackAPI
{
    public class UserProfile
    {
        public string title { get; set; } //"",
        public string phone { get; set; } //"",
        public string skype { get; set; } //"",
        public string real_name { get; set; } //"Egon Spengler",
        public string real_name_normalized { get; set; } //"Egon Spengler",
        public string display_name { get; set; } //"spengler",
        public string display_name_normalized { get; set; } //"spengler",
        public string status_text { get; set; } //"Print is dead",
        public string status_emoji { get; set; } //":books:",
        public string status_expiration { get; set; } //1502138999,
        public string avatar_hash { get; set; } //"ge3b51ca72de",
        public string first_name { get; set; } //"Matthew",
        public string last_name { get; set; } //"Johnston",
        public string email { get; set; } //"spengler@ghostbusters.example.com",
        public string image_original { get; set; } //"https://.../avatar/e3b51ca72dee4ef87916ae2b9240df50.jpg",
        public string image_24 { get; set; } //"https://.../avatar/e3b51ca72dee4ef87916ae2b9240df50.jpg",
        public string image_32 { get; set; } //"https://.../avatar/e3b51ca72dee4ef87916ae2b9240df50.jpg",
        public string image_48 { get; set; } //"https://.../avatar/e3b51ca72dee4ef87916ae2b9240df50.jpg",
        public string image_72 { get; set; } //"https://.../avatar/e3b51ca72dee4ef87916ae2b9240df50.jpg",
        public string image_192 { get; set; } //"https://.../avatar/e3b51ca72dee4ef87916ae2b9240df50.jpg",
        public string image_512 { get; set; } //"https://.../avatar/e3b51ca72dee4ef87916ae2b9240df50.jpg",
        public string team { get; set; } //"T012AB3C4"
    }
}
