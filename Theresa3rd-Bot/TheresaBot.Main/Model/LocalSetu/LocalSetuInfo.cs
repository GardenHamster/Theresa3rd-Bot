namespace TheresaBot.Main.Model.LocalSetu
{
    public class LocalSetuInfo
    {
        public FileInfo FileInfo { get; set; }
        public DirectoryInfo DirInfo { get; set; }
        public LocalSetuInfo(FileInfo fileInfo, DirectoryInfo dirInfo)
        {
            this.FileInfo = fileInfo;
            this.DirInfo = dirInfo;
        }
    }
}
