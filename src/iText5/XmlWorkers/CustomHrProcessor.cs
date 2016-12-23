using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.css;
using iTextSharp.tool.xml.exceptions;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;

// http://stackoverflow.com/questions/40802883
namespace kuujinbo.StackOverflow.iTextSharp.iText5.XmlWorkers
{
    public class CustomHorizontalRule : AbstractTagProcessor
    {
        public override IList<IElement> Start(IWorkerContext ctx, Tag tag)
        {
            IList<IElement> result;
            LineSeparator lineSeparator;
            var cssUtil = CssUtils.GetInstance();

            try
            {
                IList<IElement> list = new List<IElement>();
                HtmlPipelineContext htmlPipelineContext = this.GetHtmlPipelineContext(ctx);

                Paragraph paragraph = new Paragraph();
                IDictionary<string, string> css = tag.CSS;
                float baseValue = 12f;
                if (css.ContainsKey("font-size"))
                {
                    baseValue = cssUtil.ParsePxInCmMmPcToPt(css["font-size"]);
                }
                string text;
                css.TryGetValue("margin-top", out text);
                if (text == null) text = "0.5em";

                string text2;
                css.TryGetValue("margin-bottom", out text2);
                if (text2 == null) text2 = "0.5em";

                string border;
                css.TryGetValue(CSS.Property.BORDER_BOTTOM_STYLE, out border);
                lineSeparator = border != null && border == "dotted"
                    ? new DottedLineSeparator()
                    : new LineSeparator();

                var element = (LineSeparator)this.GetCssAppliers().Apply(
                    lineSeparator, tag, htmlPipelineContext
                );

                string color;
                css.TryGetValue(CSS.Property.BORDER_BOTTOM_COLOR, out color);
                if (color != null)
                {
                    // WebColors deprecated, but docs don't state replacement
                    element.LineColor = WebColors.GetRGBColor(color);
                }

                paragraph.SpacingBefore += cssUtil.ParseValueToPt(text, baseValue);
                paragraph.SpacingAfter += cssUtil.ParseValueToPt(text2, baseValue);
                paragraph.Leading = 0f;
                paragraph.Add(element);
                list.Add(paragraph);
                result = list;
            }
            catch (NoCustomContextException cause)
            {
                throw new RuntimeWorkerException(
                    LocaleMessages.GetInstance().GetMessage("customcontext.404"),
                    cause
                );
            }
            return result;
        }
    }

    public class CustomHrProcessor
    {
        string OUTPUT_FILE;
        public void Go()
        {
            OUTPUT_FILE = Helpers.IO.GetClassOutputPath(this);
            ConvertHtmlToPdf();
        }

        public void ConvertHtmlToPdf()
        {
            using (var stream = new FileStream(OUTPUT_FILE, FileMode.Create))
            {
                using (var document = new Document())
                {
                    var writer = PdfWriter.GetInstance(document, stream);
                    document.Open();

                    var tagProcessorFactory = Tags.GetHtmlTagProcessorFactory();
                    // custom tag processor above
                    tagProcessorFactory.AddProcessor(
                        new CustomHorizontalRule(),
                        new string[] { HTML.Tag.HR }
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
                    var xHtml = "<hr style='border:1px dotted red' />";
                    using (var stringReader = new StringReader(xHtml))
                    {
                        parser.Parse(stringReader);
                    }
                }
            }
        }
    }
}