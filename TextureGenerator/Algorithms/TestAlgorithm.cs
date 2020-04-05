using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

using LibNoise;
using LibNoise.Builder;
using LibNoise.Utils;
using LibNoise.Filter;
using LibNoise.Modifier;
using LibNoise.Primitive;
using LibNoise.Renderer;

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

			//this.DrawRandomLine(target, copy, pixelsSource);
			this.GenerateNoiseMapStuff(target, copy);
			return copy;
		}

		private PixelColor[,] DrawRandomLine(IAlgorithmTarget target, PixelColor[,] source, IPixelsSource pixelsSource)
		{
			var random = new Random();
			var direction = (SiblingDirection)random.Next(0, 7);
			var length = random.Next(20, 100);
			var currentCount = 0;
			var randomColor = System.Windows.Media.Color.FromArgb(255, (byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255)).ToPixelColor();
			var currentPixel = target.AlgorithmPixels[random.Next(0, target.AlgorithmPixels.Count - 1)];
			do
			{
				source[(int)currentPixel.Position.Y, (int)currentPixel.Position.X] = randomColor;
				currentPixel = pixelsSource.GetSibling(currentPixel, direction, true);
				++currentCount;
			}
			while (currentPixel != null && currentCount < length);
			return source;
		}

		private PixelColor[,] GenerateNoiseMapStuff(IAlgorithmTarget target, PixelColor[,] source)
		{
			var noiseMap = this.GenerateNoiseMap(source.GetLength(0), source.GetLength(1));
			var baseColor = System.Windows.Media.Colors.SaddleBrown.ToPixelColor();
			var shapeColor = System.Windows.Media.Colors.White.ToPixelColor();
			var cutoffValue = new Random().Next(0, 100);
			target.AlgorithmPixels.ForEach
			(
				pixel =>
				{
					var position = pixel.Position;
					int x = (int)position.X, y = (int)position.Y;
					var value = Math.Abs(noiseMap.GetValue(x, y));
					//source[y, x] = ((value * 100) >= cutoffValue) ? shapeColor.Blend(baseColor, value) : baseColor;
					source[y, x] = shapeColor.Blend(baseColor, value);
				}
			);
			return source;
		}

		private NoiseMap GenerateNoiseMap(int width, int height)
		{
			//module::Perlin myModule;
			var module = new LibNoise.Primitive.SimplexPerlin();
			//var module = new LibNoise.Primitive.BevinsGradient();
			//var module = new LibNoise.Primitive.ImprovedPerlin();
			module.Quality = NoiseQuality.Best;
			module.Seed = PrimitiveModule.DefaultSeed;

			//ScaleBias scale = null;

			FilterModule fModule = new Pipe();
			fModule.Primitive3D = (IModule3D)module;
			fModule.OctaveCount = 1;// FilterModule.DEFAULT_OCTAVE_COUNT;
			fModule.Frequency = FilterModule.DEFAULT_FREQUENCY;
			fModule.Gain = FilterModule.DEFAULT_GAIN;
			fModule.Lacunarity = FilterModule.DEFAULT_LACUNARITY;
			fModule.Offset = FilterModule.DEFAULT_OFFSET;
			fModule.SpectralExponent = FilterModule.DEFAULT_SPECTRAL_EXPONENT;

			NoiseMap heightMap = new NoiseMap();
			heightMap.SetSize(width, height);
			float bound = 2f;
			NoiseMapBuilderPlane heightMapBuilder = new NoiseMapBuilderPlane(bound, bound * 2, 0.0f, 100.0f, true);
			//NoiseMapBuilderPlane heightMapBuilder = new NoiseMapBuilderPlane(bound, bound * 2, bound, bound * 2, true);
			heightMapBuilder.SourceModule = (IModule3D)fModule;
			heightMapBuilder.NoiseMap = heightMap;
			heightMapBuilder.SetSize(width, height);
			//heightMapBuilder.SetBounds(6.0f, 10.0f, 1.0f, 5.0f);
			heightMapBuilder.Build();
			return heightMap;
			//NoiseMapBuilderPlane heightMapBuilder;
			//heightMapBuilder.SetSourceModule(myModule);
			//heightMapBuilder.SetDestNoiseMap(heightMap);
			//heightMapBuilder.SetDestSize(256, 256);
			//heightMapBuilder.SetBounds(6.0, 10.0, 1.0, 5.0);
			//heightMapBuilder.Build();
		}
	}
}
