using HtmlAgilityPack;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Net;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;

namespace WebCrawlerQnA
{
    public class PDFCrawler
    {
        // Define a regex pattern to match a URL

        public static async Task CrawlAsync(string url)
        {
            // Parse the URL and get the domain
            var uri = new Uri(url);
            var domain = uri.Host;

            // Create a queue to store the URLs to crawl
            var queue = new Queue<string>();
            queue.Enqueue(url);

            // Create a set to store the URLs that have already been seen (no duplicates)
            var seen = new HashSet<string> { url };

            // Create a directory to store the text files
            var textDirectoryPath = $"text/{domain}";
            Directory.CreateDirectory(textDirectoryPath);            

            // While the queue is not empty, continue crawling
            while (queue.Count > 0)
            {
                // Get the next URL from the queue
                url = queue.Dequeue();
                Console.WriteLine(url); // for debugging and to see the progress

                // Save text from the url to a <url>.txt file
                string invalidChars = new string(System.IO.Path.GetInvalidFileNameChars());
                var validFileName = new string(url.Substring(uri.Scheme.Length + 3).Select(ch => invalidChars.Contains(ch) ? '_' : ch).ToArray());
                var fileName = $"{textDirectoryPath}/{validFileName}.txt";

                // Get the text from the URL using HtmlAgilityPack

                string filePath = "C:\\Users\\kevin\\Downloads\\hio.pdf";

                if (!File.Exists(filePath))
                {
                    Console.WriteLine("The file does not exist.");
                    return;
                }

                string text = ExtractTextFromPDF(filePath);
 
                    using (var writer = new StreamWriter(fileName, false, System.Text.Encoding.UTF8))
                    {
                        // Get the text but remove the tags
                      
                        // If the crawler gets to a page that requires JavaScript, it will stop the crawl
                        if (text.Contains("You need to enable JavaScript to run this app."))
                        {
                            Console.WriteLine($"Unable to parse page {url} due to JavaScript being required");
                        }

                        // Otherwise, write the text to the file in the text directory
                        await writer.WriteAsync(text);
                    }


                              
            }

            static string ExtractTextFromPDF(string filePath)
            {
                using (PdfReader reader = new PdfReader(filePath))
                {
                    StringBuilder text = new StringBuilder();
                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                    }
                    return text.ToString();
                }
            }



        }
    }
}
