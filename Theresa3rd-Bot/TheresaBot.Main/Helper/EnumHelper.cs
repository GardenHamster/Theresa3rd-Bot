using TheresaBot.Main.Model.VO;
using TheresaBot.Main.Type;
using TheresaBot.Main.Type.GameOptions;
using TheresaBot.Main.Type.StepOptions;

namespace TheresaBot.Main.Helper
{
    public static class EnumHelper
    {
        public static Dictionary<int, string> PixivSyncOptions => new()
        {
            {(int)PixivSyncType.Merge, "合并（只添加不存在的订阅，保留在不关注列表中的订阅）"},
            {(int)PixivSyncType.Overwrite, "覆盖（移除所有原有的订阅，并将关注列表重新添加到订阅中）"},
        };

        public static Dictionary<int, string> GroupPushOptions => new()
        {
            {(int)PushType.SubscribableGroup, "所有拥有订阅权限的群"},
            {(int)PushType.CurrentGroup, "当前群"},
        };

        public static Dictionary<int, string> TagMatchOptions => new()
        {
            {(int)TagMatchType.Contain, "部分一致"},
            {(int)TagMatchType.FullMatch, "全词匹配"},
            {(int)TagMatchType.Regular, "正则匹配"}
        };

        public static Dictionary<int, string> ResendOptions => new()
        {
            {(int)ResendType.None, "不重发"},
            {(int)ResendType.WithoutImg, "不带图片重发"},
            {(int)ResendType.Rotate180, "旋转180度重发"},
            {(int)ResendType.Blur, "高斯模糊后重发"}
        };

        public static Dictionary<int, string> PixivRandomOptions => new()
        {
            {(int)PixivRandomType.RandomTag, "随机指定标签中的作品"},
            {(int)PixivRandomType.RandomSubscribe, "随机订阅中的作品"},
            {(int)PixivRandomType.RandomFollow, "随机关注画师的作品"},
            {(int)PixivRandomType.RandomBookmark, "随机收藏中的作品"},
        };

        public static Dictionary<int, string> PixivUserScanOptions => new()
        {
            {(int)PixivUserScanType.ScanFollow, "扫描Pixiv中关注用户的最新作品"},
            {(int)PixivUserScanType.ScanSubscribe, "扫描订阅命令中订阅用户的最新作品"},
        };

        public static Dictionary<int, string> TimingSetuSourceOptions => new()
        {
            {(int)TimingSetuSourceType.Local, "本地图库"},
            {(int)TimingSetuSourceType.Lolicon, "Lolicon Api"},
            {(int)TimingSetuSourceType.Lolisuki, "Lolisuki Api"},
        };

        public static Dictionary<int, string> PixivRankingSortOptions => new()
        {
            {(int)PixivRankingSortType.BookMark, "收藏数倒序排序"},
            {(int)PixivRankingSortType.BookMarkRate, "收藏率倒序排序"},
            {(int)PixivRankingSortType.Ranking, "点赞数倒序排序"},
            {(int)PixivRankingSortType.RankingRate, "点赞率倒序排序"},
        };

        public static Dictionary<int, string> DictionaryTypeOptions => new()
        {
            {(int)DictionaryType.WordCloud, "词云"}
        };

        public static Dictionary<int, string> WordcloudTypeOptions => new()
        {
            {(int)WordCloudType.NewWord, "新词"},
            {(int)WordCloudType.HiddenWord, "隐藏词"}
        };

        public static Dictionary<int, string> UCGameModes => new()
        {
            {(int)UCGameMode.Standard, "默认(3平1卧)"},
            {(int)UCGameMode.Customize, "自定义"},
            {(int)UCGameMode.Free, "自由加入"},
        };

        public static List<OptionVo> ToOptionList(this Dictionary<int, string> options)
        {
            return options.Select(o => new OptionVo(o.Key, o.Value)).ToList();
        }

        public static List<OptionVo> AddSubOptions(this List<OptionVo> optionList, int optionValue, Dictionary<int, string> subOptions)
        {
            var parentOptions = optionList.FirstOrDefault(o => o.Value == optionValue);
            if (parentOptions is not null) parentOptions.AddSubOptions(subOptions);
            return optionList;
        }

        public static string GetOptionName(this Dictionary<int, string> options, Enum value)
        {
            int intValue = Convert.ToInt32(value);
            if (options.ContainsKey(intValue))
            {
                return options[intValue];
            }
            else
            {
                return string.Empty;
            }
        }

    }
}
