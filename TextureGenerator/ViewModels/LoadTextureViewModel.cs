﻿using System;
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
				async (taskContext) =>
				{
					await Task.Run
					(
						() =>
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
		private TextureProfile LoadTextureProfile(IPixelsSource pixelsSource)
		{
			TextureProfile textureProfile = JsonConvert.DeserializeObject<TextureProfile>(File.ReadAllText(this.TextureProfilePath));
			textureProfile.Blobs.ForEach
			(
				blob =>
				{
					blob.PixelsSource = pixelsSource;
					//blob.Pixels.ForEach(pixel => pixel.PixelColor = blob.BlobColor); //Doing this cut the file size down to 1/5 of the size.
				}
			);
			return textureProfile;
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