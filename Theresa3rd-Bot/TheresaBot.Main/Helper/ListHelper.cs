using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return list.Where(o => string.IsNullOrWhiteSpace(o) == false).Select(o=>o.Trim()).ToList();
        }

    }
}
