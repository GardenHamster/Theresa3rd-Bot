using System.Text;
using TheresaBot.Main.Type;
using TheresaBot.Main.Type.StepOption;

namespace TheresaBot.Main.Helper
{
    public static class EnumHelper
    {
        public static string PixivSyncOptions()
        {
            StringBuilder optionBuilder = new StringBuilder();
            optionBuilder.AppendLine($"{(int)PixivSyncType.Merge}：合并（只添加不存在的订阅，保留在不关注列表中的订阅）");
            optionBuilder.AppendLine($"{(int)PixivSyncType.Overwrite}：覆盖（移除所有原有的订阅，并将关注列表重新添加到订阅中）");
            return optionBuilder.ToString();
        }

        public static string GroupPushOptions()
        {
            StringBuilder optionBuilder = new StringBuilder();
            optionBuilder.AppendLine($"{(int)GroupPushType.AllGroup}：所有拥有订阅权限的群");
            optionBuilder.AppendLine($"{(int)GroupPushType.CurrentGroup}：当前群");
            return optionBuilder.ToString();
        }

        public static string TagMatchOptions()
        {
            StringBuilder optionBuilder = new StringBuilder();
            optionBuilder.AppendLine($"{(int)TagMatchType.Contain}：部分一致");
            optionBuilder.AppendLine($"{(int)TagMatchType.FullMatch}：全词匹配");
            optionBuilder.AppendLine($"{(int)TagMatchType.Regular}：正则匹配");
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
