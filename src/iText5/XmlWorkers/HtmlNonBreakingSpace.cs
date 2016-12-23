using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;

// http://stackoverflow.com/questions/35874932
namespace kuujinbo.StackOverflow.iTextSharp.iText5.XmlWorkers
{
    public class HtmlNonBreakingSpace
    {
        string HTML = @"
    <div>
    <h1>HTML Encoded non breaking space</h1><table border='1'><tr><td>&amp;#160;</td></tr></table>
    <h1>HTML non breaking space</h1><table border='1'><tr><td>&#160;</td></tr></table>
    <div style='background-color:yellow;'><h1>Empty Table</h1><table><tr><td></td></tr></table></div>
    </div>
        ";
        public void Go()
        {
            var outputFile = Helpers.IO.GetClassOutputPath(this);

            HTML = HTML.Replace("&amp;#160;", "\u00A0");
            using (var stringReader = new StringReader(HTML))
            {
                using (FileStream stream = new FileStream(
                    outputFile,
                    FileMode.Create,
                    FileAccess.Write))
                {
                    using (var document = new Document())
                    {
                        PdfWriter writer = PdfWriter.GetInstance(
                            document, stream
                        );
                        document.Open();
                        XMLWorkerHelper.GetInstance().ParseXHtml(
                            writer, document, stringReader
                        );
                    }
                }
            }
        }
    }
}
