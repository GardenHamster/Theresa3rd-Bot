using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheresaBot.Main.Helper;

namespace TheresaBot.Main.Model.File
{
    public class HttpFileInfo
    {
        public string FullFileName { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }

        public HttpFileInfo(string httpUrl)
        {
            var splitStr = httpUrl.Split('?')[0].Trim();
            var splitArr = splitStr.Split('/');
            FullFileName = splitArr.Last().Trim();
            int pointIndex = FullFileName.IndexOf('.');
            FileName = FullFileName.Substring(0, pointIndex);
            FileExtension = FullFileName.Substring(pointIndex, FullFileName.Length - pointIndex);
        }

    }
}
