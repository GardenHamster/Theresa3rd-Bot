using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheresaBot.Core.Dao;
using TheresaBot.Core.Helper;
using TheresaBot.Core.Model.PO;
using TheresaBot.Core.Services;

namespace TheresaBot.Core.Datas
{
    public static class InitDatas
    {
        public static readonly PixivTagPO PixivTag_R18 = new PixivTagPO(1, "R-18", "R-18", "R-18", "R-18", "R-18");
        public static readonly PixivTagPO PixivTag_Ugoira = new PixivTagPO(2, "うごイラ", "动图", "動圖", "ugoira", "움짤");
        public static readonly PixivTagPO PixivTag_Original = new PixivTagPO(3, "オリジナル", "原创", "原創", "original works", "");
        public static readonly PixivTagPO PixivTag_Girl = new PixivTagPO(100, "女の子", "女孩子", "女孩子", "girl", "여자애");

        public static void Init()
        {
            try
            {
                int count = InitPixivTags();
                if (count > 0) LogHelper.Info("初始数据添加完毕...");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "初始数据添加完毕...");
            }
        }

        private static int InitPixivTags()
        {
            int count = 0;
            var pixivTagContext = new DbContext<PixivTagPO>();
            count += pixivTagContext.InsertOrUpdate(PixivTag_R18);
            count += pixivTagContext.InsertOrUpdate(PixivTag_Ugoira);
            count += pixivTagContext.InsertOrUpdate(PixivTag_Original);
            count += pixivTagContext.InsertOrUpdate(PixivTag_Girl);
            return count;
        }





    }
}
