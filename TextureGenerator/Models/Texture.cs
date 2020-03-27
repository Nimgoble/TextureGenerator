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
	public class PixelProcessingContainer
	{
		public Pixel Pixel { get; set; }
		private PixelBlob blob = null;
		public PixelBlob Blob 
		{ 
			get
			{
				if (this.blob != null)
					return this.blob;
				if (this.Pixel == null)
					return null;
				this.blob = 
			}
			set; 
		}
	}
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
			var pixels2D = new PixelProcessingContainer[pixelColors.GetLength(0),pixelColors.GetLength(1)];
			var pixels = new List<PixelProcessingContainer>();
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
							var pixelContainer = new PixelProcessingContainer()
							{
								Pixel = new Pixel
								{
									PixelColor = pixelColor,
									Position = new System.Windows.Point(x, y)
								}
							};
							pixels2D[y, x] = pixelContainer;
							if (this.transparencyColor.IsEqualToPixelColor(pixelColor))
							{
								//taskContext.UpdateProgress(currentIndex / total);
								continue;
							}
							pixels.Add(pixelContainer);
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
						pixelContainer =>
						{
							var pixel = pixelContainer.Pixel;
							++index;
							if (this.transparencyColor.IsEqualToPixelColor(pixel.PixelColor))
								return;

							PixelBlob blob = null;
							foreach (var siblingDirection in Enum.GetValues(typeof(SiblingDirection)).Cast<SiblingDirection>())
							{
								var siblingPoint = pixel.Position.ToDirection(siblingDirection);
								if (!pixels2D.IsWithinBounds(siblingPoint))
									continue;
								var siblingPixelContainer = pixels2D[(int)siblingPoint.Y, (int)siblingPoint.X];
								var siblingPixel = siblingPixelContainer.Pixel;
								if (!(siblingPixel.PixelColor.IsEqualTo(pixel.PixelColor)))
									continue;
								if (siblingPixelContainer.Blob != null && blob == null)
									blob = siblingPixelContainer.Blob;

								pixel.SetSibling(siblingPixel, siblingDirection);
							}
							if (blob == null)
							{
								blob = new PixelBlob();
								blob.BlobColor = pixel.PixelColor;
								this.Blobs.Add(blob);
							}

							blob.Pixels.Add(pixelContainer.Pixel);
							pixelContainer.Blob = blob;
							taskContext.UpdateProgress((int)((index / pixels.Count) * 100));
						}
					);
					taskContext.UpdateProgress(100);
				}
			);

			//var pixelGroups = pixels.GroupBy(pixel => pixel.PixelColor);
			var pixelGroups = this.blobs.GroupBy(x => x.BlobColor);

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
							this.PrintBlobsForColor(pixelGroup, taskContext);
							//var result = this.ProcessPixelGroup(pixelGroup, taskContext);
							//lock(mutex)
							//{
							//	modifiedPixelGroups.Add(result);
							//}
						}
					)
				);
			}
			Task.WaitAll(tasks.ToArray());
			ourTaskContext.UpdateProgress(100);
			//this.blobs.AddRange(modifiedPixelGroups.SelectMany(x => x));
			return true;
		}
		private void PrintBlobsForColor(IGrouping<PixelColor, PixelBlob> colorBlobsGroup, ITaskContext taskContext)
		{
			var keyDisplayName = colorBlobsGroup.Key.ToColor().ToHexString();
			taskContext.UpdateMessage($"{keyDisplayName}: {colorBlobsGroup.Count()} blobs.");
			taskContext.UpdateProgress(100);
		}
		private List<PixelBlob> ProcessPixelGroup(IGrouping<PixelColor, Pixel> pixelGroup, ITaskContext taskContext)
		{
			var pixelGroupList = pixelGroup.ToList();
			var keyDisplayName = pixelGroup.Key.ToColor().ToHexString();
			var groupBlobs = new List<PixelBlob>();
			try
			{
				double originalCount = (double)pixelGroupList.Count;
				//if (originalCount > 20000)
				//{
				//	taskContext.UpdateMessage($"{keyDisplayName}: Skipping because there are too many pixels: {pixelGroupList.Count}");
				//	taskContext.UpdateProgress(100);
				//	return groupBlobs;//Speed shit up.
				//}

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
					//var existingBlob = groupBlobs.FirstOrDefault(eb => nextPixel.Siblings.Where(sibling => sibling != null).Any(sibling => eb.Pixels.Contains(sibling)));
					//var blobPixels = (existingBlob != null) ? existingBlob.Pixels : new List<Pixel>();
					var blobPixels = new List<Pixel>();
					var currentList = new List<Pixel>();
					var siblingTransferList = new List<Pixel>();
					currentList.Add(nextPixel);
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
					//if (existingBlob == null)
					//{
						var blob = new PixelBlob()
						{
							BlobColor = pixelGroup.Key,
							Pixels = blobPixels
						};
						groupBlobs.Add(blob);
					//}
					pixelGroupList = pixelGroupList.Where(pixel => !blobPixels.Contains(pixel)).ToList();
				}

				//var current = groupBlobs.FirstOrDefault();
				//PixelBlob nextOverlap = null;
				//do
				//{
				//	taskContext.UpdateMessage($"{keyDisplayName}: Grouping blobs. {groupBlobs.Count}");
				//	nextOverlap = groupBlobs.FirstOrDefault(blob => blob != current && current.DoesOverlap(blob));
				//	if (nextOverlap != null)
				//	{
				//		current.Pixels.AddRange(nextOverlap.Pixels.Where(p => !current.Pixels.Contains(p)));
				//		groupBlobs.Remove(nextOverlap);
				//	}
				//	else
				//		current = (current == groupBlobs.Last()) ? null : groupBlobs.ElementAt(groupBlobs.IndexOf(current) + 1);
				//}
				//while (current != null);
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
