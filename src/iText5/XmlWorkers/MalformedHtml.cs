using System.IO;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using HtmlAgilityPack;

// http://stackoverflow.com/questions/12113425
namespace kuujinbo.StackOverflow.iTextSharp.iText5.XmlWorkers
{
    public class MalformedHtml
    {
        public void Go()
        {
            var OUTPUT = Helpers.IO.GetClassOutputPath(this);

            var malformedHtml = @"
<h1>Malformed HTML</h1>
<p>A paragraph <b><span>with improperly nested tags</b></span></p><hr>
<table><tr><td>Cell 1, row 1</td><td>Cell 1, row 2";
            HtmlDocument h = new HtmlDocument()
            {
                OptionFixNestedTags = true,
                OptionWriteEmptyNodes = true
            };
            h.LoadHtml(malformedHtml);

            string css = @"
h1 { font-size:1.4em; }
hr { margin-top: 4em; margin-bottom: 2em; color: #ddd; }
table { border-collapse: collapse; }
table, td { border: 1px solid black; }
td { padding: 4px; }
span { color: red; }";

            using (var stream = new MemoryStream())
            {
                using (var document = new Document())
                {
                    PdfWriter writer = PdfWriter.GetInstance(document, stream);
                    document.Open();
                    using (var htmlStream = new MemoryStream(Encoding.UTF8.GetBytes(h.DocumentNode.WriteTo())))
                    {
                        using (var cssStream = new MemoryStream(Encoding.UTF8.GetBytes(css)))
                        {
                            XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, htmlStream, cssStream);
                        }
                    }
                }
                File.WriteAllBytes(OUTPUT, stream.ToArray());
            }
        }
    }
}