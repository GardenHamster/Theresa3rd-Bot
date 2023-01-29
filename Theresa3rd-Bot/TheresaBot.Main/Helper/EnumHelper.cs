using System;
using System.Text;
using TheresaBot.Main.Type;
using TheresaBot.Main.Type.StepOption;

namespace TheresaBot.Main.Helper
{
    public static class EnumHelper
    {
        public static string PixivSyncModeOption()
        {
            StringBuilder optionBuilder = new StringBuilder();
            optionBuilder.AppendLine($"{(int)PixivSyncModeType.Merge}：合并（只添加不存在的订阅，保留在不关注列表中的订阅）");
            optionBuilder.AppendLine($"{(int)PixivSyncModeType.Overwrite}：覆盖（移除所有原有的订阅，并将关注列表重新添加到订阅中）");
            return optionBuilder.ToString();
        }

        public static string PixivSyncGroupOption()
        {
            StringBuilder optionBuilder = new StringBuilder();
            optionBuilder.AppendLine($"{(int)SubscribeGroupType.All}：所有拥有订阅权限的群");
            optionBuilder.AppendLine($"{(int)SubscribeGroupType.Current}：当前群");
            return optionBuilder.ToString();
        }

        public static string GetTypeName(this TimingSetuSourceType sourceType)
        {
            return sourceType switch
            {
                TimingSetuSourceType.Local => "本地图库",
                TimingSetuSourceType.Lolicon => "Lolicon Api",
                TimingSetuSourceType.Lolisuki => "Lolisuki Api",
                _ => string.Empty
            };
        }

    }
}
