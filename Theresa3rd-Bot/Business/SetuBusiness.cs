using System;
using System.Text;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Model.Pixiv;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Business
{
    public abstract class SetuBusiness
    {
        /// <summary>
        /// 根据配置文件设置的图片大小获取图片下载地址
        /// </summary>
        /// <param name="pixivWorkInfo"></param>
        /// <returns></returns>
        public string getDownImgUrl(string originalUrl)
        {
            string imgSize = BotConfig.GeneralConfig.PixivImgSize?.ToLower();
            if (imgSize == "original") return originalUrl;
            if (imgSize == "regular") return originalUrl.ToRegularUrl();
            if (imgSize == "small") return originalUrl.ToSmallUrl();
            if (imgSize == "thumb") return originalUrl.ToThumbUrl();
            return originalUrl.ToThumbUrl();
        }

    }
}
