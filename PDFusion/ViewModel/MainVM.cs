using Microsoft.Win32;
using PDFusion.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PDFusion.ViewModel
{
    public class MainVM : INotifyPropertyChanged
    {
        public ICommand OnSelectFiles { get; set; }


        public ICommand OnMergeClick { get; set; }

        public bool OpenAfterGeneration { get; set; }

        private ObservableCollection<MyPDFItem> _items = new ObservableCollection<MyPDFItem>();

        public ObservableCollection<MyPDFItem> PdfList
        {
            get { return _items; }
            set { _items = value; }
        }

        public MainVM()
        {
            OnMergeClick = new RelayCommand(cmd => OnMergeClickImpl());
            OnSelectFiles = new RelayCommand(cmd => OnSelectFilesImpl());
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            var handlers = PropertyChanged;
            if (handlers != null)
            {
                var args = new PropertyChangedEventArgs(property);
                handlers(this, args);
            }
        }


        private PDFMerger merger = new PDFMerger();

        public async void OnSelectFilesImpl()
        {
            try
            {
                OpenFileDialog opf = new OpenFileDialog();
                opf.Filter = "PDF Files (*.pdf)|*.pdf";
                opf.DefaultExt = "pdf";

                opf.Multiselect = true;
                if (opf.ShowDialog() == true)
                {
                    var files = opf.FileNames.ToList();
                    foreach (var file in files)
                    {
                        PdfList.Add(new MyPDFItem(file));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public async void OnMergeClickImpl()
        {
            try
            {
                if(!PdfList.Any())
                {
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PDF Files (*.pdf)|*.pdf";
                saveFileDialog.DefaultExt = "pdf";

                if (saveFileDialog.ShowDialog() == true)
                {
                    var pdfs = PdfList.Select(x => x.Name).ToList();
                    merger.Merge(pdfs, saveFileDialog.FileName, OpenAfterGeneration);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }



    }
}
