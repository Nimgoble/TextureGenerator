using System;
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
using ColorPickerWPF;
using System.IO;
using TextureGenerator.ViewModels;
namespace TextureGenerator.Views
{
	/// <summary>
	/// Interaction logic for MainView.xaml
	/// </summary>
	public partial class MainView : Page
	{
		public MainView()
		{
			InitializeComponent();
		}
		private MainViewModel ViewModel
		{
			get { return this.DataContext as MainViewModel; }
		}
		private void iSourceImage_MouseDown(object sender, MouseButtonEventArgs e)
		{
			var position = e.GetPosition(this.iSourceImage);
			var vm = this.ViewModel;
			if (vm == null)
				return;
			vm.SetReplacementColorFromSourceAtPoint(position, this.iSourceImage.ActualWidth, this.iSourceImage.ActualHeight);
		}
		private void bReplaceWithColor_Click(object sender, RoutedEventArgs e)
		{
			Color replacementColor;
			if (ColorPickerWindow.ShowDialog(out replacementColor))
			{
				var vm = this.ViewModel;
				if (vm == null)
					return;
				vm.ReplacementColor = replacementColor;
			}
		}
		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new SaveFileDialog();
			dialog.Filter = "PNG files (*.png) | *.png";
			if (dialog.ShowDialog() == true)
			{
				var fileName = dialog.FileName;
				this.ViewModel?.SaveOutput(fileName);
			}
		}

		private void btnSaveTexture_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new SaveFileDialog();
			dialog.Filter = "JSON files (*.json) | *.json";
			if (dialog.ShowDialog() == true)
			{
				var fileName = dialog.FileName;
				this.ViewModel?.SaveSourceTexture(fileName);
			}
		}

		private void btnSaveOutputAsString_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new SaveFileDialog();
			dialog.Filter = "Text files (*.txt) | *.txt";
			if (dialog.ShowDialog() == true)
			{
				var fileName = dialog.FileName;
				this.ViewModel?.SaveOutputAsString(fileName);
			}
		}
	}
}
