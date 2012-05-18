using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Scumm.Engine.IO;

namespace Scumm.Engine.Resources
{
    public class ResourceManager
    {
        private ushort roomsCount, scriptsCount, soundsCount, costumesCount, charsetsCount;
        private IList<ResourceIndexEntry> roomsIndexList;
        private IList<ResourceIndexEntry> scriptsIndexList;
        private IList<ResourceIndexEntry> soundsIndexList;
        private IList<ResourceIndexEntry> costumesIndexList;
        private IList<ResourceIndexEntry> charsetsIndexList;

        public ResourceManager(string gamePath, string gameId, int scummVersion)
        {
            // Init properties
            this.GamePath = gamePath;
            this.GameId = gameId;
            this.ScummVersion = scummVersion;

            // Read the game index file
            ReadIndexFile();
        }

        public string GameId
        {
            get;
            private set;
        }

        public string GamePath
        {
            get;
            private set;
        }

        public int ScummVersion
        {
            get;
            private set;
        }

        private void ReadIndexFile()
        {
            var indexPath = Path.Combine(this.GamePath, string.Format("{0}.000", this.GameId));

            Console.WriteLine("Reading index file '{0}'...", indexPath);
            int blockCount = 0;

            using (var reader = new ScummBinaryReader(new ScummStream(indexPath, this.ScummVersion)))
            {
                var blockType = new string(reader.ReadChars(4));
                var itemSize = reader.ReadUInt32BigEndian();

                while (reader.BaseStream.Position <= reader.BaseStream.Length)
                {
                    blockCount++;
                    Console.WriteLine("Reading index block of type '{0}', size {1}", blockType, itemSize);

                    ReadIndexBlock(reader, blockType);

                    blockType = new string(reader.ReadChars(4));
                    itemSize = reader.ReadUInt32BigEndian();

                    if (blockCount >= 9)
                    {
                        break;
                    }
                }
            }
        }

        private void ReadIndexBlock(ScummBinaryReader reader, string blockType)
        {
            if (blockType == "RNAM")
            {
                byte roomId = reader.ReadByte();
                while (roomId != 0)
                {
                    var roomNameData = reader.ReadBytes(9);
                    var roomName = string.Empty;

                    for (int i = 0; i < roomNameData.Length; i++)
                    {
                        roomName += (char)(roomNameData[i] ^ 0xFF);
                    }

                    Console.WriteLine(Convert.ToString(roomId) + ". " + roomName);
                    roomId = reader.ReadByte();
                }
            }

            else if (blockType == "MAXS")
            {
                //var variablesCount = reader.ReadUInt16();
                //
                //// Skip 2 bytes
                //reader.ReadUInt16();
                //
                //var bitVariablesCount = reader.ReadUInt16();
                //var localObjectsCount = reader.ReadUInt16();
                //var arraysCount = reader.ReadUInt16();
                //
                //// Skip 2 bytes
                //reader.ReadUInt16();
                //
                //var verbsCount = reader.ReadUInt16();
                //var floatingObjectsCount = reader.ReadUInt16();
                //var inventoryObjectsCount = reader.ReadUInt16();
                //this.roomsCount = reader.ReadUInt16();
                //this.scriptsCount = reader.ReadUInt16();
                //this.soundsCount = reader.ReadUInt16();
                //this.charsetsCount = reader.ReadUInt16();
                //this.costumesCount = reader.ReadUInt16();
                //var globalObjectsCount = reader.ReadUInt16();
                //var newNamesCount = 50;
                //var globalScriptsCount = 200;
            }

            else if (blockType == "DROO")
            {
                //this.roomsIndexList = ReadResourceReferenceList(reader, this.roomsCount);
            }

            else if (blockType == "DSCR")
            {
                //this.scriptsIndexList = ReadResourceReferenceList(reader, this.scriptsCount);
            }

            else if (blockType == "DSOU")
            {
                //this.soundsIndexList = ReadResourceReferenceList(reader, this.soundsCount);
            }

            else if (blockType == "DCOS")
            {
                //this.costumesIndexList = ReadResourceReferenceList(reader, this.costumesCount);
            }

            else if (blockType == "DCHR")
            {
                //this.charsetsIndexList = ReadResourceReferenceList(reader, this.charsetsCount);
            }

            else if (blockType == "DOBJ")
            {
                //var itemsCount = reader.ReadUInt16();
                //
                //var objectsOwnerTable = reader.ReadBytes(itemsCount);
                //var objectsStateTable = new byte[itemsCount];
                //
                //for (int i = 0; i < itemsCount; i++)
                //{
                //    objectsStateTable[i] = (byte)(objectsOwnerTable[i] >> 4);
                //    objectsOwnerTable[i] = (byte)(objectsOwnerTable[i] & 0x0F);
                //}
                //
                //var objectsClassTable = reader.ReadBytes(4 * itemsCount);
            }
            else if (blockType == "AARY")
            {
            }
            else
            {
                //throw new InvalidOperationException(string.Format("Cannot read block of type '{0}' in the index file.", blockType));
            }
        }

        //private IList<ResourceIndexEntry> ReadResourceReferenceList(ScummBinaryReader reader, ushort excpectedItemsCount)
        //{
        //    var resourceReferenceList = new List<ResourceIndexEntry>();
        //
        //    var itemsCount = reader.ReadUInt16();
        //
        //    if (itemsCount != excpectedItemsCount)
        //    {
        //        throw new InvalidDataException(string.Format("Cannot read resource reference list: the items count doesn't match the expected items count. (Readed: {0}, Expected: {1})", itemsCount, excpectedItemsCount));
        //    }
        //
        //    var roomIdList = new byte[itemsCount];
        //
        //    for (int i = 0; i < itemsCount; i++)
        //    {
        //        roomIdList[i] = reader.ReadByte();
        //    }
        //
        //    for (int i = 0; i < itemsCount; i++)
        //    {
        //        var resourceOffset = reader.ReadUInt32();
        //        resourceReferenceList.Add(new ResourceIndexEntry(roomIdList[i], resourceOffset));
        //    }
        //
        //    return resourceReferenceList;
        //}
    }
}
