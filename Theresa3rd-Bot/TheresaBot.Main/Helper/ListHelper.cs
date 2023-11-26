namespace TheresaBot.Main.Helper
{
    public static class ListHelper
    {
        public static void AddRange<T>(this List<T> list, params T[] arr)
        {
            list.AddRange(arr);
        }

        public static List<string> Trim(this List<string> list)
        {
            if (list is null) return new();
            return list.Where(o => string.IsNullOrWhiteSpace(o) == false).Select(o => o.Trim()).ToList();
        }

        public static List<TSource> Maxs<TSource>(this List<TSource> sources, Func<TSource, int> keySelector)
        {
            if (sources is null || sources.Count == 0) return new();
            var sortList = sources.OrderByDescending(keySelector);
            var maxValue = keySelector(sortList.First());
            var maxsList = new List<TSource>();
            foreach (var item in sortList)
            {
                int value = keySelector(item);
                if (value < maxValue) break;
                maxsList.Add(item);
            }
            return maxsList;
        }

    }
}
