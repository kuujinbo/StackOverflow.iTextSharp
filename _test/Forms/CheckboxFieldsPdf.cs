using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace kuujinbo.StackOverflow.iTextSharp._test.Forms
{
    public class CheckboxFieldsPdf
    {
        public const int FIELD_COUNT = 4;
        public const string TEXT_FIELD = "textfield";
        public const string ON_STATE = "Yes";
        //  potential bug in iTextSharp ^^^
        public static string[] FieldNames = new string[FIELD_COUNT];

        static CheckboxFieldsPdf()
        {
            for (int i = 0; i < FIELD_COUNT; ++i)
            {
                FieldNames[i] = string.Format("checkbox-{0}", i);
            }
        }

        public static byte[] GetPdf()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (Document document = new Document())
                {
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    document.Open();

                    int i = 0;
                    foreach (var fieldName in FieldNames)
                    {
                        var rectangle = new Rectangle(20, 800 - i * 40, 40, 780 - i * 40);
                        var checkbox = new RadioCheckField(
                            writer, rectangle, fieldName, ON_STATE
                        );
                        checkbox.CheckType = RadioCheckField.TYPE_CHECK;
                        PdfFormField field = checkbox.CheckField;
                        writer.AddAnnotation(field);
                        ++i;
                    }

                    // add textbox field for sanity-check
                    var textField = new TextField(
                        writer,
                        new Rectangle(20, 800 - i * 40, 400, 780 - i * 40),
                        TEXT_FIELD
                    );
                    writer.AddAnnotation(textField.GetTextField());
                }
                return ms.ToArray();
            }
        }

        public static byte[] GetPdfWithAllChecked()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (var reader = new PdfReader(GetPdf()))
                {
                    using (var stamper = new PdfStamper(reader, ms))
                    {
                        foreach (var name in FieldNames)
                        {
                            stamper.AcroFields.SetField(name, ON_STATE);
                        }
                    }
                }
                return ms.ToArray();
            }
        }



        #region kuujinbo
        // PDF 32000-1:2008, 12.7.4.2.3 Check Boxes - "The appearance for the off state is optional but, if present, shall be stored in the appearance dictionary under the name Off. Yes should be used as the name for the on state."
        public void WriteToDisk()
        {
            File.WriteAllBytes(Helpers.IO.GetClassOutputPath(this), GetPdfWithAllChecked());
        }
        #endregion

    }
}