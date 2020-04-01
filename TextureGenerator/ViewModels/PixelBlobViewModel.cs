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
				NotifyOfPropertyChange(() => BlobDisplayName);
			}
		}
		public string HexColor { get { return this.model.BlobColor.ToColor().ToHexString(); } }
		public string BlobDisplayName
		{
			get { return string.IsNullOrEmpty(this.Name) ? this.HexColor : this.Name; }
		}
	}
}
