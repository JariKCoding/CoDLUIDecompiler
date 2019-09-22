using PhilLibX;
using PhilLibX.IO;
using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CoDLUIDecompiler.LuaRipper
{
    class BlackOps2
    {
        /// <summary>
        /// LuaFile Asset
        /// </summary>
        public struct LuaFile
        {
            /// <summary>
            /// Asset Name Hash
            /// </summary>
            public UInt32 NamePtr { get; set; }

            /// <summary>
            /// Data Size
            /// </summary>
            public Int32 AssetSize { get; set; }

            /// <summary>
            /// Raw Data Pointer
            /// </summary>
            public Int32 RawDataPtr { get; set; }
        }

        public static void ExportLuaFIles(ProcessReader reader, long assetPoolsAddress, long assetSizesAddress, string gameType)
        {
            // Found the game
            Console.WriteLine("Found supported game: Call of Duty: Black Ops 2");

            // Validate by XModel Name
            if (reader.ReadNullTerminatedString(reader.ReadInt32(reader.ReadInt32(assetPoolsAddress + 0x14) + 4)) == "defaultvehicle")
            {
                var AssetPoolOffset = reader.ReadStruct<Int32>(assetPoolsAddress + (4 * 41)) + 4;
                var poolSize = reader.ReadStruct<Int32>(assetSizesAddress + (4 * 41));

                Directory.CreateDirectory("t6_luafiles");

                int filesExported = 0;

                for (int i = 0; i < poolSize; i++)
                {
                    var data = reader.ReadStruct<LuaFile>(AssetPoolOffset + (i * 12));

                    if (!(data.AssetSize != 0))
                        continue;

                    filesExported++;
                    var RawData = reader.ReadBytes(data.RawDataPtr, data.AssetSize);

                    string exportName = Path.Combine("t6_luafiles", reader.ReadNullTerminatedString(data.NamePtr));
                    
                    if (Path.GetExtension(exportName) != ".lua" || File.Exists(exportName) && new FileInfo(exportName).Length == data.AssetSize)
                        continue;
                    Directory.CreateDirectory(Path.GetDirectoryName(exportName));

                    File.WriteAllBytes(exportName, RawData);
                }

                Console.WriteLine("Exported {0} files", filesExported);
            }
            else
            {
                Console.WriteLine("Unsupported version of game found");
            }
        }
    }
}
