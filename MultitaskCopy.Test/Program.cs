using System;
using static System.Console;

namespace MultitaskCopy.Test
{
    class Program
    {
        static void Main()
        {
            var source = @"d:\video.wmv";
            var distance = @"d:\video_copy.wmv";

            try
            {
                WriteLine("\n**** Copy by blocks ****");
                RunCopy(source, distance, 10);
                RunCopy(source, distance, 5);
                RunCopy(source, distance, 1);

                WriteLine("\n**** Copy by byte ****");
                RunCopy(source, distance, 10, false);
                RunCopy(source, distance, 5, false);
                RunCopy(source, distance, 1, false);
            }
            catch(Exception ex)
            {
                WriteLine(ex.Message);
            }

            Write("Press any key");
            ReadKey();
        }

        static void RunCopy(string source, string distance, int count, bool isCopyBlock = true)
        {
            WriteLine($"Run copy file {count} thread");

            var startTime = DateTime.Now;
            MultitaskCopyLib.MultitaskCopy.Copy(source, distance, count, isCopyBlock);

            WriteLine($"run time: {(DateTime.Now - startTime).TotalSeconds:n2} sec\n");
        }
    }
}