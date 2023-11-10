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
        public static ProcessInfo CreateProcess(GroupCommand command)
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
                    ProcessDic[groupId].Remove(processInfo);
                    processInfo = new ProcessInfo(command);
                    ProcessDic[groupId].Add(processInfo);
                    return processInfo;
                }
                throw new ProcessException("你的另一个指令正在执行中");
            }
        }


        /// <summary>
        /// 处理一个步骤
        /// </summary>
        /// <param name="relay"></param>
        /// <returns></returns>
        public static bool HandleStep(GroupRelay relay)
        {
            ProcessInfo processInfo;
            lock (ProcessDic)
            {
                long groupId = relay.GroupId;
                long memberId = relay.MemberId;
                if (ProcessDic.ContainsKey(groupId) == false) return false;
                List<ProcessInfo> stepInfos = ProcessDic[groupId];
                if (stepInfos is null) return false;
                processInfo = ProcessDic[groupId].Where(x => x.MemberId == memberId).FirstOrDefault();
                if (processInfo is null) return false;
                if (processInfo.IsFinish) return false;
            }
            lock (processInfo)
            {
                List<StepInfo> stepInfos = processInfo.StepInfos;
                if (stepInfos.Count == 0) return false;
                StepInfo stepInfo = stepInfos.Where(x => x.IsFinish == false).FirstOrDefault();
                if (stepInfo is null) return false;
                if (stepInfo.CheckInputAsync(relay).Result == false) return true;
                stepInfo.FinishStep(relay);
                return true;
            }
        }

    }
}
