using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

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
		private Pixel[] siblings = new Pixel[8];
		public Pixel()
		{
			foreach(var siblingDirection in Enum.GetValues(typeof(SiblingDirection)).Cast<SiblingDirection>())
			{
				this.siblings[(int)siblingDirection] = null;
			}
		}
		public Point Position { get; set; }
		public PixelColor PixelColor { get; set; }
		public Pixel[] Siblings { get { return siblings; } }
		public void SetSibling(Pixel sibling, SiblingDirection siblingDirection)
		{
			this.siblings[(int)siblingDirection] = sibling;
		}
		public Pixel GetSibling(SiblingDirection siblingDirection)
		{
			return this.siblings[(int)siblingDirection];
		}
		public bool IsSibling(Pixel other)
		{
			if (other == null)
				return false;
			return this.siblings.Contains(other);
		}
	}
}
