﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

using TextureGenerator.ViewModels;
namespace TextureGenerator.Views
{
	/// <summary>
	/// Interaction logic for GenerateTextureView.xaml
	/// </summary>
	public partial class GenerateTextureView : UserControl
	{
		public GenerateTextureView()
		{
			InitializeComponent();
		}

		private GenerateTextureViewModel ViewModel
		{
			get { return this.DataContext as GenerateTextureViewModel; }
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new SaveFileDialog();
			dialog.Filter = "JSON files (*.json) | *.json";
			if (dialog.ShowDialog() == true)
			{
				var fileName = dialog.FileName;
				this.ViewModel?.SaveTextureProfile(fileName);
			}
		}
		private void bLoadImage_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog();
			dialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
			dialog.InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
			if (dialog.ShowDialog() == true)
			{
				try
				{
					var vm = this.ViewModel;
					if (vm != null)
					{
						vm.LoadSourceImageFromFile(dialog.FileName);
					}
				}
				catch (Exception ex)
				{
				}
			}
		}
		private void iSourceImage_MouseDown(object sender, MouseButtonEventArgs e)
		{
			var position = e.GetPosition(this.iSourceImage);
			var vm = this.ViewModel;
			if (vm == null)
				return;
			vm.SetTransparencyColorFromSourceAtPoint(position, this.iSourceImage.ActualWidth, this.iSourceImage.ActualHeight);
		}
	}
}
