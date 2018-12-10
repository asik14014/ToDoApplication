using System.Collections.Generic;
using System.Web;

namespace ToDoApplication.Models.Request
{
    public class TaskRequest
    {
        public long id { get; set; }
        public string title { get; set; }
        public long deadline { get; set; }
        public string description { get; set; }
        public List<AddSubtaskRequest> subTasks { get; set; }
        public List<HttpPostedFileBase> files { get; set; }
        public List<UserTaskRequest> users { get; set; }
        public List<CommentTaskRequest> comments { get; set; }
        public Repeater repeat { get; set; }
        public long remind { get; set; }
        public List<long> list { get; set; }
    }

    public class Repeater
    {
        public string type { get; set; }
        public int repeatEvery { get; set; }
    }

    public class UserTaskRequest
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class CommentTaskRequest
    {
        public UserTaskRequest user { get; set; }
        public string text { get; set; }
        public long date { get; set; }
    }

}
