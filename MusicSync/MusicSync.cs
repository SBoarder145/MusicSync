using System;
using System.Collections.Generic;
using System.IO;

namespace MusicSync
{
    class MusicSync
    {
        static string status = string.Empty;

        static void Main(string[] args)
        {
            DirectoryInfo phone = new DirectoryInfo(@"C:\Users\student\Desktop\Music");
            DirectoryInfo music = new DirectoryInfo(@"C:\Users\student\Desktop\My Music\Music");
            DirectoryInfo archive = new DirectoryInfo(@"C:\Users\student\Desktop\My Music\Music (Archive)");

            List<string> currentMusicList = new List<string>();
            List<FileInfo> filesToBeArchived = new List<FileInfo>();

            if (!phone.Exists) { Environment.Exit(0); }

            // Get list of songs on phone
            status = "Getting song list from phone...\n";
            Console.Write(status);
            int currAmt = 0;
            int totalAmt = phone.GetFiles().Length;

            foreach (FileInfo fPhone in phone.EnumerateFiles())
            {
                currentMusicList.Add(fPhone.Name.ToLower());
                currAmt++;
                UpdateConsole(currAmt, totalAmt);
            }

            // Loop through songs on computer
            status += $"\n{currAmt} of {totalAmt}\n\nRunning maintenance on computer music list...";
            Console.Clear();
            Console.Write(status);
            currAmt = 0;
            totalAmt = music.GetFiles().Length;

            foreach (FileInfo fMusic in music.GetFiles())
            {
                bool isActive = false;

                // Compare computer songs to phone songs
                foreach (FileInfo fPhone in phone.EnumerateFiles())
                {
                    if (fMusic.Name.ToLower().Equals(fPhone.Name.ToLower()))
                    {
                        isActive = true;
                        currentMusicList.Remove(fPhone.Name.ToLower());
                        break;
                    }
                }

                // Deleted from the phone, archive the song
                if (!isActive && fMusic.Extension.ToLower().Equals(".mp3"))
                {
                    filesToBeArchived.Add(fMusic);
                }

                currAmt++;
                UpdateConsole(currAmt, totalAmt);
            }

            // Confirm files should be moved
            Console.Clear();
            Console.WriteLine("The following songs will be archived:");
            foreach (FileInfo fMusic in filesToBeArchived)
            {
                Console.WriteLine(fMusic.Name);
            }
            Console.Write("\nDo you want to proceed? [Y] [N]: ");
            string input = Console.ReadLine().Substring(0, 1).ToLower();

            if (!input.Equals("y")) { Environment.Exit(0); }

            foreach (FileInfo fMusic in filesToBeArchived)
            {
                if (!File.Exists(Path.Combine(archive.FullName, fMusic.Name)))
                {
                    File.Move(fMusic.FullName, Path.Combine(archive.FullName, fMusic.Name));
                }
            }

            // Clean up songs on Desktop
            status += "\n{currAmt} of {totalAmt}\n\nCleaning up songs from phone...";
            Console.Clear();
            Console.Write(status);
            currAmt = 0;
            totalAmt = phone.GetFiles().Length;

            foreach (FileInfo fPhone in phone.EnumerateFiles())
            {
                fPhone.Delete();
                currAmt++;
                UpdateConsole(currAmt, totalAmt);
            }

            phone.Delete();
        }

        public static void UpdateConsole(int currAmt, int totalAmt)
        {
            Console.Write($"{currAmt} of {totalAmt}\n");
        }
    }
}