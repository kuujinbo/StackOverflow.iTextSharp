using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.io;

// just for fun, but not answered:
// http://stackoverflow.com/questions/33929357
namespace kuujinbo.StackOverflow.iTextSharp.ProgramCode.Fonts
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
            // return (c == ',' || c <= ' ');
            return (c == ',' || c == '.');
        }
    }

    public class ChineseSetSplitCharacter
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
                Random random = new Random();

                StreamUtil.AddToResourceSearch(("iTextAsian.dll"));
                string chunkText = " 你好世界 你好你好,";
                var font = new Font(BaseFont.CreateFont("STSong-Light", "UniGB-UCS2-H", BaseFont.NOT_EMBEDDED), 12);

                using (Document document = new Document())
                {
                    PdfWriter.GetInstance(document, stream);
                    document.Open();
                    Phrase phrase = new Phrase();
                    Chunk chunk = new Chunk("", font);
                    for (var i = 0; i < 1000; ++i)
                    {
                        var asterisk = new String('*', random.Next(1, 20));
                        chunk.Append(
                            string.Format("[{0}] {1} ", asterisk, chunkText) 
                        );
                    }
                    chunk.SetSplitCharacter(new CustomSplitCharacter());
                    phrase.Add(chunk);
                    document.Add(phrase);
                }
            }
        }


    }
}
