using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace YTDL_clean_lists_from_downloaded
{
    class Program
    {

        static string downloadedListName = "____youtubeDLdownloaded.txt";

        static Regex rxDownloadedOnes = new Regex(@"(.*?)\s+(.*?)\r?\n", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex rxToDownloadTwitter = new Regex(@"https\:\/\/([\d\w]*?\.)?twitter\.com\/([\d\w]+)\/status\/(\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static void Main(string[] args)
        {

            for(int i = 0; i < args.Length; i++)
            {


                Console.WriteLine(args[i]+": ");

                string pathToDirectory = Path.GetDirectoryName(args[i]);
                string pathToDownloadedListFile = pathToDirectory + Path.DirectorySeparatorChar + downloadedListName;

                string downloadedOnes = File.ReadAllText(pathToDownloadedListFile);
                string[] toDownload = File.ReadAllLines(args[i]);
                List<string> toDownloadOut = new List<string>(); 

                List<string[]> downloadedOnesTakenApart = new List<string[]>();

                int outputFileIndex = 0;
                string outputFileName;
                while( File.Exists(outputFileName = pathToDirectory + Path.DirectorySeparatorChar+ Path.GetFileNameWithoutExtension(args[i])+"_cleaned_"+outputFileIndex+Path.GetExtension(args[i])))
                {
                    outputFileIndex++;
                } // Number output files if it already exists

                
                MatchCollection matches = rxDownloadedOnes.Matches(downloadedOnes);

                foreach(Match match in matches)
                {
                    GroupCollection groups = match.Groups;
                    downloadedOnesTakenApart.Add(new string[2] { groups[1].Value, groups[2].Value });
                    //Console.WriteLine("{0} {1}",   groups[1].Value,  groups[2].Value);
                }

                string tmpString;

                foreach(string toDownloadOne in toDownload)
                {
                    // Basically, we just continue the loop for any one that is NOT supposed to be added (happens at end of loop)

                    // Twitter
                    matches = rxToDownloadTwitter.Matches(toDownloadOne);
                    if (matches.Count > 0)
                    {
                        GroupCollection groups = matches[0].Groups;
                        tmpString = groups[3].Value; // This is the twitter post number
                        bool isDownloaded = false;
                        foreach(string[] downloadedOne in downloadedOnesTakenApart)
                        {
                            if(downloadedOne[0] == "twitter" && downloadedOne[1] == tmpString)
                            {
                                isDownloaded = true;
                                break;
                            }
                        }
                        if(isDownloaded)
                        {
                            Console.WriteLine(tmpString + " (twitter) already downloaded, removing.");
                            continue;
                        }
                    }


                    toDownloadOut.Add(toDownloadOne);
                }

                File.WriteAllLines(outputFileName,toDownloadOut);

                Console.WriteLine("...done.");

            }

            Console.ReadKey();
        }
    }
}
