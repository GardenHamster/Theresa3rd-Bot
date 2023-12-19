using TheresaBot.Main.Command;
using TheresaBot.Main.Relay;

namespace TheresaBot.Main.Model.Process
{
    public class FriendProcess : BaseProcess
    {
        public PrivateCommand FriendCommand { get; private set; }

        public FriendProcess(PrivateCommand command) : base(command.MemberId)
        {
            FriendCommand = command;
        }

        public override FriendStep<string> CreateStep(string question, int waitSecond = 60)
        {
            var stepInfo = new FriendStep<string>(FriendCommand, question, waitSecond, o => Task.FromResult(o));
            StepInfos.Add(stepInfo);
            return stepInfo;
        }

        public override FriendStep<T> CreateStep<T>(string question, Func<string, Task<T>> checkInput, int waitSecond = 60)
        {
            var stepInfo = new FriendStep<T>(FriendCommand, question, waitSecond, checkInput);
            StepInfos.Add(stepInfo);
            return stepInfo;
        }

        public override FriendStep<T> CreateStep<T>(string question, Func<BaseRelay, Task<T>> checkInput, int waitSecond = 60)
        {
            var stepInfo = new FriendStep<T>(FriendCommand, question, waitSecond, checkInput);
            StepInfos.Add(stepInfo);
            return stepInfo;
        }

    }
}
