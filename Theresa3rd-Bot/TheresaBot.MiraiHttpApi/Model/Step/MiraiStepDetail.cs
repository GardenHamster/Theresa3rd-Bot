using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheresaBot.Main.Command;
using TheresaBot.Main.Model.Step;
using TheresaBot.Main.Relay;

namespace TheresaBot.MiraiHttpApi.Model.Step
{
    public class MiraiStepDetail : StepDetail
    {
        public MiraiStepDetail(int waitSecond, string question, Func<GroupCommand, GroupRelay, Task<bool>> checkInput)
            : base(waitSecond, question, checkInput)
        {

        }

        public override List<string> GetReplyImageUrls()
        {

        }
    }
}
