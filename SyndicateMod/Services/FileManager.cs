using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SyndicateMod.Services
{
    public class FileManager
    {
        string fileName = "MyFile.txt";

        public bool SaveData(string FileName, byte[] Data)
        {
            BinaryWriter Writer = null;
            string Name = @"C:\temp\yourfile.name";

            try
            {
                // Create a new stream to write to the file
                Writer = new BinaryWriter(File.OpenWrite(Name));

                // Writer raw data                
                Writer.Write(Data);
                Writer.Flush();
                Writer.Close();
            }
            catch
            {
                //...
                return false;
            }

            return true;
        }

        /// <summary>
        /// Convert the Binary AnyFile to Byte[] format
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        //public static byte[] ConvertANYFileToBytes(file image)
        //{
        //    byte[] imageBytes = null;
        //    BinaryReader reader = new BinaryReader(image.InputStream);
        //    imageBytes = reader.ReadBytes((int)image.ContentLength);
        //    return imageBytes;
        //}
    }
}
