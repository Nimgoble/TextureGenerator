using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using TextureGenerator.Framework;
namespace TextureGenerator.ViewModels
{
	public class TaskViewModel : Screen, ITaskContext
	{
		public TaskViewModel(System.Action<ITaskContext> action)
		{
			this.Task = new Task
				(
					() =>
					{
						action(this);
					}
				);
		}
		public void Run()
		{
			this.Task.Start();
			this.Task.GetAwaiter().OnCompleted(() => { this.OnTaskFinished(); });
		}
		public void UpdateProgress(int percentage)
		{
			this.Percentage = percentage;
		}
		public void UpdateMessage(string message)
		{
			this.Message = message;
		}
		public event EventHandler TaskFinished;
		private void OnTaskFinished()
		{
			this.IsComplete = true;
			TaskFinished?.Invoke(this, null);
		}

		public Task Task { get; private set; }
		private int percentage = 0;
		public int Percentage 
		{
			get { return this.percentage; }
			set
			{
				this.percentage = value;
				NotifyOfPropertyChange(() => Percentage);
			}
		}
		private string message = String.Empty;
		public string Message 
		{
			get { return this.message; }
			set
			{
				this.message = value;
				NotifyOfPropertyChange(() => Message);
			}
		}
		private bool isComplete = false;
		public bool IsComplete 
		{ 
			get { return isComplete; } 
			private set
			{
				this.isComplete = value;
				NotifyOfPropertyChange(() => IsComplete);
			}
		}
	}
}
