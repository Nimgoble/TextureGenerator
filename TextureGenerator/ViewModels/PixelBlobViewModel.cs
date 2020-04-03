using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

using TextureGenerator.Models;
using TextureGenerator.Framework;
namespace TextureGenerator.ViewModels
{
	public class PixelBlobViewModel : Screen
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
	}
}
