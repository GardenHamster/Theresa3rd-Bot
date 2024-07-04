using SqlSugar;
using SqlSugar.IOC;
using TheresaBot.Core.Model.PO;

namespace TheresaBot.Core.Dao
{
    public class DbContext<T> where T : BasePO, new()
    {
        protected SqlSugarScope Db => DbScoped.SugarScope;

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
        /// 插入或更新，id为0时插入，id大于0时更新
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual T InsertOrUpdate(T t)
        {
            if (t.Id > 0)
            {
                Db.Updateable(t).ExecuteCommand();
                return t;
            }
            else
            {
                return Db.Insertable(t).ExecuteReturnEntity();
            }
        }

        /// <summary>
        /// 查询Id是否存在，如果不存在则插入
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual int InsertIFNotExists(T t)
        {
            var model = GetById(t.Id);
            if (model is null)
            {
                return Db.Insertable(t).OffIdentity().ExecuteCommand();
            }
            else
            {
                return 0;
            }
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

        /// <summary>
        /// 根据id删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual int DeleteById(int id)
        {
            return Db.Deleteable<T>().Where(o => o.Id == id).ExecuteCommand();
        }

        /// <summary>
        /// 根据id删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual int DeleteByIds(List<int> ids)
        {
            return Db.Deleteable<T>(ids).ExecuteCommand();
        }

        /// <summary>
        /// 根据id删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual int DeleteByIds(int[] ids)
        {
            return Db.Deleteable<T>(ids).ExecuteCommand();
        }


    }
}
