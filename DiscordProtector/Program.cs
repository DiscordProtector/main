using System;
using System.Diagnostics;
using System.IO;
namespace DiscordProtector
{
    internal class Program
    {
        /* Print header */
        static void PrintHeader()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Discord Protector V1.0.0 [BETA]\n");
        }

        /* Main entry point */
        static void Main(string[]args)
        {
            Console.Title = "Discord Protector";
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
            Console.WriteLine("2) Uninstall Discord Protector\n3) Change Discord Protector's protection level\n4) Change Discord Protector's password");

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
                        Console.WriteLine("2) Uninstall Discord Protector\n3) Change Discord Protector's protection level\n4) Change Discord Protector's password");
                        break;
                    case 2:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("1) Install Discord Protector");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("2) Uninstall Discord Protector");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("3) Change Discord Protector's protection level\n4) Change Discord Protector's password");
                        break;
                    case 3:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("1) Install Discord Protector\n2) Uninstall Discord Protector");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("3) Change Discord Protector's protection level");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("4) Change Discord Protector's password");
                        break;
                    case 4:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("1) Install Discord Protector\n2) Uninstall Discord Protector\n3) Change Discord Protector's protection level");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("4) Change Discord Protector's password");
                        break;
                };
            };
        }

        /* Install Discord protector */
        static void InstallToDiscord(string edition)
        {
            Console.Clear();
            PrintHeader();
            Console.ForegroundColor = ConsoleColor.White;
            foreach (var a in Directory.EnumerateDirectories($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/{edition}"))
            {
                string d = a.Replace("\\", "/");
                if (File.Exists($"{d}/resources/app.asar"))
                {
                    Console.WriteLine($"Extracting asar ({d}/resources/app.asar)");
                    /* Extract asar */
                    Process.Start("node.exe",$@"unpack.js ""{d}/resources/app.asar"" ""{d}/resources/app""").WaitForExit();
                    /* Write DiscordProtector stuff */
                    Console.WriteLine($"Adding security to asar ({d}/resources/app.asar)");
                    if (!Directory.Exists($"{d}/resources/app/DiscordProtector"))
                    {
                        Directory.CreateDirectory($"{d}/resources/app/DiscordProtector");
                    };
                    var OldPath=edition;
                    if (File.Exists($"{d}/resources/app/DiscordProtector/index.js"))
                    {
                        OldPath = File.ReadAllLines($"{d}/resources/app/DiscordProtector/index.js")[2].Substring(14);
                        OldPath = OldPath.Remove(OldPath.Length-2,2);
                    };
                    File.WriteAllText($"{d}/resources/app/DiscordProtector/index.js",$"/* Discord Protector V1.0.0 [BETA] */\nconst OldDir='{OldPath}';\nconst Secret='{Guid.NewGuid().ToString().Replace("-","")}';\nconst SecurityLvl=1;const fs=require('fs');\n\n/* Gets user's data from Discord protector */\nexports.GetUserData=(userDataRoot)=>{{\nif(SecurityLvl===1){{\nif(fs.existsSync(`${{userDataRoot}}/${{OldDir}}`)){{\ntry{{fs.renameSync(`${{userDataRoot}}/${{OldDir}}`,`${{userDataRoot}}/${{Secret}}`);}}catch{{fs.rmdirSync(`{{userDataRoot}}/{{OldDir}}`);}};\n}};\nreturn(Secret);\n}};\n}};");
                    var PathsLines = File.ReadAllLines($"{d}/resources/app/common/paths.js");
                    var sqUpdateLines = File.ReadAllLines($"{d}/resources/app/app_bootstrap/squirrelUpdate.js");
                    var InterCount = 0;
                    foreach(var l in PathsLines)
                    {
                        if(l=="let installPath = null;")
                        {
                            PathsLines[InterCount] = "let installPath=null;\nlet DiscordProtector=require('../DiscordProtector/index.js');";
                        };
                        if(l=="function determineUserData(userDataRoot, buildInfo) {")
                        {
                            PathsLines[InterCount+1] = "  return _path.default.join(userDataRoot,DiscordProtector.GetUserData(userDataRoot)); // Modified by Discord Protector";
                        };
                        InterCount++;
                    };
                    File.WriteAllLines($"{d}/resources/app/common/paths.js",PathsLines);
                    InterCount = 0;
                    foreach (var l in sqUpdateLines)
                    {
                        if(l=="function restart(app, newVersion) {")
                        {
                            PathsLines[InterCount] = "function restart(app,newVersion){\napp.once('will-quit',()=>{\nconst execPath=_path.default.resolve(rootFolder,`app-${newVersion}`).replace(/\\/g,'/');\n_child_process.default.spawn(`${process.env.LOCALAPPDATA}\\DiscordProtector\\api.exe`,['--update',`\"${execPath}\"`,`\"${exeName}\"`],{\ndetached: true\n});\n});\napp.quit();\n};";
                        };
                    };
                    /* Kill discord */
                    Console.WriteLine($"Killing Discord ({d})");
                    foreach (var p in Process.GetProcessesByName(edition))
                    {
                        Console.WriteLine($"Killing Discord PID: {p.Id}");
                        p.Kill();
                    };
                    Console.WriteLine($"Packing asar ({d}/resources/app.asar)");
                    /* Re-pack */
                    Process.Start("node.exe",$@"pack.js ""{d}/resources/app"" ""{d}/resources/app.asar""").WaitForExit();
                    /* Clean up */
                    Console.WriteLine($"Cleaning up ({d}");
                    try{
                        //Directory.Delete($"{d}/resources/app",true);
                    }catch{};
                    /* Restart discord */
                    Console.WriteLine($"Starting Discord ({d})");
                    Process.Start($"{d}/{edition}.exe");
                    Console.ReadLine();
                    Console.Clear();
                };
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
            var DiscordP = false;
            var DiscordPTBP = false;
            var DiscordCanaryP = false;
            var DiscordDevP = false;

            /* Display information */
            Console.Clear();
            PrintHeader();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Which version of Discord would you like to protect?\n\nUse the arrow keys to change your choice and press enter to confirm your choice\n");
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
                            };
                            break;
                        case 3:
                            if (DiscordCanary)
                            {
                                // install
                                InstallToDiscord("discordcanary");
                            };
                            break;
                        case 4:
                            if (DiscordDev)
                            {
                                // install
                                InstallToDiscord("discorddevelopment");
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