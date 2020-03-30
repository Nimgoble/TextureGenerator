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
		}
		public List<PixelBlob> Blobs { get; set; }
		public PixelColor? TransparencyColor { get; set; }
	}
}
