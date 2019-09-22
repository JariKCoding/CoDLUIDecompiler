using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using PhilLibX;
using PhilLibX.IO;

namespace CoDLUIDecompiler
{
    /// <summary>
    /// Game Definition (AssetDB Address, Sizes Address, Game Type ID (MP, SP, ZM, etc.), Export Method
    /// </summary>
    using GameDefinition = Tuple<long, long, string, Action<ProcessReader, long, long, string>>;

    class MemoryLoading
    {
        static readonly Dictionary<string, GameDefinition> Games = new Dictionary<string, GameDefinition>()
        {
            // Call of Duty: Black Ops 2
            { "t6zm",               new GameDefinition(0xD41240,          0xD40E80,       "zm",               LuaRipper.BlackOps2.ExportLuaFIles) },
            { "t6mp",               new GameDefinition(0xD4B340,          0xD4AF80,       "mp",               LuaRipper.BlackOps2.ExportLuaFIles) },
            { "t6sp",               new GameDefinition(0xBD46B8,          0xBD42F8,       "sp",               LuaRipper.BlackOps2.ExportLuaFIles) },
            // Call of Duty: Ghosts
            { "iw6mp64_ship",       new GameDefinition(0x1409E4F20,       0x1409E4E20,    "mp",               LuaRipper.Ghosts.ExportLuaFIles) },
            { "iw6sp64_ship",       new GameDefinition(0x14086DCB0,       0x14086DBB0,    "sp",               LuaRipper.Ghosts.ExportLuaFIles) },
            // Call of Duty: Advanced Warfare
            { "s1_mp64_ship",       new GameDefinition(0x1409B40D0,       0x1409B4B90,    "mp",               LuaRipper.AdvancedWarfare.ExportLuaFIles) },
            { "s1_sp64_ship",       new GameDefinition(0x140804690,       0x140804140,    "sp",               LuaRipper.AdvancedWarfare.ExportLuaFIles) },
            // Call of Duty: Black Ops 3
            { "BlackOps3",       new GameDefinition(0x93FA290,          0x4D4F100,       "core",               LuaRipper.BlackOps3.ExportLuaFIles) },
            // Call of Duty: Infinite Warfare
            { "iw7_ship",           new GameDefinition(0x1414663D0,       0x141466290,    "core",             LuaRipper.InfiniteWarfare.ExportLuaFIles) },
            // Call of Duty: Modern Warfare Remastered
            { "h1_mp64_ship",       new GameDefinition(0x10B4460,         0x10B3C80,      "mp",               LuaRipper.ModernWarfareRM.ExportLuaFIles) },
            { "h1_sp64_ship",       new GameDefinition(0xEC9FB0,          0xEC97D0,       "sp",               LuaRipper.ModernWarfareRM.ExportLuaFIles) },
            // Call of Duty: World War II
            { "s2_mp64_ship",       new GameDefinition(0xC05370,          0xC05370,       "mp",               LuaRipper.WorldWarII.ExportLuaFIles) },
            { "s2_sp64_ship",       new GameDefinition(0x9483F0,          0xBCC5E0,       "sp",               LuaRipper.WorldWarII.ExportLuaFIles) },
            // Call of Duty: Black Ops 4
            { "BlackOps4",       new GameDefinition(0x88788D0,          0x74FDED0,       "core",               LuaRipper.BlackOps4.ExportLuaFIles) },
        };

        /// <summary>
        /// Looks for matching game and loads BSP from it
        /// </summary>
        public static void LoadGame()
        {
            try
            {
                // Get all processes
                var processes = Process.GetProcesses();

                // Loop through them, find match
                foreach (var process in processes)
                {
                    // Check for it in dictionary
                    if (Games.TryGetValue(process.ProcessName, out var game))
                    {
                        // Export it
                        game.Item4(new ProcessReader(process), game.Item1, game.Item2, game.Item3);

                        // Done
                        return;
                    }
                }

                // Failed
                Console.WriteLine("Failed to find a supported game, please ensure one of them is running.");
            }
            catch (Exception e)
            {
                Console.WriteLine("An unhandled exception has occured:");
                Console.WriteLine(e);
            }
        }
    }
}
