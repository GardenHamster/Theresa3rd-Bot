using TheresaBot.Main.Helper;

namespace TheresaBot.Main.Model.Content
{
    public class PlainContent : BaseContent
    {
        public string Content { get; set; }

        public bool NewLine { get; set; }

        public PlainContent(string content, bool newLine = true)
        {
            this.Content = content;
            this.NewLine = newLine;
        }

        public void FormatNewLine(bool isLast)
        {
            if (isLast)
            {
                Content = Content.RemoveNewLineToEnd();
                return;
            }
            if (NewLine)
            {
                Content = Content.AppendNewLineToEnd();
            }
            else
            {
                Content = Content.RemoveNewLineToEnd();
            }
        }



    }
}
