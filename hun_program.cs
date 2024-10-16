using System;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 1. Üdvözöljük a felhasználót
            Console.WriteLine("Welcome to SmartFile Sorter!\n");

            // 2. Bekérünk egy mappát (könyvtárat), amit majd rendezünk - megadjuk a formátumot
            Console.WriteLine("Please enter a directory you want to organize.");
            Console.WriteLine("Note: The directory path should be in this format: 'C:\\Users\\Username\\Documents'\n");
            string directory = Console.ReadLine();

            // 3. Megnézzük, hogy a mappa létezik-e
            if (!Directory.Exists(directory)) // .Exists(directory) egy bool-t ad vissza
            {
                // Ha nem létezik a mappa, akkor megkérdezzük, hogy szeretne-e létrehozni egyet ('y' vagy 'n'), a választ eltároljuk a response nevű változóban
                Console.WriteLine("The directory you entered does not exist. Do you want to create it? - 'y' or 'n'\n");
                string response = Console.ReadLine();

                // Ha a válasz 'y'
                if (response.ToLower() == "y")
                {
                    Directory.CreateDirectory(directory); // Létrehozzuk a mappát
                    Console.WriteLine("\nDirectory has been created successfully.");

                    // Figyelmeztetjük, hogy a most létrehozott mappa üres, és helyezzen bele fájlokat, ha szeretné őket rendezni
                    Console.WriteLine("WARNING: The directory that has just been created is empty.\nIf you want to organize files, fill up the directory with files right now and press enter to continue.\n");
                    Console.ReadLine();
                }

                // Ha a válasz bármi más (ami nem 'y') - kiírjuk, hogy nem készült el mappa, és returnnel kilépünk
                else
                {
                    Console.WriteLine("\nNo directory has been created. Goodbye!\n");
                    return;
                }
            }
            else
            {
                // Ha a mappa létezik, kiírjuk, hogy megtaláltuk a mappát
                Console.WriteLine($"\nGood News! Directory {directory} has been found.\n");

                // Megnézzük, hogy a mappa üres-e vagy nem
                if (Directory.GetFiles(directory).Length == 0 && Directory.GetDirectories(directory).Length == 0)
                {
                    // Ha üres, figyelmeztetjük a felhasználót, hogy a most létrehozott mappa üres, és helyezzen bele fájlokat, ha szeretné őket rendezni
                    Console.WriteLine("WARNING: The directory exists, but it's empty.\nPlease fill up the directory with files right now and press enter to continue.");
                    Console.ReadLine();
                }
            }

            // Megnézzük, hogy a mappa a figyelmeztetés után még mindig üres-e
            if (Directory.GetFiles(directory).Length == 0 && Directory.GetDirectories(directory).Length == 0)
            {
                // Ha még mindig üres, akkor returnnel kilépünk
                Console.WriteLine("The directory is empty. Nothing to organize. Exiting.");
                return;
            }

            // 5. Bekérünk a felhasználótól kiterjesztéseket, amit majd rendezünk, vesszővel elválasztva
            Console.WriteLine("\nEnter the file extensions to organize (for example: .txt, .jpg, .mp3, etc.), separated by commas:\n");
            string[] fileExtensions = Console.ReadLine().Split(',');

            // Ha a felhasználó nem ad meg kiterjesztést, akkor akkor returnnel kilépünk
            if (fileExtensions.Length == 1 && string.IsNullOrWhiteSpace(fileExtensions[0]))
            {
                Console.WriteLine("No file has been moved.");
                return;
            }

            // 6. A fájlok rendezése

            // A fájlok mozgatásának követése - ha sikerült fájlokat mozgatni, akkor true lesz
            bool anyFileMoved = false;

            // Végigmegyünk a felhasználó által megadott fájlkiterjesztéseken, minden extension egy külön kiterjesztés
            foreach (string extension in fileExtensions)
            {
                string ext = extension.Trim(); // Eltávolítjuk a felesleges szóközöket az elejéről és végéről

                // Ha nincs kiterjesztése, akkor azt kihagyjuk
                if (string.IsNullOrEmpty(ext))
                    continue;

                // Létrehozzuk a kiterjesztéshez tartozó mappa elérési útját
                string folderPath = Path.Combine(directory, ext.TrimStart('.')); // Levágjuk a kiterjesztés elejéről a pontot

                // Ha a kiterjesztéshez tartozó mappa nem létezik, akkor létrehozzuk azt
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Azt {ext} alapján megkeressük az összes fájlt az adott kiterjesztéssel az adott mappában
                string[] files = Directory.GetFiles(directory, $"*{ext}");
                
                // Ha a files tömb értéke 0, vagyis nincs ilyen fájl, akkor kiírja, hogy nem talált az adott kiterjesztéssel fájlt.
                if (files.Length == 0)
                {
                    Console.WriteLine($"No files with {ext} extension found.");
                    continue;
                }

                // Ha találtunk fájlokat, akkor végigmegyünk rajtuk
                foreach (string file in files)
                {
                    string fileName = Path.GetFileName(file); // Az elérési útból kinyeri a fájlnevet
                    string destFile = Path.Combine(folderPath, fileName); // Célfájl elérési útja (célmappa + fájlnév kombinációja)
                    File.Move(file, destFile); // Áthelyezzük a fájlt a célmappába
                }
                Console.WriteLine($"\nFiles with {ext} extension have been moved to {folderPath}.\n");
                
                // A változó értékét true-ra állítjuk, jelezzük, hogy volt fájlmozgatás
                anyFileMoved = true;
            }

            // 7. Kiterjesztés nélküli fájlok kezelése

            // Lekérjük az összes fájlt, megnézzük, hogy vannak-e fájlok kiterjesztés nélkül
            string[] allFiles = Directory.GetFiles(directory);

            // Ez jelzi majd, hogy van-e kiterjesztés nélküli fájl
            bool hasNoExtension = false;

            // Végigmegyünk a fájlokon, megnézzük, hogy van-e fájl kiterjesztés nélkül
            foreach (string file in allFiles)
            {
                // Lekérjük az aktuális fájl kiterjesztését
                string fileExtension = Path.GetExtension(file);

                // Ha a fájlnak nincs kiterjesztése, akkor true-ra állítjuk a változó értékét, és break-kel kilépünk, mert mehetünk tovább a következő lépéssel
                if (string.IsNullOrEmpty(fileExtension))
                {
                    hasNoExtension = true;
                    break;
                }
            }

            // Ha van fájl kiterjesztés nélkül, megkérdezzük a felhasználót, hogy akarja-e ezeket egy külön mappába helyezni.
            if (hasNoExtension)
            {
                Console.WriteLine("There are one or more files without extension(s).\nDo you want to move them to a separate folder? (Y or N)");
                string noExtAnswer = Console.ReadLine().ToLower();

                // Ha igen
                if (noExtAnswer == "y")
                {
                    // Létrehozzuk a NoExtension nevű mappát
                    string noExtensionFolder = Path.Combine(directory, "NoExtension");

                    if (!Directory.Exists(noExtensionFolder))
                    {
                        Directory.CreateDirectory(noExtensionFolder);
                    }

                    // Foreach-el végigmegyünk az egyes fájlokon
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

                // Ha nem, akkor ott hagyjuk a fájlokat
                else
                {
                    Console.WriteLine("Files without extensions were skipped.");
                }
            }

            // Elköszönünk
            Console.WriteLine("File sorting complete!");
        }
    }
}
