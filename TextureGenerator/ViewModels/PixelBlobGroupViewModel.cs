using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

using TextureGenerator.Models;
namespace TextureGenerator.ViewModels
{
	public class PixelBlobGroupViewModel : Screen
	{
		private readonly PixelBlobGroup model;
		private readonly NotifyCollectionChangedAction[] updateActions = new NotifyCollectionChangedAction[]
		{
			NotifyCollectionChangedAction.Add,
			NotifyCollectionChangedAction.Remove,
			NotifyCollectionChangedAction.Reset
		};
		public PixelBlobGroupViewModel(PixelBlobGroup model, List<PixelBlobViewModel> blobs)
		{
			this.model = model;
			this.Blobs = new ObservableCollection<PixelBlobViewModel>();
			this.Blobs.CollectionChanged += Blobs_CollectionChanged;
			if(blobs != null)
				blobs.ForEach(blob => this.Blobs.Add(blob));
		}

		#region Public Methods
		public void ToggleEdit()
		{
			this.IsEditting = !this.IsEditting;
		}
		#endregion

		#region Private Methods
		private void Blobs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Move)
				return;
			if(e.NewItems != null)
			{
				foreach (var item in e.NewItems)
				{
					var vm = item as PixelBlobViewModel;
					vm.PropertyChanged += Vm_PropertyChanged;
				}
			}
			if(e.OldItems != null)
			{
				foreach (var item in e.OldItems)
				{
					var vm = item as PixelBlobViewModel;
					vm.PropertyChanged -= Vm_PropertyChanged;
				}
			}
			if(this.updateActions.Contains(e.Action))
			{
				Model.Blobs = this.Blobs.Select(x => x.Name).ToList();
			}
		}
		private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName != nameof(PixelBlobViewModel.Name))
				return;
			Model.Blobs = this.Blobs.Select(x => x.Name).ToList();
		}
		#endregion

		#region Properties
		public PixelBlobGroup Model { get { return this.model; } }
		public ObservableCollection<PixelBlobViewModel> Blobs { get; private set; }
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
		#endregion
	}
}
