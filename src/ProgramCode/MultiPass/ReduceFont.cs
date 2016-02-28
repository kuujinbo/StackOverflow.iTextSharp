using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace kuujinbo.StackOverflow.iTextSharp.ProgramCode.MultiPass
{
    /*
     * http://stackoverflow.com/questions/34276668/
     * URL above no longer valid, closed by resident SO village idiots:
     * -- OP posted unique question relavant to iTextSharp
     * -- OP described problem well
     * -- OP provided sample code showing effort to solve problem
     * -- resident SO village idiots who voted to close question HAD
     *    ZERO (0) answers for iTextSharp tagged questions **COMBINED**.
     * -- screenshot: https://drive.google.com/file/d/0B88fJJybQ9yPWGJoay0xOGdmaW8/view?usp=sharing
     * 
     * <quote>
     * Q: I have created a pdf using itextsharp within my code... if the 
     * document spans more than one page i reduce the Font and call the 
     * "CreatePDF" method again....
     * </quote>
     */
    public class ReduceFont
    {
        const string TEST_STRING = @"
if the document spans more than one page i reduce the Font and call the 
'CreatePDF' method again...(to rewrite the pdf)...";

        public bool CreatePDF(int fontSize)
        {
            Console.WriteLine(Helpers.IO.GetClassOutputPath(this));
            var font = new Font(Font.NORMAL, fontSize, 1, new BaseColor(0, 0, 0));
            var onePageDoc = new OnePageDocument();
            // GetClassOutputPath() implementation left out for brevity
            var outputFile = Helpers.IO.GetClassOutputPath(this);
            using (FileStream stream = new FileStream(
                outputFile, 
                FileMode.Create, 
                FileAccess.Write))
            {
                using (Document document = new Document())
                {
                    var writer = PdfWriter.GetInstance(document, stream);
                    writer.PageEvent = onePageDoc;
                    document.Open();
                    for (int i = 0; i < 4; ++i)
                    {
                        document.Add(new Paragraph(TEST_STRING, font));
                    }
                }
            }
            return onePageDoc.TotalPages > 1 ? true : false;
        } 
    }

    public class OnePageDocument : PdfPageEventHelper
    {
        public int TotalPages { get; private set; }
        public override void OnEndPage(PdfWriter writer, Document document)
        {
            TotalPages = writer.PageNumber;
        }
    }


}