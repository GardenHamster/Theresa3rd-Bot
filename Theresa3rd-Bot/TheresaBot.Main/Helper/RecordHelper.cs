using TheresaBot.Main.Business;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Helper
{
    public static class RecordHelper
    {
        private static readonly RecordBusiness recordBusiness = new RecordBusiness();

        public static async Task AddImageRecords(List<string> imageUrls, PlatformType platformType, long msgId, long groupId, long memberId)
        {
            if (imageUrls is null || imageUrls.Count == 0) return;
            await recordBusiness.AddImageRecord(imageUrls, platformType, msgId, groupId, memberId);
        }

        public static async Task AddMessageRecord(List<string> plainMessages, PlatformType platformType, long msgId, long groupId, long memberId)
        {
            if (plainMessages is null || plainMessages.Count == 0) return;
            string plainMessage = string.Join(' ', plainMessages);
            if (string.IsNullOrWhiteSpace(plainMessage)) return;
            await recordBusiness.AddMessageRecord(platformType, plainMessage, msgId, groupId, memberId);
        }







    }
}
