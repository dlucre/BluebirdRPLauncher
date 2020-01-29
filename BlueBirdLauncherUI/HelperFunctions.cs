using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Win32;
using EpEren.Fivem.ServerStatus.ServerAPI;
using IWshRuntimeLibrary;
using File = System.IO.File;
using System.Security.Cryptography;

namespace BlueBirdLauncherUI
{
    public static class Help
    {

        public static string version_number = "0.22";
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
            var resident_file_install_result = true;// InstallGTAResidentFile();

            var map_file_install_result = InstallFiveMMapFiles();

            return resident_file_install_result && map_file_install_result;
        }

        public static bool InstallGTAResidentFile()
        {
            try
            {
                //GTA Resident File
                var gta_directory = GetGTAVDirectory();
                if (gta_directory != null)
                {
                    //Find GTAV SFX folder
                    var source_resident_rpf_file = Path.Combine("Assets", "FiveM", "RESIDENT.rpf");
                    var sfx_folder = Path.Combine(gta_directory, "x64", "audio", "sfx");
                    var destination_rpf_path = Path.Combine(sfx_folder, "RESIDENT.rpf");
                    if (Directory.Exists(sfx_folder))
                    {
                        //Check that we have a RESIDENT.rpf file available
                        if (!File.Exists(source_resident_rpf_file))
                        {
                            //Need to acquire RESIDENT.rpf from here:
                            //https://bluebirdrp.live/applications/core/interface/file/attachment.php?id=245
                        }

                        if (File.Exists(source_resident_rpf_file))
                        {
                            //Copy it to the sfx folder above (replace every time)
                            File.Copy(source_resident_rpf_file, destination_rpf_path, true);
                            return true;
                        }

                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                //react appropriately
                LogMessage(ex.Message);
                LogException(ex);
                return false;
            }
        }

        public static bool InstallFiveMMapFiles()
        {
            try
            {
                //FiveM Map Files
                /*
                 * https://forum.cfx.re/t/release-colored-map-pause-menu/12344?__cf_chl_captcha_tk__=f86140ec7eedc92bd61e6bc911aba46b6b63fa5a-1580288150-0-ARA4XbIEf3gFAoenWatH8MEktJzUxFaJrtZ361hdjd2atP0nWj4s7KbR5jx_Sstl6pflmnVgLBHvTkhMuJpFdLGGPx6XYyiAAEyWahWTCPuKkfYqObvJ3jgljFVsxxVpoVaz5LKHRZXHjXE17Be6mZq7yhJfTJEZVD6P1OwFi_Fhdq7EZO-Ne2N-gxC21gpgDpTFkLsigHJ9RQYayth0aau2HNieCTgoWZkU0CQx7m0Nx-2xJwFRi2q3N8eER-3xrn87qDumc21FSscMo3-Td6HtyLOU4m2T-iXX9I_IBBnbAqwZz8t9XAItsdJL7tJ_OuLtrKd-wQ2jI7BsYFaXQmd_ZiQ_hjBJiIPzme3j2FIwQkA2rk1NddgYpxbb1a54N187jDq__h6GDKia7vJx1GiotG8IdgirmHaD-VYoGDNx
                 * First file ( ui ) go in your FiveM / Citizen / Common / Data drag the file in.
                 * Second file ( cdimages ) FiveM / Citizen / Platform / Data drag the file in.
                 */

                var path_to_citizen_common_data = Path.Combine(GetFiveMAppDataPath(), "citizen", "common", "data");
                if (Directory.Exists(path_to_citizen_common_data))
                {
                    foreach (var ui_file in Directory.GetFiles(@"assets\FiveM\coloredmapfiles\ui"))
                    {
                        //Console.WriteLine(ui_file);
                        var fi = new FileInfo(ui_file);
                        var destination_file = Path.Combine(path_to_citizen_common_data, "ui", fi.Name);
                        File.Copy(ui_file, destination_file, true);
                        //Console.WriteLine(destination_file);
                    }
                }


                var path_to_citizen_platform_data = Path.Combine(GetFiveMAppDataPath(), "citizen", "platform", "data");
                if (Directory.Exists(path_to_citizen_platform_data))
                {
                    if (!Directory.Exists(Path.Combine(path_to_citizen_platform_data, "cdimages", "scaleform_generic")))
                        Directory.CreateDirectory(Path.Combine(path_to_citizen_platform_data, "cdimages", "scaleform_generic"));

                    foreach (var image_file in Directory.GetFiles(@"assets\FiveM\coloredmapfiles\cdimages\scaleform_generic"))
                    {
                        //Console.WriteLine(image_file);
                        var fi = new FileInfo(image_file);
                        var destination_file = Path.Combine(path_to_citizen_platform_data, "cdimages", "scaleform_generic", fi.Name);
                        File.Copy(image_file, destination_file, true);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                //react appropriately
                LogMessage(ex.Message);
                LogException(ex);
                return false;
            }
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
                var path_to_cache = Path.Combine(GetFiveMAppDataPath(), @"cache");
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
                    server_status_model.current_users = status_response.GetOnlinePlayersCount();

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

        #region Shortcut Handling
        public static string GetFiveMAppDataPath()
        {
            try
            {
                var path_to_five_m_appdata = Path.Combine(Environment.GetEnvironmentVariable("LOCALAPPDATA"), @"FiveM\FiveM.app");
                if (Directory.Exists(path_to_five_m_appdata))
                    return path_to_five_m_appdata;

                var path_to_fivem_shortcut = Path.Combine(Environment.GetEnvironmentVariable("APPDATA"), @"Microsoft\Windows\Start Menu\Programs\FiveM.lnk");

                if (System.IO.File.Exists(path_to_fivem_shortcut))
                {
                    // WshShellClass shell = new WshShellClass();
                    WshShell shell = new WshShell(); //Create a new WshShell Interface
                    IWshShortcut link = (IWshShortcut)shell.CreateShortcut(path_to_fivem_shortcut); //Link the interface to our shortcut

                    return link.TargetPath.Replace(@"\FiveM.exe", "");
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
            LogMessage("Couldn't find FiveM Path from Shortcut.");


            return null;
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
