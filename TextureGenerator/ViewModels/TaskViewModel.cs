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
			this.Task = new Task(() => action(this));
		}
		public void Run()
		{
			this.Task.Start();
		}
		public void UpdateProgress(int percentage)
		{
			this.Percentage = percentage;
		}
		public void UpdateMessage(string message)
		{
			this.Message = message;
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
				NotifyOfPropertyChange(() => IsComplete);
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
		public bool IsComplete { get { return this.Percentage == 100; } }
	}
}
