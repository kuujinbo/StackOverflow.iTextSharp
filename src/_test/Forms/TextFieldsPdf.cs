using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace kuujinbo.StackOverflow.iTextSharp._test.Forms
{
    public class TextFieldsPdf
    {
        public const float DEFAULT_SIZE = 12f;
        public const int FIELD_COUNT = 4;
        public static string[] FieldNames = new string[FIELD_COUNT];
        public static readonly int AUTO_SIZE_FIELD = FIELD_COUNT - 1;
        public static readonly string HELVETICA = BaseFont.HELVETICA;
        public static readonly string COURIER_OBLIQUE = BaseFont.COURIER_OBLIQUE;

        static TextFieldsPdf()
        {
            for (int i = 0; i < FIELD_COUNT; ++i)
            {
                FieldNames[i] = string.Format("field-{0}", i);
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

                    PdfPTable table = new PdfPTable(2);
                    table.SetWidths(new int[] { 1, 2 });

                    foreach (var fieldName in FieldNames)
                    {
                        PdfPCell cell = new PdfPCell()
                        {
                            HorizontalAlignment = Rectangle.ALIGN_RIGHT,
                            Phrase = new Phrase(fieldName)
                        };
                        table.AddCell(cell);
                        cell = new PdfPCell()
                        {
                            CellEvent = new TextFieldCellEvent(fieldName)
                        };
                        table.AddCell(cell);
                    }

                    document.Add(table);
                }
                return ms.ToArray();
            }
        }

        // creating the template's form fields
        public class TextFieldCellEvent : IPdfPCellEvent
        {
            string _fieldName;
            BaseFont _defaultFont = BaseFont.CreateFont(
                HELVETICA, BaseFont.WINANSI, false
            );

            public TextFieldCellEvent(string fieldName) { _fieldName = fieldName; }

            public void CellLayout(PdfPCell cell, Rectangle rectangle, PdfContentByte[] canvases)
            {
                PdfWriter writer = canvases[0].PdfWriter;
                TextField text = new TextField(writer, rectangle, _fieldName);

                switch (!_fieldName.EndsWith(AUTO_SIZE_FIELD.ToString()))
                {
                    case true:
                        text.FontSize = DEFAULT_SIZE;
                        text.Font = _defaultFont;
                        break;
                    default:
                        text.FontSize = 0;
                        text.Font = BaseFont.CreateFont(
                            COURIER_OBLIQUE, BaseFont.WINANSI, false
                        );
                        break;
                }

                PdfFormField field = text.GetTextField();
                writer.AddAnnotation(field);
            }
        }




        #region kuujinbo
        public void WriteToDisk()
        {
            File.WriteAllBytes(Helpers.IO.GetClassOutputPath(this), GetPdf());
        }
        #endregion
    }
}