using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;

namespace kuujinbo.StackOverflow.iTextSharp.ProgramCode.XmlWorkers
{
    public class EndTag
    {
        public const string HTML = @"
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
               <input type='text' />
               <!--
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

        public void Go()
        {
            var outputFile = Helpers.IO.GetClassOutputPath(this);
            StringReader xmlSnippet = new StringReader(HTML);

            using (FileStream stream = new FileStream(
                outputFile,
                FileMode.Create,
                FileAccess.Write))
            {
                using (Document document = new Document())
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