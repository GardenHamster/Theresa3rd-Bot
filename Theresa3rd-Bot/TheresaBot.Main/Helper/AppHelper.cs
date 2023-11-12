using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheresaBot.Main.Helper
{
    public static class AppHelper
    {
        public static string GetWebRootPath()
        {
            var rootPath = Path.Combine(AppContext.BaseDirectory, "wwwroot");
            var distPath = Path.Combine(AppContext.BaseDirectory, "dist");
            if (Directory.Exists(rootPath)) return rootPath;
            if (Directory.Exists(distPath)) return distPath;
            return string.Empty;
        }

    }
}
