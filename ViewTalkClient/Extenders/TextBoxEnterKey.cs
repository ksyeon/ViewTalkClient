using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ViewTalkClient.Extenders
{
    public class TextBoxEnterKey : DependencyObject
    {
        public static readonly DependencyProperty EnterKeyCommandProperty =
            DependencyProperty.RegisterAttached("EnterKeyCommand", typeof(ICommand), typeof(TextBoxEnterKey), new PropertyMetadata(null, OnEnterKeyCommandChanged));

        public static ICommand GetEnterKeyCommand(DependencyObject target)
        {
            return (ICommand)target.GetValue(EnterKeyCommandProperty);
        }

        public static void SetEnterKeyCommand(DependencyObject target, ICommand value)
        {
            target.SetValue(EnterKeyCommandProperty, value);
        }

        static void OnEnterKeyCommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            ICommand command = (ICommand)e.NewValue;
            FrameworkElement fe = (FrameworkElement)target;
            Control control = (Control)target;
            control.KeyDown += (s, args) =>
            {
                if (args.Key == Key.Enter)
                {
                    BindingExpression b = control.GetBindingExpression(TextBox.TextProperty);
                    if (b != null)
                    {
                        b.UpdateSource();
                    }
                    command.Execute(null);
                }
            };
        }
    }

    // http://mark-dot-net.blogspot.kr/2010/11/how-to-invoke-command-on-viewmodel-by.html
}
