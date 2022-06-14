using System;
using System.Text;
using Theresa3rd_Bot.Type;
using Theresa3rd_Bot.Type.StepOption;

namespace Theresa3rd_Bot.Util
{
    public static class EnumHelper
    {

        public static string MysSectionOption()
        {
            StringBuilder optionBuilder=new StringBuilder();
            optionBuilder.AppendLine($"{(int)MysSectionType.全部}：{Enum.GetName(typeof(MysSectionType), MysSectionType.全部)}");
            optionBuilder.AppendLine($"{(int)MysSectionType.崩坏3}：{Enum.GetName(typeof(MysSectionType), MysSectionType.崩坏3)}");
            optionBuilder.AppendLine($"{(int)MysSectionType.原神}：{Enum.GetName(typeof(MysSectionType), MysSectionType.原神)}");
            optionBuilder.AppendLine($"{(int)MysSectionType.崩坏2}：{Enum.GetName(typeof(MysSectionType), MysSectionType.崩坏2)}");
            optionBuilder.AppendLine($"{(int)MysSectionType.未定时间簿}：{Enum.GetName(typeof(MysSectionType), MysSectionType.未定时间簿)}");
            optionBuilder.AppendLine($"{(int)MysSectionType.大别野}：{Enum.GetName(typeof(MysSectionType), MysSectionType.大别野)}");
            optionBuilder.AppendLine($"{(int)MysSectionType.星穹铁道}：{Enum.GetName(typeof(MysSectionType), MysSectionType.星穹铁道)}");
            optionBuilder.AppendLine($"{(int)MysSectionType.绝区零}：{Enum.GetName(typeof(MysSectionType), MysSectionType.绝区零)}");
            return optionBuilder.ToString();
        }

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

        

    }
}
