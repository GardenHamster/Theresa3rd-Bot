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
            optionBuilder.AppendLine($"{(int)MysSectionType.BH3}：崩坏3");
            optionBuilder.AppendLine($"{(int)MysSectionType.YS}：原神");
            optionBuilder.AppendLine($"{(int)MysSectionType.BH2}：崩坏2");
            optionBuilder.AppendLine($"{(int)MysSectionType.WD}：未定时间簿");
            optionBuilder.AppendLine($"{(int)MysSectionType.DBY}：大别野");
            optionBuilder.AppendLine($"{(int)MysSectionType.SR}：星穹铁道");
            optionBuilder.AppendLine($"{(int)MysSectionType.ZZZ}：绝区零");
            return optionBuilder.ToString();
        } 


    }
}
