using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace ViewTalkClient.Extentions
{
    public class ListBoxAutoScroll : DependencyObject
    {
        public static readonly DependencyProperty AutoScrollToEndProperty =
            DependencyProperty.RegisterAttached("AutoScrollToEnd", typeof(bool), typeof(ListBoxAutoScroll), new UIPropertyMetadata(default(bool), OnAutoScrollToEndChanged));

        public static bool GetAutoScrollToEnd(DependencyObject target)
        {
            return (bool)target.GetValue(AutoScrollToEndProperty);
        }

        public static void SetAutoScrollToEnd(DependencyObject target, bool value)
        {
            target.SetValue(AutoScrollToEndProperty, value);
        }

        public static void OnAutoScrollToEndChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            var listBoxItems = listBox.Items;
            var source = listBoxItems.SourceCollection as INotifyCollectionChanged;

            var scrollToEndHandler = new NotifyCollectionChangedEventHandler((sender2, e2) =>
            {
                if (listBox.Items.Count > 0)
                {
                    object lastItem = listBox.Items[listBox.Items.Count - 1];

                    listBoxItems.MoveCurrentTo(lastItem);
                    listBox.ScrollIntoView(lastItem);
                }
            });

            if ((bool)e.NewValue)
            {
                source.CollectionChanged += scrollToEndHandler;
            }
            else
            {
                source.CollectionChanged -= scrollToEndHandler;
            }
        }
    }

    // https://michlg.wordpress.com/2010/01/17/listbox-automatically-scroll-to-bottom/
}
