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
                string keyWords = string.Empty;
                string bindTags = string.Empty;
                if (command.Params.Length >= 2)
                {
                    keyWords = command.Params[0];
                    bindTags = command.KeyWord.TakeAfter(" ");
                }
                else
                {
                    var processInfo = ProcessCache.CreateProcess(command);
                    var keyWordsStep = processInfo.CreateStep("请在60秒内发送关键词，多个关键词之间用逗号隔开", CheckTextAsync);
                    var bindTagsStep = processInfo.CreateStep("请在60秒内发送需要绑定的的Pixiv标签，多个标签之间用逗号隔开", CheckTextAsync);
                    await processInfo.StartProcessing();
                    keyWords = keyWordsStep.Answer;
                    bindTags = bindTagsStep.Answer;
                }
                var keyWordArr = keyWords.Split(new char[] { ',', '，' }).Where(o => !string.IsNullOrWhiteSpace(o)).ToArray();
                sugarTagService.SetSugarTags(keyWordArr, bindTags);
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
                string keyWords = command.KeyWord;
                if (string.IsNullOrWhiteSpace(keyWords))
                {
                    await command.ReplyGroupMessageWithAtAsync("没有检测到要解除绑定的标签，请确保指令格式正确");
                    return;
                }
                var keyWordArr = keyWords.Split(new char[] { ',', '，' }).Where(o => !string.IsNullOrWhiteSpace(o)).ToArray();
                sugarTagService.DelSugarTags(keyWordArr);
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
