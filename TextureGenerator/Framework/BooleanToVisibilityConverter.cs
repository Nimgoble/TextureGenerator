using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
namespace TextureGenerator.Framework
{
	public class BooleanToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (!(value is bool))
				return null;
			var visible = (bool)value;
			var notVisibleType = (parameter != null && parameter is Visibility) ? (Visibility)parameter : Visibility.Collapsed;
			return visible ? Visibility.Visible : notVisibleType;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (!(value is Visibility))
				return true;
			var visibility = (Visibility)value;
			return visibility == Visibility.Visible;
		}
	}
}
