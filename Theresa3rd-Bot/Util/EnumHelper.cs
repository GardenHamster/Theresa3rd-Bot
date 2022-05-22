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
            optionBuilder.AppendLine($"{MysSectionType.BH3}：崩坏3");
            optionBuilder.AppendLine($"{MysSectionType.YS}：原神");
            optionBuilder.AppendLine($"{MysSectionType.BH2}：崩坏2");
            optionBuilder.AppendLine($"{MysSectionType.WD}：未定时间簿");
            optionBuilder.AppendLine($"{MysSectionType.DBY}：大别野");
            optionBuilder.AppendLine($"{MysSectionType.SR}：星穹铁道");
            optionBuilder.Append($"{MysSectionType.ZZZ}：绝区零");
            return optionBuilder.ToString();
        } 


    }
}
