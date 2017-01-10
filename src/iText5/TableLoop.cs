using System;
using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

// http://stackoverflow.com/questions/41553534
namespace kuujinbo.StackOverflow.iTextSharp.iText5
{
    public class TableLoop
    {
        public string OUT_FILE;
        public TableLoop()
        {
            OUT_FILE = Helpers.IO.GetClassOutputPath(this);
        }

        public void Go()
        {
            List<string> titles = new List<string>() 
            { 
                "Name, surname:", "Personal Id:", "Phone number:", 
                "Department:", "University:", "Faculty:" 
            };
                        List<string> datas = new List<string>() 
            { 
                "0", "1", "2", "3", "4", "5"
            };
            Font timesBold = FontFactory.GetFont("Times-Roman", 8, Font.BOLD);
            Font timesNormal = FontFactory.GetFont("Times-Roman", 8, Font.NORMAL);

            FillTable(titles, datas, timesBold, timesNormal);
        }

        public void FillTable(List<string> titles, List<string> datas,
            Font titleFont, Font dataFont)
        {
            var table = new PdfPTable(2) { WidthPercentage = 100 };
            table.DefaultCell.Border = Rectangle.NO_BORDER;
            var columns = new[] { 50f, 50f };
            table.SetWidths(columns);

            using (FileStream stream = new FileStream(
                OUT_FILE,
                FileMode.Create,
                FileAccess.Write))
            {
                using (var document = new Document())
                {
                    PdfWriter.GetInstance(document, stream);
                    document.Open();
                    for (int i = 0; i < titles.Count; ++i)
                    {
                        table.AddCell(new PdfPCell(new Phrase(titles[i], titleFont)));
                        table.AddCell(new PdfPCell(new Phrase(datas[i], dataFont)));
                    }
                    document.Add(table);
                }
            }
        }
    }
}