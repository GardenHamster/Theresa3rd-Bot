using TheresaBot.Main.Business;
using TheresaBot.Main.Command;
using TheresaBot.Main.Datas;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;

namespace TheresaBot.Main.Handler
{
    internal class SugarTagHandler : BaseHandler
    {
        private SugarTagBusiness sugarTagBusiness;

        public SugarTagHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            sugarTagBusiness = new SugarTagBusiness();
        }

        public async Task BindTagSugarAsync(GroupCommand command)
        {
            try
            {
                var paramArr = command.Params;
                var keyWords = paramArr.Length > 0 ? paramArr[0] : string.Empty;
                var bindTags = paramArr.Length > 1 ? paramArr[1] : string.Empty;
                if (string.IsNullOrWhiteSpace(keyWords))
                {
                    await command.ReplyGroupMessageWithQuoteAsync("没有检测到需要绑定的关键词，请确保指令格式正确");
                    return;
                }
                var keyWordArr = keyWords.Split(new char[] { ',', '，' }).Where(o => !string.IsNullOrWhiteSpace(o)).ToArray();
                if (string.IsNullOrWhiteSpace(bindTags))
                {
                    sugarTagBusiness.DelSugarTags(keyWordArr);
                    SugarTagDatas.LoadDatas();
                    await command.ReplyGroupMessageWithQuoteAsync("已解绑相关标签");
                }
                else
                {
                    sugarTagBusiness.SetSugarTags(keyWordArr, bindTags);
                    SugarTagDatas.LoadDatas();
                    await command.ReplyGroupMessageWithQuoteAsync("标签绑定完毕！");
                }
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, "标签绑定异常");
            }
        }


    }
}
