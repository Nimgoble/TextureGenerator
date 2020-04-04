using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextureGenerator.Models
{
	public interface IPixelsSource
	{
		Pixel[,] Pixels { get; }
		List<Pixel> PixelsList { get; }
		Pixel GetSibling(Pixel target, SiblingDirection siblingDirection, bool ofSameColorConstraint);
		Pixel[] GetSiblings(Pixel target, bool ofSameColorConstraint);
		bool AreSiblings(Pixel a, Pixel b, bool ofSameColorConstraint);
		PixelColor[,] ToPixelColorArray();
	}
}
