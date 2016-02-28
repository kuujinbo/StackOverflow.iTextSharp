using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

// http://stackoverflow.com/questions/34765444
namespace kuujinbo.StackOverflow.iTextSharp.ProgramCode.Chunks
{
    public class CustomSplitCharacter : ISplitCharacter
    {
        public bool IsSplitCharacter(
            int start, int current, int end, char[] cc, PdfChunk[] ck)
        {
            char c = ck == null
                ? cc[current]
                : (char)ck[Math.Min(current, ck.Length - 1)]
                    .GetUnicodeEquivalent(cc[current])
            ;
            return (c == ')');
        }
    }

    public class SetSplitCharacter
    {
        public void Go()
        {
            // GetClassOutputPath() implementation left out for brevity
            var outputFile = Helpers.IO.GetClassOutputPath(this);

            using (FileStream stream = new FileStream(
                outputFile, 
                FileMode.Create, 
                FileAccess.Write))
            {
                string chunkText = "FirstName LastName (2016-01-13 11:13)";
                Random random = new Random();
                var font = new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD); 
                using (Document document = new Document())
                {
                    PdfWriter.GetInstance(document, stream);
                    document.Open();
                    Phrase phrase = new Phrase();
                    for (var i = 0; i < 1000; ++i)
                    {
                        var asterisk = new String('*', random.Next(1, 20));
                        Chunk chunk = new Chunk(
                            string.Format("[{0}] {1}", asterisk, chunkText), 
                            font
                        );
                        chunk.SetSplitCharacter(new CustomSplitCharacter());
                        phrase.Add(chunk);
                    }
 
                    document.Add(phrase);
                }
            }
        }


    }
}