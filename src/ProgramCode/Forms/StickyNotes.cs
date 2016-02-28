using System;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace kuujinbo.StackOverflow.iTextSharp.ProgramCode.Forms
{
    public class StickyNotes
    {

        public void Go()
        {
            var filename = "interactiveform_enabled.pdf";
            var readerPath = Helpers.IO.GetInputFilePath(filename);
            var outputFile = Helpers.IO.GetClassOutputPath(this);

            using (var reader = new PdfReader(readerPath))
            {
                var pages = reader.NumberOfPages;
                using (var stamper = new PdfStamper(
                    reader,
                    new FileStream(outputFile, FileMode.Create),
                    '\0',
                    true))
                {
                    for (int i = 1; i <= pages; ++i)
                    {
                        PdfContentByte cb = stamper.GetOverContent(i);
                        ColumnText ct = new ColumnText(cb);

                        PdfDictionary page = reader.GetPageN(i);
                        PdfArray annots = page.GetAsArray(PdfName.ANNOTS);
                        var count = 0;

                        for (int ii = 0; ii < annots.Size; ++ii)
                        {
                            PdfDictionary sticky = annots.GetAsDict(ii);
                            var subtype = sticky.Get(PdfName.SUBTYPE);
                            

                            if (subtype == PdfName.POPUP)
                            {
                                // Console.WriteLine(sticky + ":" + sticky.);
                            }
                            if (subtype == PdfName.TEXT)
                            {
                                var rect = sticky.GetAsArray(PdfName.RECT);

                                var rectangle = new Rectangle(
                                    rect.GetAsNumber(0).FloatValue,
                                    rect.GetAsNumber(1).FloatValue,
                                    rect.GetAsNumber(2).FloatValue,
                                    rect.GetAsNumber(3).FloatValue
                                );

                                if (count == 0) ct.SetSimpleColumn(new Rectangle(0, 800, 530, 36));
                                count++;

                                var pdfString = sticky.GetAsString(PdfName.CONTENTS);
                                if (pdfString != null)
                                {
                                    Console.WriteLine("{0} {1}", rect, pdfString);
                                    //ct.SetSimpleColumn(
                                    //    new Phrase(new Chunk(pdfString.ToString(), FontFactory.GetFont(
                                    //        FontFactory.HELVETICA, 18, Font.NORMAL))),
                                    //    46, 190, 530, 36, 25, 
                                    //    Element.ALIGN_LEFT | Element.ALIGN_TOP
                                    //);

                                    // ct.SetSimpleColumn(rectangle);
                                    
                                    ct.AddElement(new Phrase(new Chunk(
                                        pdfString.ToString(), 
                                        FontFactory.GetFont(FontFactory.HELVETICA, 20, Font.NORMAL))));
                                }
                                ct.Go();
                            }
                        }
                    }
                }
            }
        }
    }
}
