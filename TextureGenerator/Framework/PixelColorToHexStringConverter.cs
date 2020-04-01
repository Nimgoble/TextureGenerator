using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

using TextureGenerator.Models;
namespace TextureGenerator.Framework
{
	public class PixelColorToHexStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (!(value is PixelColor))
				return null;
			var color = (PixelColor)value;
			return color.ToColor().ToHexString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (!(value is string))
				return null;
			var color = (Color)ColorConverter.ConvertFromString("#FFFFFF");
			return color.ToPixelColor();
		}
	}
}
