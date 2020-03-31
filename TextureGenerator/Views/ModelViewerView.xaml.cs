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

using TextureGenerator.ViewModels;
namespace TextureGenerator.Views
{
	/// <summary>
	/// Interaction logic for ModelViewerView.xaml
	/// </summary>
	public partial class ModelViewerView : UserControl
	{
		public ModelViewerView()
		{
			InitializeComponent();
		}
		private ModelViewerViewModel ViewModel
		{
			get { return this.DataContext as ModelViewerViewModel; }
		}

		private void bLoadModel_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog();
			dialog.Filter = "FBX files (*.fbx) | *.fbx";
			dialog.InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
			if (dialog.ShowDialog() == true)
			{
				try
				{
					var vm = this.ViewModel;
					if (vm != null)
					{
						vm.LoadModel(dialog.FileName);
					}
				}
				catch (Exception ex)
				{
				}
			}
		}
	}
}
