using System;
using iTextSharp.text.pdf;

namespace kuujinbo.StackOverflow.iTextSharp._test.Forms
{
    public class TextFieldFont
    {
        private AcroFields _formFields;

        public void SetTemplateFont(AcroFields FormFields, BaseFont family, float size)
        {
            _formFields = FormFields;
            foreach (var key in FormFields.Fields.Keys)
            {
                var isAuto = IsAutoFont(key);

                if (isAuto)
                {
                    FormFields.SetFieldProperty(key, "textfont", family, null);
                    FormFields.SetFieldProperty(key, "textsize", size, null);
                }
            }
        }

        /// <summary>
        /// Check if a text field has auto size font.
        /// </summary>
        /// <param name="fieldName">The form field name</param>
        /// <returns>Flag if form field has auto sized font</returns>
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

                    Console.WriteLine(
                        "{0} font: {1}",
                        fieldName, daNames[1]
                    );

                    return daNames[1] != null && daNames[1].ToString() == "0"
                        ? true : false;
                }
            }

            return false;
        }


    }
}