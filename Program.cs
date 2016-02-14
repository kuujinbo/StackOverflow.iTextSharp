using kuujinbo.StackOverflow.iTextSharp._test;
using kuujinbo.StackOverflow.iTextSharp._test.Forms;
using kuujinbo.StackOverflow.iTextSharp.Helpers;
using kuujinbo.StackOverflow.iTextSharp.ProgramCode;
using kuujinbo.StackOverflow.iTextSharp.ProgramCode.Chunks;
using kuujinbo.StackOverflow.iTextSharp.ProgramCode.ColumnTexts;
using kuujinbo.StackOverflow.iTextSharp.ProgramCode.Fonts;
using kuujinbo.StackOverflow.iTextSharp.ProgramCode.Forms;
using kuujinbo.StackOverflow.iTextSharp.ProgramCode.Images;
using kuujinbo.StackOverflow.iTextSharp.ProgramCode.MultiPass;
using kuujinbo.StackOverflow.iTextSharp.ProgramCode.XmlWorkers;

namespace kuujinbo.StackOverflow.iTextSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            new CheckboxFieldsPdf().WriteToDisk();
            // new TextBoxField().Go();
            // new TextBoxField().Go(8f);
            // new TestPdfWithTextFields().WriteToDisk();
            // new WrapText().Go();
            // new RemoveImageByClass().Go();
        }

        static void ReduceFontCreatePDF()
        {
            new ReduceFont().CreatePDF(50);
            int fontStart = 50;
            var r = new ReduceFont();
            while (r.CreatePDF(fontStart))
            {
                fontStart--;
            }
        }
    }
}