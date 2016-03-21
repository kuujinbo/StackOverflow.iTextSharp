using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using iTextSharp.text;
using iTextSharp.text.pdf;

// http://stackoverflow.com/questions/36102526
namespace kuujinbo.StackOverflow.iTextSharp.ProgramCode.Bookmarks
{
    public class ExtractChapters
    {
        static string OUTPUT_DIR = Helpers.IO.OutputDirectory();
        static string outputTextFile = Helpers.IO.GetOutputPath("bookmarks.txt");
        static string input = Helpers.IO.GetInputFilePath("0524-www-AjaxWAI-1.pdf");

        public void Go()
        {
            DumpResults(input);
            BookMark.ResetNumber();
            ProcessPdf(input);
        }

        public class BookMark
        {
            static int _number;
            public BookMark() { Number = ++_number; }
            public int Number { get; private set; }
            public string Title { get; set; }
            public string PageNumberString { get; set; }
            public int PageNumberInteger { get; set; }
            public static void ResetNumber() { _number = 0; }

            // bookmarks title may have illegal filename character(s)
            public string GetFileName()
            {
                var fileTitle = Regex.Replace(
                    Regex.Replace(Title, @"\s+", "-"), 
                    @"[^-\w]", ""
                );
                return string.Format("{0:D4}-{1}.pdf", Number, fileTitle);
            }
        }

        void DumpResults(string path)
        {
            using (var reader = new PdfReader(path))
            {
                // need this call to parse page numbers
                reader.ConsolidateNamedDestinations();

                var bookmarks = ParseBookMarks(SimpleBookmark.GetBookmark(reader));
                var sb = new StringBuilder();
                foreach (var bookmark in bookmarks)
                {
                    sb.AppendLine(string.Format(
                        "{0, -4}{1, -100}{2, -25}{3}",
                        bookmark.Number, bookmark.Title,
                        bookmark.PageNumberString, bookmark.PageNumberInteger
                    ));
                }
                File.WriteAllText(outputTextFile, sb.ToString());
            }
        }

        void ProcessPdf(string path)
        {
            using (var reader = new PdfReader(path))
            {
                // need this call to parse page numbers
                reader.ConsolidateNamedDestinations();

                var bookmarks = ParseBookMarks(SimpleBookmark.GetBookmark(reader));
                for (int i = 0; i < bookmarks.Count; ++i)
                {
                    int page = bookmarks[i].PageNumberInteger;
                    int nextPage = i + 1 < bookmarks.Count
                        // if not top of page will be missing content
                        ? bookmarks[i + 1].PageNumberInteger - 1 

                        /* alternative is to potentially add redundant content:
                        ? bookmarks[i + 1].PageNumberInteger
                        */

                        : reader.NumberOfPages;
                    string range = string.Format("{0}-{1}", page, nextPage);

                    // DEMO!
                    if (i < 1000)
                    {
                        var outputPath = Path.Combine(OUTPUT_DIR, bookmarks[i].GetFileName());
                        using (var readerCopy = new PdfReader(reader))
                        {
                            var number = bookmarks[i].Number;
                            readerCopy.SelectPages(range);
                            using (FileStream stream = new FileStream(outputPath, FileMode.Create))
                            {
                                using (var document = new Document())
                                {
                                    using (var copy = new PdfCopy(document, stream))
                                    {
                                        document.Open();
                                        int n = readerCopy.NumberOfPages;
                                        for (int j = 0; j < n; )
                                        {
                                            copy.AddPage(copy.GetImportedPage(readerCopy, ++j));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        List<BookMark> ParseBookMarks(IList<Dictionary<string, object>> bookmarks)
        {
            int page;
            var result = new List<BookMark>();
            foreach (var bookmark in bookmarks)
            {
                // add top-level bookmarks
                var stringPage = bookmark["Page"].ToString();
                if (Int32.TryParse(stringPage.Split()[0], out page))
                {
                    result.Add(new BookMark() {
                        Title = bookmark["Title"].ToString(),
                        PageNumberString = stringPage,
                        PageNumberInteger = page
                    });
                }

                // recurse
                if (bookmark.ContainsKey("Kids"))
                {
                    var kids = bookmark["Kids"] as IList<Dictionary<string, object>>;
                    if (kids != null && kids.Count > 0)
                    {
                        result.AddRange(ParseBookMarks(kids));
                    }
                }
            }
            return result;
        }

    }
}