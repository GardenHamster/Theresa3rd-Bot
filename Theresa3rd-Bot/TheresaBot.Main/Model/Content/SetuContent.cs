using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheresaBot.Main.Model.Content
{
    public class SetuContent
    {
        public List<BaseContent> SetuInfos { get; set; }

        public List<FileInfo> SetuImages { get; set; }

        public SetuContent(BaseContent setuInfo, List<FileInfo> images)
        {
            this.SetuInfos = new() { setuInfo };
            this.SetuImages = images;
        }

        public SetuContent(List<BaseContent> setuInfos, List<FileInfo> images)
        {
            this.SetuInfos = setuInfos;
            this.SetuImages = images;
        }

        public SetuContent(string setuInfo, List<FileInfo> images)
        {
            this.SetuInfos = new() { new PlainContent(setuInfo) };
            this.SetuImages = images;
        }

    }
}
