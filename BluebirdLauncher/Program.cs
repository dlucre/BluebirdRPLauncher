using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BluebirdLauncher
{
    class Program
    {
        private static string bluebird_hostname = "fivem.bluebirdrp.com";
        private static Int32 bluebird_port = 30111;

        static void Main(string[] args)
        {
            //Clear fivem cache

            var path_to_cache = Path.Combine(Environment.GetEnvironmentVariable("LOCALAPPDATA"), @"FiveM\FiveM.app\cache");

            Console.WriteLine("Looking for cache here: {0}", path_to_cache);

            //C:\Users\dlucre\AppData\Local\FiveM\FiveM.app\cache
            if (System.IO.Directory.Exists(path_to_cache))
            {
                //Found cache
                Console.WriteLine("Found cache!  Time to delete stuff!");
                
                //Loop through all folders in cache folder, except 'Game' and delete everything
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
                Console.WriteLine("Cache wasn't in the usual place.  V2 will let you change the cache path.");
            }

            //Launch FiveM and connect to BBRP
            Console.WriteLine("Connecting to BlueBird RP!  Please hold...");

            //fivem://connect/fivem.bluebirdrp.com:30111
            System.Diagnostics.Process.Start(string.Format("fivem://connect/{0}:{1}", bluebird_hostname, bluebird_port));

            System.Threading.Thread.Sleep(3000);
        }
    }
}
