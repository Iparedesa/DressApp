﻿using System.Windows;
using System.Windows.Data;

namespace DressApp.Converters
{
    
    // Converts boolean values to Visibility
   
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((Visibility)value == Visibility.Visible)
                return true;
            return false;
        }
    }
}
