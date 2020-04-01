using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

using TextureGenerator.Models;
namespace TextureGenerator.Framework
{
	public class TextureProfileWriter
	{
		public void SaveTextureProfile(string outputFile, TextureProfile textureProfile)
		{
			if (string.IsNullOrEmpty(outputFile))
				return;
			//var outputObject = new
			//{
			//	Blobs =
			//		from blob
			//		in this.texture.Blobs
			//		select new
			//		{
			//			BlobColor = blob.BlobColor,
			//			Pixels =
			//				from pixel
			//				in blob.Pixels
			//				select new
			//				{
			//					Position = pixel.Position
			//				}
			//		}
			//};
			File.WriteAllText(outputFile, JsonConvert.SerializeObject(textureProfile));
		}
	}
}
