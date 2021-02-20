using System;
using System.IO;
using SDG.Framework.Modules;
using SDG.Unturned;
using UnturnedMapMigrator.Models;

namespace UnturnedMapMigrator
{
    public class Main : IModuleNexus
    {
        public INIFile Config;

        public void initialize()
        {
            Console.WriteLine("Loading UnturnedMapMigrator...");
            if (!File.Exists(PathHelper.ConfigFile))
            {
                Config = new INIFile();
                Config.WriteComment("UnturnedMapMigrator auto-generated Config.");

                Config["RunMigration"] = false;
                Config["FromMapName"] = "OldMapNameHere";
                Config["ToMapName"] = "NewMapNameHere";
                Config.WriteLine();
                Config.WriteComment("UnturnedMapMigrator v1.0 by ShimmyMySherbet");
                Config.Save(PathHelper.ConfigFile);
            }
            else
            {
                Config = new INIFile(PathHelper.ConfigFile);

                Config.PatchKey("RunMigration", false);
                Config.PatchKey("FromMapName", "OldMapNameHere");
                Config.PatchKey("ToMapName", "NewMapNameHere");
                if (Config.HasUnsavedChanges) Config.Save();
            }

            if (Config.Val<bool>("RunMigration"))
            {
                Console.WriteLine("Running map migration!");

                string from = Config.Val<string>("FromMapName");
                string to = Config.Val<string>("ToMapName");
                Console.WriteLine($"FromMap: '{from}', to: '{to}'");
                int migrated = 0;
                foreach (string playerDir in Directory.GetDirectories(PathHelper.PlayersDirectory))
                {
                    Console.WriteLine($"chk: {playerDir}");
                    if (!Directory.Exists(Path.Combine(playerDir, to)))
                    {
                        if (Directory.Exists(Path.Combine(playerDir, from)))
                        {
                            try
                            {
                                Directory.Move(Path.Combine(playerDir, from), Path.Combine(playerDir, to));
                                migrated++;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"ERROR: {ex.Message}");
                            }
                        } else
                        {
                            Console.WriteLine("from map doesn't exist");
                        }
                    } else
                    {
                        Console.WriteLine("To map already exists!");
                    }
                }

                Console.WriteLine($"Migrated {migrated} players to new map.");

                Config["RunMigration"] = false;
                Config.Save();
            }
        }

        public void shutdown()
        {
        }
    }

    public static class PathHelper
    {
        public static readonly string UnturnedDirectory = Environment.CurrentDirectory;

        public static string ModulesDirectory
        {
            get
            {
                return Path.Combine(UnturnedDirectory, "Modules");
            }
        }

        public static string ProfilerDirectory
        {
            get
            {
                return Path.Combine(ModulesDirectory, "UnturnedMapMigrator");
            }
        }

        public static string ConfigFile
        {
            get
            {
                return Path.Combine(ProfilerDirectory, "Config.ini");
            }
        }

        public static string ServersDirectory
        {
            get
            {
                return Path.Combine(UnturnedDirectory, "Servers");
            }
        }

        public static string ServerDirectory
        {
            get
            {
                return Path.Combine(ServersDirectory, Provider.serverID);
            }
        }

        public static string PlayersDirectory
        {
            get
            {
                return Path.Combine(ServerDirectory, "Players");
            }
        }
    }
}