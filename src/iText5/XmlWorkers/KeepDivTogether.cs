using System;
using System.IO;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;

// http://stackoverflow.com/questions/41514467
namespace kuujinbo.StackOverflow.iTextSharp.iText5.XmlWorkers
{
    public class KeepDivTogether
    {
        string OUTPUT_FILE;
        public KeepDivTogether()
        {
            OUTPUT_FILE = Helpers.IO.GetClassOutputPath(this);
        }

        public string GetHtml()
        {
            var html = new StringBuilder();
            var repeatCount = 15;
            for (int i = 0; i < repeatCount; ++i) { html.Append("<h1>h1</h1>"); }

            var text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer vestibulum sollicitudin luctus. Curabitur at eros bibendum, porta risus a, luctus justo. Phasellus in libero vulputate, fermentum ante nec, mattis magna. Nunc viverra viverra sem, et pulvinar urna accumsan in. Quisque ultrices commodo mauris, et convallis magna. Duis consectetur nisi non ultrices dignissim. Aenean imperdiet consequat magna, ac ornare magna suscipit ac. Integer fermentum velit vitae porttitor vestibulum. Morbi iaculis sed massa nec ultricies. Aliquam efficitur finibus dolor, et vulputate turpis pretium vitae. In lobortis lacus diam, ut varius tellus varius sed. Integer pulvinar, massa quis feugiat pulvinar, tortor nisi bibendum libero, eu molestie est sapien quis odio. Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
            html.Append(text);
            for (int i = 0; i < repeatCount; ++i)
            {
                html.AppendFormat(
                    "<div style='page-break-inside:avoid;>{0}</div>",
                    text
                );
            }
            return html.ToString();
        }

        public void Go()
        {
            using (var html = new StringReader(GetHtml()))
            {
                using (FileStream stream = new FileStream(
                    OUTPUT_FILE,
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
                            writer, document, html
                        );
                    }
                }
            }
        }
    }
}