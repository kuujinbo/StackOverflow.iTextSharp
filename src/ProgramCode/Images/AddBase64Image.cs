using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using iTextSharp.text;
using iTextSharp.text.pdf;

// http://stackoverflow.com/questions/30241333
namespace kuujinbo.StackOverflow.iTextSharp.ProgramCode.Images
{
    public class AddBase64Image
    {
        public const string NAME = "lname";
        byte[] GetTestReader()
        {
            using (var stream = new MemoryStream())
            {
                using (Document document = new Document())
                {
                    PdfWriter.GetInstance(document, stream);
                    document.Open();

                    PdfPTable table = new PdfPTable(2);
                    table.SetWidths(new int[] { 1, 2 });
                    table.AddCell("Name:");
                    var cell = new PdfPCell()
                    {
                        CellEvent = new FormFieldTestReader() { FieldName = NAME }
                    };
                    table.AddCell(cell);

                    document.Add(table);
                }
                return stream.ToArray();
            }
        }

        public void Go()
        {
            using (PdfReader reader = new PdfReader(GetTestReader()))
            {
                var outputFile = Helpers.IO.GetClassOutputPath(this);

                using (var stream = new FileStream(outputFile, FileMode.Create))
                {
                    using (PdfStamper stamper = new PdfStamper(reader, stream))
                    {
                        AcroFields form = stamper.AcroFields;
                        var fldPosition = form.GetFieldPositions(NAME)[0];
                        Rectangle rectangle = fldPosition.position;
                        string base64Image = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAAHElEQVQI12P4//8/w38GIAXDIBKE0DHxgljNBAAO9TXL0Y4OHwAAAABJRU5ErkJggg==";
                        Regex regex = new Regex(@"^data:image/(?<mediaType>[^;]+);base64,(?<data>.*)");
                        Match match = regex.Match(base64Image);
                        Image image = Image.GetInstance(
                            Convert.FromBase64String(match.Groups["data"].Value)
                        );
                        // best fit if image bigger than form field
                        if (image.Height > rectangle.Height || image.Width > rectangle.Width)
                        {
                            image.ScaleAbsolute(rectangle);
                        }
                        // form field top left - change parameters as needed to set different position 
                        image.SetAbsolutePosition(rectangle.Left + 2, rectangle.Top - 8);
                        stamper.GetOverContent(fldPosition.page).AddImage(image);
                    }
                }
            }
        }
    }

    class FormFieldTestReader : IPdfPCellEvent
    {
        public string FieldName { get; set; }

        public void CellLayout(PdfPCell cell, Rectangle rectangle, PdfContentByte[] canvases)
        {
            PdfWriter writer = canvases[0].PdfWriter;
            TextField text = new TextField(
                writer, rectangle, FieldName ?? AddBase64Image.NAME
            );
            PdfFormField field = text.GetTextField();
            writer.AddAnnotation(field);
        }
    }   


}