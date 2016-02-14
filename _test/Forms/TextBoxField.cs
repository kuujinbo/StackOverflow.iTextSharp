using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;

// fit a delimited string into a AcroField
// TODO: single line / check if AcroFields.FIELD_TYPE_TEXT??
namespace kuujinbo.StackOverflow.iTextSharp._test.Forms
{
    public class TextBoxField
    {
        char _delimiter = ',';
        public char Delimiter
        {
            get { return _delimiter; }
            set { value = _delimiter; }
        }

        /// <summary>
        /// Get a Standard Type 1 (14) font from a text field
        /// </summary>
        /// <param name="acroFields">AcroFields</param>
        /// <param name="fieldName">Text field name</param>
        /// <returns>
        /// Standard Type 1 (14) font if possible, or BaseFont.HELVETICA
        /// </returns>
        public static BaseFont GetStandardFont(AcroFields acroFields, string fieldName)
        {
            var baseFont = BaseFont.CreateFont();
            try
            {
                var item = acroFields.GetFieldItem(fieldName);
                var pdfDictionary = item.GetMerged(0);
                var textField = new TextField(null, null, null);
                acroFields.DecodeGenericDictionary(pdfDictionary, textField);
                baseFont = BaseFont.CreateFont(
                    // keep next line for reference: Google and StackOverflow aren't always right...
                    // textField.Font.FullFontName[0][3],
                    textField.Font.PostscriptFontName,
                    BaseFont.WINANSI,
                    false
                );
            }
            catch
            {
                // iTextSharp.text.DocumentException, but we don't care
            }
            return baseFont;
        }

        /// <summary>
        /// Set font family and font size on all text fields for consistency
        /// </summary>
        /// <param name="stamperFields">
        /// PdfStamper.AcroFields - so we can set the form field value here.
        /// </param>
        /// <param name="size">Desired size</param>
        public static void SetTemplateFont(AcroFields stamperFields, float size)
        {
            SetTemplateFont(stamperFields, BaseFont.CreateFont(), size);
        }
        /// <summary>
        /// Set font family and font size on all text fields for consistency
        /// </summary>
        /// <param name="stamperFields">
        /// PdfStamper.AcroFields - so we can set the form field value here.
        /// </param>
        /// <param name="family">BaseFont</param>
        /// <param name="size">Desired size</param>
        public static void SetTemplateFont(AcroFields stamperFields, BaseFont family, float size)
        {
            // ignore everything except text fields
            var textFields = stamperFields.Fields.Keys
                .Where(x => stamperFields.GetFieldType(x) == AcroFields.FIELD_TYPE_TEXT
                    && GetFontSize(stamperFields, x) != 0f
                )
                .ToDictionary(k => k);

            Console.WriteLine(string.Join(" :: ", textFields.Keys.ToArray()));

            foreach (var key in textFields.Keys)
            {
                stamperFields.SetFieldProperty(key, "textfont", family, null);
                stamperFields.SetFieldProperty(key, "textsize", size, null);
            }
        }

        /// <summary>
        /// Get text field font size
        /// </summary>
        /// <param name="acroFields">AcroFields</param>
        /// <param name="fieldName">Text field name</param>
        /// <returns>text field font size</returns>
        public static float GetFontSize(AcroFields acroFields, string fieldName)
        {
            if (acroFields.GetFieldType(fieldName) == AcroFields.FIELD_TYPE_TEXT)
            {
                // need the dictionary field appearance
                var pdfDictionary = acroFields.GetFieldItem(fieldName).GetMerged(0);
                var pdfString = pdfDictionary.GetAsString(PdfName.DA);
                if (pdfString != null)
                {
                    var daNames = AcroFields.SplitDAelements(pdfString.ToString());

                    float size;
                    return daNames[1] != null
                        && Single.TryParse(daNames[1].ToString(), out size)
                        ? size : 0f;
                }
            }

            return 0f; // text field auto-sized font
        }

        /// <summary>
        /// Flag whether text field has auto font size
        /// </summary>
        /// <param name="acroFields">AcroFields</param>
        /// <param name="fieldName">Text field name</param>
        /// <returns>Flag whether text field has auto font size</returns>
        public static bool IsAutoFont(AcroFields acroFields, string fieldName)
        {
            return GetFontSize(acroFields, fieldName).ToString() == "0";
        }


        public static bool IsMultiLine(AcroFields acroFields, string fieldName)
        {
 		  var item = acroFields.Fields[fieldName];
  		  var flags = item.GetMerged(0).GetAsNumber(PdfName.FF).IntValue;
  		  return (flags & BaseField.MULTILINE) > 0;
        }



        private void ValidateFitSingleLineText(AcroFields acroFields, string fieldName)
        {
            if (acroFields.GetFieldType(fieldName) != AcroFields.FIELD_TYPE_TEXT)
            {
                throw new InvalidOperationException(string.Format(
                    "field [{0}] is not a TextField",
                    fieldName
                ));
            }

            if (IsMultiLine(acroFields, fieldName))
            {
                throw new InvalidOperationException(string.Format(
                    "only single line TextField is allowed; field [{0}] is multiline.",
                    fieldName
                ));
            }
        }

        // would be nice if we could use ColumnText, but there's no way to get **remaining** text:
        // http://itext.2136553.n4.nabble.com/Remain-Text-on-ColumnText-td2146469.html
        public void FitSingleLine(AcroFields acroFields,
            string toFit, string fieldName,
            out string fits, out string overflows)
        {
            ValidateFitSingleLineText(acroFields, fieldName);

            var fieldWidth = acroFields.GetFieldPositions(fieldName)[0].position.Width;
            var fontSize = GetFontSize(acroFields, fieldName);
            var baseFont = GetStandardFont(acroFields, fieldName);

            var delimiter = Delimiter.ToString();
            var splitter = new char[] { Delimiter };
            var split = toFit.Split(splitter, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim()).ToArray();

            var paddedWidth = fieldWidth - baseFont.GetWidthPoint("0", fontSize) * 2;

            int count = 0;
            for (; count < split.Length; )
            {
                string testString = string.Join(delimiter.ToString(), split.Take(++count).ToArray());
                var testWidth = baseFont.GetWidthPoint(testString, fontSize);
                if (paddedWidth < testWidth)
                {
                    --count;
                    break;
                }
            }

            fits = string.Join(delimiter, split.Take(count).ToArray());
            overflows = string.Join(delimiter, split.Skip(count).Take(split.Length - count).ToArray());
        }




        #region kuujinbo
        // try to get max font size that fit in rectangle
        public void VerticalFitSize(Rectangle rectangle, string text)
        {
            var baseFont = BaseFont.CreateFont();
            int height = baseFont.GetAscent(text) - baseFont.GetDescent(text);
            float size = 1000f * rectangle.Height / height;
        }

        const string fileName = "datasheet.pdf";
        
        public void Go(float fontSize)
        {
            var outputFile = Helpers.IO.GetClassOutputPath(this);

            // using (var reader = new PdfReader(TestPdfWithTextFields.GetBytes()))
            using (var reader = Helpers.IO.GetInputReader(fileName))
            {
                using (var stream = new FileStream(outputFile, FileMode.Create))
                {
                    using (var stamper = new PdfStamper(reader, stream))
                    {
                        var fields = stamper.AcroFields;
                        SetTemplateFont(fields, fontSize);

                        //foreach (var name in TextFieldsCreator.FieldNames)
                        foreach (var name in fields.Fields.Keys)
                        {
                            Console.WriteLine("in Go(float fontSize), field name: {0}", name);

                            fields.SetField(name, name);
                        }
                    }
                }
            }
        }

        public void Go()
        {
            var outputFile = Helpers.IO.GetClassOutputPath(this);
            var testField = "title";
            var testText = "0";
            List<string> testTextList = new List<string>();
            for (int i = 1; i <= 76; ++i)
            {
                testTextList.Add(string.Format("{0}[{1}]", testText, i));
            }
            var baseFont = BaseFont.CreateFont();
            float testSize = 8f;
            char delimiter = ',';
            var testJoined = string.Join(delimiter.ToString(), testTextList.ToArray());

            using (var reader = Helpers.IO.GetInputReader(fileName))
            {
                using (var stream = new FileStream(outputFile, FileMode.Create))
                {
                    using (var stamper = new PdfStamper(reader, stream))
                    {
                        AcroFields fields = stamper.AcroFields;
                        var width = fields.GetFieldPositions("title")[0].position.Width;

                        fields.SetFieldProperty(
                            testField, "textfont", baseFont, null
                        );
                        fields.SetFieldProperty(
                            testField, "textsize", testSize, null
                        );

                        string fit;
                        string over;
                        Console.WriteLine("TEST STRING: {0}\n", testJoined);

                        FitSingleLine(fields, testJoined, "title", out fit, out over);
                        Console.WriteLine("fit: {0}\n", fit);
                        Console.WriteLine("over: {0}\n", over);

                        fields.SetField(testField, fit);
                    }
                }
            }
        }
        #endregion 
    }
}