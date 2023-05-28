using System;
using System.IO;
using twr.common;

namespace DownloadFromUrl
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");


            var url = "https://www.kdnuggets.com/2021/03/more-data-science-cheatsheets.html";
            var outputFile = Path.GetTempFileName();

            Internet.DownloadFile(url, outputFile);

        }
    }
}
