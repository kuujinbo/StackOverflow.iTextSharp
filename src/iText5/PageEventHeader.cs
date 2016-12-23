using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

// http://stackoverflow.com/questions/41250603
namespace kuujinbo.StackOverflow.iTextSharp.iText5
{
    public class PageEventHeader : PdfPageEventHelper
    {
        public string HeaderText { get; set; }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            float cellHeight = document.TopMargin;
            Rectangle page = document.PageSize;
            PdfPTable table = new PdfPTable(1) { TotalWidth = page.Width };
            table.AddCell(new PdfPCell(new Phrase(HeaderText))
            {
                Border = PdfPCell.NO_BORDER,
                FixedHeight = cellHeight,
                HorizontalAlignment = Element.ALIGN_CENTER
            });
            table.WriteSelectedRows(
                0, -1, 0,
                page.Height - cellHeight + table.TotalHeight,
                writer.DirectContent
            );
        }
    }

    public class Header
    {
        public void Go()
        {
            var OUTPUT_FILE = Helpers.IO.GetClassOutputPath(this);

            using (var stream = new MemoryStream())
            {
                var header = new PageEventHeader();
                using (Document document = new Document())
                {
                    var writer = PdfWriter.GetInstance(document, stream);
                    document.Open();

                    writer.PageEvent = header;
                    header.HeaderText = "Header 0";
                    document.Add(new Phrase("Header 0"));
                    document.NewPage();
                    header.HeaderText = "Header 1";
                    document.Add(new Phrase("Header 1"));
                }
                File.WriteAllBytes(OUTPUT_FILE, stream.ToArray());
            }

        }
    }
}