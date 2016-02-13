using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace kuujinbo.StackOverflow.iTextSharp._test.Forms
{
    public class TestPdfWithTextFields 
    {
        public const int FIELD_COUNT = 4;
        public static string[] FieldNames = new string[FIELD_COUNT];

        static TestPdfWithTextFields()
        {
            for (int i = 1; i <= FIELD_COUNT; ++i)
            {
                FieldNames[i - 1] = string.Format("field-{0}", i);
            }
        }

        public static byte[] GetBytes()
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
                        PdfPCell cell = new PdfPCell() { 
                            HorizontalAlignment = Rectangle.ALIGN_RIGHT, 
                            Phrase = new Phrase(fieldName)
                        };
                        table.AddCell(cell);
                        cell = new PdfPCell() 
                        {
                            CellEvent = new TestCellEvent(fieldName) 
                        };
                        table.AddCell(cell);
                    }

                    document.Add(table);
                }
                return ms.ToArray();
            }
        }

        public void WriteToDisk()
        {
            File.WriteAllBytes(Helpers.IO.GetClassOutputPath(this), GetBytes());
        }


        public class TestCellEvent : IPdfPCellEvent
        {
            string _fieldName;
            BaseFont _default = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.WINANSI, false);

            public TestCellEvent(string fieldName) { _fieldName = fieldName; }

            public void CellLayout(PdfPCell cell, Rectangle rectangle, PdfContentByte[] canvases)
            {
                PdfWriter writer = canvases[0].PdfWriter;
                TextField text = new TextField(writer, rectangle, _fieldName);

                switch (_fieldName.EndsWith("4"))
                {
                    case true:
                        text.FontSize = 12f;
                        text.Font = _default;
                        break;
                    default:
                        text.FontSize = 0;
                        text.Font = _default;
                        break;
                }

                PdfFormField field = text.GetTextField();
                writer.AddAnnotation(field);
            }
        }


    }
}