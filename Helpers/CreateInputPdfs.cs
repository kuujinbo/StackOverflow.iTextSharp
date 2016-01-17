using System;
using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace kuujinbo.StackOverflow.iTextSharp.Helpers
{
    public static class CreateInputPdfs
    {
        public static List<string> FourTestPdfPaths()
        {
            List<string> results = new List<string>();
            for (int i = 0; i < 4; ++i)
            {
                results.Add(IO.GetInputFilePath(string.Format("{0}.pdf", i)));
            }
            return results;
        }

        public static void CreateFourTestPdfs()
        {
            foreach (var outputFile in FourTestPdfPaths())
            {
                using (FileStream stream = new FileStream(
                    outputFile,
                    FileMode.Create,
                    FileAccess.Write))
                {
                    using (Document document = new Document())
                    {
                        PdfWriter.GetInstance(document, stream);
                        document.Open();
                        document.Add(new Phrase(
                            Path.GetFileNameWithoutExtension(outputFile)
                        ));
                    }
                }
            }
        }


    }
}