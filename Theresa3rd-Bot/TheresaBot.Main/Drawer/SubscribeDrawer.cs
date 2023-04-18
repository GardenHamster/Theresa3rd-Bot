using SkiaSharp;
using TheresaBot.Main.Model.PO;
using TheresaBot.Main.Model.Subscribe;

namespace TheresaBot.Main.Drawer
{
    internal class SubscribeDrawer : BaseDrawer
    {
        private const int canvaPadding = 15;
        private const int TitleMargin = 10;
        private const int TitleFontSize = 30;
        private const int HeaderMargin = 5;
        private const int HeaderFontSize = 18;
        private const int DetailMargin = 5;
        private const int DetailFontSize = 15;

        private const int subIdWidth = 70;
        private const int subGroupWidth = 80;
        private const int subCodeWidth = 120;
        private const int subNameWidth = 400;

        private SKPaint TitlePaint => getDefaultPaint(SKColors.DarkRed, TitleFontSize);
        private SKPaint HeaderPaint => getDefaultPaint(SKColors.Black, HeaderFontSize, true);
        private SKPaint DetailPaint1 => getDefaultPaint(SKColors.DarkGreen, DetailFontSize);
        private SKPaint DetailPaint2 => getDefaultPaint(SKColors.DarkBlue, DetailFontSize);

        public FileInfo DrawSubscribe(List<SubscribeInfo> miyousheSubList, List<SubscribeInfo> pixivUserSubList, List<SubscribeInfo> pixivTagSubList, string fullSavePath)
        {
            int startX = canvaPadding;
            int startY = canvaPadding;
            int miyousheCount = miyousheSubList.Count;
            int pixivUserCount = pixivUserSubList.Count;
            int pixivTagCount = pixivTagSubList.Count;
            int titleAreaHeight = TitleFontSize + TitleMargin * 2;
            int heaerAreaHeight = HeaderFontSize + HeaderMargin * 2;
            int detailAreaHeight = DetailFontSize + DetailMargin * 2;
            int miyousheAreaHeight = titleAreaHeight + heaerAreaHeight + miyousheCount * detailAreaHeight;
            int pixivUserAreaHeight = titleAreaHeight + heaerAreaHeight + pixivUserCount * detailAreaHeight;
            int pixivTagAreaHeight = titleAreaHeight + heaerAreaHeight + pixivTagCount * detailAreaHeight;
            int canvasWidth = canvaPadding * 2 + subIdWidth + subCodeWidth + subNameWidth + subGroupWidth;
            int canvasHeight = canvaPadding * 2 + miyousheAreaHeight + pixivUserAreaHeight + pixivTagAreaHeight;

            var imgInfo = new SKImageInfo(canvasWidth, canvasHeight);
            using SKSurface surface = SKSurface.Create(imgInfo);
            SKCanvas canvas = surface.Canvas;
            canvas.Clear(SKColors.White);

            startY += TitleMargin;
            DrawTitle(canvas, "米游社用户", startX, startY);
            startY += titleAreaHeight;
            DrawHeader(canvas, startX, startY);
            startY += heaerAreaHeight;
            for (int i = 0; i < miyousheSubList.Count; i++)
            {
                DrawDetail(canvas, miyousheSubList[i], i, startX, startY);
                startY = startY + detailAreaHeight;
            }

            startY += TitleMargin;
            DrawTitle(canvas, "Pixiv标签", startX, startY);
            startY += titleAreaHeight;
            DrawHeader(canvas, startX, startY);
            startY += heaerAreaHeight;
            for (int i = 0; i < pixivTagSubList.Count; i++)
            {
                DrawDetail(canvas, pixivTagSubList[i], i, startX, startY);
                startY = startY + detailAreaHeight;
            }

            startY += TitleMargin;
            DrawTitle(canvas, "Pixiv画师", startX, startY);
            startY += titleAreaHeight;
            DrawHeader(canvas, startX, startY);
            startY += heaerAreaHeight;
            for (int i = 0; i < pixivUserSubList.Count; i++)
            {
                DrawDetail(canvas, pixivUserSubList[i], i, startX, startY);
                startY = startY + detailAreaHeight;
            }

            using SKImage image = surface.Snapshot();
            using SKData data = image.Encode(SKEncodedImageFormat.Jpeg, 100);
            using FileStream outputStream = File.OpenWrite(fullSavePath);
            data.SaveTo(outputStream);
            return new FileInfo(fullSavePath);
        }

        private void DrawTitle(SKCanvas canvas, string title, int startX, int startY)
        {
            int x = startX;
            int y = startY + TitleMargin;
            canvas.DrawText(title, new SKPoint(x, y), TitlePaint);
        }

        private void DrawHeader(SKCanvas canvas, int startX, int startY)
        {
            canvas.DrawText("Id", new SKPoint(startX, startY), HeaderPaint);
            startX += subIdWidth;
            canvas.DrawText("Group", new SKPoint(startX, startY), HeaderPaint);
            startX += subGroupWidth;
            canvas.DrawText("Code", new SKPoint(startX, startY), HeaderPaint);
            startX += subCodeWidth;
            canvas.DrawText("Name", new SKPoint(startX, startY), HeaderPaint);
            startX += subNameWidth;
        }

        private void DrawDetail(SKCanvas canvas, SubscribeInfo subInfo, int index, int startX, int startY)
        {
            SKPaint paint = index % 2 == 1 ? DetailPaint1 : DetailPaint2;
            canvas.DrawText(subInfo.SubscribeId.ToString(), new SKPoint(startX, startY), paint);
            startX += subIdWidth;
            canvas.DrawText(subInfo.GroupId == 0 ? "所有群" : "当前群", new SKPoint(startX, startY), paint);
            startX += subGroupWidth;
            canvas.DrawText(subInfo.SubscribeCode, new SKPoint(startX, startY), paint);
            startX += subCodeWidth;
            canvas.DrawText(subInfo.SubscribeName, new SKPoint(startX, startY), paint);
            startX += subNameWidth;
        }




    }
}
