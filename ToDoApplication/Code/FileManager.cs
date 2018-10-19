using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TodoData.Dao;
using TodoData.Models.Attachment;
using ToDoData.Enum;

namespace ToDoApplication.Code
{
    public class FileManager
    {
        private const string bucketName = "todoappdata";
        // Specify your bucket region (an example region is shown).
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.EUCentral1;
        private static IAmazonS3 s3Client;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public static string accessKey = "AKIAJKUTZUZE3W322FDA";
        public static string secretKey = "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";
        private static Random random = new Random();


        public FileManager()
        {
            AWSCredentials credentials = new BasicAWSCredentials(accessKey, secretKey);
            AmazonS3Config config = new AmazonS3Config();
            config.ServiceURL = "s3.amazonaws.com";
            config.RegionEndpoint = RegionEndpoint.EUCentral1;//Amazon.RegionEndpoint.GetBySystemName("us-east-1");
            s3Client = new AmazonS3Client(credentials, config);


            //s3Client = new AmazonS3Client(bucketRegion);
        }

        public async Task UploadFileAsync(string filePath, string keyName)
        {
            try
            {
                var fileTransferUtility = new TransferUtility(s3Client);
                await fileTransferUtility.UploadAsync(filePath, bucketName, keyName);
            }
            catch (AmazonS3Exception e)
            {
                logger.Log(LogLevel.Error, $"Error encountered on server. Message:'{e.Message}' when writing an object");
            }
            catch (Exception e)
            {
                logger.Log(LogLevel.Error, $"Unknown encountered on server. Message:'{e.Message}' when writing an object");
            }

        }

        public async Task<string> UploadFileAsync(byte[] inputData, string keyName)
        {
            try
            {
                Stream input = new MemoryStream(inputData);
                var fileTransferUtility = new TransferUtility(s3Client);
                await fileTransferUtility.UploadAsync(input, bucketName, keyName);
                return null;
            }
            catch (AmazonS3Exception e)
            {
                logger.Log(LogLevel.Error, $"Error encountered on server. Message:'{e.Message}' when writing an object");
                return e.ToString();
            }
            catch (Exception e)
            {
                logger.Log(LogLevel.Error, $"Unknown encountered on server. Message:'{e.Message}' when writing an object");
                return e.ToString();
            }

        }

        public static string RandomString(int length = 10)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public bool SaveAttachment(string filename, string url, long taskId)
        {
            try
            {
                var manager = new AttachmentDaoManager();
                var attachment = new Attachment()
                {
                    Id = 0,
                    FileType = (int)AttachmentTypeEnum.UNKNOWN,
                    FileName = filename,
                    FileUrl = url,
                    TaskId = taskId,
                    LastUpdate = DateTime.Now
                };
                manager.Save(attachment);

            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"SaveAttachment error: {ex}");
            }
            return true;
        }

        public List<Attachment> GetAttachments(long taskId)
        {
            try
            {
                var manager = new AttachmentDaoManager();
                var result = manager.GetAllByTask(taskId);

                return result.ToList();
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, $"GetAttachments error: {ex}");
            }
            return null;
        }
    }
}