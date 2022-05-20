using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using System.Collections.Generic;
using System.Linq;
using Theresa3rd_Bot.Model.Cache;

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
        public static StepInfo CreateStep(long groupId, long memberId)
        {
            lock (StepInfoDic)
            {
                if (!StepInfoDic.ContainsKey(groupId)) StepInfoDic[groupId] = new List<StepInfo>();
                StepInfo stepInfo = StepInfoDic[groupId].Where(x => x.MemberId == memberId).FirstOrDefault();
                if (stepInfo == null)
                {
                    stepInfo = new StepInfo(groupId, memberId);
                    StepInfoDic[groupId].Add(stepInfo);
                    return stepInfo;
                }
                if (stepInfo.IsActive == false)
                {
                    stepInfo = new StepInfo(groupId, memberId);
                    return stepInfo;
                }
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static bool HandleStep(IMiraiHttpSession session, IGroupMessageEventArgs args, string message)
        {
            lock (StepInfoDic)
            {
                long groupId = args.Sender.Group.Id;
                long memberId = args.Sender.Id;
                if (StepInfoDic.ContainsKey(groupId) == false) return false;
                List<StepInfo> stepInfos = StepInfoDic[groupId];
                if (stepInfos == null) return false;
                StepInfo stepInfo = StepInfoDic[groupId].Where(x => x.MemberId == memberId).FirstOrDefault();
                if (stepInfo == null) return false;
                if (stepInfo.IsActive == false) return false;
                List<StepDetail> stepDetails = stepInfo.StepDetails;
                if (stepDetails == null || stepDetails.Count == 0) return false;
                StepDetail stepDetail = stepDetails.Where(x => x.IsFinish == false).FirstOrDefault();
                if (stepDetail == null) return false;



                stepDetail.FinishStep(args, message);
                return true;
            }
        }

    }
}
