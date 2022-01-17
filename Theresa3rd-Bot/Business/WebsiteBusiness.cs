using System;
using Theresa3rd_Bot.Dao;
using Theresa3rd_Bot.Model.PO;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Business
{
    public class WebsiteBusiness
    {
        private WebsiteDao websiteDao;

        public WebsiteBusiness()
        {
            websiteDao = new WebsiteDao();
        }

        public WebsitePO updateWebsite(string code, string cookie, int expireMinutes)
        {
            WebsitePO website = getOrInsertWebsite(code);
            website.Cookie = cookie;
            website.UpdateDate = DateTime.Now;
            website.CookieExpireDate = DateTimeHelper.getDateTimeAfterMinutes(expireMinutes);
            websiteDao.Update(website);
            return website;
        }

        public WebsitePO getOrInsertWebsite(string code)
        {
            WebsitePO website = websiteDao.getByCode(code);
            if (website != null) return website;
            website = new WebsitePO();
            website.Code = code;
            website.Cookie = "";
            website.UpdateDate = DateTime.Now;
            website.CookieExpireDate = DateTime.Now;
            return websiteDao.Insert(website);
        }




    }
}
