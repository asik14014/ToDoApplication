
using System.Collections.Generic;

namespace ToDoApplication.Models
{
    public class UserModel
    {
        public long id { get; set; }
        public string email { get; set; }
        public string username { get; set; }
        public string avatar { get; set; }
        public bool push { get; set; }
        public bool emailNotification { get; set; }
        //public string username { get; set; }
        public List<InfoListModel> lists { get; set; }
    }
}