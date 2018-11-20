
namespace ToDoApplication.Models
{
    public class UserModel
    {
        public long id { get; set; }
        public string email { get; set; }
        public string username { get; set; }
        public string avatar { get; set; }
        public bool push { get; set; }
        //public string username { get; set; }
        public InfoListModel lists { get; set; }
    }
}