using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.io;
using iTextSharp.text.pdf;

// http://stackoverflow.com/questions/43908241
namespace kuujinbo.StackOverflow.iTextSharp.iText5
{
    public class Hyphens
    {
        public void Go()
        {
            var OUT_FILE = Helpers.IO.GetClassOutputPath(this);

            var content = @"
Allein ist besser als mit Schlechten im Verein: mit Guten im Verein, ist besser als allein.
            ";
            var table = new PdfPTable(1);
            // make sure .dll is in correct /bin directory
            StreamUtil.AddToResourceSearch("itext-hyph-xml.dll");

            using (var stream = new MemoryStream())
            {
                using (var document = new Document(PageSize.A8.Rotate()))
                {
                    PdfWriter.GetInstance(document, stream);
                    document.Open();
                    var chunk = new Chunk(content)
                        .SetHyphenation(new HyphenationAuto("de", "DR", 3, 3));
                    table.AddCell(new Phrase(chunk));
                    document.Add(table);
                }
                File.WriteAllBytes(OUT_FILE, stream.ToArray());
            }
        }
    }
}