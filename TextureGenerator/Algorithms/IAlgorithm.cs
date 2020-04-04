using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TextureGenerator.Models;
namespace TextureGenerator.Algorithms
{
	public interface IAlgorithm
	{
		string AlgorithmName { get; }
		PixelColor[,] DrawAlgorithm(IAlgorithmTarget target);
	}
}
