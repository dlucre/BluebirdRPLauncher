using System;
using System.IO;

namespace BluebirdLauncher
{
    class Program
    {
        private static string bluebird_hostname = "fivem.bluebirdrp.com";
        private static Int32 bluebird_port = 30111;

        static void Main(string[] args)
        {
            //Find fivem cache folder
            //e.g. C:\Users\USERNAME\AppData\Local\FiveM\FiveM.app\cache
            var path_to_cache = Path.Combine(Environment.GetEnvironmentVariable("LOCALAPPDATA"), @"FiveM\FiveM.app\cache");
            Console.WriteLine("Looking for cache here: {0}", path_to_cache);

            if (System.IO.Directory.Exists(path_to_cache))
            {
                //Found cache folder
                Console.WriteLine("Found cache!  Time to delete stuff!");

                //Loop through all folders in cache folder, except 'Game' and delete everything we find
                foreach (var dir in Directory.GetDirectories(path_to_cache))
                {
                    if (!dir.EndsWith("game"))
                    {
                        Console.WriteLine("Deleting: {0}", dir);
                        Directory.Delete(dir, true);
                    }
                }

                foreach (var file in Directory.GetFiles(path_to_cache))
                {
                    Console.WriteLine("Deleting: {0}", file);
                    File.Delete(file);
                }
            }
            else
            {
                //Cache not found
                Console.WriteLine("Cache wasn't in the usual place.  In the future we will let you change the cache path.");
            }

            //Launch FiveM and connect to BBRP
            Console.WriteLine("Connecting to BlueBird RP!  Please hold...");

            //Example URL
            //fivem://connect/fivem.bluebirdrp.com:30111
            System.Diagnostics.Process.Start(string.Format("fivem://connect/{0}:{1}", bluebird_hostname, bluebird_port));

            //Sleep so the user sees the status of the application before closing
            System.Threading.Thread.Sleep(3000);
        }
    }
}
