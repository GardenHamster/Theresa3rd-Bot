using TheresaBot.Main.Command;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Model.Process;
using TheresaBot.Main.Relay;

namespace TheresaBot.Main.Cache
{
    public static class ProcessCache
    {
        private static Dictionary<long, List<ProcessInfo>> ProcessDic = new Dictionary<long, List<ProcessInfo>>();

        /// <summary>
        /// 创建一个流程
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        /// <exception cref="ProcessException"></exception>
        public static ProcessInfo CreateProcessAsync(GroupCommand command)
        {
            lock (ProcessDic)
            {
                long memberId = command.MemberId;
                long groupId = command.GroupId;
                if (!ProcessDic.ContainsKey(groupId)) ProcessDic[groupId] = new List<ProcessInfo>();
                ProcessInfo processInfo = ProcessDic[groupId].Where(x => x.MemberId == memberId).FirstOrDefault();
                if (processInfo is null)
                {
                    processInfo = new ProcessInfo(command);
                    ProcessDic[groupId].Add(processInfo);
                    return processInfo;
                }
                if (processInfo.IsFinish)
                {
                    processInfo = new ProcessInfo(command);
                    return processInfo;
                }
                throw new ProcessException("你的另一个指令正在执行中");
            }
        }

        public static bool HandleStep(GroupRelay relay, long groupId, long memberId)
        {
            if (string.IsNullOrWhiteSpace(relay.Answer)) return false;
            if (ProcessDic.ContainsKey(groupId) == false) return false;
            List<ProcessInfo> stepInfos = ProcessDic[groupId];
            if (stepInfos is null) return false;
            ProcessInfo stepInfo = ProcessDic[groupId].Where(x => x.MemberId == memberId).FirstOrDefault();
            if (stepInfo is null) return false;
            lock (stepInfo)
            {
                if (stepInfo.IsFinish) return false;
                List<StepInfo> stepDetails = stepInfo.StepInfos;
                if (stepDetails.Count == 0) return false;
                StepInfo stepDetail = stepDetails.Where(x => x.IsFinish == false).FirstOrDefault();
                if (stepDetail is null) return false;
                if (stepDetail.CheckInputAsync(relay).Result == false) return true;
                stepDetail.FinishStep(relay);
                return true;
            }
        }




    }
}
