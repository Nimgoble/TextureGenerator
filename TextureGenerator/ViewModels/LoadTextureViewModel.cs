using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using System.IO;
using Newtonsoft.Json;

using TextureGenerator.Models;
using TextureGenerator.Framework;
namespace TextureGenerator.ViewModels
{
	public class LoadTextureViewModel : Screen
	{
		private readonly IWindowManager windowManager;
		public LoadTextureViewModel(IWindowManager windowManager)
		{
			this.windowManager = windowManager;
		}
		#region Methods
		public void Load()
		{
			var image = this.LoadImage();
			var pixelColors = image.CopyPixels();
			var pixelsSource = PixelsSource.FromPixelColors(pixelColors);
			var dialogViewModel = new DialogViewModel();
			Texture outputTexture = null;
			dialogViewModel.AddTask
			(
				(taskContext) =>
				{
					try
					{
						taskContext.UpdateMessage("Trying to load file.");
						var textureProfile = this.LoadTextureProfile(pixelsSource);
						outputTexture = new Texture(image, textureProfile);
					}
					catch (Exception ex)
					{
						taskContext.UpdateMessage($"Error loading file: {ex.Message}");
						return;
					}
					taskContext.UpdateProgress(100);
				}
			);
			this.windowManager.ShowDialog(dialogViewModel);
			this.OutputTexture = outputTexture;
			this.TryClose(this.OutputTexture != null);
		}
		public bool CanLoad { get { return !string.IsNullOrEmpty(this.ImagePath) && !string.IsNullOrEmpty(this.TextureProfilePath); } }
		public void Cancel()
		{
			this.TryClose(false);
		}
		public void GenerateTextureForImage()
		{
			var generateTextureViewModel = new GenerateTextureViewModel(this.windowManager, this.LoadImage());
			var result = this.windowManager.ShowDialog(generateTextureViewModel);
			if (result != true)
				return;
			this.TextureProfilePath = generateTextureViewModel.SavedFile;
		}
		public bool CanGenerateTextureForImage { get { return !string.IsNullOrEmpty(this.ImagePath); } }
		private TextureProfile LoadTextureProfile(IPixelsSource pixelsSource)
		{
			return new TextureProfileReader().ReadTextureProfile(this.TextureProfilePath, pixelsSource);
		}
		private BitmapImage LoadImage()
		{
			if (string.IsNullOrEmpty(this.ImagePath))
				return null;
			return new BitmapImage(new Uri(this.ImagePath));
		}
		#endregion
		#region Properties
		public Texture OutputTexture { get; private set; }
		private string imagePath = string.Empty;
		public string ImagePath 
		{
			get { return this.imagePath; }
			set
			{
				this.imagePath = value;
				NotifyOfPropertyChange(() => ImagePath);
				NotifyOfPropertyChange(() => CanLoad);
				NotifyOfPropertyChange(() => CanGenerateTextureForImage);
			}
		}
		private string textureProfilePath = string.Empty;
		public string TextureProfilePath
		{
			get { return this.textureProfilePath; }
			set
			{
				this.textureProfilePath = value;
				NotifyOfPropertyChange(() => TextureProfilePath);
				NotifyOfPropertyChange(() => CanLoad);
			}
		}
		#endregion
	}
}
