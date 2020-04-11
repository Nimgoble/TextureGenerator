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
	public class TextureProfileReader
	{
		public TextureProfile ReadTextureProfile(string fileName, IPixelsSource pixelsSource)
		{
			TextureProfile textureProfile = JsonConvert.DeserializeObject<TextureProfile>(File.ReadAllText(fileName));
			textureProfile.Blobs.ForEach
			(
				blob =>
				{
					blob.PixelsSource = pixelsSource;
					//Reconnect the pixels to their color, since we removed them while writing to save space.
					blob.Pixels.ForEach(pixel => pixel.PixelColor = blob.BlobColor); //Doing this cut the file size down to 1/5 of the size.
				}
			);
			return textureProfile;
		}
	}
}
