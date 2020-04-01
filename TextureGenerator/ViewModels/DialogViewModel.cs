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
			taskViewModel.TaskFinished += TaskViewModel_TaskFinished;
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

		private void TaskViewModel_TaskFinished(object sender, EventArgs e)
		{
			var taskViewModel = sender as TaskViewModel;
			taskViewModel.TaskFinished -= TaskViewModel_TaskFinished;
			NotifyOfPropertyChange(() => CanCloseDialog);
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
