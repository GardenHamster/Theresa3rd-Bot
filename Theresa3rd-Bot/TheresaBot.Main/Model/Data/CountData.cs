using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheresaBot.Main.Model.Data
{
    public record CountData
    {
        public int TotalHandle { get; set; }

        public int TotalPixivPush { get; set; }

        public int TotalPixivScan { get; set; }

        public int TotalPixivScanError { get; set; }

    }
}
