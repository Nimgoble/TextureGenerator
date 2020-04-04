using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

using TextureGenerator.Algorithms;
using TextureGenerator.Models;
using TextureGenerator.Framework;
namespace TextureGenerator.ViewModels
{
	public class PixelBlobViewModel : Screen, IAlgorithmTarget
	{
		private readonly PixelBlob model;
		public PixelBlobViewModel(PixelBlob model)
		{
			this.model = model;
			this.IsEditting = false;
			if (string.IsNullOrEmpty(this.Name))
				this.Name = this.HexColor;
		}

		public PixelBlob Model { get { return this.model; } }
		public string Name
		{
			get { return this.model.Name; }
			set
			{
				if (value == this.model.Name)
					return;
				this.model.Name = value;
				NotifyOfPropertyChange(() => Name);
				NotifyOfPropertyChange(() => AlgorithmTargetName);
			}
		}
		public string HexColor { get { return this.model.BlobColor.ToColor().ToHexString(); } }
		private bool isEditting = false;
		public bool IsEditting 
		{ 
			get { return this.isEditting; }
			set
			{
				if (value == this.isEditting)
					return;
				this.isEditting = value;
				NotifyOfPropertyChange(() => IsEditting);
			}
		}
		#region IAlgorithmTarget
		public string AlgorithmTargetName { get { return this.Name; } }
		public IPixelsSource GetPixelsSource()
		{
			return this.Model.PixelsSource;
		}
		public List<Pixel> AlgorithmPixels { get { return this.Model.Pixels; } }
		public List<Pixel> AlgorithmBorderPixels { get { return this.Model.Border; } }
		#endregion
	}
}
