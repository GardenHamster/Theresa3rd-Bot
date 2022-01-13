using Theresa3rd_Bot.Model.PO;

namespace Theresa3rd_Bot.Dao
{
    public class WebsiteDao : DbContext<WebsitePO>
    {
        public WebsitePO getByCode(string code)
        {
            return Db.Queryable<WebsitePO>().Where(o => o.Code == code).Single();
        }
    }
}
