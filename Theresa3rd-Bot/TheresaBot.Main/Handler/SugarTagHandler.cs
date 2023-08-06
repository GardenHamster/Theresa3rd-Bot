using System.IO;
using TheresaBot.Main.Business;
using TheresaBot.Main.Cache;
using TheresaBot.Main.Command;
using TheresaBot.Main.Datas;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Process;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Handler
{
    internal class SugarTagHandler : BaseHandler
    {
        private SugarTagBusiness sugarTagBusiness;

        public SugarTagHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            sugarTagBusiness = new SugarTagBusiness();
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
                    ProcessInfo processInfo = ProcessCache.CreateProcess(command);
                    StepInfo keyWordsStep = processInfo.CreateStep("请在60秒内发送关键词，多个关键词之间用逗号隔开", CheckTextAsync);
                    StepInfo bindTagsStep = processInfo.CreateStep("请在60秒内发送需要绑定的的Pixiv标签，多个标签之间用逗号隔开", CheckTextAsync);
                    await processInfo.StartProcessing();
                    keyWords = keyWordsStep.AnswerForString();
                    bindTags = bindTagsStep.AnswerForString();
                }
                var keyWordArr = keyWords.Split(new char[] { ',', '，' }).Where(o => !string.IsNullOrWhiteSpace(o)).ToArray();
                sugarTagBusiness.SetSugarTags(keyWordArr, bindTags);
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
                sugarTagBusiness.DelSugarTags(keyWordArr);
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
