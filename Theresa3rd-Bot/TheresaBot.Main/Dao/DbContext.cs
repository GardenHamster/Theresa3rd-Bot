using SqlSugar;
using SqlSugar.IOC;
using TheresaBot.Main.Model.PO;

namespace TheresaBot.Main.Dao
{
    public class DbContext<T> where T : BasePO, new()
    {
        protected SqlSugarScope Db
        {
            get { return DbScoped.SugarScope; }
        }

        /// <summary>
        /// 根据id查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T GetById(int id)
        {
            return Db.Queryable<T>().Where(o => o.Id == id).First();
        }

        /// <summary>
        /// 添加一条记录
        /// </summary>
        /// <returns></returns>
        public virtual T Insert(T t)
        {
            return Db.Insertable(t).ExecuteReturnEntity();
        }

        /// <summary>
        /// 添加多条记录
        /// </summary>
        /// <returns></returns>
        public virtual List<T> Insert(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = Db.Insertable(list[i]).ExecuteReturnEntity();
            }
            return list;
        }

        /// <summary>
        /// 更新一条记录
        /// </summary>
        /// <returns></returns>
        public virtual int Update(T t)
        {
            return Db.Updateable(t).ExecuteCommand();
        }

        /// <summary>
        /// 根据主键删除
        /// </summary>
        /// <returns></returns>
        public virtual int Delete(T t)
        {
            return Db.Deleteable(t).ExecuteCommand();
        }


    }
}
