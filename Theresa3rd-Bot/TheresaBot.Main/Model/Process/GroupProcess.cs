using TheresaBot.Main.Command;
using TheresaBot.Main.Relay;

namespace TheresaBot.Main.Model.Process
{
    public class GroupProcess : BaseProcess
    {
        public GroupCommand GroupCommand { get; set; }

        public GroupProcess(GroupCommand command) : base(command.MemberId)
        {
            GroupCommand = command;
        }

        public override GroupStep<string> CreateStep(string question, int waitSecond = 60)
        {
            var stepInfo = new GroupStep<string>(GroupCommand, question, waitSecond, o => Task.FromResult(o));
            StepInfos.Add(stepInfo);
            return stepInfo;
        }

        public override GroupStep<T> CreateStep<T>(string question, Func<string, Task<T>> checkInput, int waitSecond = 60)
        {
            var stepInfo = new GroupStep<T>(GroupCommand, question, waitSecond, checkInput);
            StepInfos.Add(stepInfo);
            return stepInfo;
        }

        public override GroupStep<T> CreateStep<T>(string question, Func<BaseRelay, Task<T>> checkInput, int waitSecond = 60)
        {
            var stepInfo = new GroupStep<T>(GroupCommand, question, waitSecond, checkInput);
            StepInfos.Add(stepInfo);
            return stepInfo;
        }

    }
}
