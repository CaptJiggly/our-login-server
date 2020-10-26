using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Serialization.Formatters.Binary;
    public class PacketWriter : BinaryWriter
    {
        //This will hold our packet bytes.
        private MemoryStream _ms;
        //We will use this to serialize (Not in this tutorial)
        private BinaryFormatter _bf;

        public PacketWriter()
            : base()
        {
            //Initialize our variables
            _ms = new MemoryStream();
            _bf = new BinaryFormatter();
            //Set the stream of the underlying BinaryWriter to our memory stream.
            OutStream = _ms;
        }

        public void Write(Image image)
        {
            var ms = new MemoryStream(); //Create a memory stream to store our image bytes.

            image.Save(ms, ImageFormat.Png); //Save the image to the stream.

            ms.Close(); //Close the stream.

            byte[] imageBytes = ms.ToArray(); //Grab the bytes from the stream.

            //Write the image bytes to our memory stream
            //Length then bytes
            Write(imageBytes.Length);
            Write(imageBytes);
        }

        public void WriteT(object obj)
        {
            //We use the BinaryFormatter to serialize our object to the stream.
            _bf.Serialize(_ms, obj);
        }

        public byte[] GetBytes()
        {
            Close(); //Close the Stream. We no longer have need for it.

            byte[] data = _ms.ToArray(); //Grab the bytes and return.

            return data;
        }
    }

    public class PacketReader : BinaryReader
    {
        //This will be used for deserializing
        private BinaryFormatter _bf;
        public PacketReader(byte[] data)
            : base(new MemoryStream(data))
        {
            _bf = new BinaryFormatter();
        }

        public Image ReadImage()
        {
            //Read the length first as we wrote it.
            int len = ReadInt32();
            //Read the bytes
            byte[] bytes = ReadBytes(len);

            Image img; //This will hold the image.

            using (MemoryStream ms = new MemoryStream(bytes))
            {
                img = Image.FromStream(ms); //Get the image from the stream of the bytes.
            }

            return img; //Return the image.
        }

        public T ReadObject<T>()
        {
            //Use the BinaryFormatter to deserialize the object and return it casted as T
            /* MSDN Generics
             * http://msdn.microsoft.com/en-us/library/ms379564%28v=vs.80%29.aspx
             */
            return (T)_bf.Deserialize(BaseStream);
        }
    }
