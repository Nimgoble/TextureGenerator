using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

using TextureGenerator.Models;
namespace TextureGenerator.ViewModels
{
	public class TextureViewModel : Screen
	{
		private Texture model;
		public TextureViewModel(Texture model)
		{
			this.model = model;
			this.Blobs = (from blob in model.Blobs select new PixelBlobViewModel(blob)).OrderBy(x => x.Name).ToList();
			this.BlobGroups = new ObservableCollection<PixelBlobGroupViewModel>();
			var blobGroups = 
				(
					from blobGroup
					in model.BlobGroups
					select new PixelBlobGroupViewModel(blobGroup, this.Blobs.Where(x => blobGroup.Blobs.Contains(x.Name)).ToList())
				);
			foreach(var blobGroup in blobGroups)
				this.BlobGroups.Add(blobGroup);
		}

		#region Public Methods
		public void AddEmptyPixelBlobGroup()
		{
			var newModel = new PixelBlobGroup();
			this.Model.BlobGroups.Add(newModel);
			this.BlobGroups.Add(new PixelBlobGroupViewModel(newModel, null));
		}
		#endregion

		#region Private Methods
		#endregion

		#region Properties
		public Texture Model { get { return this.model; } }
		public List<PixelBlobViewModel> Blobs { get; set; }
		public ObservableCollection<PixelBlobGroupViewModel> BlobGroups { get; set; }
		#endregion
	}
}
