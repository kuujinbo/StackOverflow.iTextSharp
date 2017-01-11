using System.Collections.Generic;
using System.IO;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.html.table;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;

// http://stackoverflow.com/questions/35283892
namespace kuujinbo.StackOverflow.iTextSharp.iText5.XmlWorkers
{
    public class TableNoRowSplit
    {
        string OUTPUT_FILE;
        public TableNoRowSplit()
        {
            OUTPUT_FILE = Helpers.IO.GetClassOutputPath(this);
        }

        public string GetHtml()
        {
            var html = new StringBuilder();
            var repeatCount = 15;
            for (int i = 0; i < repeatCount; ++i) { html.Append("<h1>h1</h1>"); }

            var text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer vestibulum sollicitudin luctus. Curabitur at eros bibendum, porta risus a, luctus justo. Phasellus in libero vulputate, fermentum ante nec, mattis magna. Nunc viverra viverra sem, et pulvinar urna accumsan in. Quisque ultrices commodo mauris, et convallis magna. Duis consectetur nisi non ultrices dignissim. Aenean imperdiet consequat magna, ac ornare magna suscipit ac. Integer fermentum velit vitae porttitor vestibulum. Morbi iaculis sed massa nec ultricies. Aliquam efficitur finibus dolor, et vulputate turpis pretium vitae. In lobortis lacus diam, ut varius tellus varius sed. Integer pulvinar, massa quis feugiat pulvinar, tortor nisi bibendum libero, eu molestie est sapien quis odio. Lorem ipsum dolor sit amet, consectetur adipiscing elit.";

            // default iTextSharp.tool.xml.html.table.Table (AbstractTagProcessor)
            // is at the <table>, **not <tr> level
            html.Append("<table style='page-break-inside:avoid;'>");
            html.AppendFormat(
                @"<tr><td style='border:1px solid #000;'>DEFAULT IMPLEMENTATION</td>
            <td style='border:1px solid #000;'>{0}</td></tr>",
                text
            );
            html.Append("</table>");

            // overriden implementation uses a custom HTML attribute to keep:
            // <tr> together - see TableProcessor
            html.Append("<table no-row-split style='page-break-inside:avoid;'>");
            for (int i = 0; i < repeatCount; ++i)
            {
                html.AppendFormat(
                    @"<tr><td style='border:1px solid #000;'>{0}</td>
            <td style='border:1px solid #000;'>{1}</td></tr>",
                    i, text
                );
            }
            html.Append("</table>");
            return html.ToString();
        }

        public void Go()
        {

            using (var stream = new FileStream(OUTPUT_FILE, FileMode.Create))
            {
                using (var document = new Document())
                {
                    PdfWriter writer = PdfWriter.GetInstance(
                        document, stream
                    );
                    document.Open();

                    // instantiate custom tag processor and add to `HtmlPipelineContext`.
                    var tagProcessorFactory = Tags.GetHtmlTagProcessorFactory();
                    tagProcessorFactory.AddProcessor(
                        new TableProcessor(),
                        new string[] { HTML.Tag.TABLE }
                    );
                    var htmlPipelineContext = new HtmlPipelineContext(null);
                    htmlPipelineContext.SetTagFactory(tagProcessorFactory);

                    var pdfWriterPipeline = new PdfWriterPipeline(document, writer);
                    var htmlPipeline = new HtmlPipeline(htmlPipelineContext, pdfWriterPipeline);

                    var cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(true);
                    var cssResolverPipeline = new CssResolverPipeline(
                        cssResolver, htmlPipeline
                    );

                    var worker = new XMLWorker(cssResolverPipeline, true);
                    var parser = new XMLParser(worker);
                    using (var stringReader = new StringReader(GetHtml()))
                    {
                        parser.Parse(stringReader);
                    }
                }
            }
        }
    }

    public class TableProcessor : Table
    {
        // custom HTML attribute to keep <tr> on same page if possible
        public const string NO_ROW_SPLIT = "no-row-split";
        public override IList<IElement> End(IWorkerContext ctx, Tag tag, IList<IElement> currentContent)
        {
            IList<IElement> result = base.End(ctx, tag, currentContent);
            var table = (PdfPTable)result[0];

            if (tag.Attributes.ContainsKey(NO_ROW_SPLIT))
            {
                // if not set,  table **may** be forwarded to next page
                table.KeepTogether = false;
                // next two properties keep <tr> together if possible
                table.SplitRows = true;
                table.SplitLate = true;
            }
            return new List<IElement>() { table };
        }
    }

    /* can also implement this way, but will override default behavior!
    if (tag.CSS.ContainsKey(CSS.Property.PAGE_BREAK_INSIDE)
        && string.Equals(
            tag.CSS[CSS.Property.PAGE_BREAK_INSIDE],
            "avoid",
            StringComparison.OrdinalIgnoreCase)
        )
    {
        // if not set,  table **may** be forwarded to next page
        table.KeepTogether = false;
        // next two properties keep <tr> together if possible
        table.SplitRows = true;
        table.SplitLate = true;
    }
    */
}