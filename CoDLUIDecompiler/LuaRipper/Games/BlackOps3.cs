using PhilLibX;
using PhilLibX.IO;
using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CoDLUIDecompiler.LuaRipper
{
    class BlackOps3
    {
        public struct AssetPool
        {
            /// <summary>
            /// A pointer to the asset pool
            /// </summary>
            public long PoolPointer { get; set; }

            /// <summary>
            /// Entry Size
            /// </summary>
            public int AssetSize { get; set; }

            /// <summary>
            /// Max Asset Count/Pool Size
            /// </summary>
            public int PoolSize { get; set; }

            /// <summary>
            /// Null Padding
            /// </summary>
            public int Padding { get; set; }

            /// <summary>
            /// Numbers of Assets in this Pool
            /// </summary>
            public int AssetCount { get; set; }

            /// <summary>
            /// Next Free Header/Slot
            /// </summary>
            public long NextSlot { get; set; }
        }

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
            public Int32 AssetSize { get; set; }

            /// <summary>
            /// Raw Data Pointer
            /// </summary>
            public long RawDataPtr { get; set; }
        }

        public static void ExportLuaFIles(ProcessReader reader, long assetPoolsAddress, long assetSizesAddress, string gameType)
        {
            // Found the game
            Console.WriteLine("Found supported game: Call of Duty: Black Ops 3");

            // Get Base Address for ASLR and Scans
            long baseAddress = reader.GetBaseAddress();

            // Validate by XModel Name
            if (reader.ReadNullTerminatedString(reader.ReadInt64(reader.ReadInt64(baseAddress + assetPoolsAddress + 0x80))) == "void")
            {
                AssetPool LuaPoolData = reader.ReadStruct<AssetPool>(baseAddress + assetPoolsAddress + 0x20 * 47);

                long address = LuaPoolData.PoolPointer;
                long endAddress = LuaPoolData.PoolSize * LuaPoolData.AssetSize + address;

                Directory.CreateDirectory("t7_luafiles");
                int filesExported = 0;

                for (int i = 0; i < LuaPoolData.PoolSize; i++)
                {
                    var data = reader.ReadStruct<LuaFile>(address + (i * LuaPoolData.AssetSize));

                    if (!(data.AssetSize != 0))
                        continue;

                    filesExported++;
                    var RawData = reader.ReadBytes(data.RawDataPtr, data.AssetSize);

                    string exportName = Path.Combine("t7_luafiles", reader.ReadNullTerminatedString(data.NamePtr));

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
