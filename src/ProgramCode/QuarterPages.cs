using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

// http://stackoverflow.com/questions/35959487
namespace kuujinbo.StackOverflow.iTextSharp.ProgramCode
{
    public class QuarterPages
    {
        public static string outputFile;
        public QuarterPages()
        {
            outputFile = Helpers.IO.GetClassOutputPath(this);
        }

        byte[] GetMasterDocument(int count)
        {
            using (var stream = new MemoryStream())
            {
                using (var document = new Document(PageSize.A6))
                {
                    PdfWriter.GetInstance(document, stream);
                    document.Open();
                    for (int i = 1; i <= count; ++i)
                    {
                        document.Add(new Paragraph(string.Format(
@"Real name: real-name-{0:D4}
User name: user-name-{0:D4}
Password: password-{0:D4}
Email address: email-{0:D4}@invalid.com",
                         i)));
                        if (i < count) document.NewPage();
                    }
                }
                return stream.ToArray();
            }
        }

        public void Go()
        {
            PdfReader reader = new PdfReader(GetMasterDocument(38));
            Rectangle pageSize = reader.GetPageSize(1);
            using (FileStream stream = new FileStream(
                outputFile,
                FileMode.Create,
                FileAccess.Write))
            {
                using (Document document = new Document(pageSize, 0, 0, 0, 0))
                {
                    PdfWriter writer = PdfWriter.GetInstance(document, stream);
                    document.Open();
                    PdfPTable table = new PdfPTable(2);
                    table.TotalWidth = pageSize.Width;
                    table.LockedWidth = true;
                    table.DefaultCell.Border = Rectangle.NO_BORDER;
                    table.DefaultCell.FixedHeight = pageSize.Height / 2;
 
                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        PdfImportedPage page = writer.GetImportedPage(reader, i);
                        table.AddCell(Image.GetInstance(page));
                    }
                    document.Add(table);
                }
            }
        }
    }
}