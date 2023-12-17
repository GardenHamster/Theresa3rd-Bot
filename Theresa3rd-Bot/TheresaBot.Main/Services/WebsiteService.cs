using TheresaBot.Main.Dao;
using TheresaBot.Main.Model.PO;

namespace TheresaBot.Main.Services
{
    internal class WebsiteService
    {
        private WebsiteDao websiteDao;

        public WebsiteService()
        {
            websiteDao = new WebsiteDao();
        }

        public WebsitePO UpdateWebsite(string code, string cookie, long userid, int expireSeconds)
        {
            WebsitePO website = GetOrInsert(code);
            website.Cookie = cookie;
            website.UserId = userid;
            website.UpdateDate = DateTime.Now;
            website.CookieExpireDate = DateTime.Now.AddSeconds(expireSeconds);
            websiteDao.Update(website);
            return website;
        }

        public WebsitePO UpdateWebsite(string code, string cookie, long userid, DateTime expireDate)
        {
            WebsitePO website = GetOrInsert(code);
            website.Cookie = cookie;
            website.UserId = userid;
            website.UpdateDate = DateTime.Now;
            website.CookieExpireDate = expireDate;
            websiteDao.Update(website);
            return website;
        }

        public WebsitePO GetOrInsert(string code)
        {
            WebsitePO website = websiteDao.GetByCode(code);
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
