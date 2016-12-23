using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using HtmlAgilityPack;

// http://stackoverflow.com/questions/35216731 - PART 1
namespace kuujinbo.StackOverflow.iTextSharp.iText5.XmlWorkers
{
    public class RemoveImageByClass
    {
        const string HTML = @"
<div>
    <div>
        <img src='somepath/desktop.jpg' class='img-desktop'>Desktop</img><hr>
        <img src='somepath/mobile.jpg' class='img-mobile'>Mobile</img>
    </div>
</div>
        ";

        string RemoveImage(string htmlToParse)
        {
            var hDocument = new HtmlDocument()
            {
                OptionWriteEmptyNodes = true,
                OptionAutoCloseOnEnd = true
            };
            hDocument.LoadHtml(htmlToParse);
            var root = hDocument.DocumentNode;
            var imagesDesktop = root.SelectNodes("//img[@class='img-desktop']"); 
            foreach (var image in imagesDesktop)
            {
                var imageText = image.NextSibling;
                imageText.Remove();
                image.Remove();
            }
            return root.WriteTo();
        }

        public void Go()
        {
            var outputFile = Helpers.IO.GetClassOutputPath(this);
            var parsedHtml = RemoveImage(HTML);
            Console.WriteLine(parsedHtml);

            using (var xmlSnippet = new StringReader(parsedHtml))
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
                          writer, document, xmlSnippet
                        );
                    }
                }
            }
        }


    }
}
