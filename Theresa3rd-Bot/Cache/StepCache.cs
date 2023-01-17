using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theresa3rd_Bot.Model.Cache;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Cache
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
        public static async Task<StepInfo> CreateStepAsync(IMiraiHttpSession session, IGroupMessageEventArgs args, bool isRemindTimeout = true)
        {
            lock (StepInfoDic)
            {
                long memberId = args.Sender.Id;
                long groupId = args.Sender.Group.Id;
                if (!StepInfoDic.ContainsKey(groupId)) StepInfoDic[groupId] = new List<StepInfo>();
                StepInfo stepInfo = StepInfoDic[groupId].Where(x => x.MemberId == memberId).FirstOrDefault();
                if (stepInfo == null)
                {
                    stepInfo = new StepInfo(groupId, memberId, isRemindTimeout);
                    StepInfoDic[groupId].Add(stepInfo);
                    return stepInfo;
                }
                if (stepInfo.IsActive == false)
                {
                    StepInfoDic[groupId].Remove(stepInfo);
                    stepInfo = new StepInfo(groupId, memberId, isRemindTimeout);
                    StepInfoDic[groupId].Add(stepInfo);
                    return stepInfo;
                }
            }
            await session.SendGroupMessageWithAtAsync(args, new PlainMessage(" 你的一个指令正在执行中"));
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static bool HandleStep(IMiraiHttpSession session, IGroupMessageEventArgs args, string value)
        {
            long memberId = args.Sender.Id;
            long groupId = args.Sender.Group.Id;
            if (StepInfoDic.ContainsKey(groupId) == false) return false;
            List<StepInfo> stepInfos = StepInfoDic[groupId];
            if (stepInfos == null) return false;
            StepInfo stepInfo = StepInfoDic[groupId].Where(x => x.MemberId == memberId).FirstOrDefault();
            if (stepInfo == null) return false;
            lock (stepInfo)
            {
                if (stepInfo.IsActive == false) return false;
                List<StepDetail> stepDetails = stepInfo.StepDetails;
                if (stepDetails == null || stepDetails.Count == 0) return false;
                StepDetail stepDetail = stepDetails.Where(x => x.IsFinish == false).FirstOrDefault();
                if (stepDetail == null) return false;
                if (stepDetail.CheckInput != null && stepDetail.CheckInput(session, args, value).Result == false) return true;
                stepDetail.FinishStep(args, value);
                return true;
            }
        }


    }
}
