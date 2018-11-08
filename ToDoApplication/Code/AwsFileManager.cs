using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using NLog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ToDoApplication.Code
{
    public class AwsFileManager
    {
        private const string bucketName = "todoappdata";
        // Specify your bucket region (an example region is shown).
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.EUCentral1;
        private static IAmazonS3 s3Client;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public static string accessKey = "";
        public static string secretKey = "";

        public AwsFileManager()
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
    }
}