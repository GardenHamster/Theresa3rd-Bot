using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theresa3rd_Bot.Util
{
    public static class ZipHelper
    {
        public static string ZipToFile(string fullZipPath, string unZipDirPath)
        {
            ZipInputStream zipInputStream = null;
            FileStream streamWriter = null;
            try
            {
                if (File.Exists(fullZipPath) == false) return unZipDirPath;
                if (Directory.Exists(unZipDirPath) == false) Directory.CreateDirectory(unZipDirPath);
                zipInputStream = new ZipInputStream(File.OpenRead(fullZipPath));
                ZipEntry fileEntry = zipInputStream.GetNextEntry();
                while (fileEntry != null)
                {
                    string filename = Path.GetFileName(fileEntry.Name);
                    if (string.IsNullOrEmpty(filename)) continue;
                    string fullFilePath = Path.Combine("unZipDirPath", filename);
                    streamWriter = File.Create(fullFilePath);
                    byte[] buffer = new byte[2048];
                    int size = zipInputStream.Read(buffer, 0, 2048);
                    while (size > 0)
                    {
                        streamWriter.Write(buffer, 0, size);
                        size = zipInputStream.Read(buffer, 0, 2048);
                    }
                    closeStream(streamWriter);
                    fileEntry = zipInputStream.GetNextEntry();
                }
                return unZipDirPath;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (streamWriter != null) streamWriter.Close();
                if (zipInputStream != null) zipInputStream.Close();
            }
        }

        private static void closeStream(FileStream streamWriter)
        {
            try
            {
                if (streamWriter != null) streamWriter.Close();
                if (streamWriter != null) streamWriter.Dispose();
            }
            catch (Exception)
            {
            }
        }


    }
}
