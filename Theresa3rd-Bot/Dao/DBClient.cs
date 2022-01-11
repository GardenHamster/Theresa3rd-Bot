using SqlSugar.IOC;
using System;
using Theresa3rd_Bot.Model.PO;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Dao
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
                DbScoped.SugarScope.CodeFirst.InitTables(typeof(WebsitePO));
            }
            catch (Exception ex)
            {
                LogHelper.Info("自动建表失败...");
                LogHelper.Error(ex);
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
