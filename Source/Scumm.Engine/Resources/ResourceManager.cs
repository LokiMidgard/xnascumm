using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Scumm.Engine.IO;
using Scumm.Engine.Resources.Loaders;
using Scumm.Engine.Resources.Scripts;
using Scumm.Engine.Resources.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Scumm.Engine.Resources
{
    public class ResourceManager
    {
        private static readonly IDictionary<string, object> emptyParameters = new Dictionary<string, object>();
        private ushort roomsCount, scriptsCount, soundsCount, costumesCount, charsetsCount;

        private IDictionary<byte, uint> roomsIndexList;
        private ResourceIndexEntry[] scriptsIndexList;
        private ResourceIndexEntry[] soundsIndexList;
        private ResourceIndexEntry[] costumesIndexList;
        private ResourceIndexEntry[] charsetsIndexList;

        private Actor[] actors;
        private Object[] objects;
        private Verb[] verbs;

        private ScummBinaryReader dataFileReader;
        private Dictionary<string, ResourceLoader> loaders;

        internal Dictionary<string, Resource> cache;

        public ResourceManager(string gamePath, string gameId, int scummVersion)
        {
            // Init properties
            this.GamePath = gamePath;
            this.GameId = gameId;
            this.ScummVersion = scummVersion;

            this.cache = new Dictionary<string, Resource>();

            loaders = new Dictionary<string, ResourceLoader>();

            actors = new Actor[13];
            for (int i = 0; i < 13; ++i)
                actors[i] = new Actor();

            verbs = new Verb[300];
            for (int i = 0; i < 300; ++i)
                verbs[i] = new Verb("");
        }

        public void AddLoader(String key, ResourceLoader loader)
        {
            loader.ResourceManager = this;
            loaders.Add(key, loader);
        }

        public void LoadGame()
        {
            // Read the game index file
            ReadIndexFile();

            // Read the game data file
            ReadDataFile();
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

            int blockCount = 0;

            using (var reader = new ScummBinaryReader(new ScummStream(indexPath, this.ScummVersion)))
            {
                var blockType = new string(reader.ReadChars(4));
                var itemSize = reader.ReadUInt32BigEndian();

                while (reader.BaseStream.Position <= reader.BaseStream.Length)
                {
                    blockCount++;

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

                    roomId = reader.ReadByte();
                }
            }

            else if (blockType == "MAXS")
            {
                var variablesCount = reader.ReadUInt16();
                
                // Skip 2 bytes
                reader.ReadUInt16();
                
                uint bitVariablesCount = reader.ReadUInt16();
                uint localObjectsCount = reader.ReadUInt16();
                uint newNamesCount = reader.ReadUInt16();
                uint charsetsCount = reader.ReadUInt16();
                uint verbsCount = reader.ReadUInt16();
                uint arraysCount = reader.ReadUInt16();
                uint inventoryObjectsCount = reader.ReadUInt16();
            }

            else if (blockType == "DROO")
            {
                // just read, not keeping anything besides roomsCount
                ReadResourceReferenceList(reader, ref roomsCount);
            }

            else if (blockType == "DSCR")
            {
                this.scriptsIndexList = ReadResourceReferenceList(reader, ref scriptsCount);
            }

            else if (blockType == "DSOU")
            {
                this.soundsIndexList = ReadResourceReferenceList(reader, ref soundsCount);
            }

            else if (blockType == "DCOS")
            {
                this.costumesIndexList = ReadResourceReferenceList(reader, ref costumesCount);
            }

            else if (blockType == "DCHR")
            {
                this.charsetsIndexList = ReadResourceReferenceList(reader, ref charsetsCount);
            }

            else if (blockType == "DOBJ")
            {
                uint itemsCount = reader.ReadUInt16();
                objects = new Object[itemsCount];

                for (int i = 0; i < itemsCount; i++) {
                    objects[i] = new Object();
                    objects[i].Id = (UInt16)i;
                    objects[i].OwnerState = reader.ReadByte();
                }
                for (int i = 0; i < itemsCount; i++)
                {
                    objects[i].ClassData = reader.ReadUInt32();
                }   
            }
            else if (blockType == "AARY")
            {
            }
            else
            {
                //throw new InvalidOperationException(string.Format("Cannot read block of type '{0}' in the index file.", blockType));
            }
        }

        private ResourceIndexEntry[] ReadResourceReferenceList(ScummBinaryReader reader, ref ushort itemsCount)
        {
            itemsCount = reader.ReadUInt16();

            var resourceReferenceList = new ResourceIndexEntry[itemsCount];
            var roomIdList = new byte[itemsCount];
        
            for (int i = 0; i < itemsCount; i++)
            {
                roomIdList[i] = reader.ReadByte();
            }
            for (int i = 0; i < itemsCount; i++)
            {
                var resourceOffset = reader.ReadUInt32();
                resourceReferenceList[i] = new ResourceIndexEntry(roomIdList[i], resourceOffset);
            }
        
            return resourceReferenceList;
        }

        private void ReadDataFile()
        {
            // Read the entire game data file into memory for now
            var dataPath = Path.Combine(this.GamePath, string.Format("{0}.001", this.GameId));
            this.dataFileReader = new ScummBinaryReader(new ScummStream(dataPath, this.ScummVersion));

            // Read first block with room offset - other offsets are just wrong
            if (dataFileReader.FindDataBlock("LOFF") > 0)
            {
                this.roomsCount = this.dataFileReader.ReadByte();
                this.roomsIndexList = new Dictionary<byte, uint>();

                for (int i = 0; i < this.roomsCount; i++)
                {
                    var roomId = this.dataFileReader.ReadByte();
                    var roomOffset = this.dataFileReader.ReadUInt32();

                    this.roomsIndexList.Add(roomId, roomOffset);
                }
            }
        }

        public T Load<T>(string resourceType, byte resourceId) where T : Resource
        {
            return Load<T>(resourceType, resourceId, emptyParameters);
        }

        public T Load<T>(string resourceType, byte resourceId, ScummBinaryReader reader) where T : Resource
        {
            return Load<T>(resourceType, resourceId, reader, emptyParameters);
        }

        public T Load<T>(string resourceType, int resourceId, IDictionary<string, object> parameters) where T : Resource
        {
            var cacheKey = string.Format("{0}_{1}", resourceType, resourceId);

            if (!this.cache.ContainsKey(cacheKey))
            {
                if (this.loaders.ContainsKey(resourceType))
                {
                    var loader = this.loaders[resourceType];
                    ResourceIndexEntry roomReference;

                    if (resourceType == "ROOM")
                    {
                        roomReference = new ResourceIndexEntry((byte)resourceId, 0);
                    }

                    else
                    {
                        roomReference = FindIndexFromResourceId(resourceType, resourceId);
                    }

                    // Find the room offset
                    var roomOffset = this.roomsIndexList[roomReference.RoomId];

                    // Change the position of the stream
                    this.dataFileReader.BaseStream.Position = roomOffset;

                    if (resourceType != "ROOM")
                    {
                        this.dataFileReader.BaseStream.Seek(roomReference.Offset, SeekOrigin.Current);
                    }

                    // Load the resource
                    var resource = loader.LoadResourceData(dataFileReader, cacheKey, parameters);

                    // Add the created resource to the cache
                    this.cache.Add(cacheKey, resource);

                    // Return the resource
                    return (T)resource;
                }

                else
                {
                    throw new InvalidOperationException(string.Format("No resource loaders for blockType '{0}' were found.", resourceType));
                }
            }

            else
            {
                return (T)this.cache[cacheKey];
            }
        }

        public T Load<T>(string resourceType, int resourceId, ScummBinaryReader reader, IDictionary<string, object> parameters) where T : Resource
        {
            if (this.loaders.ContainsKey(resourceType))
            {
                var loader = this.loaders[resourceType];

                // Load the resource
                string resourceIdName = string.Format("{0}_{1}", resourceType, resourceId);
                var resource = loader.LoadResourceData(reader, resourceIdName, parameters);

                // Return the resource
                return (T)resource;
            }
            else
            {
                throw new InvalidOperationException(string.Format("No resource loaders for blockType '{0}' were found.", resourceType));
            }
        }

        public T Load<T>(string resourceType, IDictionary<string, object> parameters) where T : Resource
        {
            if (this.loaders.ContainsKey(resourceType))
            {
                var loader = this.loaders[resourceType];
       
                // Load the resource
                var resource = loader.LoadResourceData(dataFileReader, null, parameters);
       
                // Return the resource
                return (T)resource;
            }
            else
            {
                throw new InvalidOperationException(string.Format("No resource loaders for blockType '{0}' were found.", resourceType));
            }
        }

        public ResourceIndexEntry FindIndexFromResourceId(string resourceType, int resourceId)
        {
            if (resourceType == "COST")
            {
                return this.costumesIndexList[resourceId];
            }
            if (resourceType == "SCRP")
            {
                return this.scriptsIndexList[resourceId];
            }
            if (resourceType == "CHRS")
            {
                return this.charsetsIndexList[resourceId];
            }

            throw new InvalidOperationException("Resource Id not found.");
        }

        public Actor FindActor(int actorId)
        {
            if (actorId >= actors.Count())
                throw new IndexOutOfRangeException();
            return actors[actorId];
        }

        public Object FindObject(int objectId)
        {
            if (objectId >= objects.Count()) 
                throw new IndexOutOfRangeException();
            return objects[objectId];
        }

        public Verb FindVerb(int verbId)
        {
            if (verbId >= verbs.Count())
                throw new IndexOutOfRangeException();
            return verbs[verbId];
        }
    }
}
