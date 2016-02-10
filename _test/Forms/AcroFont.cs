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
        private AcroFields _formFields;

        public bool IsAutoFont(string fieldName)
        {
            // need the dictionary field appearance
            if (_formFields.GetFieldType(fieldName) == AcroFields.FIELD_TYPE_TEXT)
            {
                var pdfDictionary = _formFields.GetFieldItem(fieldName).GetMerged(0);
                var pdfString = pdfDictionary.GetAsString(PdfName.DA);
                if (pdfString != null)
                {
                    var daNames = AcroFields.SplitDAelements(pdfString.ToString());

                    float size;
                    if (daNames[1] != null && Single.TryParse(daNames[1].ToString(), out size))
                    {
                        Console.WriteLine("Form field [{0}] :: size {1}", fieldName, size);
                    }

                    return daNames[1] != null && daNames[1].ToString() == "0"
                        ? true : false;
                }
            }

            return false;
        }

        public void Go(float fontSize)
        {
            var fileName = "text_fields.pdf";
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
                        _formFields = stamper.AcroFields;
                        foreach (var kv in _formFields.Fields)
                        {
                            Console.WriteLine(
                                "{0} is auto font: {1}", 
                                kv.Key, IsAutoFont(kv.Key)
                            );

                            _formFields.SetFieldProperty(
                                kv.Key, "textfont", baseFont, null
                            );
                            _formFields.SetFieldProperty(
                                kv.Key, "textsize", fontSize, null
                            );
                            _formFields.SetField(
                                kv.Key, string.Format("Form field name : [{0}]", kv.Key)
                            );
                        }
                    }
                }
            }
        }
    }
}