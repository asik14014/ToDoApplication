using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TodoData.Dao;
using TodoData.Models.Attachment;

namespace ToDoApplication.Code
{
    public static class FileManager
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static AttachmentDaoManager attachmentDaoManager = new AttachmentDaoManager();
        private static AttachmentTypeDaoManager attachmentTypeDaoManager = new AttachmentTypeDaoManager();
        private static TaskDaoManager taskDaoManager = new TaskDaoManager();

        public static void SaveAttachment(Attachment file)
        {
            attachmentDaoManager.Save(file);
        }

        public static IList<Attachment> GetAttachments(long taskId)
        {
            return attachmentDaoManager.GetAllByTaskId(taskId);
        }
    }
}