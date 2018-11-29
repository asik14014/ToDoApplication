namespace ToDoApplication.Models
{
    public class CommentModel
    {
        public string text { get; set; }
        public long date { get; set; }
        public UserListModel user { get; set; }
    }
}