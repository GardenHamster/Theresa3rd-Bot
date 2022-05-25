using System;
using System.Text;
using Theresa3rd_Bot.Type;

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


    }
}
