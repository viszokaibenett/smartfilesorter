using System;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 1. Welcoming the user
            Console.WriteLine("Welcome to SmartFile Sorter!\n");

            // 2. Getting the directory from the user, storing it into a variable
            Console.WriteLine("Please enter a directory you want to organize.");
            Console.WriteLine("Note: The directory path should be in this format: 'C:\\Users\\Username\\Documents'\n");
            string directory = Console.ReadLine();

            // 3. Checking if the directory exists - if not, we ask the user if he/she wants to create one
            if (!Directory.Exists(directory)) // .Exists(string) method returns a boolean
            {
                // If the directory does not exists, the app asks the user
                Console.WriteLine("The directory you entered does not exist. Do you want to create it? - 'y' or 'n'\n");
                string response = Console.ReadLine();

                // If the answer is "y"
                if (response.ToLower() == "y")
                {
                    Directory.CreateDirectory(directory);
                    Console.WriteLine("\nDirectory has been created successfully.");
                    Console.WriteLine("WARNING: The directory that has just been created is empty.\nIf you want to organize files, fill up the directory with files right now.\n");
                    Console.WriteLine("Press enter to continue.\n");
                    Console.ReadLine();
                }

                // If the answer is anything else
                else
                {
                    Console.WriteLine("\nNo directory has been created. Goodbye!\n");
                    return;
                }
            }
            else
            {
                // If the directory exists
                Console.WriteLine($"\nGood News! Directory {directory} has been found.\n");

                // Checking if the directory we want to work with is empty
                if (Directory.GetFiles(directory).Length == 0 && Directory.GetDirectories(directory).Length == 0)
                {
                    Console.WriteLine("The directory exists, but it's empty.\nPlease fill up the directory with files right now and press enter to continue.");
                    Console.ReadLine();
                }
            }

            // Check if the directory is still empty
            if (Directory.GetFiles(directory).Length == 0 && Directory.GetDirectories(directory).Length == 0)
            {
                Console.WriteLine("The directory is empty. Nothing to organize. Exiting.");
                return; // Exit the program since there's nothing to organize
            }

            // 5. Ask the user for file extensions to sort
            Console.WriteLine("\nEnter the file extensions to organize (for example: .txt, .jpg, .mp3, etc.), separated by commas:\n");
            string[] fileExtensions = Console.ReadLine().Split(',');

            // If no extensions are provided
            if (fileExtensions.Length == 1 && string.IsNullOrWhiteSpace(fileExtensions[0]))
            {
                Console.WriteLine("No file has been moved.");
                return;
            }

            // 6. Sorting the files

            // Tracking if any file has been moved
            bool anyFileMoved = false;
            foreach (string extension in fileExtensions)
            {
                string ext = extension.Trim();
                if (string.IsNullOrEmpty(ext))
                    continue; // Skip empty extensions

                string folderPath = Path.Combine(directory, ext.TrimStart('.'));

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string[] files = Directory.GetFiles(directory, $"*{ext}");
                if (files.Length == 0)
                {
                    Console.WriteLine($"No files with {ext} extension found.");
                    continue;
                }

                foreach (string file in files)
                {
                    string fileName = Path.GetFileName(file);
                    string destFile = Path.Combine(folderPath, fileName);
                    File.Move(file, destFile);
                }
                Console.WriteLine($"\nFiles with {ext} extension have been moved to {folderPath}.\n");
                anyFileMoved = true;
            }

            // 7. Handle files without extensions
            string[] allFiles = Directory.GetFiles(directory);
            bool hasNoExtension = false;

            foreach (string file in allFiles)
            {
                string fileExtension = Path.GetExtension(file);

                if (string.IsNullOrEmpty(fileExtension))
                {
                    hasNoExtension = true;
                    break;
                }
            }

            if (hasNoExtension)
            {
                Console.WriteLine("There are one or more files without extension(s).\nDo you want to move them to a separate folder? (Y or N)");
                string noExtAnswer = Console.ReadLine().ToLower();

                if (noExtAnswer == "y")
                {
                    string noExtensionFolder = Path.Combine(directory, "NoExtension");

                    if (!Directory.Exists(noExtensionFolder))
                    {
                        Directory.CreateDirectory(noExtensionFolder);
                    }

                    foreach (string file in allFiles)
                    {
                        string fileExtension = Path.GetExtension(file);

                        if (string.IsNullOrEmpty(fileExtension))
                        {
                            string fileName = Path.GetFileName(file);
                            string destFile = Path.Combine(noExtensionFolder, fileName);
                            File.Move(file, destFile);
                        }
                    }
                    Console.WriteLine("Files without extensions have been moved to the NoExtension folder.");
                }
                else
                {
                    Console.WriteLine("Files without extensions were skipped.");
                }
            }
            Console.WriteLine("File sorting complete!");
        }
    }
}
