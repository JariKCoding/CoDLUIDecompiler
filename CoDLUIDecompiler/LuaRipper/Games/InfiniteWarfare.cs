using PhilLibX;
using PhilLibX.IO;
using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CoDLUIDecompiler.LuaRipper
{
    class InfiniteWarfare
    {
        /// <summary>
        /// LuaFile Asset
        /// </summary>
        public struct LuaFile
        {
            /// <summary>
            /// Asset Name Hash
            /// </summary>
            public long NamePtr { get; set; }

            /// <summary>
            /// Data Size
            /// </summary>
            public Int32 DataSize { get; set; }
            public Int32 DataSize2 { get; set; }

            /// <summary>
            /// Asset Hash
            /// </summary>
            public long startLocation { get; set; }
        }

        public static void ExportLuaFIles(ProcessReader reader, long assetPoolsAddress, long assetSizesAddress, string gameType)
        {
            // Found the game
            Console.WriteLine("Found supported game: Call of Duty: Infinite Warfare");

            // Validate by XModel Name
            if (reader.ReadNullTerminatedString(reader.ReadInt64(reader.ReadInt64(assetPoolsAddress + 0x40) + 8)) == "viewmodel_default")
            {
                var AssetPoolOffset = reader.ReadStruct<Int64>(assetPoolsAddress + (8 * 59));
                var poolSize = reader.ReadStruct<UInt32>(assetSizesAddress + (4 * 59));

                Directory.CreateDirectory("iw7_luafiles");

                int filesExported = 0;

                for (int i = 0; i < poolSize; i++)
                {
                    var data = reader.ReadStruct<LuaFile>(AssetPoolOffset + 8 + (i * 24));

                    if (!(data.DataSize != 0 && data.DataSize2 == 2))
                        continue;

                    filesExported++;

                    var RawData = reader.ReadBytes(data.startLocation, data.DataSize);

                    string exportName = Path.Combine("iw7_luafiles", reader.ReadNullTerminatedString(data.NamePtr));

                    if (File.Exists(exportName) && new FileInfo(exportName).Length == data.DataSize)
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
