using Aliyun.OSS;
using Aliyun.OSS.Common;
using TheresaBot.Core.Common;

namespace TheresaBot.Core.Helper
{
    public static class OSSHelper
    {

        public static async Task UploadFile(FileInfo uploadFile, string ossPath)
        {
            try
            {
                var client = CreateClient();
                string bucketName = BotConfig.PixivCollectionConfig.OSSBucketName;
                ossPath = ossPath.Replace("\\", "/");
                client.PutObject(bucketName, ossPath, uploadFile.FullName);
                await Task.CompletedTask;
            }
            catch (OssException ex)
            {
                throw new Exception($"文件上传到OSS失败，ErrorCode={ex.ErrorCode}，Message={ex.Message}，File={uploadFile.Name}");
            }
        }

        private static OssClient CreateClient()
        {
            var config = BotConfig.PixivCollectionConfig;
            var endpoint = config.OSSEndpoint;
            var accessKeyId = config.OSSAccessKeyId;
            var accessKeySecret = config.OSSAccessKeySecret;
            var bucketName = config.OSSBucketName;
            if (string.IsNullOrEmpty(endpoint)) throw new Exception("未配置OSSEndpoint");
            if (string.IsNullOrEmpty(accessKeyId)) throw new Exception("未配置OSSAccessKeyId");
            if (string.IsNullOrEmpty(accessKeySecret)) throw new Exception("未配置OSSAccessKeySecret");
            if (string.IsNullOrEmpty(bucketName)) throw new Exception("未配置OSSBucketName");
            return new OssClient(endpoint, accessKeyId, accessKeySecret);
        }


    }
}
