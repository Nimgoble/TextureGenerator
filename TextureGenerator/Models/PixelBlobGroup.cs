using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextureGenerator.Models
{
	public class PixelBlobGroup
	{
		public PixelBlobGroup()
		{
			Blobs = new List<string>();
		}
		public string Name { get; set; }
		public List<string> Blobs { get; set; } 
	}
}
