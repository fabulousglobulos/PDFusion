using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFusion.Model
{
    public class PDFMerger
    {
        public void Merge(List<string> files, string output, bool openAfterGeneration)
        {
            //var files = GetFiles();

            // Open the output document
            PdfDocument outputDocument = new PdfDocument();

            // Iterate files
            foreach (string file in files)
            {
                // Open the document to import pages from it.
                PdfDocument inputDocument = PdfReader.Open(file, PdfDocumentOpenMode.Import);

                // Iterate pages
                int count = inputDocument.PageCount;
                for (int idx = 0; idx < count; idx++)
                {
                    // Get the page from the external document...
                    PdfPage page = inputDocument.Pages[idx];
                    // ...and add it to the output document.
                    outputDocument.AddPage(page);
                }
            }

            outputDocument.Save(output);
            // ...and start a viewer.
            if (openAfterGeneration)
            {
                Process.Start(output);
            }
        }


        private List<string> GetFiles()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(@"C:\Users\Vincent\Documents\Visual Studio 2017\Projects\PDFusion\samples");
            FileInfo[] fileInfos = dirInfo.GetFiles("*.pdf");
            List<string> list = new List<string>();
            foreach (FileInfo info in fileInfos)
            {
                // HACK: Just skip the protected samples file...
                if (info.Name.IndexOf("protected") == -1)
                    list.Add(info.FullName);
            }
            return list;
        }
    }
}
