using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace kuujinbo.StackOverflow.iTextSharp.iText5
{
    public class ImageWrapText
    {
        public void Go()
        {
            var outputFile = Helpers.IO.GetClassOutputPath(this);
            var imagePath = Helpers.IO.GetInputFilePath("100x100.jpg");
            var imageSpacerPath = Helpers.IO.GetInputFilePath("20x20.png");
            var testText = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.";

            using (FileStream stream = new FileStream(
                outputFile,
                FileMode.Create,
                FileAccess.Write))
            {
                using (Document document = new Document())
                {
                    Image img = Image.GetInstance(imagePath);
                    PdfWriter writer = PdfWriter.GetInstance(document, stream);
                    // writer.InitialLeading = 108;
                    document.Open();

                    // document.Add(new Paragraph(img.Height.ToString()));
                    img.Alignment = Image.ALIGN_LEFT | Image.TEXTWRAP;
                    img.BorderWidth = 10;
                    img.ScaleToFit(1000, 72);
                    document.Add(img);
                    //img.BorderColor = BaseColor.RED;
                    var p = new Paragraph();
                    p.Add(testText);
                    document.Add(p);


                    PdfContentByte cb = writer.DirectContent;
                    ColumnText ct = new ColumnText(cb);
                    ct.SetSimpleColumn(new Phrase(new Chunk(testText, FontFactory.GetFont(FontFactory.HELVETICA, 18, Font.NORMAL))),
                                       46, 190, 530, 36, 25, Element.ALIGN_LEFT | Element.ALIGN_TOP);
                    ct.Go(); 

                    // document.Add(new Paragraph(testText));
                    // document.Add(new Chunk(testText));

                }
            }
        }


    }
}