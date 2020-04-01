using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using Newtonsoft.Json;

using TextureGenerator.Framework;
using TextureGenerator.Models;
namespace TextureGenerator.ViewModels
{
	public class GenerateTextureViewModel : Screen
	{
		private readonly IWindowManager windowManager;
		private DialogViewModel dialogViewModel = null;
		public GenerateTextureViewModel(IWindowManager windowManager)
		{
			this.windowManager = windowManager;
			this.UseTolerance = false;
		}
		#region Methods
		public void LoadSourceImageFromFile(string fileName)
		{
			this.originalSource = new BitmapImage(new Uri(fileName));
			this.SourceImage = new WriteableBitmap(this.originalSource);
		}
		public void GenerateTextureProfile()
		{
			if (this.transparencyColor == null || this.dialogViewModel != null)
				return;
			var textureProcessor = new TextureProcessor();
			this.dialogViewModel = new DialogViewModel();
			var pixelColors = this.sourceImage.CopyPixels();
			TextureProfile textureProfile = null;
			this.dialogViewModel.AddTask
			(
				async (taskContext) =>
				{
					textureProfile = await textureProcessor.GenerateTextureProfileAsync(pixelColors, this.transparencyColor, this.dialogViewModel, taskContext);
				}
			);
			this.windowManager.ShowDialog(this.dialogViewModel);
			this.dialogViewModel = null;
			this.Texture = new Texture(this.originalSource, textureProfile);
			this.DrawTextureOnOutput();
		}
		public bool CanGenerateTextureProfile { get { return this.SourceImage != null; } }
		public void SetTransparencyColorFromSourceAtPoint(Point point, double actualWidth, double actualHeight)
		{
			var xScale = this.sourceImage.PixelWidth / actualWidth;
			var yScale = this.sourceImage.PixelHeight / actualHeight;
			var actualX = (int)(point.X * xScale);
			var actualY = (int)(point.Y * yScale);
			var pixels = this.sourceImage.CopyPixels();
			var pixel = pixels[actualY, actualX];
			this.TransparencyColor = new Color() { R = pixel.Red, G = pixel.Green, B = pixel.Blue, A = pixel.Alpha };
		}
		public void SaveTextureProfile(string outputFile)
		{
			if (!this.CanWriteTextureProfile || this.dialogViewModel != null)
				return;
			this.dialogViewModel = new DialogViewModel();
			bool success = true;
			this.dialogViewModel.AddTask
			(
				(taskContext) =>
				{
					try
					{
						new TextureProfileWriter().SaveTextureProfile(outputFile, this.texture.GetTextureProfile());
					}
					catch (Exception ex)
					{
						taskContext.UpdateMessage($"Error writing texture profile: {ex.Message}");
						success = false;
					}
					taskContext.UpdateProgress(100);
				}
			);
			this.windowManager.ShowDialog(this.dialogViewModel);
			this.dialogViewModel = null;
			if (success)
				this.TryClose(true);
		}
		public bool CanWriteTextureProfile
		{
			get { return this.Texture != null; }
		}
		public void Cancel()
		{
			this.TryClose(false);
		}
		private void DrawTextureOnOutput()
		{
			if (this.Texture == null)
				return;
			DrawingVisual dv = new DrawingVisual();
			var src = this.sourceImage;
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
		private BitmapImage originalSource = null;
		private WriteableBitmap sourceImage;
		public WriteableBitmap SourceImage
		{
			get { return this.sourceImage; }
			protected set
			{
				this.sourceImage = value;
				NotifyOfPropertyChange(() => SourceImage);
				NotifyOfPropertyChange(() => CanGenerateTextureProfile);
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
		private Texture texture = null;
		public Texture Texture
		{
			get { return texture; }
			private set
			{
				this.texture = value;
				NotifyOfPropertyChange(() => Texture);
				NotifyOfPropertyChange(() => CanWriteTextureProfile);
			}
		}
		public bool? UseTolerance { get; set; }
		public string Tolerance { get; set; }
		#endregion
	}
}
