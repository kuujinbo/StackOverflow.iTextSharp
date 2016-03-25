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
namespace kuujinbo.StackOverflow.iTextSharp.ProgramCode.XmlWorkers
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
        public void Go()
        {
            var OUTPUT_FILE = Helpers.IO.GetClassOutputPath(this);


    string XHTML = @"
    <table border='1'><tr>
    <td style='writing-mode:sideways-lr;text-align:center;width:40px;'>First</td>
    <td style='writing-mode:sideways-lr;text-align:center;width:40px;'>Second</td></tr>
    <tr><td style='text-align:center'>1</td>
    <td style='text-align:center'>2</td></tr></table>";

    using (FileStream stream = new FileStream(OUTPUT_FILE, FileMode.Create))
    {
        using (Document document = new Document())
        {
            var writer = PdfWriter.GetInstance(document, stream);
            document.Open();

            var tagProcessorFactory = Tags.GetHtmlTagProcessorFactory();
            tagProcessorFactory.AddProcessor(
                new TableDataProcessor(), 
                new string[] { HTML.Tag.TD }
            );

            var htmlPipelineContext = new HtmlPipelineContext(null);
            htmlPipelineContext.SetTagFactory(tagProcessorFactory);

            var pdfWriterPipeline = new PdfWriterPipeline(document, writer);
            var htmlPipeline = new HtmlPipeline(htmlPipelineContext, pdfWriterPipeline);
            var cssResolverPipeline = new CssResolverPipeline(
                XMLWorkerHelper.GetInstance().GetDefaultCssResolver(true), 
                htmlPipeline
            );
            XMLWorker worker = new XMLWorker(cssResolverPipeline, true);
            XMLParser parser = new XMLParser(worker);
            using (var stringReader = new StringReader(XHTML))
            {
                parser.Parse(stringReader);
            }
        }
    }

        }
    }
}