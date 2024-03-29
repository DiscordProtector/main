﻿using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
namespace DiscordProtector
{
    internal class Program
    {
        static string DPDataPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/DiscordProtector";
        /* Print header */
        static void PrintHeader()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Discord Protector v1.0.3 by Harriet, xynny & Siekiera\n");
            Console.WriteLine("\nDiscord Protector is no longer maintained (You may still contribute on Github)\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        /* Main entry point */
        static void Main(string[]args)
        {
            if (!Directory.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/DiscordProtector"))
            {
                System.Windows.Forms.MessageBox.Show("Discord Protector is not installed correctly, Please run the setup again.","Discord Protector",System.Windows.Forms.MessageBoxButtons.OK,System.Windows.Forms.MessageBoxIcon.Error);
                Environment.Exit(0);
            };
            try
            {
                var Installed = false;
                if (Registry.CurrentUser.OpenSubKey("DiscordProtector") != null)
                {
                    if (Registry.CurrentUser.OpenSubKey("DiscordProtector").GetValue("Installed") != null)
                    {
                        Installed = true;
                    };
                };
                if (!Installed)
                {
                    System.Windows.Forms.MessageBox.Show("Discord Protector is not installed correctly, Please run the setup again.", "Discord Protector", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    Environment.Exit(0);
                };
            }catch{
                System.Windows.Forms.MessageBox.Show("Discord Protector is not installed correctly, Please run the setup again.", "Discord Protector", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                Environment.Exit(0);
            };
            Environment.CurrentDirectory = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/DiscordProtector";
            Console.Title = "Discord Protector";
            if (args.Length > 0) {
                if (args[0]=="--update")
                {
                    try
                    {
                        var Edition = args[3].ToLower();
                        Console.WriteLine("Closing Discord");
                        foreach (var p in Process.GetProcessesByName(Edition))
                        {
                            p.Kill();
                        };
                        Console.WriteLine("Waiting for files to be released.");
                        Thread.Sleep(5000);
                        Console.WriteLine("Removing old directory");
                        if (Directory.Exists(args[2]))
                        {
                            Directory.Delete(args[2],true);
                        };
                        InstallToDiscord(Edition);
                    }
                    catch
                    {
                        System.Windows.Forms.MessageBox.Show("Something went wrong while updating your Discord, Please restart Discord Protector and install manually.","Discord Protector",System.Windows.Forms.MessageBoxButtons.OK,System.Windows.Forms.MessageBoxIcon.Exclamation);
                    };
                    return;
                };
            }
            while (true)
            {
                Console.CursorVisible = false;
                HomePage();
            };
        }

        /* Show home page */
        static void HomePage()
        {
            var CurrentChoice = 1;
            Console.Clear();
            PrintHeader();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Use the arrow keys to change your choice and press enter to confirm your choice\n");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("1) Install Discord Protector");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("2) Uninstall Discord Protector\n3) Change protection levels\n4) Change protection password");

            /* Read user input */
            while (true)
            {
                /* Read key */
                var Key = Console.ReadKey(true).Key;

                /* Change current choice */
                if ((Key == ConsoleKey.UpArrow) && (CurrentChoice > 1))
                {
                    CurrentChoice--;
                };
                if ((Key == ConsoleKey.DownArrow) && (CurrentChoice < 4))
                {
                    CurrentChoice++;
                };

                /* Check if cofirm */
                if (Key == ConsoleKey.Enter)
                {
                    switch (CurrentChoice)
                    {
                        case 1:
                            ShowInstallPage();
                            break;
                        case 2:
                            ShowUninstallPage();
                            break;
                        case 3:
                            break;
                        case 4:
                            break;
                    };
                    break;
                };

                /* Re print */
                Console.CursorLeft = 0;
                Console.CursorTop = 4;
                switch (CurrentChoice)
                {
                    case 1:
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("1) Install Discord Protector");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("2) Uninstall Discord Protector\n3) Change protection levels\n4) Change protection password");
                        break;
                    case 2:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("1) Install Discord Protector");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("2) Uninstall Discord Protector");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("3) Change protection levels\n4) Change protection password");
                        break;
                    case 3:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("1) Install Discord Protector\n2) Uninstall Discord Protector");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("3) Change protection levels");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("4) Change protection password");
                        break;
                    case 4:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("1) Install Discord Protector\n2) Uninstall Discord Protector\n3) Change protection levels");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("4) Change protection password");
                        break;
                };
            };
        }

        /* Uninstall Discord protector from discord */
        static void UninstallFromDiscord(string edition)
        {
            Console.Clear();
            PrintHeader();
            Console.ForegroundColor = ConsoleColor.Magenta;
            var InstallDir = "";
            foreach (var a in Directory.EnumerateDirectories($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/{edition}"))
            {
                if (a.Contains("\\app-"))
                {
                    InstallDir = a;
                };
            };
            string d = InstallDir.Replace("\\", "/");
            if (File.Exists($"{d}/resources/app.asar"))
            {
                Console.WriteLine($"Creating backup of app.asar");
                File.Copy($"{d}/resources/app.asar",$"{d}/resources/app.asar.backup",true);
            };
            /* Kill discord */
            Console.WriteLine($"Killing Discord ({d})");
            foreach (var p in Process.GetProcessesByName(edition))
            {
                Console.WriteLine($"Killing Discord PID: {p.Id}");
                p.Kill();
            };
            Console.WriteLine("Waiting for files to be released");
            /* Wait for discord to fully close */
            Thread.Sleep(5000);
            Console.WriteLine("Killed Discord");
            /* Register changes with api */
            Console.WriteLine("Unregistering installation");
            Process.Start($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/DiscordProtector/api.exe", $@"--uninstallinstall ""{d}"" ""{edition}""").WaitForExit();
            /* Remove asar */
            File.Delete($"{d}/resources/app.asar");
            File.Delete($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/{edition}/protected.discordprotector");
            /* Copy original */
            File.Copy($"{d}/resources/app.asar.original",$"{d}/resources/app.asar");
            /* Restart discord */
            Console.WriteLine($"Starting Discord ({d})");
            Process.Start("cmd.exe",$"/C \"start {d}/{edition}.exe\"");
            /* Success */
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Discord Protector successfully uninstalled from {d}");
            Thread.Sleep(5000);
        }

        /* Install Discord protector */
        static void InstallToDiscord(string edition)
        {
            Console.Clear();
            PrintHeader();
            Console.ForegroundColor = ConsoleColor.Magenta;
            var InstallDir = "";
                foreach (var a in Directory.EnumerateDirectories($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/{edition}"))
                {
                    if (a.Contains("\\app-"))
                    {
                        InstallDir = a;
                    };
                };
                string d = InstallDir.Replace("\\", "/");
                if (File.Exists($"{d}/resources/app.asar"))
                {
                    Console.WriteLine($"Creating backup of app.asar");
                    File.Copy($"{d}/resources/app.asar",$"{d}/resources/app.asar.backup",true);
                    try
                    {
                        File.Copy($"{d}/resources/app.asar",$"{d}/resources/app.asar.original");
                    }catch{};
                    Console.WriteLine($"Extracting asar ({d}/resources/app.asar)");
                    /* Extract asar */
                    var EPSI = new ProcessStartInfo();
                    EPSI.CreateNoWindow = true;
                    EPSI.WindowStyle = ProcessWindowStyle.Hidden;
                    EPSI.FileName = "node.exe";
                    EPSI.Arguments = $@"unpack.js ""{d}/resources/app.asar"" ""{d}/resources/appasar""";
                    Process.Start(EPSI).WaitForExit();
                    /* Write DiscordProtector stuff */
                    Console.WriteLine($"Adding security to asar ({d}/resources/app.asar)");
                    if (!Directory.Exists($"{d}/resources/appasar/DiscordProtector"))
                    {
                        Directory.CreateDirectory($"{d}/resources/appasar/DiscordProtector");
                    };
                    File.WriteAllText($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/{edition}/protected.discordprotector","");
                    File.WriteAllText($"{d}/resources/appasar/DiscordProtector/api.js","/* Discord Protector Version 1.0.2 https://github.com/DiscordProtector/main */\n\n/* Variables */\nconst APIPath=`${process.env.LOCALAPPDATA}\\\\DiscordProtector\\\\api.exe`;\nconst ChildProcess=require('child_process');\n\n/* Make api call */\nexports.call=(args,returnstring)=>{\nlet STDOUT=ChildProcess.spawnSync(APIPath,args||[],{detached: true,windowsHide: true}).stdout.toString();\nif(returnstring){\nreturn(STDOUT);\n}else{\nSTDOUT=STDOUT.split('\\n');\nSTDOUT.pop();\nreturn(STDOUT);\n};\n};");
                    File.WriteAllText($"{d}/resources/appasar/DiscordProtector/index.js",$"/* Discord Protector Version 1.0.2 https://github.com/DiscordProtector/main */\n\n/* Variables */\nconst API=require('./api').call;\nconst versionpath=\"{d}\"\nconst Edition=\"{edition}\"\nlet DataDir;\n\n/* Get user data path */\nexports.GetUserData=()=>{{\nlet ApiData=API(['--getuserdata',`${{versionpath}}`,`${{Edition}}`])[0];if(ApiData){{\nApiData=ApiData.trim();DataDir=ApiData;return(DataDir);\n}}else{{\nthrow new Error(\"Something went wrong while retrieving data.\");\n}};\n}};");
                    var PathsLines = File.ReadAllLines($"{d}/resources/appasar/common/paths.js");
                    var sqUpdateLines = File.ReadAllLines($"{d}/resources/appasar/app_bootstrap/squirrelUpdate.js");
                    var bootstrapLines = File.ReadAllLines($"{d}/resources/appasar/app_bootstrap/bootstrap.js");
                    var indexLines = File.ReadAllLines($"{d}/resources/appasar/app_bootstrap/index.js");
                    var InterCount = 0;
                    foreach(var l in PathsLines)
                    {
                        if(l=="let installPath = null;")
                        {
                            PathsLines[InterCount] = "let installPath;\nlet DiscordProtector=require('../DiscordProtector/index.js');";
                        };
                        if(l=="function determineUserData(userDataRoot, buildInfo) {")
                        {
                            PathsLines[InterCount+1] = "  return(_path.default.join(userDataRoot,DiscordProtector.GetUserData()));";
                        };
                        if(l=="  return app.getPath('appData');")
                        {
                            PathsLines[InterCount] = "  return(`${app.getPath('home')}/AppData/LocalLow/DiscordProtector/clientdata`);";
                        };
                        InterCount++;
                    };
                    if (PathsLines[0] != "/* Discord Protector Version 1.0.2 https://github.com/DiscordProtector/main */")
                    {
                        PathsLines[0] = $"/* Discord Protector Version 1.0.2 https://github.com/DiscordProtector/main */\n\n{PathsLines[0]}";
                    };
                    File.WriteAllLines($"{d}/resources/appasar/common/paths.js",PathsLines);
                    InterCount = 0;
                    var RemoveLines = false;
                    foreach (var l in sqUpdateLines)
                    {
                        if(l=="function restart(app, newVersion) {")
                        {
                            sqUpdateLines[InterCount] = $"function restart(app,newVersion){{\napp.once('will-quit',()=>{{\nconst execPath=_path.default.resolve(rootFolder,`app-${{newVersion}}`).replace(/\\\\/g,'/');\n_child_process.default.spawn(`${{process.env.LOCALAPPDATA}}\\\\DiscordProtector\\\\DiscordProtector.exe`,['--update',`\"${{execPath}}\"`,\"{d}\",\"{edition}\"],{{\ndetached: true\n}});\n}});\napp.quit();\n}};\nfunction OLDRESTART(){{";
                        };
                        if(l=="function OLDRESTART(){")
                        {
                            RemoveLines = true;
                        };
                        if (RemoveLines)
                        {
                            sqUpdateLines[InterCount] = null;
                        };
                        if(l=="app.quit();"&&sqUpdateLines[InterCount+1]=="}")
                        {
                            RemoveLines = false;
                        };
                        InterCount++;
                    };
                    File.WriteAllLines($"{d}/resources/appasar/app_bootstrap/squirrelUpdate.js",sqUpdateLines);
                    InterCount = 0;
                    if (bootstrapLines[0] != "/* Discord Protector Version 1.0.2 https://github.com/DiscordProtector/main */")
                    {
                        bootstrapLines[0] = $"/* Discord Protector Version 1.0.2 https://github.com/DiscordProtector/main */\n\n{bootstrapLines[0]}";
                    };
                    foreach (var l in bootstrapLines)
                    {
                        if (l == "// bootstrap, or what runs before the rest of desktop does")
                        {
                            bootstrapLines[InterCount] = $"/* Bootstrap (what runs before the rest of desktop does) */\nexports.RunBootstrap=(paths)=>{{\n";
                            bootstrapLines[bootstrapLines.Length-1] = $"{bootstrapLines[bootstrapLines.Length-1]}\n}};";
                        };
                        if(l=="const paths = require('../common/paths');")
                        {
                            bootstrapLines[InterCount] = "//const paths = require('../common/paths');";
                        };
                        if(l=="paths.init(buildInfo);")
                        {
                            bootstrapLines[InterCount] = "//paths.init(buildInfo);";
                        };
                        InterCount++;
                    };
                    File.WriteAllLines($"{d}/resources/appasar/app_bootstrap/bootstrap.js",bootstrapLines);
                    InterCount = 0;
                    if (indexLines[0] != "/* Discord Protector Version 1.0.2 https://github.com/DiscordProtector/main */")
                    {
                        indexLines[0] = $"/* Discord Protector Version 1.0.2 https://github.com/DiscordProtector/main */\n\n{indexLines[0]}";
                    };
                    foreach (var l in indexLines)
                    {
                        if (l == "  require('./bootstrap');")
                        {
                            indexLines[InterCount] = $"  require('./bootstrap').RunBootstrap(paths);";
                        };
                        InterCount++;
                    };
                    File.WriteAllLines($"{d}/resources/appasar/app_bootstrap/index.js",indexLines);
                    /* Kill discord */
                    Console.WriteLine($"Killing Discord ({d})");
                    foreach (var p in Process.GetProcessesByName(edition))
                    {
                        Console.WriteLine($"Killing Discord PID: {p.Id}");
                        p.Kill();
                    };
                    Console.WriteLine("Waiting for files to be released");
                    /* Wait for discord to fully close */
                    Thread.Sleep(5000);
                    Console.WriteLine("Killed Discord");
                    Console.WriteLine($"Packing asar ({d}/resources/app.asar)");
                    /* Register changes with api */
                    Console.WriteLine("Registering installation");
                    /* Create dirs if missing */
                    if (!Directory.Exists(DPDataPath))
                    {
                        Directory.CreateDirectory(DPDataPath);
                    };
                    if (!Directory.Exists($"{DPDataPath}/versions"))
                    {
                        Directory.CreateDirectory($"{DPDataPath}/versions");
                    };
                    if (!Directory.Exists($"{DPDataPath}/protections"))
                    {
                        Directory.CreateDirectory($"{DPDataPath}/protections");
                    };
                    if (!Directory.Exists($"{DPDataPath}/clientdata"))
                    {
                        Directory.CreateDirectory($"{DPDataPath}/clientdata");
                    };
                    if (!Directory.Exists($"{DPDataPath}/hashes"))
                    {
                        Directory.CreateDirectory($"{DPDataPath}/hashes");
                    };
                    /* Start api and register new install */
                    Process.Start($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/DiscordProtector/api.exe",$@"--registerinstallation ""{d}"" ""{edition}""").WaitForExit();
                    /* Re-pack */
                    var PPSI = new ProcessStartInfo();
                    PPSI.CreateNoWindow = true;
                    PPSI.WindowStyle = ProcessWindowStyle.Hidden;
                    PPSI.FileName = "node.exe";
                    PPSI.Arguments = $@"pack.js ""{d}/resources/appasar"" ""{d}/resources/app.asar""";
                    Process.Start(PPSI).WaitForExit();
                    /* Clean up */
                    Console.WriteLine($"Cleaning up ({d})");
                    try{
                        Directory.Delete($"{d}/resources/appasar",true);
                    }catch{};
                    /* Restart discord */
                    Console.WriteLine($"Starting Discord ({d})");
                    Process.Start("cmd.exe",$"/C \"start {d}/{edition}.exe\"");
                    /* Success */
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Discord Protector successfully installed to {d}");
                    Thread.Sleep(5000);
                };
        }

        /* Show install page */
        static void ShowInstallPage()
        {
            /* Check which versions are installed */
            var Discord = Directory.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/discord");
            var DiscordPTB = Directory.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/discordptb");
            var DiscordCanary = Directory.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/discordcanary");
            var DiscordDev = Directory.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/discorddevelopment");

            /* Check if protected */
            var DiscordP = File.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/discord/protected.discordprotector");
            var DiscordPTBP = File.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/discordptb/protected.discordprotector");
            var DiscordCanaryP = File.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/discordcanary/protected.discordprotector");
            var DiscordDevP = File.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/discorddevelopment/protected.discordprotector");

            /* Display information */
            Console.Clear();
            PrintHeader();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Which version of Discord would you like to protect?\n\nUse the arrow keys to change your choice and press enter to confirm your choice or press ESC to go back\n");
            var CurrentChoice = 1;
            Console.ForegroundColor = ConsoleColor.Green;
            if (Discord)
            {
                if (DiscordP)
                {
                    Console.WriteLine("Discord | Protected");
                }
                else
                {
                    Console.WriteLine("Discord | Not protected");
                };
            }
            else
            {
                Console.WriteLine("Discord | Not installed");
            };
            Console.ForegroundColor = ConsoleColor.Red;
            if (DiscordPTB)
            {
                if (DiscordPTBP)
                {
                    Console.WriteLine("Discord PTB | Protected");
                }
                else
                {
                    Console.WriteLine("Discord PTB | Not protected");
                };
            }
            else
            {
                Console.WriteLine("Discord PTB | Not installed");
            };
            if (DiscordCanary)
            {
                if (DiscordCanaryP)
                {
                    Console.WriteLine("Discord Canary | Protected");
                }
                else
                {
                    Console.WriteLine("Discord Canary | Not protected");
                };
            }
            else
            {
                Console.WriteLine("Discord Canary | Not installed");
            };
            if (DiscordDev)
            {
                if (DiscordDevP)
                {
                    Console.WriteLine("Discord Development | Protected");
                }
                else
                {
                    Console.WriteLine("Discord Development | Not protected");
                };
            }
            else
            {
                Console.WriteLine("Discord Development | Not installed");
            };

            /* Read user input */
            while (true)
            {
                /* Read key */
                var Key = Console.ReadKey(true).Key;

                /* Change current choice */
                if ((Key == ConsoleKey.UpArrow) && (CurrentChoice > 1))
                {
                    CurrentChoice--;
                };
                if ((Key == ConsoleKey.DownArrow) && (CurrentChoice < 4))
                {
                    CurrentChoice++;
                };

                /* Check if return */
                if(Key == ConsoleKey.Escape)
                {
                    break;
                };

                /* Check if cofirm */
                if (Key == ConsoleKey.Enter)
                {
                    var Break = false;
                    switch (CurrentChoice)
                    {
                        case 1:
                            if (Discord)
                            {
                                // install
                                InstallToDiscord("discord");
                                Break = true;
                            };
                            break;
                        case 2:
                            if (DiscordPTB)
                            {
                                // install
                                InstallToDiscord("discordptb");
                                Break = true;
                            };
                            break;
                        case 3:
                            if (DiscordCanary)
                            {
                                // install
                                InstallToDiscord("discordcanary");
                                Break = true;
                            };
                            break;
                        case 4:
                            if (DiscordDev)
                            {
                                // install
                                InstallToDiscord("discorddevelopment");
                                Break = true;
                            };
                            break;
                    };
                    if (Break)
                    {
                        break;
                    };
                };

                /* Re print */
                Console.CursorLeft = 0;
                Console.CursorTop = 6;
                Console.ForegroundColor = ConsoleColor.Red;
                switch (CurrentChoice)
                {
                    case 1:
                        Console.ForegroundColor = ConsoleColor.Green;
                        if (Discord)
                        {
                            if (DiscordP)
                            {
                                Console.WriteLine("Discord | Protected");
                            }
                            else
                            {
                                Console.WriteLine("Discord | Not protected");
                            };
                        }
                        else
                        {
                            Console.WriteLine("Discord | Not installed");
                        };
                        Console.ForegroundColor = ConsoleColor.Red;
                        if (DiscordPTB)
                        {
                            if (DiscordPTBP)
                            {
                                Console.WriteLine("Discord PTB | Protected");
                            }
                            else
                            {
                                Console.WriteLine("Discord PTB | Not protected");
                            };
                        }
                        else
                        {
                            Console.WriteLine("Discord PTB | Not installed");
                        };
                        if (DiscordCanary)
                        {
                            if (DiscordCanaryP)
                            {
                                Console.WriteLine("Discord Canary | Protected");
                            }
                            else
                            {
                                Console.WriteLine("Discord Canary | Not protected");
                            };
                        }
                        else
                        {
                            Console.WriteLine("Discord Canary | Not installed");
                        };
                        if (DiscordDev)
                        {
                            if (DiscordDevP)
                            {
                                Console.WriteLine("Discord Development | Protected");
                            }
                            else
                            {
                                Console.WriteLine("Discord Development | Not protected");
                            };
                        }
                        else
                        {
                            Console.WriteLine("Discord Development | Not installed");
                        };
                        break;
                    case 2:
                        Console.ForegroundColor = ConsoleColor.Red;
                        if (Discord)
                        {
                            if (DiscordP)
                            {
                                Console.WriteLine("Discord | Protected");
                            }
                            else
                            {
                                Console.WriteLine("Discord | Not protected");
                            };
                        }
                        else
                        {
                            Console.WriteLine("Discord | Not installed");
                        };
                        Console.ForegroundColor = ConsoleColor.Green;
                        if (DiscordPTB)
                        {
                            if (DiscordPTBP)
                            {
                                Console.WriteLine("Discord PTB | Protected");
                            }
                            else
                            {
                                Console.WriteLine("Discord PTB | Not protected");
                            };
                        }
                        else
                        {
                            Console.WriteLine("Discord PTB | Not installed");
                        };
                        Console.ForegroundColor = ConsoleColor.Red;
                        if (DiscordCanary)
                        {
                            if (DiscordCanaryP)
                            {
                                Console.WriteLine("Discord Canary | Protected");
                            }
                            else
                            {
                                Console.WriteLine("Discord Canary | Not protected");
                            };
                        }
                        else
                        {
                            Console.WriteLine("Discord Canary | Not installed");
                        };
                        if (DiscordDev)
                        {
                            if (DiscordDevP)
                            {
                                Console.WriteLine("Discord Development | Protected");
                            }
                            else
                            {
                                Console.WriteLine("Discord Development | Not protected");
                            };
                        }
                        else
                        {
                            Console.WriteLine("Discord Development | Not installed");
                        };
                        break;
                    case 3:
                        Console.ForegroundColor = ConsoleColor.Red;
                        if (Discord)
                        {
                            if (DiscordP)
                            {
                                Console.WriteLine("Discord | Protected");
                            }
                            else
                            {
                                Console.WriteLine("Discord | Not protected");
                            };
                        }
                        else
                        {
                            Console.WriteLine("Discord | Not installed");
                        };
                        if (DiscordPTB)
                        {
                            if (DiscordPTBP)
                            {
                                Console.WriteLine("Discord PTB | Protected");
                            }
                            else
                            {
                                Console.WriteLine("Discord PTB | Not protected");
                            };
                        }
                        else
                        {
                            Console.WriteLine("Discord PTB | Not installed");
                        };
                        Console.ForegroundColor = ConsoleColor.Green;
                        if (DiscordCanary)
                        {
                            if (DiscordCanaryP)
                            {
                                Console.WriteLine("Discord Canary | Protected");
                            }
                            else
                            {
                                Console.WriteLine("Discord Canary | Not protected");
                            };
                        }
                        else
                        {
                            Console.WriteLine("Discord Canary | Not installed");
                        };
                        Console.ForegroundColor = ConsoleColor.Red;
                        if (DiscordDev)
                        {
                            if (DiscordDevP)
                            {
                                Console.WriteLine("Discord Development | Protected");
                            }
                            else
                            {
                                Console.WriteLine("Discord Development | Not protected");
                            };
                        }
                        else
                        {
                            Console.WriteLine("Discord Development | Not installed");
                        };
                        break;
                    case 4:
                        Console.ForegroundColor = ConsoleColor.Red;
                        if (Discord)
                        {
                            if (DiscordP)
                            {
                                Console.WriteLine("Discord | Protected");
                            }
                            else
                            {
                                Console.WriteLine("Discord | Not protected");
                            };
                        }
                        else
                        {
                            Console.WriteLine("Discord | Not installed");
                        };
                        if (DiscordPTB)
                        {
                            if (DiscordPTBP)
                            {
                                Console.WriteLine("Discord PTB | Protected");
                            }
                            else
                            {
                                Console.WriteLine("Discord PTB | Not protected");
                            };
                        }
                        else
                        {
                            Console.WriteLine("Discord PTB | Not installed");
                        };
                        if (DiscordCanary)
                        {
                            if (DiscordCanaryP)
                            {
                                Console.WriteLine("Discord Canary | Protected");
                            }
                            else
                            {
                                Console.WriteLine("Discord Canary | Not protected");
                            };
                        }
                        else
                        {
                            Console.WriteLine("Discord Canary | Not installed");
                        };
                        Console.ForegroundColor = ConsoleColor.Green;
                        if (DiscordDev)
                        {
                            if (DiscordDevP)
                            {
                                Console.WriteLine("Discord Development | Protected");
                            }
                            else
                            {
                                Console.WriteLine("Discord Development | Not protected");
                            };
                        }
                        else
                        {
                            Console.WriteLine("Discord Development | Not installed");
                        };
                        break;
                };
            };
        }

        /* Show install page */
        static void ShowUninstallPage()
        {
            /* Check which versions are installed */
            var Discord = Directory.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/discord");
            var DiscordPTB = Directory.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/discordptb");
            var DiscordCanary = Directory.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/discordcanary");
            var DiscordDev = Directory.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/discorddevelopment");

            /* Check if protected */
            var DiscordP = File.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/discord/protected.discordprotector");
            var DiscordPTBP = File.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/discordptb/protected.discordprotector");
            var DiscordCanaryP = File.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/discordcanary/protected.discordprotector");
            var DiscordDevP = File.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/discorddevelopment/protected.discordprotector");

            /* Display information */
            Console.Clear();
            PrintHeader();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Which version of Discord would you like to unprotect?\n\nUse the arrow keys to change your choice and press enter to confirm your choice or press ESC to go back\n");
            var CurrentChoice = 1;
            Console.ForegroundColor = ConsoleColor.Green;
            if (Discord)
            {
                if (DiscordP)
                {
                    Console.WriteLine("Discord | Protected");
                }
                else
                {
                    Console.WriteLine("Discord | Not protected");
                };
            }
            else
            {
                Console.WriteLine("Discord | Not installed");
            };
            Console.ForegroundColor = ConsoleColor.Red;
            if (DiscordPTB)
            {
                if (DiscordPTBP)
                {
                    Console.WriteLine("Discord PTB | Protected");
                }
                else
                {
                    Console.WriteLine("Discord PTB | Not protected");
                };
            }
            else
            {
                Console.WriteLine("Discord PTB | Not installed");
            };
            if (DiscordCanary)
            {
                if (DiscordCanaryP)
                {
                    Console.WriteLine("Discord Canary | Protected");
                }
                else
                {
                    Console.WriteLine("Discord Canary | Not protected");
                };
            }
            else
            {
                Console.WriteLine("Discord Canary | Not installed");
            };
            if (DiscordDev)
            {
                if (DiscordDevP)
                {
                    Console.WriteLine("Discord Development | Protected");
                }
                else
                {
                    Console.WriteLine("Discord Development | Not protected");
                };
            }
            else
            {
                Console.WriteLine("Discord Development | Not installed");
            };

            /* Read user input */
            while (true)
            {
                /* Read key */
                var Key = Console.ReadKey(true).Key;

                /* Change current choice */
                if ((Key == ConsoleKey.UpArrow) && (CurrentChoice > 1))
                {
                    CurrentChoice--;
                };
                if ((Key == ConsoleKey.DownArrow) && (CurrentChoice < 4))
                {
                    CurrentChoice++;
                };

                /* Check if return */
                if(Key == ConsoleKey.Escape)
                {
                    break;
                };

                /* Check if cofirm */
                if (Key == ConsoleKey.Enter)
                {
                    var Break = false;
                    switch (CurrentChoice)
                    {
                        case 1:
                            if (DiscordP)
                            {
                                UninstallFromDiscord("discord");
                                Break = true;
                            };
                            break;
                        case 2:
                            if (DiscordPTBP)
                            {
                                UninstallFromDiscord("discordptb");
                                Break = true;
                            };
                            break;
                        case 3:
                            if (DiscordCanaryP)
                            {
                                UninstallFromDiscord("discordcanary");
                                Break = true;
                            };
                            break;
                        case 4:
                            if (DiscordDevP)
                            {
                                UninstallFromDiscord("discorddevelopment");
                                Break = true;
                            };
                            break;
                    };
                    if (Break)
                    {
                        break;
                    };
                };

                /* Re print */
                Console.CursorLeft = 0;
                Console.CursorTop = 6;
                Console.ForegroundColor = ConsoleColor.Red;
                switch (CurrentChoice)
                {
                    case 1:
                        Console.ForegroundColor = ConsoleColor.Green;
                        if (Discord)
                        {
                            if (DiscordP)
                            {
                                Console.WriteLine("Discord | Protected");
                            }
                            else
                            {
                                Console.WriteLine("Discord | Not protected");
                            };
                        }
                        else
                        {
                            Console.WriteLine("Discord | Not installed");
                        };
                        Console.ForegroundColor = ConsoleColor.Red;
                        if (DiscordPTB)
                        {
                            if (DiscordPTBP)
                            {
                                Console.WriteLine("Discord PTB | Protected");
                            }
                            else
                            {
                                Console.WriteLine("Discord PTB | Not protected");
                            };
                        }
                        else
                        {
                            Console.WriteLine("Discord PTB | Not installed");
                        };
                        if (DiscordCanary)
                        {
                            if (DiscordCanaryP)
                            {
                                Console.WriteLine("Discord Canary | Protected");
                            }
                            else
                            {
                                Console.WriteLine("Discord Canary | Not protected");
                            };
                        }
                        else
                        {
                            Console.WriteLine("Discord Canary | Not installed");
                        };
                        if (DiscordDev)
                        {
                            if (DiscordDevP)
                            {
                                Console.WriteLine("Discord Development | Protected");
                            }
                            else
                            {
                                Console.WriteLine("Discord Development | Not protected");
                            };
                        }
                        else
                        {
                            Console.WriteLine("Discord Development | Not installed");
                        };
                        break;
                    case 2:
                        Console.ForegroundColor = ConsoleColor.Red;
                        if (Discord)
                        {
                            if (DiscordP)
                            {
                                Console.WriteLine("Discord | Protected");
                            }
                            else
                            {
                                Console.WriteLine("Discord | Not protected");
                            };
                        }
                        else
                        {
                            Console.WriteLine("Discord | Not installed");
                        };
                        Console.ForegroundColor = ConsoleColor.Green;
                        if (DiscordPTB)
                        {
                            if (DiscordPTBP)
                            {
                                Console.WriteLine("Discord PTB | Protected");
                            }
                            else
                            {
                                Console.WriteLine("Discord PTB | Not protected");
                            };
                        }
                        else
                        {
                            Console.WriteLine("Discord PTB | Not installed");
                        };
                        Console.ForegroundColor = ConsoleColor.Red;
                        if (DiscordCanary)
                        {
                            if (DiscordCanaryP)
                            {
                                Console.WriteLine("Discord Canary | Protected");
                            }
                            else
                            {
                                Console.WriteLine("Discord Canary | Not protected");
                            };
                        }
                        else
                        {
                            Console.WriteLine("Discord Canary | Not installed");
                        };
                        if (DiscordDev)
                        {
                            if (DiscordDevP)
                            {
                                Console.WriteLine("Discord Development | Protected");
                            }
                            else
                            {
                                Console.WriteLine("Discord Development | Not protected");
                            };
                        }
                        else
                        {
                            Console.WriteLine("Discord Development | Not installed");
                        };
                        break;
                    case 3:
                        Console.ForegroundColor = ConsoleColor.Red;
                        if (Discord)
                        {
                            if (DiscordP)
                            {
                                Console.WriteLine("Discord | Protected");
                            }
                            else
                            {
                                Console.WriteLine("Discord | Not protected");
                            };
                        }
                        else
                        {
                            Console.WriteLine("Discord | Not installed");
                        };
                        if (DiscordPTB)
                        {
                            if (DiscordPTBP)
                            {
                                Console.WriteLine("Discord PTB | Protected");
                            }
                            else
                            {
                                Console.WriteLine("Discord PTB | Not protected");
                            };
                        }
                        else
                        {
                            Console.WriteLine("Discord PTB | Not installed");
                        };
                        Console.ForegroundColor = ConsoleColor.Green;
                        if (DiscordCanary)
                        {
                            if (DiscordCanaryP)
                            {
                                Console.WriteLine("Discord Canary | Protected");
                            }
                            else
                            {
                                Console.WriteLine("Discord Canary | Not protected");
                            };
                        }
                        else
                        {
                            Console.WriteLine("Discord Canary | Not installed");
                        };
                        Console.ForegroundColor = ConsoleColor.Red;
                        if (DiscordDev)
                        {
                            if (DiscordDevP)
                            {
                                Console.WriteLine("Discord Development | Protected");
                            }
                            else
                            {
                                Console.WriteLine("Discord Development | Not protected");
                            };
                        }
                        else
                        {
                            Console.WriteLine("Discord Development | Not installed");
                        };
                        break;
                    case 4:
                        Console.ForegroundColor = ConsoleColor.Red;
                        if (Discord)
                        {
                            if (DiscordP)
                            {
                                Console.WriteLine("Discord | Protected");
                            }
                            else
                            {
                                Console.WriteLine("Discord | Not protected");
                            };
                        }
                        else
                        {
                            Console.WriteLine("Discord | Not installed");
                        };
                        if (DiscordPTB)
                        {
                            if (DiscordPTBP)
                            {
                                Console.WriteLine("Discord PTB | Protected");
                            }
                            else
                            {
                                Console.WriteLine("Discord PTB | Not protected");
                            };
                        }
                        else
                        {
                            Console.WriteLine("Discord PTB | Not installed");
                        };
                        if (DiscordCanary)
                        {
                            if (DiscordCanaryP)
                            {
                                Console.WriteLine("Discord Canary | Protected");
                            }
                            else
                            {
                                Console.WriteLine("Discord Canary | Not protected");
                            };
                        }
                        else
                        {
                            Console.WriteLine("Discord Canary | Not installed");
                        };
                        Console.ForegroundColor = ConsoleColor.Green;
                        if (DiscordDev)
                        {
                            if (DiscordDevP)
                            {
                                Console.WriteLine("Discord Development | Protected");
                            }
                            else
                            {
                                Console.WriteLine("Discord Development | Not protected");
                            };
                        }
                        else
                        {
                            Console.WriteLine("Discord Development | Not installed");
                        };
                        break;
                };
            };
        }
    };
};