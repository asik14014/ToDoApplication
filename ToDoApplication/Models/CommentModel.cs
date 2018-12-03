using System;

namespace ToDoApplication.Models
{
    public class CommentModel
    {
        public string text { get; set; }
        public DateTime date { get; set; }
        public UserListModel user { get; set; }
    }
}