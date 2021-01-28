using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using PDFusion.ViewModel;

namespace PDFusion
{
    public class DragAndDropListBox<T> : ListBox
    {



        private Point _dragStartPoint;

        private P FindVisualParent<P>(DependencyObject child) where P : DependencyObject
        {
            if( child.GetType().Name== typeof(System.Windows.Documents.Run).Name)
            {
                return null;
            }
            var parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null)
                return null;

            P parent = parentObject as P;
            if (parent != null)
                return parent;

            return FindVisualParent<P>(parentObject);
        }

        public DragAndDropListBox()
        {
            this.PreviewMouseMove += ListBox_PreviewMouseMove;

            var style = new Style(typeof(ListBoxItem));

            style.Setters.Add(new Setter(ListBoxItem.AllowDropProperty, true));

            style.Setters.Add(
                new EventSetter(
                    ListBoxItem.PreviewMouseLeftButtonDownEvent,
                    new MouseButtonEventHandler(ListBoxItem_PreviewMouseLeftButtonDown)));

            style.Setters.Add(
                    new EventSetter(
                        ListBoxItem.DropEvent,
                        new DragEventHandler(ListBoxItem_Drop)));

        

            this.ItemContainerStyle = style;

            this.KeyDown += DragAndDropListBox_KeyDown;
        }


        private void DragAndDropListBox_KeyDown(object sender, KeyEventArgs e)
        {

            if (SelectedIndex<0)
            {
                return;
            }
            var item = SelectedItem as MyPDFItem;
            if( item== null)
            {
                return;
            }
            

            if(e.Key == Key.Subtract)
            {
                if( SelectedIndex ==0)
                {
                    return;
                }

                var selectedID = SelectedIndex;
                Move(item, selectedID, selectedID - 1);
                SelectedIndex = selectedID - 1;

            }
            if (e.Key == Key.Add)
            {
                if( SelectedIndex>= Items.Count-1)
                {
                    return;
                }

                var selectedID = SelectedIndex;
                Move(item, selectedID, selectedID + 1);
                SelectedIndex = selectedID + 1;
            }

            if (e.Key == Key.Delete || e.Key == Key.Back)
            {
                var dc = this.DataContext as MainVM;
                if (dc != null)
                {
                    var items = dc.PdfList;
                    if (items != null)
                    {
                        items.RemoveAt(SelectedIndex);
                    }
                }
            }
        }

        private void ListBox_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Point point = e.GetPosition(null);
            Vector diff = _dragStartPoint - point;
            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                var lb = sender as ListBox;
                var lbi = FindVisualParent<ListBoxItem>(((DependencyObject)e.OriginalSource));
                if (lbi != null)
                {
                    DragDrop.DoDragDrop(lbi, lbi.DataContext, DragDropEffects.Move);
                }
            }
        }

        private void ListBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragStartPoint = e.GetPosition(null);
        }

        private void ListBoxItem_Drop(object sender, DragEventArgs e)
        {
            if (sender is ListBoxItem)
            {
                var source = e.Data.GetData(typeof(MyPDFItem)) as MyPDFItem;
                var target = ((ListBoxItem)(sender)).DataContext as MyPDFItem;


                int sourceIndex = this.Items.IndexOf(source);
                int targetIndex = this.Items.IndexOf(target);

                Move(source, sourceIndex, targetIndex);
            }
        }

        private void Move(MyPDFItem source, int sourceIndex, int targetIndex)
        {
            if (sourceIndex < targetIndex)
            {
                var dc = this.DataContext as MainVM;
                if (dc != null)
                {
                    var items = dc.PdfList;
                    if (items != null)
                    {
                        items.Insert(targetIndex + 1, source );
                        items.RemoveAt(sourceIndex);
                    }
                }
            }
            else
            {
                var dc = this.DataContext as MainVM;
                if (dc != null)
                {
                    var items = dc.PdfList;
                    if (items != null)
                    {
                        int removeIndex = sourceIndex + 1;
                        if (items.Count + 1 > removeIndex)
                        {
                            items.Insert(targetIndex, source);
                            items.RemoveAt(removeIndex);
                        }
                    }
                }
            }
        }
    }

    public class MyPDFItem
    {
        public string Name { get; set; }

        public MyPDFItem(string name)
        {
            this.Name = name;
        }
    }

    public class ItemDragAndDropListBox : DragAndDropListBox<MyPDFItem> { }


}
