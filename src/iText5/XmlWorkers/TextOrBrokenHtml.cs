using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using HtmlAgilityPack;
using System;

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
            PASTEBIN = File.ReadAllText(Helpers.IO.GetInputFilePath("TextOrBrokenHtml.html"));
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

    // UPDATED to remove HtmlCommentNode
    var comments = h.DocumentNode.SelectNodes("//comment()");
    if (comments != null) 
    {
        foreach (var node in comments) { node.Remove(); }
    }

    return h.DocumentNode.SelectNodes("child::*") != null
        //                            ^^^^^^^^^^
        // XPath above: string plain-text or contains markup/tags
        ? h.DocumentNode.WriteTo()
        : string.Format("<span>{0}</span>", broken);
}

        public void Go()
        {
var fixedMarkup = FixBrokenMarkup(PASTEBIN);
// swap initialization to verify plain-text works too
// var fixedMarkup = FixBrokenMarkup("some text");

using (var stream = new MemoryStream())
{
    using (var document = new Document())
    {
        PdfWriter writer = PdfWriter.GetInstance(document, stream);
        document.Open();
        using (var mailBody = new StringReader(fixedMarkup))
        {
            XMLWorkerHelper.GetInstance().ParseXHtml(
                writer, document, mailBody
            );
        }
    }
    File.WriteAllBytes(OUTPUT, stream.ToArray());
}
        }
    }
}