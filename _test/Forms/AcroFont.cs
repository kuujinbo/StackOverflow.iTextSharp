using System;
using System.IO;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;

// set all form _formFields to use a Standard Type 1 (14) font with explicit size
// https://en.wikipedia.org/wiki/Portable_Document_Format#Standard_Type_1_Fonts_.28Standard_14_Fonts.29
namespace kuujinbo.StackOverflow.iTextSharp._test.Forms
{
    public class AcroFont
    {
        public void Go(float fontSize)
        {
            var fileName = "text_fields.pdf";
            var readerPath = Helpers.IO.GetInputFilePath(fileName);
            var outputFile = Helpers.IO.GetClassOutputPath(this);


            var baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, false);
            using (var reader = new PdfReader(TestPdfWithTextFields.GetBytes()))
            {
                using (var stream = new FileStream(outputFile, FileMode.Create))
                {
                    using (var stamper = new PdfStamper(reader, stream))
                    {
                        var fields = stamper.AcroFields;
                        new TextFieldFont().SetTemplateFont(
                            fields, BaseFont.CreateFont(), fontSize
                        );

                        foreach (var name in TestPdfWithTextFields.FieldNames)
                        {
                            fields.SetField(name, name);
                        }
                    }
                }
            }
        }
    }
}