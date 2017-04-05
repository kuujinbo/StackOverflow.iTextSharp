using System.IO;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;

// http://stackoverflow.com/questions/42990317
namespace kuujinbo.StackOverflow.iTextSharp.iText5.XmlWorkers
{
    public class SupTag
    {
        public void Go()
        {
            var OUTPUT = Helpers.IO.GetClassOutputPath(this);

            string HTML = @"
<html><head>
<title>Test HTML</title>
</head><body>
<div>The 1<sup>st</sup> day of the month</div>
</body></html>
";

            string css = "sup { vertical-align: super; font-size: 0.8em; }";
            using (var stream = new MemoryStream())
            {
                using (var document = new Document())
                {
                    PdfWriter writer = PdfWriter.GetInstance(document, stream);
                    document.Open();

                    using (var htmlStream = new MemoryStream(Encoding.UTF8.GetBytes(HTML)))
                    {
                        using (var cssStream = new MemoryStream(Encoding.UTF8.GetBytes(css)))
                        {
                            XMLWorkerHelper.GetInstance().ParseXHtml(
                                writer, document, htmlStream, cssStream
                            );
                        }
                    }
                }
                File.WriteAllBytes(OUTPUT, stream.ToArray());
            }
        }
    }
}