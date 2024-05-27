using TheresaBot.Core.Helper;

namespace TheresaBot.Core.Datas
{
    public static class DataManager
    {
        public static void LoadInitDatas()
        {
            CountDatas.LoadDatas();
            RunningDatas.LoadDatas();
            RunningDatas.SaveDatas();
            WebsiteDatas.LoadWebsite();
            SubscribeDatas.LoadSubscribeTask();
            BanTagDatas.LoadDatas();
            LogHelper.Info("加载屏蔽标签列表完毕...");
            BanPixiverDatas.LoadDatas();
            LogHelper.Info("加载屏蔽画师列表完毕...");
            BanMemberDatas.LoadDatas();
            LogHelper.Info("加载屏蔽用户列表完毕...");
            SugarTagDatas.LoadDatas();
        }

    }
}
