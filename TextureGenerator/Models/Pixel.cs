using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Newtonsoft.Json;
namespace TextureGenerator.Models
{
	public enum SiblingDirection
	{
		Up = 0,
		Right = 1,
		Down = 2,
		Left = 3,
		UpRight = 4,
		DownRight = 5,
		DownLeft = 6,
		UpLeft = 7
	}

	public class Pixel
	{
		public Pixel()
		{
		}
		public Point Position { get; set; }
		public PixelColor PixelColor { get; set; }
	}
}
