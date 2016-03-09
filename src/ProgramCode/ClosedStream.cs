using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

// http://stackoverflow.com/questions/35733224
namespace kuujinbo.StackOverflow.iTextSharp.ProgramCode
{
    public class ClosedStream
    {
        public void Go()
        {
            var outputFile = Helpers.IO.GetClassOutputPath(this);
            byte[] pdf = null;

            PdfWriter writer = null;
            try
            {
                using (var stream = new MemoryStream())
                {
                    using (var document = new Document())
                    {
                        writer = PdfWriter.GetInstance(document, stream);
                        document.Open();
                        document.Add(new Chunk("test"));
                    }
                    pdf = stream.ToArray();
                }
            }
            finally { writer.Dispose(); }

            File.WriteAllBytes(outputFile, pdf);
        }

        // ObjectDisposedException: Cannot access a closed Stream.
        public void Go00()
        {
            var outputFile = Helpers.IO.GetClassOutputPath(this);
            byte[] pdf = null;
            using (var stream = new MemoryStream())
            {
                using (var document = new Document())
                {
                    using (var writer = PdfWriter.GetInstance(document, stream)) { }
                    document.Open();
                    document.Add(new Chunk("test"));
                }
                pdf = stream.ToArray();
            }
            File.WriteAllBytes(outputFile, pdf);
        }


        // ObjectDisposedException: Cannot access a closed Stream.
        public void Go01()
        {
            var outputFile = Helpers.IO.GetClassOutputPath(this);
            byte[] pdf = null;
            using (var stream = new MemoryStream())
            {
                using (var document = new Document())
                {
                    using (var writer = PdfWriter.GetInstance(document, stream))
                    {
                        document.Open();
                        document.Add(new Chunk("test"));
                    }
                }
                pdf = stream.ToArray();
            }
            File.WriteAllBytes(outputFile, pdf);
        }
    }
}