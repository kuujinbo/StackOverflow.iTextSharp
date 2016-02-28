﻿using System;
using System.IO;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;

// http://stackoverflow.com/questions/33653460
namespace kuujinbo.StackOverflow.iTextSharp.ProgramCode.ColumnTexts
{
    public class EveryOther
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
                using (Document doc = new Document())
                {
                    var writer = PdfWriter.GetInstance(doc, stream);
                    doc.Open();

                    Font font = FontFactory.GetFont("Arial", 20);
                    string word = "word word word word";
                    StringBuilder sb = new StringBuilder();

                    PdfContentByte cb = writer.DirectContent;
                    ColumnText ct = new ColumnText(cb);
                    ct.Alignment = Element.ALIGN_JUSTIFIED;

                    Paragraph hazardTitle = new Paragraph("Hazards", font);
                    hazardTitle.Alignment = Element.ALIGN_CENTER;

                    var pos = writer.GetVerticalPosition(false) - 40;
                    float gutter = 15f;
                    float colwidth = (doc.Right - doc.Left - gutter) / 2;
                    float col0right = doc.Left + colwidth;
                    float col1left = col0right + gutter;
                    float col1right = col1left + colwidth;
                    float[][] COLUMNS = 
                    {
                        new float[] { doc.Left, doc.Bottom, col0right, pos },
                        new float[] { col1left, doc.Bottom, col1right, pos }
                    };

                    for (int i = 1; i <= 40; ++i)
                    {
                        ct.AddText(new Phrase(string.Format(
                            "- [{0}] {1}",
                            i, sb.Append(word).ToString()
                        )));
                        ct.AddText(Chunk.NEWLINE);
                    }

                    int status = 0;
                    int column = 0;
                    while (ColumnText.HasMoreText(status))
                    {
                        ct.SetSimpleColumn(
                            COLUMNS[column][0], COLUMNS[column][1],
                            COLUMNS[column][2], COLUMNS[column][3]
                        );
                        status = ct.Go();
                        column = Math.Abs(column - 1);
                        if (column == 0)
                        {
                            doc.Add(hazardTitle);
                            doc.NewPage();
                        }
                    }
                    // add last title if column 1 stll has space
                    if (column != 0)
                    {
                        doc.Add(hazardTitle);
                    } 
                }
            }
        }


    }
}