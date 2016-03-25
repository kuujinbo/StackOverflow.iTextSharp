using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using HtmlAgilityPack;

// http://stackoverflow.com/questions/35200436
namespace kuujinbo.StackOverflow.iTextSharp.ProgramCode.XmlWorkers
{
    public class EndTag
    {
        const string HTML = @"
<div class='col-xs-12 GPAMainForm'>
  <div class='col-xs-10 col-xs-offset-1'>
    <div style='border: 1px double black; padding: 2px'>
      <div>
        <table class='table'>
          <tr class='border'>
            <td class='border'>
              <label class='control-label'>Company Name</label>
            </td>
            <td class='border'>
<!-- after edit -->
<input name='ctl00$MainContent$TextBox2' id='MainContent_TextBox2' type='text'>
               <!--
               <input type='text' />
               If I remove input tag, pdf gets generated else it show error like:
               `Invalid nested tag td found, expected closing tag input`
               -->
            </td>
          </tr>
        </table>
      </div>
    </div>
  </div>
</div>
";
        // been a while; forgot how **BROKEN** server control markup can be
        string FixBrokenServerControlMarkup(string brokenHtml)
        {
            HtmlDocument hDocument = new HtmlDocument()
            {
                OptionOutputAsXml = true,
                OptionAutoCloseOnEnd = true
            };
            hDocument.LoadHtml(brokenHtml);
            return hDocument.DocumentNode.WriteTo();
        }

        public void Go()
        {
            var outputFile = Helpers.IO.GetClassOutputPath(this);
            var fixedHtml = FixBrokenServerControlMarkup(HTML);
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
                    using (var xmlSnippet = new StringReader(fixedHtml))
                    {
                        XMLWorkerHelper.GetInstance().ParseXHtml(
                            writer, document, xmlSnippet
                        );
                    }

                }
            }
        }
    }
}