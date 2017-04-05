using System.IO;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;

// http://stackoverflow.com/questions/42674940
namespace kuujinbo.StackOverflow.iTextSharp.iText5.XmlWorkers
{
    public class JustifyCell
    {
        public void Go()
        {
            var OUTPUT = Helpers.IO.GetClassOutputPath(this);
            var html = @"
<html><head>
<title>Test HTML</title>
<style type='text/css'>
td { 
    border:1px solid #eaeaea; 
    padding: 4px;
    text-align: right; 
    font-size: 1.4em; 
}
</style>
</head><body>
<table width='50%'><tr>
<td>
but I can not see justification of text. 
I tried using only paragraph without using table, 
and it does justification but I need to display 
things in table
</td></tr></table>
</body></html>
            ";

            // CSS specificity selector: apply style below without changing existing styles
            string css = "tr td { text-align: justify; }";

            using (var memoryStream = new MemoryStream())
            {
                using (var document = new Document())
                {
                    PdfWriter writer = PdfWriter.GetInstance(
                        document, memoryStream
                    );
                    document.Open();
                    using (var htmlStream = new MemoryStream(Encoding.UTF8.GetBytes(html)))
                    {
                        using (var cssStream = new MemoryStream(Encoding.UTF8.GetBytes(css)))
                        {
                            XMLWorkerHelper.GetInstance().ParseXHtml(
                                writer, document, htmlStream, cssStream
                            );
                        }
                    }
                }
                File.WriteAllBytes(OUTPUT, memoryStream.ToArray());
            }



        }

    }
}
