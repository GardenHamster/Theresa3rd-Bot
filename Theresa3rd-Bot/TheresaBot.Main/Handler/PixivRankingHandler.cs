using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheresaBot.Main.Business;
using TheresaBot.Main.Command;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Handler
{
    public class PixivRankingHandler : SetuHandler
    {
        private PixivBusiness pixivBusiness;

        public PixivRankingHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            pixivBusiness = new PixivBusiness();
        }

        public Task sendDailyRanking(GroupCommand command)
        {
            return Task.CompletedTask;
        }

        public Task sendDailyAIRanking(GroupCommand command)
        {
            return Task.CompletedTask;
        }

        public Task sendDailyMaleRanking(GroupCommand command)
        {
            return Task.CompletedTask;
        }

        public Task sendWeeklyRanking(GroupCommand command)
        {
            return Task.CompletedTask;
        }

        public Task sendMonthlyRanking(GroupCommand command)
        {
            return Task.CompletedTask;
        }




    }
}
