﻿using TheresaBot.Main.Helper;

namespace TheresaBot.Main.Datas
{
    public static class DataManager
    {
        public static void LoadInitDatas()
        {
            RunningDatas.LoadDatas();
            WebsiteDatas.LoadWebsite();
            SubscribeDatas.LoadSubscribeTask();
            BanTagDatas.LoadDatas();
            LogHelper.Info("加载屏蔽标签列表完毕...");
            BanMemberDatas.LoadDatas();
            LogHelper.Info("加载屏蔽用户列表完毕...");
            SugarTagDatas.LoadDatas();
        }

    }
}
