using Mirai.CSharp.HttpApi.Models.EventArgs;
using System.Collections.Generic;
using System.Linq;
using Theresa3rd_Bot.Model.Cache;

namespace Theresa3rd_Bot.Cache
{
    public static class StepCache
    {
        /// <summary>
        /// 操作步骤字典
        /// </summary>
        private static Dictionary<long, List<StepInfo>> StepInfoDic = new Dictionary<long, List<StepInfo>>();

        /// <summary>
        /// 开始分步执行
        /// </summary>
        /// <param name="stepList"></param>
        /// <returns></returns>
        public static List<StepInfo> StartStep(long groupId,long memberId, StepInfo stepInfo)
        {
            if (!StepInfoDic.ContainsKey(groupId)) StepInfoDic[groupId] = new List<StepInfo>();
            List<StepInfo> stepInfoList = StepInfoDic[groupId];
            StepInfo previous = stepInfoList.Where(o => o.MemberId == memberId).FirstOrDefault();
            if (previous != null)
            {
                previous.IsActive = false;//使上一个分步操作失效
                stepInfoList.Remove(previous);//移除上一个分步操作
            }
            stepInfoList.Add(stepInfo);
            return stepInfoList;



        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void HandleStep(IGroupMessageEventArgs args)
        {

        }


    }
}
