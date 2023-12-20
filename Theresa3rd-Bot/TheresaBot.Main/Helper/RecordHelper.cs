using System.Text.RegularExpressions;
using TheresaBot.Main.Services;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Helper
{
    public static class RecordHelper
    {
        private static readonly RecordService recordService = new RecordService();

        public static async Task AddImageRecords(List<string> imageUrls, PlatformType platformType, long msgId, long groupId, long memberId)
        {
            if (imageUrls is null || imageUrls.Count == 0) return;
            await recordService.InsertImageRecord(imageUrls, platformType, msgId, groupId, memberId);
        }

        public static async Task AddMessageRecord(List<string> messageList, PlatformType platformType, long msgId, long groupId, long memberId)
        {
            var filterList = messageList.FilterMessage();
            if (filterList.Count == 0) return;
            string plainMessage = string.Join(' ', filterList);
            if (string.IsNullOrWhiteSpace(plainMessage)) return;
            await recordService.InsertMessageRecord(platformType, plainMessage, msgId, groupId, memberId);
        }

        private static List<string> FilterMessage(this List<string> messages)
        {
            var returnList = new List<string>();
            foreach (string item in messages)
            {
                string message = item.FilterMessage();
                if (string.IsNullOrWhiteSpace(message)) continue;
                returnList.Add(message);
            }
            return returnList;
        }

        private static string FilterMessage(this string message)
        {
            string msgLower = message.ToLower();
            if (msgLower.Contains("ftp:")) return string.Empty;
            if (msgLower.Contains("http:")) return string.Empty;
            if (msgLower.Contains("https:")) return string.Empty;
            if (msgLower.Contains("<?xml")) return string.Empty;
            if (msgLower.Contains("<xml")) return string.Empty;
            message = Regex.Replace(message, @"\[.*\]", string.Empty);
            message = Regex.Replace(message, @"\{.*\}", string.Empty);
            message = Regex.Replace(message, @"\<.*\>", string.Empty);
            message = message.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");
            return message;
        }

    }
}
