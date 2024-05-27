namespace TheresaBot.Core.Model.Infos
{
    public record HttpFileInfo
    {
        public string FullFileName { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }

        public HttpFileInfo(string httpUrl)
        {
            var splitStr = httpUrl.Split('?')[0].Trim();
            var splitArr = splitStr.Split('/');
            FullFileName = splitArr.Last().Trim();
            int pointIndex = FullFileName.IndexOf('.');
            FileName = FullFileName.Substring(0, pointIndex);
            FileExtension = String.Empty;
            if (pointIndex < FullFileName.Length - 1)
            {
                FileExtension = FullFileName.Substring(pointIndex + 1, FullFileName.Length - pointIndex - 1);
            }
        }

    }
}
