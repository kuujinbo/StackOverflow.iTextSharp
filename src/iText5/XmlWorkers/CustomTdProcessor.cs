using System.Collections.Generic;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.html.table;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;

// http://stackoverflow.com/questions/36180131
namespace kuujinbo.StackOverflow.iTextSharp.iText5.XmlWorkers
{
    public class TableDataProcessor : TableData
    {
        /*
         * a **very** simple implementation of the CSS writing-mode property:
         * https://developer.mozilla.org/en-US/docs/Web/CSS/writing-mode
         */
        bool HasWritingMode(IDictionary<string, string> attributeMap)
        {
            bool hasStyle = attributeMap.ContainsKey("style");
            return hasStyle
                    && attributeMap["style"].Split(new char[] { ';' })
                    .Where(x => x.StartsWith("writing-mode:"))
                    .Count() > 0
                ? true : false;
        }

        public override IList<IElement> End(
            IWorkerContext ctx,
            Tag tag,
            IList<IElement> currentContent)
        {
            var cells = base.End(ctx, tag, currentContent);
            var attributeMap = tag.Attributes;
            if (HasWritingMode(attributeMap))
            {
                var pdfPCell = (PdfPCell) cells[0];
                // **always** 'sideways-lr'
                pdfPCell.Rotation = 90;
            }
            return cells;
        }
    }

    public class CustomTdProcessor
    {
        string OUTPUT_FILE;
        string XHTML = @"
        <h1>Table with Vertical Text</h1>
        <table><tr>
        <td style='writing-mode:sideways-lr;text-align:center;width:40px;'>First</td>
        <td style='writing-mode:sideways-lr;text-align:center;width:40px;'>Second</td></tr>
        <tr><td style='text-align:center'>1</td>
        <td style='text-align:center'>2</td></tr></table>

        <h1>Table <u>without</u> Vertical Text</h1>
        <table width='50%'>
        <tr><td class='light-yellow'>0</td></tr>
        <tr><td>1</td></tr>
        <tr><td class='light-yellow'>2</td></tr>
        <tr><td>3</td></tr>
        </table>";

        string CSS = @"
            body {font-size: 12px;}
            table {border-collapse:collapse; margin:8px;}
            .light-yellow {background-color:#ffff99;}
            td {border:1px solid #ccc;padding:4px;}
        ";

        public void Go()
        {
            OUTPUT_FILE = Helpers.IO.GetClassOutputPath(this);
            ConvertHtmlToPdf(XHTML, CSS);
        }

        public void ConvertHtmlToPdf(string xHtml, string css)
        {
            using (var stream = new FileStream(OUTPUT_FILE, FileMode.Create))
            {
                using (var document = new Document())
                {
                    var writer = PdfWriter.GetInstance(document, stream);
                    document.Open();

                    // instantiate custom tag processor and add to `HtmlPipelineContext`.
                    var tagProcessorFactory = Tags.GetHtmlTagProcessorFactory();
                    tagProcessorFactory.AddProcessor(
                        new TableDataProcessor(), 
                        new string[] { HTML.Tag.TD }
                    );
                    var htmlPipelineContext = new HtmlPipelineContext(null);
                    htmlPipelineContext.SetTagFactory(tagProcessorFactory);

                    var pdfWriterPipeline = new PdfWriterPipeline(document, writer);
                    var htmlPipeline = new HtmlPipeline(htmlPipelineContext, pdfWriterPipeline);

                    // get an ICssResolver and add the custom CSS
                    var cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(true);
                    cssResolver.AddCss(css, "utf-8", true);
                    var cssResolverPipeline = new CssResolverPipeline(
                        cssResolver, htmlPipeline
                    );

                    var worker = new XMLWorker(cssResolverPipeline, true);
                    var parser = new XMLParser(worker);
                    using (var stringReader = new StringReader(xHtml))
                    {
                        parser.Parse(stringReader);
                    }
                }
            }
        }
    }
}