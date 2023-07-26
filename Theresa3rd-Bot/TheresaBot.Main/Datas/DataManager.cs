using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheresaBot.Main.Datas
{
    public static class DataManager
    {
        public static void LoadInitDatas()
        {
            WebsiteDatas.LoadWebsite();
            SubscribeDatas.LoadSubscribeTask();
            BanTagDatas.LoadDatas();
            BanMemberDatas.LoadDatas();
            SugarTagDatas.LoadDatas();
        }

    }
}
