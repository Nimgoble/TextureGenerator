using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using TextureGenerator.Framework;
namespace TextureGenerator.Models
{
	public class PixelsSource : IPixelsSource
	{
		private Vector[] siblingDirectionToVectorMap = new Vector[8];
		private List<Pixel> pixelsList;
		public PixelsSource(Pixel[,] pixels, List<Pixel> pixelsList)
		{
			this.Pixels = pixels;
			this.pixelsList = pixelsList;
			foreach (var siblingDirection in Enum.GetValues(typeof(SiblingDirection)).Cast<SiblingDirection>())
			{
				this.siblingDirectionToVectorMap[(int)siblingDirection] = siblingDirection.ToVector();
			}
		}
		public Pixel GetSibling(Pixel target, SiblingDirection siblingDirection, bool ofSameColorConstraint)
		{
			var vector = this.siblingDirectionToVectorMap[(int)siblingDirection];
			return this.GetSibling(target, vector, ofSameColorConstraint);
		}
		private Pixel GetSibling(Pixel target, Vector siblingDirectionVector, bool ofSameColorConstraint)
		{
			var newPoint = target.Position + siblingDirectionVector;
			if (!this.Pixels.IsWithinBounds(newPoint))
				return null;
			var sibling  = this.Pixels[(int)newPoint.Y, (int)newPoint.X];
			return (ofSameColorConstraint) ? (sibling.PixelColor.IsEqualTo(target.PixelColor) ? sibling : null) : sibling;
		}
		public Pixel[] GetSiblings(Pixel target, bool ofSameColorConstraint)
		{
			return this.siblingDirectionToVectorMap.Select(vec => this.GetSibling(target, vec, ofSameColorConstraint)).ToArray();
		}
		public bool AreSiblings(Pixel a, Pixel b, bool ofSameColorConstraint)
		{
			if (a == null || b == null)
				return false;
			return this.GetSiblings(a, ofSameColorConstraint).Contains(b);
		}
		public Pixel[,] Pixels { get; }
		public List<Pixel> PixelsList { get { return this.pixelsList; } }

		public PixelColor[,] ToPixelColorArray()
		{
			return this.Pixels.Convert((x, y, p) => p.PixelColor);
		}

		public static PixelsSource FromPixelColors(PixelColor[,] pixelColors)
		{
			var pixelsList = new List<Pixel>();
			var pixels = pixelColors.Convert
				(
					(x, y, pixelColor) =>
					{
						var pixel = new Pixel
						{
							PixelColor = pixelColor,
							Position = new System.Windows.Point(x, y)
						};
						pixelsList.Add(pixel);
						return pixel;
					}
				);
			return new PixelsSource(pixels, pixelsList);
		}
	}
}
