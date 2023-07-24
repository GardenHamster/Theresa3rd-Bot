using JiebaNet.Segmenter;
using TheresaBot.Main.Common;
using TheresaBot.Main.Dao;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Business
{
    public class WordCloudBusiness
    {
        private DictionaryDao dictionaryDao;
        private MessageRecordDao messageRecordDao;

        public WordCloudBusiness()
        {
            dictionaryDao = new DictionaryDao();
            this.messageRecordDao = new MessageRecordDao();
        }

        public List<string> getCloudWords(long groupId, DateTime startTime, DateTime endTime)
        {
            int wordCount = BotConfig.WordCloudConfig.MaxWords;
            var messageRecords = messageRecordDao.getRecords(groupId, startTime, endTime);
            var messageList = messageRecords.Select(o => o.MessageText).ToList();
            var messagesString = string.Join(",", messageList);
            var newWords = LoadWordCloudNewWords();
            var hidWords = LoadWordCloudHiddenWords();
            var hidWordUppers = hidWords.Select(o => o.ToUpper().Trim());
            var jiebaSegmenter = new JiebaSegmenter();
            foreach (var word in newWords) jiebaSegmenter.AddWord(word);
            var wordWeights = new JiebaNet.Analyser.TfidfExtractor(jiebaSegmenter).ExtractTagsWithWeight(messagesString, wordCount);
            var extractWords = wordWeights.OrderByDescending(o => o.Weight).Select(o => o.Word).ToList();
            var returnWords = extractWords.Where(o => hidWordUppers.Contains(o.ToUpper().Trim()) == false).ToList();
            return returnWords;
        }

        public List<string> LoadWordCloudNewWords()
        {
            var dicList = dictionaryDao.getDictionary(WordType.WordCloud, (int)WordCloudSubType.NewWord);
            return dicList.Select(o => o.Words).ToList();
        }

        public List<string> LoadWordCloudHiddenWords()
        {
            var dicList = dictionaryDao.getDictionary(WordType.WordCloud, (int)WordCloudSubType.HiddenWord);
            return dicList.Select(o => o.Words).ToList();
        }





    }
}
