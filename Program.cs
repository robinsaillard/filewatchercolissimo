using System;
using System.IO;
using static System.Console;
using static System.ConsoleColor;
using System.Globalization;
using System.Runtime.InteropServices;

namespace FileSystemWatcherSample
{
    class Program
    {
       
        static void Main(string[] args)
        {
            // Nouvelle instance De FileWatcher
            var fileSystemWatcher = new FileSystemWatcher();

            // Association d'evenement
            fileSystemWatcher.Created += FileSystemWatcher_Created;
            fileSystemWatcher.Changed += FileSystemWatcher_Changed;
            fileSystemWatcher.Deleted += FileSystemWatcher_Deleted;
            fileSystemWatcher.Renamed += FileSystemWatcher_Renamed;

            // Path du dossier que l'on souhaite écouter
            fileSystemWatcher.Path = KnownFolders.GetPath(KnownFolder.Downloads);
            string targetPath = KnownFolders.GetPath(KnownFolder.Documents);
            string pathString = System.IO.Path.Combine(targetPath, "colissimoFile");
            if (!Directory.Exists(pathString))
            {
                System.IO.Directory.CreateDirectory(pathString);
                WriteLine($"Dossier ajouté : {pathString}");
            }

            

            // You must add this line - this allows events to fire.
            fileSystemWatcher.EnableRaisingEvents = true;


            fileSystemWatcher.Filter = "exportColissimo.csv";
            ForegroundColor = White;
            WriteLine("----- File Manager Colissimo ----- ");
            WriteLine($"Path source CSV: {fileSystemWatcher.Path}");
            WriteLine($"Path Colissimo: {pathString}");
            ReadLine();
        }


        private static void DeplacerFichier(string fullpath, string name)
        {
            string fileName = name;
            string path = fullpath;
            string targetPath = KnownFolders.GetPath(KnownFolder.Documents);
            string pathString = System.IO.Path.Combine(targetPath, "colissimoFile");
            string dir = System.IO.Path.Combine(pathString, fileName);
            if (!Directory.Exists(path))
            {
                Directory.Move(fullpath, dir);
            }
            else
            {
                Directory.Move(fullpath, dir);
                
            }
                
        }

        private static void DateTimeTotay()
        {
            DateTime localDate = DateTime.Now;
            DateTime utcDate = DateTime.UtcNow;
            String[] cultureNames = {"fr-FR"};
            foreach (var cultureName in cultureNames)
            {
                ForegroundColor = White;
                var culture = new CultureInfo(cultureName);
                WriteLine($"======================= {localDate.ToString(culture)} =======================");
            }
        }


        private static void FileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            DateTimeTotay();
            ForegroundColor = Yellow;
            WriteLine($"Le fichier  : {e.OldName} a été renommer en : {e.Name}");
            DeplacerFichier(e.FullPath, e.Name);
            ForegroundColor = Green;
            WriteLine($"Fichier {e.Name} déplacé");

        }

        private static void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            ForegroundColor = Red;
            WriteLine($"Fichier supprimé - {e.Name}");
        }

        private static void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            DateTimeTotay();
            ForegroundColor = Green;
            WriteLine($"Le fichier a changé - {e.Name}");
            DeplacerFichier(e.FullPath, e.Name);
            ForegroundColor = Green;
            WriteLine($"Fichier {e.Name} déplacé");
        }

        private static void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            DateTimeTotay();
            ForegroundColor = Blue;            
            WriteLine($"Nouveau fichier ajouté - {e.Name}");
            DeplacerFichier(e.FullPath, e.Name);
            ForegroundColor = Green;
            WriteLine($"Fichier {e.Name} déplacé");
        }

        public static class KnownFolders
        {
            private static string[] _knownFolderGuids = new string[]
            {
        "{56784854-C6CB-462B-8169-88E350ACB882}", // Contacts
        "{B4BFCC3A-DB2C-424C-B029-7FE99A87C641}", // Desktop
        "{FDD39AD0-238F-46AF-ADB4-6C85480369C7}", // Documents
        "{374DE290-123F-4565-9164-39C4925E467B}", // Downloads
        "{1777F761-68AD-4D8A-87BD-30B759FA33DD}", // Favorites
        "{BFB9D5E0-C6A9-404C-B2B2-AE6DB6AF4968}", // Links
        "{4BD8D571-6D19-48D3-BE97-422220080E43}", // Music
        "{33E28130-4E1E-4676-835A-98395C3BC3BB}", // Pictures
        "{4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4}", // SavedGames
        "{7D1D3A04-DEBB-4115-95CF-2F29DA2920DA}", // SavedSearches
        "{18989B1D-99B5-455B-841C-AB7C74E4DDFC}", // Videos
            };

            /// <summary>
            /// Gets the current path to the specified known folder as currently configured. This does
            /// not require the folder to be existent.
            /// </summary>
            /// <param name="knownFolder">The known folder which current path will be returned.</param>
            /// <returns>The default path of the known folder.</returns>
            /// <exception cref="System.Runtime.InteropServices.ExternalException">Thrown if the path
            ///     could not be retrieved.</exception>
            public static string GetPath(KnownFolder knownFolder)
            {
                return GetPath(knownFolder, false);
            }

            /// <summary>
            /// Gets the current path to the specified known folder as currently configured. This does
            /// not require the folder to be existent.
            /// </summary>
            /// <param name="knownFolder">The known folder which current path will be returned.</param>
            /// <param name="defaultUser">Specifies if the paths of the default user (user profile
            ///     template) will be used. This requires administrative rights.</param>
            /// <returns>The default path of the known folder.</returns>
            /// <exception cref="System.Runtime.InteropServices.ExternalException">Thrown if the path
            ///     could not be retrieved.</exception>
            public static string GetPath(KnownFolder knownFolder, bool defaultUser)
            {
                return GetPath(knownFolder, KnownFolderFlags.DontVerify, defaultUser);
            }

            private static string GetPath(KnownFolder knownFolder, KnownFolderFlags flags,
                bool defaultUser)
            {
                int result = SHGetKnownFolderPath(new Guid(_knownFolderGuids[(int)knownFolder]),
                    (uint)flags, new IntPtr(defaultUser ? -1 : 0), out IntPtr outPath);
                if (result >= 0)
                {
                    string path = Marshal.PtrToStringUni(outPath);
                    Marshal.FreeCoTaskMem(outPath);
                    return path;
                }
                else
                {
                    throw new ExternalException("Unable to retrieve the known folder path. It may not "
                        + "be available on this system.", result);
                }
            }

            [DllImport("Shell32.dll")]
            private static extern int SHGetKnownFolderPath(
                [MarshalAs(UnmanagedType.LPStruct)]Guid rfid, uint dwFlags, IntPtr hToken,
                out IntPtr ppszPath);

            [Flags]
            private enum KnownFolderFlags : uint
            {
                SimpleIDList = 0x00000100,
                NotParentRelative = 0x00000200,
                DefaultPath = 0x00000400,
                Init = 0x00000800,
                NoAlias = 0x00001000,
                DontUnexpand = 0x00002000,
                DontVerify = 0x00004000,
                Create = 0x00008000,
                NoAppcontainerRedirection = 0x00010000,
                AliasOnly = 0x80000000
            }
        }

        /// <summary>
        /// Standard folders registered with the system. These folders are installed with Windows Vista
        /// and later operating systems, and a computer will have only folders appropriate to it
        /// installed.
        /// </summary>
        public enum KnownFolder
        {
            Contacts,
            Desktop,
            Documents,
            Downloads,
            Favorites,
            Links,
            Music,
            Pictures,
            SavedGames,
            SavedSearches,
            Videos
        }
    }
}