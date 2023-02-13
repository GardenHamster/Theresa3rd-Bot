using TheresaBot.Main.Model.PO;

namespace TheresaBot.Main.Dao
{
    public class WebsiteDao : DbContext<WebsitePO>
    {
        public WebsitePO getByCode(string code)
        {
            return Db.Queryable<WebsitePO>().Where(o => o.Code == code).Single();
        }
    }
}
