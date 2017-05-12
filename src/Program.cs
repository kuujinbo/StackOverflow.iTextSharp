using kuujinbo.StackOverflow.iTextSharp.Helpers;
using kuujinbo.StackOverflow.iTextSharp.iText5;
using kuujinbo.StackOverflow.iTextSharp.iText5.XmlWorkers;
using kuujinbo.StackOverflow.iTextSharp.iText7.Element;

namespace kuujinbo.StackOverflow.iTextSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            new Hyphens().Go();
            // ReduceFontCreatePDF();
        }

        static void ReduceFontCreatePDF()
        {
            // new ReduceFont().CreatePDF(50);
            int fontStart = 50;
            var r = new MultiPassReduceFont();
            while (r.CreatePDF(fontStart))
            {
                fontStart--;
            }
        }
    }
}