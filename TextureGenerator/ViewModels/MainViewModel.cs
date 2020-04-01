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
using Newtonsoft.Json;
using HelixToolkit.Wpf.SharpDX;

using TextureGenerator.Framework;
using TextureGenerator.Models;
namespace TextureGenerator.ViewModels
{
	public class MainViewModel : Screen
	{
		private IWindowManager windowManager;
		private DialogViewModel dialogViewModel;
		private readonly IEffectsManager effectsManager;
		public MainViewModel(IWindowManager windowManager, IEffectsManager effectsManager)
		{
			this.windowManager = windowManager;
			this.effectsManager = effectsManager;
			this.modelViewerViewModel = new ModelViewerViewModel(windowManager, effectsManager);
			this.DoReplacementAdditive = true;
		}

		#region Methods
		public void LoadSource()
		{
			var loadTextureViewModel = new LoadTextureViewModel(this.windowManager);
			var result = this.windowManager.ShowDialog(loadTextureViewModel);
			if(result == true)
			{
				this.SourceTexture = loadTextureViewModel.OutputTexture;
				this.CopyToOutputImage(this.SourceTexture.Source);
			}
		}
		public void SetReplacementColorFromSourceAtPoint(Point point, double actualWidth, double actualHeight)
		{
			var xScale = this.SourceTexture.Source.PixelWidth / actualWidth;
			var yScale = this.SourceTexture.Source.PixelHeight / actualHeight;
			var actualX = (int)(point.X * xScale);
			var actualY = (int)(point.Y * yScale);
			var pixels = this.SourceTexture.Source.CopyPixels();
			var pixel = pixels[actualY, actualX];
			this.SelectedColor = new Color() { R = pixel.Red, G = pixel.Green, B = pixel.Blue, A = pixel.Alpha };
		}
		public void DoReplacement()
		{
			var pixels = this.SourceTexture.Source.CopyPixels();
			var outputPixels = this.DoReplacementAdditive.HasValue && this.DoReplacementAdditive.Value ? this.outputImage.CopyPixels() : pixels;
			var tolerance = 0;/*this.GetTolerance();*/
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
			this.UpdateModelMaterial();
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
		private void DrawTextureOnOutput()
		{
			if (this.SourceTexture == null)
				return;
			DrawingVisual dv = new DrawingVisual();
			var src = this.outputImage;
			using (DrawingContext dc = dv.RenderOpen())
			{
				dc.DrawImage(src, new Rect(0, 0, src.PixelWidth, src.PixelHeight));
				SourceTexture.Blobs.ForEach
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
			this.UpdateModelMaterial();
		}
		private void UpdateModelMaterial()
		{
			var targetMemoryStream = this.OutputImage.ToMemoryStream();
			this.modelViewerViewModel.TestMaterial = new PhongMaterial()
			{
				DiffuseMap = targetMemoryStream,
				EmissiveMap = targetMemoryStream
			};
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
		#endregion

		#region Properties
		private Texture sourceTexture = null;
		public Texture SourceTexture
		{
			get { return this.sourceTexture; }
			set
			{
				this.sourceTexture = value;
				NotifyOfPropertyChange(() => SourceTexture);
				NotifyOfPropertyChange(() => SourceImage);
			}
		}
		public BitmapSource SourceImage { get { return this.SourceTexture?.Source; } }
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
		public bool? DoReplacementAdditive { get; set; }
		private ModelViewerViewModel modelViewerViewModel = null;
		public ModelViewerViewModel ModelViewerViewModel { get { return this.modelViewerViewModel; } }
		#endregion
	}
}
