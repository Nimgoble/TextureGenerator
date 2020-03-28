using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using TextureGenerator.Framework;
namespace TextureGenerator.Models
{
	public class Texture
	{
		private BitmapSource source;
		private List<PixelBlob> blobs = new List<PixelBlob>();
		private Color transparencyColor;
		public Texture(BitmapSource source, Color transparencyColor)
		{
			this.source = source;
			this.transparencyColor = transparencyColor;
		}
		public async Task<bool> ProcessSourceAsync(IDialogContext dialogContext, ITaskContext ourTaskContext)
		{
			var pixelColors = source.CopyPixels();
			return await Task.Run(() => this.ProcessSource(pixelColors, dialogContext, ourTaskContext));
		}
		public async Task<bool> ProcessSource(PixelColor[,] pixelColors, IDialogContext dialogContext, ITaskContext ourTaskContext)
		{
			BitmapSource source = this.source;
			ourTaskContext.UpdateMessage("Starting processing of image");
			//var pixelColors = source.CopyPixels();
			var pixels2D = new Pixel[pixelColors.GetLength(0),pixelColors.GetLength(1)];
			var pixels = new List<Pixel>();
			await dialogContext.AddTask
			(
				(taskContext) =>
				{
					var yLength = pixelColors.GetLength(0);
					var xLength = pixelColors.GetLength(1);
					var total = yLength * xLength;
					taskContext.UpdateMessage("Converting pixels");
					for (int y = 0; y < yLength; ++y)
					{
						for (int x = 0; x < xLength; ++x)
						{
							var currentIndex = (y * yLength) + x;
							//taskContext.UpdateMessage($"Converting pixel {currentIndex} out of {total}");
							var pixelColor = pixelColors[y, x];
							var pixel = new Pixel
							{
								PixelColor = pixelColor,
								Position = new System.Windows.Point(x, y)
							};
							pixels2D[y, x] = pixel;
							if (this.transparencyColor.IsEqualToPixelColor(pixelColor))
							{
								//taskContext.UpdateProgress(currentIndex / total);
								continue;
							}
							pixels.Add(pixel);
							//taskContext.UpdateProgress(currentIndex / total);
						}
					}
					taskContext.UpdateProgress(100);
				}
			);
			

			ourTaskContext.UpdateMessage("Setting pixel siblings.");
			await dialogContext.AddTask
			(
				(taskContext) =>
				{
					double index = 0;
					taskContext.UpdateMessage($"Assigning siblings for {pixels.Count} pixels.");
					pixels.ForEach
					(
						pixel =>
						{
							++index;
							if (this.transparencyColor.IsEqualToPixelColor(pixel.PixelColor))
								return;

							foreach (var siblingDirection in Enum.GetValues(typeof(SiblingDirection)).Cast<SiblingDirection>())
							{
								var siblingPoint = pixel.Position.ToDirection(siblingDirection);
								if (!pixels2D.IsWithinBounds(siblingPoint))
									continue;
								var potentialSiblingPixel = pixels2D[(int)siblingPoint.Y, (int)siblingPoint.X];
								var siblingPixel = (potentialSiblingPixel.PixelColor.IsEqualTo(pixel.PixelColor)) ? potentialSiblingPixel : null;

								pixel.SetSibling(siblingPixel, siblingDirection);
							}
							taskContext.UpdateProgress((int)((index / pixels.Count) * 100));
						}
					);
					taskContext.UpdateProgress(100);
				}
			);

			var pixelGroups = pixels.GroupBy(pixel => pixel.PixelColor);

			var tasks = new List<Task>();
			var modifiedPixelGroups = new List<List<PixelBlob>>();
			object mutex = new object();
			foreach(var pixelGroup in pixelGroups)
			{
				tasks.Add
				(
					dialogContext.AddTask
					(
						(taskContext) => 
						{
							var result = this.ProcessPixelGroup(pixelGroup, taskContext);
							lock(mutex)
							{
								modifiedPixelGroups.Add(result);
							}
						}
					)
				);
			}
			Task.WaitAll(tasks.ToArray());
			ourTaskContext.UpdateProgress(100);
			this.blobs.AddRange(modifiedPixelGroups.SelectMany(x => x));
			return true;
		}
		private List<PixelBlob> ProcessPixelGroup(IGrouping<PixelColor, Pixel> pixelGroup, ITaskContext taskContext)
		{
			var pixelGroupList = pixelGroup.ToList();
			var keyDisplayName = pixelGroup.Key.ToColor().ToHexString();
			var groupBlobs = new List<PixelBlob>();
			try
			{
				double originalCount = (double)pixelGroupList.Count;
				Func<int> calculatePercentage = () => { return Math.Min((int)(100 - (((double)pixelGroupList.Count / originalCount) * 100)), 99); };
				int percentageDone = 0;
				Action updatePercentage = () =>
				{
					taskContext.UpdateMessage($"{keyDisplayName}: Processing pixel group. {pixelGroupList.Count} left out of {originalCount}");
					percentageDone = calculatePercentage();
					taskContext.UpdateProgress(percentageDone); //Only go up to 99 so the task doesn't end.
				};
				while (pixelGroupList.Count > 0)
				{
					updatePercentage();
					var nextPixel = pixelGroupList.First();
					var blobPixels = new List<Pixel>();
					var currentList = new List<Pixel>();
					currentList.Add(nextPixel);
					var siblingTransferList = new List<Pixel>();
					do
					{
						blobPixels.AddRange(currentList);
						currentList.ForEach
						(
							x =>
							{
								pixelGroupList.Remove(x);
								foreach(var sibling in x.Siblings)
								{
									if (sibling == null || blobPixels.Contains(sibling) || siblingTransferList.Contains(sibling))
										continue;
									siblingTransferList.Add(sibling);
								}
							}
						);
						currentList.Clear();
						currentList.AddRange(siblingTransferList);
						siblingTransferList.Clear();
						updatePercentage();
					}
					while (currentList.Any());
					var blob = new PixelBlob()
					{
						BlobColor = pixelGroup.Key,
						Pixels = blobPixels
					};
					groupBlobs.Add(blob);
					pixelGroupList = pixelGroupList.Where(pixel => !blobPixels.Contains(pixel)).ToList();
				}
				taskContext.UpdateMessage($"{keyDisplayName}: {groupBlobs.Count} blobs.");
				taskContext.UpdateProgress(100);
			}
			catch (Exception ex)
			{
				string stopHere = ex.Message;
			}
			return groupBlobs;
		}
		private Pixel FlattenSiblings(Pixel pixel, List<Pixel> total, List<Pixel> currentIteration, ref int depth)
		{
			try
			{
				if (depth >= 10)
					return pixel;
				if (!total.Contains(pixel))
					currentIteration.Add(pixel);
				var validSiblings = pixel.Siblings.Where(sibling => sibling != null && !total.Contains(sibling)).ToList();
				Pixel nextPixel = null;
				++depth;
				foreach (var sibling in validSiblings)
				{
					nextPixel = this.FlattenSiblings(sibling, total, currentIteration, ref depth);
					if (nextPixel != null)
						break;
				}
				//return nextPixel != null ? pixel : null; //Return the current pixel, because we need to go through the siblings again.
				return nextPixel;
			}
			catch(Exception ex)
			{
				string stopHere = ex.Message;
			}
			return null;
		}
		public List<PixelBlob> Blobs { get { return this.blobs; } }
		public BitmapSource Source { get { return this.source; } }
	}
}
