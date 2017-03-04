using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using HtmlAgilityPack;

// http://stackoverflow.com/questions/42577638
namespace kuujinbo.StackOverflow.iTextSharp.iText5.XmlWorkers
{
    public class TextOrBrokenHtml
    {
        string OUTPUT;
        string PASTEBIN;
        public TextOrBrokenHtml() 
        { 
            OUTPUT = Helpers.IO.GetClassOutputPath(this); 
            PASTEBIN = File.ReadAllText(Helpers.IO.GetInputFilePath("TextOrBrokenHtml.txt"));
        }
        string FixBrokenMarkup(string broken)
        {
            HtmlDocument h = new HtmlDocument()
            {
                OptionAutoCloseOnEnd = true,
                OptionFixNestedTags = true,
                OptionWriteEmptyNodes = true
            };
            h.LoadHtml(broken);
            return h.DocumentNode.SelectNodes("child::*") != null
                //                            ^^^^^^^^^^
                // XPath above: string plain-text or contains markup/tags
                ? h.DocumentNode.WriteTo()
                : string.Format("<span>{0}</span>", broken);
        }

        public void Go()
        {
            var fixedMarkup = FixBrokenMarkup(PASTEBIN);
            // swap inititialization to verify plain-text works too
            // var fixedMarkup = FixBrokenMarkup("some text");

            using (var stream = new MemoryStream())
            {
                using (var document = new Document())
                {
                    PdfWriter writer = PdfWriter.GetInstance(
                        document, stream
                    );
                    document.Open();
                    using (var xmlSnippet = new StringReader(fixedMarkup))
                    {
                        XMLWorkerHelper.GetInstance().ParseXHtml(
                            writer, document, xmlSnippet
                        );
                    }
                }
                File.WriteAllBytes(OUTPUT, stream.ToArray());
            }
        }
    }
}