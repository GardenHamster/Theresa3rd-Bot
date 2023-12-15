using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Datas;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Services;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Handler
{
    internal class SugarTagHandler : BaseHandler
    {
        private SugarTagService sugarTagService;

        public SugarTagHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            sugarTagService = new SugarTagService();
        }

        public async Task BindPixivTagAsync(GroupCommand command)
        {
            try
            {
                var processInfo = ProcessCache.CreateProcess(command);
                var keyWordsStep = processInfo.CreateStep("请在60秒内发送关键词，多个关键词之间用逗号隔开", WaitParamsAsync);
                var bindTagsStep = processInfo.CreateStep("请在60秒内发送与之绑定的Pixiv标签，多个标签之间用逗号隔开", WaitParamsAsync);
                await processInfo.StartProcessing();
                var keyWords = keyWordsStep.Answer;
                var bindTags = bindTagsStep.Answer;
                sugarTagService.SetSugarTags(keyWords, bindTags.JoinToString(","));
                SugarTagDatas.LoadDatas();
                await command.ReplyGroupMessageWithQuoteAsync("标签绑定完毕！");
            }
            catch (ProcessException ex)
            {
                await command.ReplyGroupMessageWithAtAsync(ex.RemindMessage);
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, "标签绑定异常");
            }
        }

        public async Task UnBindPixivTagAsync(GroupCommand command)
        {
            try
            {
                var keyWords = command.KeyWord.SplitParams();
                if (keyWords.Length == 0)
                {
                    var processInfo = ProcessCache.CreateProcess(command);
                    var keyWordsStep = processInfo.CreateStep("请在60秒内发送需要解绑的关键词，多个关键词之间用逗号隔开", WaitParamsAsync);
                    await processInfo.StartProcessing();
                    keyWords = keyWordsStep.Answer;
                }
                sugarTagService.DelByKeyword(keyWords);
                SugarTagDatas.LoadDatas();
                await command.ReplyGroupMessageWithQuoteAsync("已解绑相关标签");
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, "标签解绑异常");
            }
        }

    }
}
