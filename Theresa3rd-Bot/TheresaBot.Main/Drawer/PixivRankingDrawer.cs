using SkiaSharp;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Cache;
using TheresaBot.Main.Model.Drawing;
using TheresaBot.Main.Model.Pixiv;

namespace TheresaBot.Main.Drawer
{
    internal class PixivRankingDrawer : BasePreviewDrawer
    {
        public async Task<FileInfo> DrawPreview(PixivRankingInfo rankingInfo, List<PixivRankingDetail> details, string fullSavePath)
        {
            int areaX = 0;
            int areaY = 0;
            int startX = 0;
            int startY = 0;
            var drawingList = details.Select(o => new PixivRankingDrawing(o)).ToList();
            var arrangeList = await ArrangeDrawingAsync(drawingList);
            int maxRow = arrangeList.Max(o => o.Row);
            int headerAreaHeight = HeaderMargin * 2 + HeaderFontSize;
            int explainAreaHeight = ExplainMargin * 2 + ExplainFontSize;
            int workAreaWidth = MaxColumn * CellWidth + (MaxColumn + 1) * CellMargin;
            int workAreaHeight = headerAreaHeight + maxRow * CellHeight + maxRow * CellMargin;
            int watermarkAreaHeight = WatermarkMargin * 2 + WatermarkFontSize;

            int canvasWidth = workAreaWidth;
            int canvasHeight = headerAreaHeight + explainAreaHeight + workAreaHeight + watermarkAreaHeight;

            var imgInfo = new SKImageInfo(canvasWidth, canvasHeight);
            using SKSurface surface = SKSurface.Create(imgInfo);
            SKCanvas canvas = surface.Canvas;
            canvas.Clear(SKColors.LightGray);

            startX = CellMargin;
            startY = areaY;
            DrawHeader(canvas, rankingInfo, startX, startY);
            areaY += headerAreaHeight;

            startX = CellMargin;
            startY = areaY;
            DrawExplanation(canvas, rankingInfo, startX, startY);
            areaY += explainAreaHeight;

            for (int i = 0; i < arrangeList.Count; i++)
            {
                var drawing = arrangeList[i];
                int row = drawing.Row, column = drawing.Column;
                bool isHorizontal = drawing.IsHorizontal;
                SKBitmap originBitmap = drawing.OriginBitmap;
                PixivRankingDetail detail = drawing.Detail.RankingDetail;
                PixivWorkInfo WorkInfo = drawing.Detail.RankingDetail.WorkInfo;
                startX = areaX + CellMargin * column + CellWidth * (column - 1);
                startY = areaY + CellMargin * row + CellHeight * (row - 1);
                DrawImage(canvas, originBitmap, startX, startY, WorkInfo.IsR18, isHorizontal);
                DrawTitle(canvas, detail, startX, startY);
            }
            areaY += workAreaHeight;

            startX = CellMargin;
            startY = areaY;
            DrawWatermark(canvas, startX, startY);
            areaY += watermarkAreaHeight;

            using SKImage image = surface.Snapshot();
            using SKData data = image.Encode(SKEncodedImageFormat.Jpeg, 100);
            using FileStream outputStream = File.OpenWrite(fullSavePath);
            data.SaveTo(outputStream);
            return new FileInfo(fullSavePath);
        }

        private void DrawHeader(SKCanvas canvas, PixivRankingInfo rankingInfo, int startX, int startY)
        {
            int x = startX;
            int y = startY + HeaderMargin + HeaderFontSize;
            string headerText = $"{rankingInfo.RankingDate} {rankingInfo.RankingMode.Name}";
            canvas.DrawText(headerText, new SKPoint(x, y), HeaderPaint);
        }

        private void DrawExplanation(SKCanvas canvas, PixivRankingInfo rankingInfo, int startX, int startY)
        {
            int x = startX;
            int y = startY + ExplainMargin + ExplainFontSize;
            string headerText = $"#排名 PID 点赞率%/收藏率%";
            canvas.DrawText(headerText, new SKPoint(x, y), ExplainPaint);
        }

        private void DrawTitle(SKCanvas canvas, PixivRankingDetail detail, int startX, int startY)
        {
            int x = startX;
            int y = startY;
            PixivRankingContent content = detail.RankingContent;
            string likeRate = detail.WorkInfo.likeRate.toPercent();
            string bookRate = detail.WorkInfo.bookmarkRate.toPercent();
            string drawText = $"#{content.rank} {content.illust_id} {likeRate}/{bookRate}";
            canvas.DrawText(drawText, new SKPoint(x, y), TitlePaint);
        }

        private void DrawImage(SKCanvas canvas, SKBitmap bitmap, int startX, int startY, bool isR18, bool isHorizontal)
        {
            if (bitmap is null) return;
            int x = startX;
            int y = startY + TitleFontSize / 2;
            int drawWidth = isHorizontal ? CellWidth + CellMargin + CellWidth : IllustWidth;
            int drawHeight = IllustHeight;
            double scaleX = Convert.ToDouble(drawWidth) / bitmap.Width;
            double scaleY = Convert.ToDouble(drawHeight) / bitmap.Height;
            double scale = Math.Max(scaleX, scaleY);
            int imgWidth = (int)(bitmap.Width * scale);
            int imgHeight = (int)(bitmap.Height * scale);
            var imgInfo = new SKImageInfo(imgWidth, imgHeight);
            using SKBitmap originBitmap = bitmap;
            using SKBitmap resizeBitmap = originBitmap.Resize(imgInfo, SKFilterQuality.Low);
            using SKBitmap drawBitmap = isR18 ? Blur(resizeBitmap) : resizeBitmap;
            var source = new SKRect(0, 0, drawWidth, drawHeight);
            var dest = new SKRect(x, y, x + drawWidth, y + drawHeight);
            canvas.DrawBitmap(drawBitmap, source, dest);
        }

        private void DrawWatermark(SKCanvas canvas, int startX, int startY)
        {
            int x = startX;
            int y = startY + WatermarkMargin;
            string watermarkText = $"Create by Theresa-Bot v{BotConfig.BotVersion} Doc {BotConfig.BotHomepage}";
            canvas.DrawText(watermarkText, new SKPoint(x, y), WatermarkPaint);
        }

    }
}
