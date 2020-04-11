using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

using TextureGenerator.Models;
namespace TextureGenerator.Framework
{
	public class TextureProfileWriter
	{
		public void SaveTextureProfile(string outputFile, TextureProfile textureProfile)
		{
			if (string.IsNullOrEmpty(outputFile))
				return;
			var outputObject = this.ConvertTextureProfileToSaveSpace(textureProfile);
			//var outputObject = textureProfile;
			var outputString = JsonConvert.SerializeObject(outputObject);
			File.WriteAllText(outputFile, outputString);
			
		}
		private void WriteBson(string outputFile, object outputObject)
		{
			var bsonFileName = Path.ChangeExtension(outputFile, ".bson");
			MemoryStream ms = new MemoryStream();
			using (BsonDataWriter writer = new BsonDataWriter(ms))
			{
				JsonSerializer serializer = new JsonSerializer();
				serializer.Serialize(writer, outputObject);
				using (FileStream file = new FileStream(bsonFileName, FileMode.OpenOrCreate, System.IO.FileAccess.Write))
				{
					byte[] bytes = new byte[ms.Length];
					ms.Read(bytes, 0, (int)ms.Length);
					file.Write(bytes, 0, bytes.Length);
					ms.Close();
				}
			}
		}
		private object ConvertTextureProfileToSaveSpace(TextureProfile textureProfile)
		{
			return new
			{
				Blobs =
					from blob
					in textureProfile.Blobs
					select new
					{
						Name = blob.Name,
						BlobColor = blob.BlobColor,
						Pixels =
							from pixel
							in blob.Pixels
							select new
							{
								Position = pixel.Position
							}
					},
				BlobGroups = textureProfile.BlobGroups,
				TransparencyColor = textureProfile.TransparencyColor
			};
		}
	}
}
