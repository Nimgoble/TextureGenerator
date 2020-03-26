using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextureGenerator.Framework
{
	public interface ITaskContext
	{
		void UpdateProgress(int percentage);
		void UpdateMessage(string message);
	}
}
