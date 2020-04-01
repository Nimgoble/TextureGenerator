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
	/// Interaction logic for LoadTextureView.xaml
	/// </summary>
	public partial class LoadTextureView : UserControl
	{
		public LoadTextureView()
		{
			InitializeComponent();
		}
		private LoadTextureViewModel ViewModel
		{
			get { return this.DataContext as LoadTextureViewModel; }
		}
		private void SelectImage_Click(object sender, RoutedEventArgs e)
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
						vm.ImagePath = dialog.FileName;
					}
				}
				catch (Exception ex)
				{
				}
			}
		}
		private void SelectTextureProfile_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog();
			dialog.Filter = "JSON file (*.json) | *.json";
			dialog.InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
			if (dialog.ShowDialog() == true)
			{
				try
				{
					var vm = this.ViewModel;
					if (vm != null)
					{
						vm.TextureProfilePath = dialog.FileName;
					}
				}
				catch (Exception ex)
				{
				}

			}
		}
	}
}