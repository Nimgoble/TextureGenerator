using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextureGenerator.Models
{
	public class PixelBlob
	{
		public PixelColor BlobColor { get; set; }
		public List<Pixel> Pixels { get; set; }
		public List<Pixel> Border 
		{ 
			get
			{
				return this.Pixels.Where
				(
					pixel =>
					pixel.GetSibling(SiblingDirection.Up) == null ||
					pixel.GetSibling(SiblingDirection.Down) == null ||
					pixel.GetSibling(SiblingDirection.Left) == null ||
					pixel.GetSibling(SiblingDirection.Right) == null
				).ToList();
			} 
		}
		public bool DoesOverlap(PixelBlob other)
		{
			return other.Pixels.Any(otherPixel => this.Pixels.Contains(otherPixel) || this.Pixels.Any(pixel => otherPixel.IsSibling(pixel)));
		}
	}
}
