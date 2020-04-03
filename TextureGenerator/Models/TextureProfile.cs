using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextureGenerator.Models
{
	public class TextureProfile
	{
		public TextureProfile()
		{
			this.Blobs = new List<PixelBlob>();
			this.BlobGroups = new List<PixelBlobGroup>();
		}
		public List<PixelBlob> Blobs { get; set; }
		public PixelColor? TransparencyColor { get; set; }
		public List<PixelBlobGroup> BlobGroups { get; set; }
	}
}
