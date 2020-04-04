using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TextureGenerator.Models;
namespace TextureGenerator.Algorithms
{
	public interface IAlgorithmTarget
	{
		string AlgorithmTargetName { get; }
		IPixelsSource GetPixelsSource();
		List<Pixel> AlgorithmPixels { get; }
		List<Pixel> AlgorithmBorderPixels { get; }
	}
}
