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
		private Color transparencyColor;
		public Texture(BitmapSource source, Color transparencyColor, List<PixelBlob> blobs)
		{
			this.source = source;
			this.transparencyColor = transparencyColor;
			this.blobs = blobs;
		}
		public List<PixelBlob> Blobs { get { return this.blobs; } }
		public BitmapSource Source { get { return this.source; } }
	}
}
