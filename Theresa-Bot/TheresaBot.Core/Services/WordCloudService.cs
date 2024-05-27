using JiebaNet.Segmenter;
using TheresaBot.Core.Common;
using TheresaBot.Core.Dao;
using TheresaBot.Core.Type;

namespace TheresaBot.Core.Services
{
    public class WordCloudService
    {
        private DictionaryDao dictionaryDao;
        private MessageRecordDao messageRecordDao;

        public WordCloudService()
        {
            dictionaryDao = new DictionaryDao();
            messageRecordDao = new MessageRecordDao();
        }

        public List<string> GetWords(long groupId, DateTime startTime, DateTime endTime)
        {
            int wordCount = BotConfig.WordCloudConfig.MaxWords;
            var messageRecords = messageRecordDao.GetRecords(groupId, startTime, endTime);
            var messageList = messageRecords.Select(o => o.MessageText).ToList();
            var messagesString = string.Join(",", messageList);
            var newWords = LoadNewWords();
            var hidWords = LoadHiddenWords();
            var hidWordUppers = hidWords.Select(o => o.ToUpper().Trim());
            var jiebaSegmenter = new JiebaSegmenter();
            foreach (var word in newWords) jiebaSegmenter.AddWord(word);
            var wordWeights = new JiebaNet.Analyser.TfidfExtractor(jiebaSegmenter).ExtractTagsWithWeight(messagesString, wordCount);
            var extractWords = wordWeights.OrderByDescending(o => o.Weight).Select(o => o.Word).ToList();
            var returnWords = extractWords.Where(o => hidWordUppers.Contains(o.ToUpper().Trim()) == false).ToList();
            return returnWords;
        }

        public List<string> LoadNewWords()
        {
            var newWords = dictionaryDao.GetDictionary(DictionaryType.WordCloud, (int)WordCloudType.NewWord);
            var hiddenWords = dictionaryDao.GetDictionary(DictionaryType.WordCloud, (int)WordCloudType.HiddenWord);
            return newWords.Concat(hiddenWords).Select(o => o.Words).Distinct().ToList();
        }

        public List<string> LoadHiddenWords()
        {
            var dicList = dictionaryDao.GetDictionary(DictionaryType.WordCloud, (int)WordCloudType.HiddenWord);
            return dicList.Select(o => o.Words).ToList();
        }

    }
}
