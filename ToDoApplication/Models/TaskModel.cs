using System.Collections.Generic;

namespace ToDoApplication.Models
{
    public class TaskModel
    {
        public long id { get; set; }

        public string title { get; set; }

        public string deadline { get; set; }

        public string description { get; set; }

        public List<string> subTasks { get; set; }

        public List<FileModel> files { get; set; }

        public List<UserListModel> users { get; set; }

        public List<CommentModel> comments { get; set; }

        public IterationModule repeat { get; set; }

        public long remind { get; set; }

        public List<int> list { get; set; }
    }
}