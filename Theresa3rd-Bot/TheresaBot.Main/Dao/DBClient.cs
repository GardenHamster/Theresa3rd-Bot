using SqlSugar.IOC;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.PO;

namespace TheresaBot.Main.Dao
{
    public class DBClient
    {
        /// <summary>
        /// 创建数据库和表
        /// </summary>
        public void CreateDB()
        {
            try
            {
                DbScoped.SugarScope.DbMaintenance.CreateDatabase();
                DbScoped.SugarScope.CodeFirst.InitTables(typeof(BanMemberPO));
                DbScoped.SugarScope.CodeFirst.InitTables(typeof(BanTagPO));
                DbScoped.SugarScope.CodeFirst.InitTables(typeof(DictionaryPO));
                DbScoped.SugarScope.CodeFirst.InitTables(typeof(ImageRecordPO));
                DbScoped.SugarScope.CodeFirst.InitTables(typeof(MessageRecordPO));
                DbScoped.SugarScope.CodeFirst.InitTables(typeof(PixivRecordPO));
                DbScoped.SugarScope.CodeFirst.InitTables(typeof(RequestRecordPO));
                DbScoped.SugarScope.CodeFirst.InitTables(typeof(SubscribeGroupPO));
                DbScoped.SugarScope.CodeFirst.InitTables(typeof(SubscribePO));
                DbScoped.SugarScope.CodeFirst.InitTables(typeof(SubscribeRecordPO));
                DbScoped.SugarScope.CodeFirst.InitTables(typeof(SugarTag));
                DbScoped.SugarScope.CodeFirst.InitTables(typeof(WebsitePO));
            }
            catch (Exception ex)
            {
                LogHelper.FATAL(ex, "自动建表失败");
                throw;
            }
        }

        /// <summary>
        /// 检查表是否存在
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool CheckTable(System.Type type)
        {
            string tableName = DbScoped.SugarScope.EntityMaintenance.GetTableName(type);
            return DbScoped.SugarScope.DbMaintenance.IsAnyTable(tableName, false);
        }

        /// <summary>
        /// 检查表是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool CheckTable<T>()
        {
            string tableName = DbScoped.SugarScope.EntityMaintenance.GetTableName(typeof(T));
            return DbScoped.SugarScope.DbMaintenance.IsAnyTable(tableName, false);
        }

        /// <summary>
        /// 检查表是否存在
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public bool CheckTable(string TableName)
        {
            return DbScoped.SugarScope.DbMaintenance.IsAnyTable(TableName, false);
        }

    }
}
