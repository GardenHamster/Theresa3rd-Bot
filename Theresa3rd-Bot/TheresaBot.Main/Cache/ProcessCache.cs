using TheresaBot.Main.Command;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Model.Process;
using TheresaBot.Main.Relay;

namespace TheresaBot.Main.Cache
{
    public static class ProcessCache
    {
        private static Dictionary<long, FriendProcess> FriendProcessDic = new Dictionary<long, FriendProcess>();

        private static Dictionary<long, List<GroupProcess>> GroupProcessDic = new Dictionary<long, List<GroupProcess>>();

        /// <summary>
        /// 创建一个流程
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        /// <exception cref="ProcessException"></exception>
        public static GroupProcess CreateProcess(GroupCommand command)
        {
            lock (GroupProcessDic)
            {
                long memberId = command.MemberId;
                long groupId = command.GroupId;
                if (!GroupProcessDic.ContainsKey(groupId)) GroupProcessDic[groupId] = new List<GroupProcess>();
                GroupProcess processInfo = GroupProcessDic[groupId].Where(x => x.MemberId == memberId).FirstOrDefault();
                if (processInfo is null)
                {
                    processInfo = new GroupProcess(command);
                    GroupProcessDic[groupId].Add(processInfo);
                    return processInfo;
                }
                if (processInfo.IsFinish)
                {
                    GroupProcessDic[groupId].Remove(processInfo);
                    processInfo = new GroupProcess(command);
                    GroupProcessDic[groupId].Add(processInfo);
                    return processInfo;
                }
                throw new ProcessException("你的另一个指令正在执行中");
            }
        }

        /// <summary>
        /// 创建一个流程
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        /// <exception cref="ProcessException"></exception>
        public static FriendProcess CreateProcess(FriendCommand command)
        {
            lock (FriendProcessDic)
            {
                long memberId = command.MemberId;
                FriendProcess processInfo = FriendProcessDic.ContainsKey(memberId) ? FriendProcessDic[memberId] : null;
                if (processInfo is null || processInfo.IsFinish)
                {
                    processInfo = new FriendProcess(command);
                    FriendProcessDic[memberId] = processInfo;
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
            GroupProcess processInfo;
            lock (GroupProcessDic)
            {
                long groupId = relay.GroupId;
                long memberId = relay.MemberId;
                if (GroupProcessDic.ContainsKey(groupId) == false) return false;
                List<GroupProcess> stepInfos = GroupProcessDic[groupId];
                if (stepInfos is null) return false;
                processInfo = GroupProcessDic[groupId].Where(x => x.MemberId == memberId).FirstOrDefault();
                if (processInfo is null) return false;
                if (processInfo.IsFinish) return false;
            }
            lock (processInfo)
            {
                List<BaseStep> stepInfos = processInfo.StepInfos;
                if (stepInfos.Count == 0) return false;
                BaseStep stepInfo = stepInfos.Where(x => x.IsFinish == false).FirstOrDefault();
                if (stepInfo is null) return false;
                if (stepInfo.CheckInputAsync(relay).Result == false) return true;
                stepInfo.Finish(relay);
                return true;
            }
        }

        /// <summary>
        /// 处理一个步骤
        /// </summary>
        /// <param name="relay"></param>
        /// <returns></returns>
        public static bool HandleStep(FriendRelay relay)
        {
            FriendProcess processInfo;
            lock (FriendProcessDic)
            {
                long memberId = relay.MemberId;
                if (FriendProcessDic.ContainsKey(memberId) == false) return false;
                processInfo = FriendProcessDic.ContainsKey(memberId) ? FriendProcessDic[memberId] : null;
                if (processInfo is null) return false;
                if (processInfo.IsFinish) return false;
            }
            lock (processInfo)
            {
                List<BaseStep> stepInfos = processInfo.StepInfos;
                if (stepInfos.Count == 0) return false;
                BaseStep stepInfo = stepInfos.Where(x => x.IsFinish == false).FirstOrDefault();
                if (stepInfo is null) return false;
                if (stepInfo.CheckInputAsync(relay).Result == false) return true;
                stepInfo.Finish(relay);
                return true;
            }
        }

    }
}
