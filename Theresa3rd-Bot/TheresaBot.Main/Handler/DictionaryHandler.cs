using TheresaBot.Main.Command;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Services;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Handler
{
    internal class DictionaryHandler : BaseHandler
    {
        private DictionaryService dictionaryService;

        public DictionaryHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            dictionaryService = new DictionaryService();
        }

        public async Task AddCloudWordAsync(GroupCommand command)
        {
            try
            {
                string words = command.KeyWord;
                if (string.IsNullOrEmpty(words))
                {
                    await command.ReplyGroupMessageWithQuoteAsync("没有检测到需要添加的词汇，请确保指令格式正确");
                    return;
                }

                string[] wordArr = words.SplitParams();
                List<DictionaryPO> existsList = new List<DictionaryPO>();
                foreach (string word in wordArr)
                {
                    var dictionary = dictionaryService.GetDictionary(DictionaryType.WordCloud, (int)WordCloudType.NewWord, word.Trim());
                    if (dictionary is not null && dictionary.Count > 0)
                    {
                        existsList.AddRange(dictionary);
                        continue;
                    }
                    dictionaryService.InsertDictionary(DictionaryType.WordCloud, word, (int)WordCloudType.NewWord);
                }

                if (existsList.Count > 0)
                {
                    var existsWords = existsList.Select(o => o.Words).Distinct().ToList();
                    var existsWordStrs = string.Join('，', existsWords);
                    await command.ReplyGroupMessageWithQuoteAsync($"添加完毕！其中词汇：{existsWordStrs}已存在");
                }
                else
                {
                    await command.ReplyGroupMessageWithQuoteAsync("添加完毕！");
                }
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, "添加词汇异常");
            }
        }

        public async Task HideCloudWordAsync(GroupCommand command)
        {
            try
            {
                string words = command.KeyWord;
                if (string.IsNullOrEmpty(words))
                {
                    await command.ReplyGroupMessageWithQuoteAsync("没有检测到需要添加的词汇，请确保指令格式正确");
                    return;
                }

                string[] wordArr = words.SplitParams();
                List<DictionaryPO> existsList = new List<DictionaryPO>();
                foreach (string word in wordArr)
                {
                    var dictionary = dictionaryService.GetDictionary(DictionaryType.WordCloud, (int)WordCloudType.HiddenWord, word.Trim());
                    if (dictionary is not null && dictionary.Count > 0)
                    {
                        existsList.AddRange(dictionary);
                        continue;
                    }
                    dictionaryService.InsertDictionary(DictionaryType.WordCloud, word, (int)WordCloudType.HiddenWord);
                }

                if (existsList.Count > 0)
                {
                    var existsWords = existsList.Select(o => o.Words).Distinct().ToList();
                    var existsWordStrs = string.Join('，', existsWords);
                    await command.ReplyGroupMessageWithQuoteAsync($"隐藏完毕！其中词汇：{existsWordStrs}已隐藏");
                }
                else
                {
                    await command.ReplyGroupMessageWithQuoteAsync("隐藏完毕！");
                }
            }
            catch (Exception ex)
            {
                await LogAndReplyError(command, ex, "隐藏词汇异常");
            }
        }


    }
}
