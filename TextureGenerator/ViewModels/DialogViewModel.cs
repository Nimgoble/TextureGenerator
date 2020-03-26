using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

using TextureGenerator.Framework;
namespace TextureGenerator.ViewModels
{
	public class DialogViewModel : Screen, IDialogContext
	{
		public DialogViewModel()
		{
			this.Tasks = new ObservableCollection<TaskViewModel>();
		}
		public Task AddTask(System.Action<ITaskContext> action)
		{
			var taskViewModel = new TaskViewModel(action);
			Execute.OnUIThread
			(
				() =>
				{
					Tasks.Add(taskViewModel);
					taskViewModel.Run();
				}
			);
			return taskViewModel.Task;
		}
		public void CloseDialog()
		{
			this.TryClose(true);
		}
		public bool CanCloseDialog
		{
			get { return this.Tasks.All(x => x.IsComplete); }
		}

		public ObservableCollection<TaskViewModel> Tasks { get; set; }
	}
}
