using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using TextureGenerator.Framework;
namespace TextureGenerator.Models
{
	public class Texture
	{
		private BitmapSource source;
		private List<PixelBlob> blobs = new List<PixelBlob>();
		private PixelColor? transparencyColor;
		private List<PixelBlobGroup> blobGroups = new List<PixelBlobGroup>();
		public Texture()
		{
			this.source = null;
			this.blobs = new List<PixelBlob>();
			this.transparencyColor = Colors.Transparent.ToPixelColor();
		}
		public Texture(BitmapSource source, Color transparencyColor, List<PixelBlob> blobs, List<PixelBlobGroup> blobGroups)
		{
			this.source = source;
			this.transparencyColor = transparencyColor.ToPixelColor();
			this.blobs = blobs;
			this.blobGroups = blobGroups;
		}
		public Texture(BitmapSource source, TextureProfile profile)
		{
			this.source = source;
			this.transparencyColor = profile.TransparencyColor;
			this.blobs = profile.Blobs;
			this.blobGroups = profile.BlobGroups;
		}
		public List<PixelBlob> Blobs { get { return this.blobs; } }
		public BitmapSource Source { get { return this.source; } }
		public List<PixelBlobGroup> BlobGroups { get { return this.blobGroups; } }
		public TextureProfile GetTextureProfile()
		{
			return new TextureProfile()
			{
				Blobs = this.blobs,
				TransparencyColor = this.transparencyColor,
				BlobGroups = this.blobGroups
			};
		}
	}
}
