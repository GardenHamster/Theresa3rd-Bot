﻿using TheresaBot.Main.Business;
using TheresaBot.Main.Command;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Handler
{
    internal class DictionaryHandler : BaseHandler
    {
        private DictionaryBusiness dictionaryBusiness;

        public DictionaryHandler(BaseSession session, BaseReporter reporter) : base(session, reporter)
        {
            dictionaryBusiness = new DictionaryBusiness();
        }

        public async Task AddCloudWordAsync(GroupCommand command)
        {
            try
            {
                string words = command.KeyWord;
                if (string.IsNullOrEmpty(words))
                {
                    await command.ReplyGroupMessageWithAtAsync("没有检测需要添加的词汇，请确保指令格式正确");
                    return;
                }

                string[] wordArr = words.splitParams();
                List<DictionaryPO> existsList = new List<DictionaryPO>();
                foreach (string word in wordArr)
                {
                    var dictionary = dictionaryBusiness.getDictionary(WordType.WordCloud, (int)WordCloudSubType.NewWord, word.Trim());
                    if (dictionary is not null && dictionary.Count > 0)
                    {
                        existsList.AddRange(dictionary);
                        continue;
                    }
                    dictionaryBusiness.addDictionary(WordType.WordCloud, word, (int)WordCloudSubType.NewWord);
                }

                if (existsList.Count > 0)
                {
                    var existsWords = existsList.Select(o => o.Words).Distinct().ToList();
                    var existsWordStrs = string.Join('，', existsWords);
                    await command.ReplyGroupMessageWithAtAsync($"添加完毕！其中词汇：{existsWordStrs}已存在");
                }
                else
                {
                    await command.ReplyGroupMessageWithAtAsync("添加完毕！");
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "AddCloudWordAsync异常");
                await Reporter.SendError(ex, "AddCloudWordAsync异常");
            }
        }

        public async Task HideCloudWordAsync(GroupCommand command)
        {
            try
            {
                string words = command.KeyWord;
                if (string.IsNullOrEmpty(words))
                {
                    await command.ReplyGroupMessageWithAtAsync("没有检测需要添加的词汇，请确保指令格式正确");
                    return;
                }

                string[] wordArr = words.splitParams();
                List<DictionaryPO> existsList = new List<DictionaryPO>();
                foreach (string word in wordArr)
                {
                    var dictionary = dictionaryBusiness.getDictionary(WordType.WordCloud, (int)WordCloudSubType.HiddenWord, word.Trim());
                    if (dictionary is not null && dictionary.Count > 0)
                    {
                        existsList.AddRange(dictionary);
                        continue;
                    }
                    dictionaryBusiness.addDictionary(WordType.WordCloud, word, (int)WordCloudSubType.HiddenWord);
                }

                if (existsList.Count > 0)
                {
                    var existsWords = existsList.Select(o => o.Words).Distinct().ToList();
                    var existsWordStrs = string.Join('，', existsWords);
                    await command.ReplyGroupMessageWithAtAsync($"隐藏完毕！其中词汇：{existsWordStrs}已隐藏");
                }
                else
                {
                    await command.ReplyGroupMessageWithAtAsync("隐藏完毕！");
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "HideCloudWordAsync异常");
                await Reporter.SendError(ex, "HideCloudWordAsync异常");
            }
        }


    }
}
