using TheresaBot.Core.Model.PO;

namespace TheresaBot.Core.Dao
{
    public class WebsiteDao : DbContext<WebsitePO>
    {
        public WebsitePO GetByCode(string code)
        {
            return Db.Queryable<WebsitePO>().Where(o => o.Code == code).Single();
        }
    }
}
