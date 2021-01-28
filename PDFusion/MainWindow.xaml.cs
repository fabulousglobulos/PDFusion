using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using PDFusion.ViewModel;

namespace PDFusion
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainVM();
        }

        private void Image_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var pdf = (e.Source as Image).DataContext as MyPDFItem;
                var pdfs = GetPDFs();
                if (pdfs != null)
                {
                    pdfs.Remove(pdf);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

 

        private ObservableCollection<MyPDFItem> GetPDFs()
        {
            var dataContext = this.DataContext as MainVM;
            if (DataContext != null)
            {
                return dataContext.PdfList;
            }
            return null;
        }



        private void btnDragDrop_Drop(object sender, DragEventArgs e)
        {
            var pdfs = GetPDFs();
            if (pdfs == null)
            {
                return;
            }

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                var files = ((string[])e.Data.GetData(DataFormats.FileDrop)).ToList();
                if (files != null)
                {
                    foreach (var file in files)
                    {
                        var pdfItem = new MyPDFItem(file);
                        pdfs.Add(pdfItem);
                    }
                }
            }
        }
    }
}
