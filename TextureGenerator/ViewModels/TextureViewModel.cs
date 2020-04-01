﻿using System;
using System.Collections.Generic;
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
		}
		public Texture Model { get { return this.model; } }
	}
}