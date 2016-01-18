using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace kuujinbo.StackOverflow.iTextSharp.Helpers
{
    public static class IO
    {
        #region IN
        public static string InputDirectory()
        {
            return Path.Combine(
              Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName
              , "__INPUT"
            );
        }

        public static string GetInputFilePath(string fileName)
        {
            return Path.Combine(InputDirectory(), fileName);
        }

        public static PdfReader GetInputReader(string fileName)
        {
            return new PdfReader(GetInputFilePath(fileName));
        }

        public static PdfReader GetTestReader()
        {
            using (var stream = new MemoryStream())
            {
                using (Document document = new Document())
                {
                    PdfWriter.GetInstance(document, stream);
                    document.Open();
                    document.Add(new Phrase("A PDF used for testing"));
                }
                return new PdfReader(stream.ToArray());
            }
        }
        #endregion


        #region OUT
        public static string OutputDirectory()
        {
            return Path.Combine(
              Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName
              , "__OUTPUT"
            );
        }

        public static string GetClassOutputPath(object o)
        {
            return Path.Combine(
                OutputDirectory(), 
                string.Format("{0}.pdf", Misc.ClassName(o))
            );
        }


        public static string GetOutputPath(string fileName)
        {
            return Path.Combine(OutputDirectory(), fileName);
        }

        // take original file name, remove extension, add wanted extension
        public static string GetOutputPath(string fileName, string extension)
        {
            var newName = extension.StartsWith(".")
                ? Path.GetFileNameWithoutExtension(fileName) + extension
                : Path.GetFileNameWithoutExtension(fileName) + "." + extension
            ;
            return Path.Combine(OutputDirectory(), newName);
        }

        public static PdfStamper GetOutputStamper(PdfReader reader, string fileName)
        {
            var outputPath = GetOutputPath(fileName);
            return new PdfStamper(
                reader,
                new FileStream(outputPath, FileMode.Create),
                '\0',
                true
            );
        }
        #endregion


    }
}