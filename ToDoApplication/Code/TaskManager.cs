using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http;
using ToDoApplication.Models;
using ToDoApplication.Models.Request;
using TodoData.Dao;
using TodoData.Enum;
using TodoData.Models.Group;
using TodoData.Models.Shared;
using TodoData.Models.Task;

namespace ToDoApplication.Code
{
    [Authorize]
    public static class TaskManager
    {
        private static TaskDaoManager taskDaoManager = new TaskDaoManager();
        private static AttachmentDaoManager attachmentDaoManager = new AttachmentDaoManager();
        private static SharedTaskDaoManager sharedTaskDaoManager = new SharedTaskDaoManager();
        private static UserDaoManager userDaoManager = new UserDaoManager();
        private static CommentDaoManager commentDaoManager = new CommentDaoManager();
        private static SubtaskDaoManager subtaskDaoManager = new SubtaskDaoManager();
        private static GroupDaoManager groupDaoManager = new GroupDaoManager();

        public static List<TaskModel> GetAllTasks()
        {
            var result = new List<TaskModel>();
            var items = taskDaoManager.GetAll();

            foreach (var item in items)
            {
                result.Add(GetTask(item.Id));
            }

            return result;
        }

        public static List<TaskModel> GetAllTasksByUser(long userId)
        {
            var result = new List<TaskModel>();
            var items = taskDaoManager.GetAllByUserId(userId);

            foreach (var item in items)
            {
                result.Add(GetTask(item.Id));
            }

            return result;
        }

        public static List<TaskModel> GetAllTasksByUser(long userId, DateTime date)
        {
            var result = new List<TaskModel>();
            var items = taskDaoManager.GetAllByUserId(userId);

            foreach (var item in items)
            {
                result.Add(GetTask(item.Id));
            }

            return result;
        }

        public static IList<SharedTasks> GetAllSharedTasksByUser(long userId)
        {
            return sharedTaskDaoManager.GetAllByUserId(userId);
        }

        public static IList<Task> GetAllTasksByGroup(long groupId)
        {
            return taskDaoManager.GetAllByGroupId(groupId);
        }

        public static Task Create(Task task)
        {
            return taskDaoManager.Save(task);
        }

        public static void Delete(long taskId)
        {
            var entity = taskDaoManager.GetById(taskId);
            taskDaoManager.Delete(entity);
        }

        public static TaskModel GetTask(long id)
        {
            var item = taskDaoManager.GetById(id);
            if (item == null) return null;

            var result = new TaskModel()
            {
                id = item.Id,
                title = item.Name,
                deadline = item.EndDate.HasValue ? item.EndDate.Value.ToLongDateString() : "0",
                description = item.Description,
                repeat = new IterationModule()
                {
                    repeatEvery = 0,
                    type = "NEVER"
                },
                remind = 0
            };

            //files
            var files = attachmentDaoManager.GetAllByTaskId(id);
            result.files = (files != null) ? files.Select(f => new FileModel() { name = f.FileName, link = f.FileUrl }).ToList() : null;

            //users
            var temp = sharedTaskDaoManager.GetAllByTaskId(id);
            var sharedUsers = temp.Select(t => t.UserId);
            var users = new List<UserListModel>();
            if (sharedUsers != null)
            {
                foreach (var userId in sharedUsers)
                {
                    var user = userDaoManager.GetById(userId);
                    users.Add(new UserListModel()
                    {
                        id = user.Id,
                        name = user.UserName
                    });
                }
                result.users = users;
            }

            //comments
            var comments = commentDaoManager.GetAllByTaskId(id);
            var commentsModel = new List<CommentModel>();
            if (comments != null)
            {
                foreach (var comment in comments)
                {
                    var userItem = userDaoManager.GetById(comment.UserId);
                    commentsModel.Add(new CommentModel()
                    {
                        text = comment.Text,
                        date = comment.LastUpdate,
                        user = new UserListModel()
                        {
                            id = userItem.Id,
                            name = userItem.UserName
                        }
                    });
                }
                result.comments = commentsModel;
            }

            //subtasks
            var subtasks = subtaskDaoManager.GetAllByTaskId(id);
            var subtasksModel = new List<AddSubtaskRequest>();
            if (subtasks != null)
            {
                foreach (var subtask in subtasks)
                {
                    subtasksModel.Add(new AddSubtaskRequest() { title = subtask.Title, status = subtask.Status});
                }
                result.subTasks = subtasksModel;
            }

            return result;
        }

        public static Task SaveOrUpdate(TaskRequest task, long userId)
        {
            long? groupId = null;
            if (task.list != null) groupId = task.list.FirstOrDefault();
            var entity = new Task()
            {
                GroupId = groupId,
                UserId = userId,
                Status = 1,
                CreationDate = DateTime.Now,
                LastUpdate = DateTime.Now
            };
            return taskDaoManager.SaveOrUpdate(entity);
        }

        public static TaskRequest Save(TaskRequest task, long userId)
        {
            var entity = new Task()
            {
                UserId = userId,
                Status = 1,
                CreationDate = DateTime.Now,
                LastUpdate = DateTime.Now,
                Description = task.description,
                Name = task.title,
                Deadline = task.deadline,
                RepeatType = task.repeat.type,
                RepeatTime = task.repeat.repeatEvery,
                Remind = task.remind,
            };
            if (task.list != null) entity.GroupId = task.list.FirstOrDefault();
            taskDaoManager.Save(entity);

            #region subtask
            if (task.subTasks != null && task.subTasks.Any())
            {
                foreach (var subtask in task.subTasks)
                {
                    subtaskDaoManager.Save(new Subtask()
                    {
                        Id = 0,
                        TaskId = entity.Id,
                        Title = subtask.title,
                        Status = true
                    });
                }
            }
            #endregion

            #region users
            if (task.users != null && task.users.Any())
            {
                foreach (var user in task.users)
                {
                    sharedTaskDaoManager.Save(new SharedTasks()
                    {
                        Id = 0,
                        TaskId = entity.Id,
                        UserId = userId,
                        ShareType = 1,
                        LastUpdate = DateTime.Now,
                        IsActive = true
                    });
                }
            }
            #endregion

            #region comments
            if (task.comments != null && task.comments.Any())
            {
                foreach (var comment in task.comments)
                {
                    commentDaoManager.Save(new Comment()
                    {
                        Id = 0,
                        UserId = comment.user.id,
                        TaskId = entity.Id,
                        Text = comment.text,
                        LastUpdate = DateTime.Now
                    });
                }
            }
            #endregion

            #region files
            if (task.files != null && task.files.Any())
            {
                var fileManager = new AzureFileManager();
                foreach (var file in task.files)
                {
                    var filename = DateTime.Now.ToString("HHmmSS") + file.FileName.Trim('\"');
                    var buffer = ReadFully(file.InputStream);
                    var result = fileManager.UploadFileAsync(buffer, filename);

                    if (string.IsNullOrEmpty(result.Result))
                    {
                        continue;
                    }

                    //save attachment
                    attachmentDaoManager.Save(new TodoData.Models.Attachment.Attachment()
                    {
                        Id = 0,
                        FileType = 1,
                        FileName = file.FileName,
                        FileUrl = result.Result,
                        TaskId = entity.Id,
                        LastUpdate = DateTime.Now
                    });
                }
            }
            #endregion

            task.id = entity.Id;

            return task;
        }

        public static TaskRequest Update(TaskRequest task, long userId)
        {
            var entity = taskDaoManager.GetById(task.id);

            entity.UserId = userId;
            entity.Description = task.description;
            entity.Name = task.title;
            entity.Deadline = task.deadline;
            entity.RepeatType = task.repeat.type;
            entity.RepeatTime = task.repeat.repeatEvery;
            entity.Remind = task.remind;

            if (task.list != null && task.list.Any()) entity.GroupId = task.list.FirstOrDefault();
            taskDaoManager.Update(entity);

            #region subtask
            if (task.subTasks != null && task.subTasks.Any())
            {
                var subs = subtaskDaoManager.GetAllByTaskId(entity.Id);
                foreach (var s in subs)
                {
                    subtaskDaoManager.Delete(s);
                }

                foreach (var subtask in task.subTasks)
                {
                    subtaskDaoManager.Save(new Subtask()
                    {
                        Id = 0,
                        TaskId = entity.Id,
                        Title = subtask.title,
                        Status = subtask.status
                    });
                }
            }
            #endregion

            #region users
            if (task.users != null && task.users.Any())
            {
                var sh = sharedTaskDaoManager.GetAllByTaskId(entity.Id);
                foreach (var s in sh)
                {
                    sharedTaskDaoManager.Delete(s);
                }
                foreach (var user in task.users)
                {
                    sharedTaskDaoManager.Save(new SharedTasks()
                    {
                        Id = 0,
                        TaskId = entity.Id,
                        UserId = user.id,
                        ShareType = 1,
                        LastUpdate = DateTime.Now,
                        IsActive = true
                    });
                }
            }
            #endregion

            #region comments
            if (task.comments != null && task.comments.Any())
            {
                var tempList = commentDaoManager.GetAllByTaskId(entity.Id);
                foreach (var temp in tempList)
                {
                    commentDaoManager.Delete(temp);
                }

                foreach (var comment in task.comments)
                {
                    commentDaoManager.Save(new Comment()
                    {
                        Id = 0,
                        UserId = comment.user.id,
                        TaskId = entity.Id,
                        Text = comment.text,
                        LastUpdate = DateTime.Now
                    });
                }
            }
            #endregion

            #region files
            if (task.files != null && task.files.Any())
            {
                
                var fileManager = new AzureFileManager();
                foreach (var file in task.files)
                {
                    var attachments = attachmentDaoManager.GetAllByTask(entity.Id);

                    if (!attachments.Any(a => a.FileName.Contains(file.FileName.Trim('\"'))))
                    {
                        var filename = DateTime.Now.ToString("HHmmSS") + file.FileName.Trim('\"');
                        var buffer = ReadFully(file.InputStream);
                        var result = fileManager.UploadFileAsync(buffer, filename);

                        if (string.IsNullOrEmpty(result.Result))
                        {
                            continue;
                        }

                        //save attachment
                        attachmentDaoManager.Save(new TodoData.Models.Attachment.Attachment()
                        {
                            Id = 0,
                            FileType = 1,
                            FileName = file.FileName,
                            FileUrl = result.Result,
                            TaskId = entity.Id,
                            LastUpdate = DateTime.Now
                        });
                    }
                }
            }
            #endregion

            task.id = entity.Id;

            return task;
        }

        private static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        /*
        public static Subtask AddSubtask(AddSubtaskRequest request)
        {
            var temp = subtaskDaoManager.Find(request.taskId, request.subTaskId);
            if (temp != null) return temp;

            var result = subtaskDaoManager.SaveOrUpdate(new Subtask()
            {
                Id = 0,
                TaskId = request.taskId,
                SubtaskId = request.subTaskId,
                Registration = DateTime.Now
            });
            return result;
        }

        public static bool DeleteSubtask(AddSubtaskRequest request)
        {
            var temp = subtaskDaoManager.Find(request.taskId, request.subTaskId);
            if (temp == null) return true;

            subtaskDaoManager.Delete(temp);

            return true;
        }*/

        public static Comment AddComment(AddCommentRequest request, long userId)
        {
            return commentDaoManager.Save(new Comment()
            {
                Id = 0,
                UserId = userId,
                TaskId = request.taskId,
                Text = request.text,
                LastUpdate = DateTime.Now
            });
        }

        public static SharedTasks AddUser(AddUserRequest request, long userId)
        {
            return sharedTaskDaoManager.Save(new SharedTasks()
            {
                Id = 0,
                TaskId = request.taskId,
                UserId = request.userId,
                ShareType = 1,
                LastUpdate = DateTime.Now,
                IsActive = true
            });
        }

        public static Task UpdateRepetition(IterationModule request)
        {
            var task = taskDaoManager.GetById(request.taskId);

            task.RepeatTime = request.repeatEvery;
            task.RepeatType = request.type;

            return taskDaoManager.Update(task);
        }

        public static bool AddFavorite(long taskId, long userId)
        {
            var favorite = groupDaoManager.GetByType(userId, 1);

            if (favorite != null && favorite.Any())
            {
                var groupId = favorite.First().Id;

                var task = taskDaoManager.GetById(taskId);
                task.GroupId = groupId;

                taskDaoManager.Update(task);

                return true;
            }

            return false;
        }

        public static IList<Task> FindTask(long userId, string name)
        {
            return taskDaoManager.GetAllByName(userId, name);
        }

        private static DateTime getDateTimeFromUnixTimeStamp(long timestamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(timestamp);
        }

        public static IList<Task> GetAllForMonth(long userId)
        {
            var temp = taskDaoManager.GetAllByUserId(userId);
            if (temp != null && temp.Any())
            {
                var today = DateTime.Now;
                temp = temp.Where(task =>
                {
                    var buf = getDateTimeFromUnixTimeStamp(task.Deadline);
                    if (buf.Year == today.Year && buf.Month == today.Month) return true;
                    return false;
                }).ToList();
            }
            return temp;
        }

        public static IList<Task> GetAllForToday(long userId)
        {
            var temp = taskDaoManager.GetAllByUserId(userId);
            if (temp != null && temp.Any())
            {
                var today = DateTime.Now;
                temp = temp.Where(task =>
                {
                    var buf = getDateTimeFromUnixTimeStamp(task.Deadline);
                    if (buf.Year == today.Year && buf.Month == today.Month && buf.Day == today.Day) return true;
                    return false;
                }).ToList();
            }
            return temp;
        }

        public static IList<Task> GetFavorite(long userId)
        {
            IList<Task> result = null;
            var buffer = groupDaoManager.GetAllByUserId(userId);
            var group = buffer.FirstOrDefault(f => f.GroupType == (int)GroupTypeEnum.FAVOURITES);

            if (group != null)
            {
                result = taskDaoManager.GetAllByGroupId(group.Id);
            }

            return result;
        }
    }
}
