using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Win32;
using EpEren.Fivem.ServerStatus.ServerAPI;

namespace BlueBirdLauncherUI
{
    public static class Help
    {

        public static string version_number = "0.2";
        private static string fivem_bluebird_hostname = "fivem.bluebirdrp.com";
        private static Int32 fivem_bluebird_port = 30111;


        #region GTA V
        private static string gta5_install_path = null;
        /// <summary>
        /// Searches through all steam libraries for the GTAV install directory
        /// </summary>
        /// <returns>The path to the GTAV game files, or null if GTAV is not found</returns>
        public static string GetGTAVDirectory()
        {
            if (gta5_install_path != null) return gta5_install_path;

            string GTAVDirectory = string.Empty;

            //Look for all steam libraries

            var steam_libraries = GetAllSteamLibraries();

            foreach (var steam_library in steam_libraries)
            {
                var possible_gta_path = Path.Combine(steam_library, "steamapps", "common", "Grand Theft Auto V");
                if (Directory.Exists(possible_gta_path))
                {
                    var possible_gta_executable_path = Path.Combine(possible_gta_path, "GTA5.exe");
                    if (File.Exists(possible_gta_executable_path))
                    {
                        gta5_install_path = possible_gta_path;
                        return possible_gta_path;
                    }
                }
            }

            LogMessage("Couldn't find GTA V.");
            return null;
        }

        public static bool GTAVInstallAdditionalAssets()
        {
            var gta_directory = GetGTAVDirectory();
            if (gta_directory != null)
            {
                //Find GTAV SFX folder
                var source_resident_rpf_file = Path.Combine("Assets", "FiveM", "RESIDENT.rpf");
                var sfx_folder = Path.Combine(gta_directory, "x64", "audio", "sfx");
                if (Directory.Exists(sfx_folder))
                {
                    //Check that we have a RESIDENT.rpf file available
                    if (!File.Exists(source_resident_rpf_file))
                    {
                        //Need to acquire RESIDENT.rpf from somewhere                        
                    }

                    if (File.Exists(source_resident_rpf_file))
                    {
                        //Copy it to the sfx folder above
                        File.Copy(source_resident_rpf_file, Path.Combine(sfx_folder, "RESIDENT.rpf"), true);
                        return true;
                    }

                }
            }

            return false;
        }
        #endregion

        #region Steam
        private static string steam_install_path = null;
        /// <summary>
        /// Reads the registry looking for the install path for Steam
        /// </summary>
        /// <returns>The install path for steam, or null if Steam is not found</returns>
        public static string GetPathToSteam()
        {
            //Read registry to find path to steam path
            //HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam
            //InstallPath 

            if (steam_install_path != null) return steam_install_path;

            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Valve\Steam"))
                {
                    if (key != null)
                    {
                        Object o = key.GetValue("InstallPath");
                        if (o != null)
                        {
                            string path = (string)o;
                            steam_install_path = path;
                            return path;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //react appropriately
                LogMessage(ex.Message);
                LogException(ex);
                return null;
            }


            //Didn't find steam
            LogMessage("Couldn't find steam.");
            return null;
        }

        /// <summary>
        /// Reads the libraryfolders.vdf
        /// </summary>
        /// <returns>a list of all configured steam libraries, or null if none found</returns>
        public static List<string> GetAllSteamLibraries()
        {
            //All Steam Libraries are listed in C:\Program Files (x86)\Steam\steamapps\libraryfolders.vdf
            /*
             e.g.
             "LibraryFolders"
                {
	                "TimeNextStatsReport"		"1234565"
	                "ContentStatsID"		"1234234"
	                "1"		"F:\\Steam"
	                "2"		"G:\\Games\\Steam"
	                "3"		"H:\\Steam"
	                "4"		"J:\\Steam"
	                "5"		"I:\\Steam"
                }
             */
            try
            {
                var all_library_paths = new List<string>();
                var steam_path = GetPathToSteam();
                if (steam_path != null)
                {
                    var library_folders_path = Path.Combine(steam_path, "steamapps", "libraryfolders.vdf");
                    if (File.Exists(library_folders_path))
                    {
                        //This is a dumb way of reading the vdf file, but I couldn't figure out how to use the Gameloop.Vdf project (https://github.com/shravan2x/Gameloop.Vdf)
                        var contents = File.ReadAllLines(library_folders_path);
                        foreach (var line in contents)
                        {
                            var stringArray = line.Split('"');
                            if (stringArray.Length == 5)
                            {
                                var key = stringArray[1];
                                var v = stringArray[3];
                                Console.WriteLine("Key: {0}", key);
                                Console.WriteLine("Value: {0}", v);

                                if (key.IsNumeric() && Directory.Exists(v))
                                {
                                    all_library_paths.Add(v);
                                }
                            }
                        }

                        if (all_library_paths.Count == 0)
                            throw new Exception("Couldn't find any steam libraries defined in libraryfolders.vdf.");

                        return all_library_paths;
                    }
                    else
                    {
                        throw new Exception("Couldn't find any steam libraries defined, libraryfolders.vdf not found.");
                    }
                }
                else
                {
                    throw new Exception("Steam isn't installed or couldn't be found.");
                }
            }
            catch (Exception ex)
            {
                //react appropriately
                LogMessage(ex.Message);
                LogException(ex);
                return null;
            }

        }

        #endregion

        #region FiveM

        public static string GetPathToFiveMCache()
        {
            try
            {
                var path_to_cache = Path.Combine(Environment.GetEnvironmentVariable("LOCALAPPDATA"), @"FiveM\FiveM.app\cache");
                if (System.IO.Directory.Exists(path_to_cache))
                {
                    return path_to_cache;
                }
            }
            catch (Exception ex)
            {
                //react appropriately
                LogMessage(ex.Message);
                LogException(ex);
                return null;
            }


            //Didn't find FiveM
            LogMessage("Couldn't find FiveM.");
            return null;
        }

        public static bool ClearFiveMCache()
        {
            try
            {
                var path_to_fivem_cache = GetPathToFiveMCache();
                if (path_to_fivem_cache != null)
                {
                    //Found cache folder
                    LogMessage("Found FiveM cache!  Time to delete stuff!");

                    //Loop through all folders in cache folder, except 'Game' and delete everything we find
                    foreach (var dir in Directory.GetDirectories(path_to_fivem_cache))
                    {
                        if (!dir.EndsWith("game"))
                        {
                            LogMessage(string.Format("Deleting: {0}", dir));
                            Directory.Delete(dir, true);
                        }
                    }

                    foreach (var file in Directory.GetFiles(path_to_fivem_cache))
                    {
                        LogMessage(string.Format("Deleting: {0}", file));
                        File.Delete(file);
                    }
                    return true;
                }

                throw new Exception("FiveM Cache was not found in the default location.");
            }
            catch (Exception ex)
            {
                //react appropriately
                LogMessage(ex.Message);
                LogException(ex);
                return false;
            }
        }

        public static void ConnectToFiveMBlueBird()
        {
            //Launch FiveM and connect to BBRP
            LogMessage("Connecting to BlueBird RP!  Please hold...");

            //Example URL
            //fivem://connect/fivem.bluebirdrp.com:30111
            System.Diagnostics.Process.Start(string.Format("fivem://connect/{0}:{1}", fivem_bluebird_hostname, fivem_bluebird_port));
        }

        public static FiveMServerStatusReturnModel CheckFiveMServerPlayerCount()
        {
            //https://github.com/ErenKrt/Fivem-Server-Status
            var server_status_model = new FiveMServerStatusReturnModel();

            try
            {
                var status_response = new Fivem(string.Format("{0}:{1}", fivem_bluebird_hostname, fivem_bluebird_port));
                if (status_response.GetStatu())
                {
                    server_status_model.server_online = true;
                    server_status_model.raw_server_response = status_response;
                    
                    var ClassObject = status_response.GetObject();

                    server_status_model.max_users = status_response.GetMaxPlayersCount();
                    server_status_model.current_users  = status_response.GetOnlinePlayersCount();

                    //Console.WriteLine(status_response.GetGameName()); //string
                    //Console.WriteLine(status_response.GetMaxPlayersCount());  //int
                    //Console.WriteLine(status_response.GetOnlinePlayersCount());  //int
                    //Console.WriteLine(status_response.GetPlayers()); //object list
                    //Console.WriteLine(status_response.GetResources()); //string list
                    //Console.WriteLine(status_response.GetServerHost());  //string
                    //Console.WriteLine(status_response.GetStatu()); // server online(bool=true) or ofline(bool=false)
                    //Console.WriteLine(status_response.GetVars()); //object list
                    //Console.WriteLine(status_response.GetVars());
                    //var xD = status_response.GetVars();
                    //for (int i = 0; i < xD.Count; i++)
                    //{
                    //    var name = xD[i].key;
                    //    var value = xD[i].value;

                    //    Console.WriteLine("{0}:{1}", name, value);
                    //}
                }
                else
                {
                    throw new Exception("Server connection failed!  Couldn't check status!");
                }
            }
            catch (Exception ex)
            {
                LogMessage(ex.Message);
                LogException(ex);
                server_status_model.server_online = false;
            }
            return server_status_model;
        }
        #endregion


        #region Extensions
        public static bool IsNumeric(this string text)
        {
            double test;
            return double.TryParse(text, out test);
        }
        #endregion

        #region Logging
        public static List<string> LogEntries = new List<string>();
        public static void LogMessage(string message)
        {
            LogEntries.Add(message);
        }

        public static List<Exception> Exceptions = new List<Exception>();
        public static void LogException(Exception ex)
        {
            Exceptions.Add(ex);
        }
        #endregion
    }
}
