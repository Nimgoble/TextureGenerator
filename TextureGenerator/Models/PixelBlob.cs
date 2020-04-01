using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TextureGenerator.Models
{
	public class PixelBlob
	{
		private IPixelsSource pixelsSource;
		public PixelBlob(IPixelsSource pixelsSource)
		{
			this.pixelsSource = pixelsSource;
		}
		public string Name { get; set; }
		public PixelColor BlobColor { get; set; }
		public List<Pixel> Pixels { get; set; }
		[JsonIgnore]
		public List<Pixel> Border 
		{ 
			get
			{
				return this.Pixels.Where
				(
					pixel =>
					this.pixelsSource.GetSibling(pixel, SiblingDirection.Up, true) == null ||
					this.pixelsSource.GetSibling(pixel, SiblingDirection.Right, true) == null ||
					this.pixelsSource.GetSibling(pixel, SiblingDirection.Down, true) == null ||
					this.pixelsSource.GetSibling(pixel, SiblingDirection.Left, true) == null
				).ToList();
			} 
		}
		public bool DoesOverlap(PixelBlob other)
		{
			return other.Pixels.Any(otherPixel => this.Pixels.Contains(otherPixel) || this.Pixels.Any(pixel => this.pixelsSource.AreSiblings(pixel, otherPixel, true)));
		}
		[JsonIgnore]
		public IPixelsSource PixelsSource 
		{ 
			get { return this.pixelsSource; } 
			set
			{
				this.pixelsSource = value;
			}
		}
	}
}
