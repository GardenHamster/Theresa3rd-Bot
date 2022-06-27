using System;
using Theresa3rd_Bot.Dao;
using Theresa3rd_Bot.Model.PO;

namespace Theresa3rd_Bot.Business
{
    public class WebsiteBusiness
    {
        private WebsiteDao websiteDao;

        public WebsiteBusiness()
        {
            websiteDao = new WebsiteDao();
        }

        public WebsitePO updateWebsite(string code, string cookie, long userid, int expireSeconds)
        {
            WebsitePO website = getOrInsertWebsite(code);
            website.Cookie = cookie;
            website.UserId = userid;
            website.UpdateDate = DateTime.Now;
            website.CookieExpireDate = DateTime.Now.AddSeconds(expireSeconds);
            websiteDao.Update(website);
            return website;
        }

        public WebsitePO updateWebsite(string code, string cookie, long userid, DateTime expireDate)
        {
            WebsitePO website = getOrInsertWebsite(code);
            website.Cookie = cookie;
            website.UserId = userid;
            website.UpdateDate = DateTime.Now;
            website.CookieExpireDate = expireDate;
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
            website.UserId = 0;
            website.UpdateDate = DateTime.Now;
            website.CookieExpireDate = DateTime.Now;
            return websiteDao.Insert(website);
        }




    }
}
