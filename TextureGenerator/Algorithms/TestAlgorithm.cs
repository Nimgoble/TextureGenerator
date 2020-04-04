using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

using TextureGenerator.Framework;
using TextureGenerator.Models;
namespace TextureGenerator.Algorithms
{
	public class TestAlgorithm : IAlgorithm
	{
		public string AlgorithmName { get { return "Test Algorithm"; } }
		public PixelColor[,] DrawAlgorithm(IAlgorithmTarget target)
		{
			var pixelsSource = target.GetPixelsSource();
			var copy = pixelsSource.ToPixelColorArray();

			if (!target.AlgorithmPixels.Any())
				return copy;
			var random = new Random();
			var direction = (SiblingDirection)random.Next(0, 7);
			var length = random.Next(20, 100);
			var currentCount = 0;
			var randomColor = Color.FromArgb(255, (byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255)).ToPixelColor();
			var currentPixel = target.AlgorithmPixels[random.Next(0, target.AlgorithmPixels.Count - 1)];
			do
			{
				copy[(int)currentPixel.Position.Y, (int)currentPixel.Position.X] = randomColor;
				currentPixel = pixelsSource.GetSibling(currentPixel, direction, true);
				++currentCount;
			}
			while (currentPixel != null && currentCount < length);
			return copy;
		}
	}
}
