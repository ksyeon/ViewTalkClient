using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;

namespace ViewTalkClient.Extenders
{
    public class ControlFocus : DependencyObject
    {
        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.RegisterAttached("IsFocused", typeof(bool), typeof(ControlFocus), new UIPropertyMetadata(default(bool), OnIsFocusedPropertyChanged));

        public static bool GetIsFocused(DependencyObject target)
        {
            return (bool)target.GetValue(IsFocusedProperty);
        }

        public static void SetIsFocused(DependencyObject target, bool value)
        {
            target.SetValue(IsFocusedProperty, value);
        }

        private static void OnIsFocusedPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var uiElement = (UIElement)sender;

            if ((bool)e.NewValue)
            {
                uiElement.Focus();
            }
        }
    }
}
