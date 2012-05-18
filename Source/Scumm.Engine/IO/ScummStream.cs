using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Scumm.Engine.IO
{
    public class ScummStream : Stream
    {
        private Stream fileStream;
        private byte encryptionByte;

        public ScummStream(string path, int scummVersion)
        {
            this.Path = path;
            this.ScummVersion = scummVersion;
            this.fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);

            if (scummVersion == 5)
            {
                this.encryptionByte = 0x69;
            }
        }

        public string Path
        {
            get;
            private set;
        }

        public int ScummVersion
        {
            get;
            private set;
        }

        public override long Length
        {
            get
            {
                return this.fileStream.Length;
            }
        }

        public override long Position
        {
            get
            {
                return this.fileStream.Position;
            }
            set
            {
                this.fileStream.Position = value;
            }
        }

        #region Stream Capabilities
        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return true;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override bool CanTimeout
        {
            get
            {
                return false;
            }
        }
        #endregion

        public override void Close()
        {
            this.fileStream.Close();
        }

        public override void Flush()
        {
            this.fileStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var realLength = this.fileStream.Read(buffer, offset, count);

            // Check if we need to decrypt the readed data
            if (this.encryptionByte > 0)
            {
                for (int i = 0; i < realLength; i++)
                {
                    buffer[i] ^= this.encryptionByte;
                }
            }

            return realLength;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return this.fileStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }

}
