using TheresaBot.Main.Command;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Step;
using TheresaBot.Main.Relay;

namespace TheresaBot.Main.Cache
{
    public static class StepCache
    {
        /// <summary>
        /// key:群号,value:List<StepInfo>
        /// </summary>
        private static Dictionary<long, List<StepInfo>> StepInfoDic = new Dictionary<long, List<StepInfo>>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public static async Task<StepInfo> CreateStepAsync(GroupCommand command, bool isRemindTimeout = true)
        {
            lock (StepInfoDic)
            {
                long memberId = command.MemberId;
                long groupId = command.GroupId;
                if (!StepInfoDic.ContainsKey(groupId)) StepInfoDic[groupId] = new List<StepInfo>();
                StepInfo stepInfo = StepInfoDic[groupId].Where(x => x.MemberId == memberId).FirstOrDefault();
                if (stepInfo is null)
                {
                    stepInfo = new StepInfo(command, isRemindTimeout);
                    StepInfoDic[groupId].Add(stepInfo);
                    return stepInfo;
                }
                if (stepInfo.IsActive == false)
                {
                    StepInfoDic[groupId].Remove(stepInfo);
                    stepInfo = new StepInfo(command, isRemindTimeout);
                    StepInfoDic[groupId].Add(stepInfo);
                    return stepInfo;
                }
                throw new StepException("你的另一个指令正在执行中");
            }
        }


        public static bool HandleStep(GroupRelay relay, long groupId, long memberId)
        {
            if (StepInfoDic.ContainsKey(groupId) == false) return false;
            List<StepInfo> stepInfos = StepInfoDic[groupId];
            if (stepInfos is null) return false;
            StepInfo stepInfo = StepInfoDic[groupId].Where(x => x.MemberId == memberId).FirstOrDefault();
            if (stepInfo is null) return false;
            lock (stepInfo)
            {
                if (stepInfo.IsActive == false) return false;
                List<StepDetail> stepDetails = stepInfo.StepDetails;
                if (stepDetails is null || stepDetails.Count == 0) return false;
                StepDetail stepDetail = stepDetails.Where(x => x.IsFinish == false).FirstOrDefault();
                if (stepDetail is null) return false;
                if (stepDetail.CheckInput != null && stepDetail.CheckInput(stepInfo.GroupCommand, relay).Result == false) return true;
                stepDetail.FinishStep(relay);
                return true;
            }
        }


    }
}
