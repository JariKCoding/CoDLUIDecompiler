using PhilLibX;
using PhilLibX.IO;
using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CoDLUIDecompiler.LuaRipper
{
    class BlackOps4
    {
        /// <summary>
        /// Asset Pool Data
        /// </summary>
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
            public long Hash { get; set; }

            /// <summary>
            /// Asset Hash
            /// </summary>
            public ulong NullPointer { get; set; }

            /// <summary>
            /// Data Size
            /// </summary>
            public Int32 DataSize { get; set; }

            /// <summary>
            /// Asset Hash
            /// </summary>
            public Int32 UnknownHash1 { get; set; }

            /// <summary>
            /// Data startLocation
            /// </summary>
            public long startLocation { get; set; }
        }

        public static void ExportLuaFIles(ProcessReader reader, long assetPoolsAddress, long assetSizesAddress, string gameType)
        {
            // Found the game
            Console.WriteLine("Found supported game: Call of Duty: Black Ops 4");

            // Get Base Address for ASLR and Scans
            long baseAddress = reader.GetBaseAddress();

            // Validate by XModel Name
            if (reader.ReadUInt64(reader.ReadStruct<AssetPool>(baseAddress + assetPoolsAddress + 0x20 * 0x4).PoolPointer) == 0x04647533e968c910)
            {
                AssetPool LuaPoolData = reader.ReadStruct<AssetPool>(baseAddress + assetPoolsAddress + 0x20 * 0x67);

                long address = LuaPoolData.PoolPointer;
                long endAddress = LuaPoolData.PoolSize * LuaPoolData.AssetSize + address;

                Directory.CreateDirectory("t8_luafiles");
                int filesExported = 0;
                for (int i = 0; i < LuaPoolData.PoolSize; i++)
                {
                    var data = reader.ReadStruct<LuaFile>(address + (i * LuaPoolData.AssetSize));
                    
                    if (data.NullPointer != 0 || data.Hash == 0)
                        continue;
                    if (data.Hash >= address && data.Hash <= endAddress)
                        continue;

                    filesExported++;
                    var RawData = reader.ReadBytes(data.startLocation, data.DataSize);
                    ulong assetHash = (ulong)data.Hash & 0xFFFFFFFFFFFFFFF;
                    string HashString, fileName;
                    //if (!Program.AssetNameCache.Entries.TryGetValue(assetHash, out HashString))
                        fileName = String.Format("t8_luafiles\\LuaFile_{0:x}.lua", assetHash);
                    //else
                    //fileName = String.Format("t8_luafiles\\{0}.lua", HashString);
                    if (File.Exists(fileName) && new FileInfo(fileName).Length == data.DataSize)
                        continue;
                    File.WriteAllBytes(fileName, RawData);
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
