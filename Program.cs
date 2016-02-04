using kuujinbo.StackOverflow.iTextSharp._scratchSpace;
using kuujinbo.StackOverflow.iTextSharp.Helpers;
using kuujinbo.StackOverflow.iTextSharp.ProgramCode;
using kuujinbo.StackOverflow.iTextSharp.ProgramCode.Chunks;
using kuujinbo.StackOverflow.iTextSharp.ProgramCode.ColumnTexts;
using kuujinbo.StackOverflow.iTextSharp.ProgramCode.Fonts;
using kuujinbo.StackOverflow.iTextSharp.ProgramCode.Forms;
using kuujinbo.StackOverflow.iTextSharp.ProgramCode.Images;
using kuujinbo.StackOverflow.iTextSharp.ProgramCode.MultiPass;

namespace kuujinbo.StackOverflow.iTextSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            new FitTextInField().Go();
            // new AcroFont().Go(4f);
            // new ChineseSetSplitCharacter().Go();
            // new AddBase64Image().Go();
            // CreateInputPdfs.CreateFourTestPdfs();
            // new EveryOther().Go();
            // new SetSplitCharacter().Go();
            // ReduceFontCreatePDF();
        }

        static void ReduceFontCreatePDF()
        {
            new ReduceFont().CreatePDF(50);
            int fontStart = 50;
            var r = new ReduceFont();
            while (r.CreatePDF(fontStart))
            {
                // Console.WriteLine(fontStart);
                fontStart--;
            }
        }
    }
}