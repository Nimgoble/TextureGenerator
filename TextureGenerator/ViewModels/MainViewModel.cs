using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Caliburn.Micro;

using TextureGenerator.Framework;
using TextureGenerator.Models;
namespace TextureGenerator.ViewModels
{
	public class MainViewModel : Screen
	{
		private IWindowManager windowManager;
		private DialogViewModel dialogViewModel;
		public MainViewModel(IWindowManager windowManager)
		{
			this.windowManager = windowManager;
			this.UseTolerance = false;
			this.DoReplacementAdditive = true;
		}

		#region Methods
		public void LoadSourceImageFromFile(string fileName)
		{
			this.SourceImage = new BitmapImage(new Uri(fileName));
			this.CopyToOutputImage(this.sourceImage);
		}
		public void SetReplacementColorFromSourceAtPoint(Point point, double actualWidth, double actualHeight)
		{
			var xScale = this.sourceImage.PixelWidth / actualWidth;
			var yScale = this.sourceImage.PixelHeight / actualHeight;
			var actualX = (int)(point.X * xScale);
			var actualY = (int)(point.Y * yScale);
			var pixels = this.sourceImage.CopyPixels();
			var pixel = pixels[actualY, actualX];
			this.SelectedColor = new Color() { R = pixel.Red, G = pixel.Green, B = pixel.Blue, A = pixel.Alpha };
		}
		public void DoReplacement()
		{
			var pixels = this.sourceImage.CopyPixels();
			var outputPixels = this.DoReplacementAdditive.HasValue && this.DoReplacementAdditive.Value ? this.outputImage.CopyPixels() : pixels;
			var tolerance = this.GetTolerance();
			for (int y = 0; y < pixels.GetLength(0); ++y)
			{
				for (int x = 0; x < pixels.GetLength(1); ++x)
				{
					var pixel = pixels[y, x];
					var pixelColor = pixel.ToColor();
					var distance = Math.Abs(this.selectedColor.GetDistance(pixelColor));
					if (this.selectedColor.Equals(pixelColor) || distance <= tolerance)
						outputPixels[y, x] = this.replacementColor.ToPixelColor();
				}
			}
			this.outputImage.PutPixels(outputPixels, 0, 0);
		}
		public void SaveOutput(string fileName)
		{
			if (fileName == string.Empty)
				return;
			using (FileStream stream5 = new FileStream(fileName, FileMode.Create))
			{
				PngBitmapEncoder encoder5 = new PngBitmapEncoder();
				encoder5.Frames.Add(BitmapFrame.Create(this.outputImage));
				encoder5.Save(stream5);
			}
		}
		public void DrawRandomStuff()
		{
			DrawingVisual dv = new DrawingVisual();
			var src = this.outputImage;
			using (DrawingContext dc = dv.RenderOpen())
			{
				//this.DrawingAlgorithmOne(dc, src);
				this.DrawingAlgorithmTwo(dc, src);
			}
			RenderTargetBitmap rtb = new RenderTargetBitmap(src.PixelWidth, src.PixelHeight, 96, 96, PixelFormats.Pbgra32);
			rtb.Render(dv);
			this.CopyToOutputImage(rtb);
		}
		public void DrawDividingLine()
		{
			if (this.transparencyColor == null)
				return;
			var pixels = this.sourceImage.CopyPixels();
			var blobYs = new List<Tuple<int, int>>();
			var inBlob = false;
			var startY = 0;
			for (int y = 0; y < pixels.GetLength(0); ++y)
			{
				var hadColor = false;
				for (int x = 0; x < pixels.GetLength(1); ++x)
				{
					var pixel = pixels[y, x];
					var pixelColor = pixel.ToColor();
					if (pixelColor.Equals(this.transparencyColor))
						continue;
					hadColor = true;
					//var distance = Math.Abs(this.selectedColor.GetDistance(pixelColor));
					//if (this.selectedColor.Equals(pixelColor) || distance <= tolerance)
					//	outputPixels[y, x] = this.replacementColor.ToPixelColor();

				}
				if (hadColor && !inBlob)
				{
					inBlob = true;
					startY = y;
				}

				if (!hadColor && inBlob)
				{
					inBlob = false;
					blobYs.Add(new Tuple<int, int>(startY, y));
				}
			}

			if (blobYs.Count < 2)
				return;

			DrawingVisual dv = new DrawingVisual();
			var src = this.outputImage;
			using (DrawingContext dc = dv.RenderOpen())
			{
				dc.DrawImage(src, new Rect(0, 0, src.PixelWidth, src.PixelHeight));
				for (int i = 0; i < blobYs.Count - 1; ++i)
				{
					var current = blobYs[i];
					var next = blobYs[i + 1];
					var difference = next.Item1 - current.Item2;
					var y = next.Item1 - (difference / 2);
					dc.DrawLine(new Pen(Brushes.White, 1), new Point(0, y), new Point(src.Width - 1, y));
				}
			}
			RenderTargetBitmap rtb = new RenderTargetBitmap(src.PixelWidth, src.PixelHeight, 96, 96, PixelFormats.Pbgra32);
			rtb.Render(dv);
			this.CopyToOutputImage(rtb);
		}
		public void SetTransparencyColorToSelectedColor()
		{
			if (this.selectedColor == null)
				return;
			this.TransparencyColor = this.selectedColor;
		}
		public void TestTheTexture()
		{
			if (this.selectedColor == null || this.dialogViewModel != null)
				return;
			var texture = new Texture(this.sourceImage, this.selectedColor);
			this.dialogViewModel = new DialogViewModel();
			var pixelColors = this.sourceImage.CopyPixels();
			this.dialogViewModel.AddTask(async (taskContext) => await texture.ProcessSource(pixelColors, this.dialogViewModel, taskContext));
			this.windowManager.ShowDialog(this.dialogViewModel);
			this.dialogViewModel = null;
			DrawingVisual dv = new DrawingVisual();
			var src = this.outputImage;
			using (DrawingContext dc = dv.RenderOpen())
			{
				dc.DrawImage(src, new Rect(0, 0, src.PixelWidth, src.PixelHeight));
				texture.Blobs.ForEach
				(
					blob =>
					{
						var brush = new SolidColorBrush(Colors.White);
						var pen = new Pen(brush, 5);
						var size = new Size(1, 1);
						blob.Border.ForEach(pixel => dc.DrawRectangle(brush, pen, new Rect(pixel.Position, size)));
					}
				);
			}
			RenderTargetBitmap rtb = new RenderTargetBitmap(src.PixelWidth, src.PixelHeight, 96, 96, PixelFormats.Pbgra32);
			rtb.Render(dv);
			this.CopyToOutputImage(rtb);
		}
		private void CopyToOutputImage(BitmapSource source)
		{
			// Quick and dirty, get the BitmapSource from an existing <Image> element
			// in the XAML

			// Calculate stride of source
			int stride = source.PixelWidth * (source.Format.BitsPerPixel / 8);

			// Create data array to hold source pixel data
			byte[] data = new byte[stride * source.PixelHeight];

			// Copy source image pixels to the data array
			source.CopyPixels(data, stride, 0);

			// Create WriteableBitmap to copy the pixel data to.      
			WriteableBitmap target = new WriteableBitmap(
			  source.PixelWidth,
			  source.PixelHeight,
			  source.DpiX, source.DpiY,
			  source.Format, null);

			// Write the pixel data to the WriteableBitmap.
			target.WritePixels(
			  new Int32Rect(0, 0, source.PixelWidth, source.PixelHeight),
			  data, stride, 0);

			this.OutputImage = target;
		}
		private void DrawingAlgorithmOne(DrawingContext dc, BitmapSource src)
		{
			var rand = new Random();
			dc.DrawImage(src, new Rect(0, 0, src.PixelWidth, src.PixelHeight));
			//dc.DrawRectangle(Brushes.Green, null, new Rect(20, 20, 150, 100));
			dc.DrawEllipse(Brushes.White, new Pen(), new Point(rand.Next(0, 1023), rand.Next(0, 1023)), rand.Next(1, 50), rand.Next(1, 50));
		}

		private void DrawingAlgorithmTwo(DrawingContext dc, BitmapSource src)
		{
			var rand = new Random();
			dc.DrawImage(src, new Rect(0, 0, src.PixelWidth, src.PixelHeight));
			var centerPoint = new Point(rand.Next(0, 1023), rand.Next(0, 1023));
			var width = rand.Next(1, 50);
			var height = rand.Next(1, 50);
			var numberOfDraws = Math.Min(width, height);
			var pixels = src.CopyPixels();
			var colorAtCenter = pixels[(int)centerPoint.Y, (int)centerPoint.X].ToColor();
			var randomColor = Color.FromArgb(255, (byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255));
			for (int i = 0; i < numberOfDraws; ++i)
			{
				var blendPercentage = (double)((double)(numberOfDraws - i) / (double)numberOfDraws);
				var drawColor = colorAtCenter.Blend(randomColor, blendPercentage);
				dc.DrawEllipse(new SolidColorBrush(drawColor), new Pen(), centerPoint, width - i, height - i);
			}
		}
		private int GetTolerance()
		{
			if (this.UseTolerance != true)
				return 0;
			int tolerance = 35000;
			if (!Int32.TryParse(this.Tolerance, out tolerance))
				tolerance = 35000;
			return tolerance;
		}
		#endregion

		#region Properties
		private BitmapImage sourceImage;
		public BitmapImage SourceImage 
		{ 
			get { return this.sourceImage; } 
			protected set
			{
				this.sourceImage = value;
				NotifyOfPropertyChange(() => SourceImage);
			}
		}
		private WriteableBitmap outputImage;
		public WriteableBitmap OutputImage
		{
			get { return this.outputImage; }
			protected set
			{
				this.outputImage = value;
				NotifyOfPropertyChange(() => OutputImage);
			}
		}
		private Color replacementColor;
		public Color ReplacementColor
		{
			get { return this.replacementColor; }
			set
			{
				this.replacementColor = value;
				NotifyOfPropertyChange(() => ReplacementColor);
			}
		}
		private Color selectedColor;
		public Color SelectedColor
		{
			get { return this.selectedColor; }
			set
			{
				this.selectedColor = value;
				NotifyOfPropertyChange(() => SelectedColor);
			}
		}
		private Color transparencyColor;
		public Color TransparencyColor
		{
			get { return this.transparencyColor; }
			set
			{
				this.transparencyColor = value;
				NotifyOfPropertyChange(() => TransparencyColor);
			}
		}
		public bool? DoReplacementAdditive { get; set; }
		public bool? UseTolerance { get; set; }
		public string Tolerance { get; set; }
		#endregion
	}
}
