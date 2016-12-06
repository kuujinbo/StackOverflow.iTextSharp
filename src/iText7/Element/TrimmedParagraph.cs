using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System;
using System.Text.RegularExpressions;
using iText.Kernel.Font;
using iText.IO.Font;

// http://stackoverflow.com/questions/40708500
namespace kuujinbo.StackOverflow.iTextSharp.iText7.Element
{
    public class TrimmedParagraph
    {
        public void Go()
        {
            var OUTPUT_FILE = Helpers.IO.GetClassOutputPath(this);
            using (PdfWriter writer = new PdfWriter(OUTPUT_FILE))
            {
                using (PdfDocument pdf = new PdfDocument(writer))
                {
                    using (Document doc = new Document(pdf))
                    {
                        // tabbed whitespace
                        Paragraph p = new Paragraph("Line 0\n")
                            .AddTabStops(new TabStop(8f))
                            // change to your needs  ^^
                            .Add(new Tab())
                            .Add("Line 1");
                        doc.Add(p);

                        // inline trimmed whitespace workaround
                        string[] lines = "0\n1\n    2\n        3\n".Split(
                            new string[] { "\n" }, 
                            StringSplitOptions.RemoveEmptyEntries
                        );
                        p = new Paragraph().AddStyle(
                            new Style().SetFont(PdfFontFactory.CreateFont(FontConstants.COURIER))
                        );
                        foreach (var l in lines)
                        {
                            if (Regex.IsMatch(l, @"^\s+"))
                            {
                                p.Add(" ")  // all spaces stripped, whether one or more characters
                                    .Add(l) // now leading whitespace preserved
                                    .Add("\n");
                            }
                            else
                            {
                                p.Add(l).Add("\n");
                            }
                        }
                        doc.Add(p);
                    }
                }
            }
        }
    }
}