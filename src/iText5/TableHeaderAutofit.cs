using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

// http://stackoverflow.com/questions/42788260
namespace kuujinbo.StackOverflow.iTextSharp.iText5
{
    public class TableHeaderAutofit
    {
        public float[] GetHeaderWidths(Font font, params string[] headers)
        { 
            var total = 0;
            var columns = headers.Length;
            var widths = new int[columns];
            for (var i = 0; i < columns; ++i)
            {
                var w = font.GetCalculatedBaseFont(true).GetWidth(headers[i]);
                total += w;
                widths[i] = w;
            }
            var result = new float[columns];
            for (var i = 0; i < columns; ++i) 
            {
                result[i] = (float)widths[i] / total * 100;
            }
            return result;
        }

        public void Go()
        {
            var OUT_FILE = Helpers.IO.GetClassOutputPath(this);


            string[] headers = new string[]
            { 
                "Order Id", "Customer Id", "Customer Name", "Product Id",
                "Product Description", "Quantity", "Product Received"
            };
            Font font = new Font(Font.FontFamily.COURIER, 14, Font.ITALIC);
            font.Color = BaseColor.BLUE;
            var table = new PdfPTable(headers.Length) { WidthPercentage = 100 };
            table.SetWidths(GetHeaderWidths(font, headers));

            using (var stream = new MemoryStream())
            {
                using (var document = new Document(PageSize.A4.Rotate()))
                {
                    PdfWriter.GetInstance(document, stream);
                    document.Open();
                    for (int i = 0; i < headers.Length; ++i)
                    {
                        table.AddCell(new PdfPCell(new Phrase(headers[i], font)));
                    }
                    document.Add(table);
                }
                File.WriteAllBytes(OUT_FILE, stream.ToArray());
            }
       

        }
    }
}