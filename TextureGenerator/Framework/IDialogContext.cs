using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextureGenerator.Framework
{
	public interface IDialogContext
	{
		Task AddTask(System.Action<ITaskContext> action);
	}
}
