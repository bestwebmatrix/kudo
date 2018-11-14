//
// Copyright 2018 Web Matrix Pty Ltd
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//

using Kudo;
using Kudo.Kudu;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace kudo
{
    class Program
    {
        static Int32 Main(string[] args)
        {
            if (TryInit(args, out Rest rest, out KuduCommand cmd))
            {
                try
                {
                    KuduCommandResult result = rest.Post<KuduCommandResult>("api/command", cmd);
                    if (rest != null)
                    {
                        if (!String.IsNullOrWhiteSpace(result.Output))
                        {
                            Console.Write(result.Output);
                        }

                        if (!String.IsNullOrWhiteSpace(result.Error))
                        {
                            Console.Error.Write(result.Error);
                        }

                        return result.ExitCode;
                    }

                    return -1;
                }
                catch (Exception ex)
                {
                    Console.Error.Write(ex.Message);
                    return -2;
                }
            }
            else
            {
                Console.WriteLine("Kudo [-cfg <config filename>] [-baseUri <kudu instance uri>] [-user <username>] [-pass <password>] [-cmd <command>] [-dir <dir>]");
                Console.WriteLine(" -cfg:     The configuration file to load. Defaults to %HOME%/.kudo/config.json");
                Console.WriteLine(" -baseUri: The uri of the Kudu endpoint to run the command against.");
                Console.WriteLine(" -user:    The username to authenticate with.");
                Console.WriteLine(" -pass:    The password to authenticate with.");
                Console.WriteLine(" -cmd:     The command to run on the Kudu server.");
                Console.WriteLine(" -dir:     The directory on the Kudu server to run the command in.");

                return -1;
            }
        }

        private static Boolean TryInit(String[] args, out Rest rest, out KuduCommand cmd)
        {
            String baseUri = null;
            String username = null;
            String password = null;
            String command = null;
            String dir = null;
            String site = null;
            String cfgFile = "%HOME%/.kudo/config.json";

            site = FindArg(args, "site") ?? String.Empty;
            cfgFile = FindArg(args, "cfg") ?? cfgFile;

            LoadConfig(cfgFile, site, ref baseUri, ref username, ref password, ref command, ref dir);

            baseUri = FindArg(args, "baseUri") ?? baseUri;
            username = FindArg(args, "user") ?? username;
            password = FindArg(args, "pass") ?? password;
            command = FindArg(args, "cmd") ?? command;
            dir = FindArg(args, "dir") ?? dir;

            if (username == null)
            {
                username = PromptFor("Please Enter Username:", false);
            }

            if (password == null)
            {
                password = PromptFor("Please Enter Password:", true);
            }

            if(!String.IsNullOrWhiteSpace(baseUri) &&
               !String.IsNullOrWhiteSpace(username) &&
               !String.IsNullOrWhiteSpace(password) &&
               !String.IsNullOrWhiteSpace(command) &&
               !String.IsNullOrWhiteSpace(dir))
            {
                rest = new Rest()
                {
                    BaseUri = baseUri,
                    Username = username,
                    Password = password
                };

                cmd = new KuduCommand()
                {
                    Command = command,
                    Dir = dir
                };

                return true;
            }

            rest = null;
            cmd = null;
            return false;
        }

        private static string PromptFor(String message, Boolean mask)
        {
            Boolean done = false;
            Stack<Char> pass = new Stack<Char>();

            Console.Error.WriteLine(message);
            while(!done)
            {
                if(Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    if(keyInfo.Key == ConsoleKey.Enter)
                    {
                        Console.Error.WriteLine();
                        done = true;
                        return new String(pass.Reverse().ToArray());
                    }

                    if(keyInfo.Key == ConsoleKey.Backspace)
                    {
                        if (pass.Count > 0)
                        {
                            pass.Pop();

                            if (mask)
                            {
                                Console.Error.Write("\r" + new String(pass.Reverse().ToArray()) + " \b");
                            }
                        }
                    }

                    if(!Char.IsControl(keyInfo.KeyChar))
                    {
                        pass.Push(keyInfo.KeyChar);

                        if (mask)
                        {
                            Console.Error.Write("\r" + new String('*', pass.Count));
                        }
                        else
                        {
                            Console.Error.Write("\r" + new String(pass.Reverse().ToArray()));
                        }
                    }
                }
                else
                {
                    Thread.Sleep(0);
                }
            }

            return null;
        }

        private static void LoadConfig(String cfgFile, String site, ref String baseUri, ref String username, ref String password, ref String command, ref String dir)
        {
            if(cfgFile.Contains("%"))
            {
                cfgFile = Environment.ExpandEnvironmentVariables(cfgFile);
            }

            if(File.Exists(cfgFile))
            {
                ConfigFile config = JsonConvert.DeserializeObject<ConfigFile>(File.ReadAllText(cfgFile));
                if(config != null)
                {
                    SiteConfig cfg = config.ContainsKey(site) ? config[site] : config.FirstOrDefault().Value;
                    if (cfg != null)
                    {
                        baseUri = cfg.BaseUri;
                        username = cfg.Username;
                        password = cfg.Password;
                        command = cfg.Command;
                        dir = cfg.DefaultDir;

                        return;
                    }
                }
            }
            else
            {
                ConfigFile newConfig = new ConfigFile();
                newConfig["Default"] = new SiteConfig();
                File.WriteAllText(cfgFile, JsonConvert.SerializeObject(newConfig, Formatting.Indented));
            }

            baseUri = null;
            username = null;
            password = null;
            command = null;
            dir = null;
        }

        private static String FindArg(String[] args, String arg)
        {
            for(Int32 i = 0; i < args.Length; i++)
            {
                if (args[i].ToLower() == $"-{arg.ToLower()}" || args[i] == $"/{arg.ToLower()}")
                {
                    return args[i + 1];
                }
            }

            return null;
        }
    }
}
