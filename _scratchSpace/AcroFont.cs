using System;
using System.IO;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;

// set all form fields to use a Standard Type 1 (14) font with explicit size
// https://en.wikipedia.org/wiki/Portable_Document_Format#Standard_Type_1_Fonts_.28Standard_14_Fonts.29
namespace kuujinbo.StackOverflow.iTextSharp._scratchSpace
{
    public class AcroFont
    {
        public void Go(float fontSize)
        {
            var fileName = "datasheet.pdf";
            var readerPath = Helpers.IO.GetInputFilePath(fileName);
            var outputFile = Helpers.IO.GetClassOutputPath(this);

            // Standard Type 1 (14) fonts
            // BaseFont.HELVETICA
            // BaseFont.COURIER
            // BaseFont.TIMES_ROMAN
            var baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, false);
            using (var reader = new PdfReader(readerPath))
            {
                using (var stream = new FileStream(outputFile, FileMode.Create))
                {
                    using (var stamper = new PdfStamper(reader, stream))
                    {
                        AcroFields fields = stamper.AcroFields;
                        foreach (var kv in fields.Fields)
                        {
                            fields.SetFieldProperty(
                                kv.Key, "textfont", baseFont, null
                            );
                            fields.SetFieldProperty(
                                kv.Key, "textsize", fontSize, null
                            );
                            fields.SetField(
                                kv.Key, string.Format("Form field name : [{0}]", kv.Key)
                            );
                        }
                    }
                }
            }
        }
    }
}