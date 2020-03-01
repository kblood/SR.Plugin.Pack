using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using System.Xml.Serialization;
using LoadCustomDataMod.DTOs;

namespace LoadCustomDataMod.Services
{
    public class FileManager
    {
        public static string ExecPath
        {
            get
            {
                return Manager.GetPluginManager().PluginPath;
            }
        }

        public static bool SaveData(byte[] Data, string FileName = @"C:\temp\TestFileDoc.xml")
        {
            BinaryWriter Writer = null;
            string Name = @"C:\temp\TestFileDoc.xml";

            try
            {
                // Create a new stream to write to the file
                Writer = new BinaryWriter(File.OpenWrite(FileName));

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

        static public void SaveAsXML<T>(T data, string fileName)
        {
            string fileWithPath = FilePathCheck(fileName);

            // Create an instance of System.Xml.Serialization.XmlSerializer
            XmlSerializer serializer = new XmlSerializer(data.GetType());

            // Create an instance of System.IO.TextWriter 
            // to save the serialized object to disk
            using (TextWriter textWriter = new StreamWriter(fileWithPath))
            {
                // Serialize the employeeList object
                serializer.Serialize(textWriter, data);
            }
        }

        static public void SaveAsXML(List<TranslationElementDTO> data, string fileName)
        {
            string fileWithPath = FilePathCheck(fileName);

            TranslationsDTO list = new TranslationsDTO() { Translations = data };

            // Create an instance of System.Xml.Serialization.XmlSerializer
            XmlSerializer serializer = new XmlSerializer(list.GetType());

            // Create an instance of System.IO.TextWriter 
            // to save the serialized object to disk
            using (TextWriter textWriter = new StreamWriter(fileWithPath))
            {
                // Serialize the employeeList object
                serializer.Serialize(textWriter, list);
            }
        }

        static public void SaveAsXML(List<ItemData> data, string fileName)
        {
            string fileWithPath = FilePathCheck(fileName);

            ItemDataList list = new ItemDataList();

            list.Items = data;

            // Create an instance of System.Xml.Serialization.XmlSerializer
            XmlSerializer serializer = new XmlSerializer(list.GetType());

            // Create an instance of System.IO.TextWriter 
            // to save the serialized object to disk
            using (TextWriter textWriter = new StreamWriter(fileWithPath))
            {
                // Serialize the employeeList object
                serializer.Serialize(textWriter, list);
            }
        }

        static public List<TranslationElementDTO> LoadTranslationsXML(string fileName)
        {
            string fileWithPath = FilePathCheck(fileName);
            SRInfoHelper.Log("Loading " + fileWithPath);
            TranslationsDTO list = new TranslationsDTO();

            // Create an instance of System.Xml.Serialization.XmlSerializer
            XmlSerializer serializer = new XmlSerializer(list.GetType());

            // Create an instance of System.IO.TextReader 
            // to load the serialized data from disk
            if (File.Exists(fileWithPath))
            using (TextReader textReader = new StreamReader(fileWithPath))
            {
                // Assign the deserialized object to the new employeeList object
                list = (TranslationsDTO)serializer.Deserialize(textReader);
            }

            return list.Translations;
        }

        static public List<ItemData> LoadXML(string fileName)
        {
            string fileWithPath = FilePathCheck(fileName);
            SRInfoHelper.Log("Loading " + fileWithPath);
            ItemDataList list = new ItemDataList();

            // Create an instance of System.Xml.Serialization.XmlSerializer
            XmlSerializer serializer = new XmlSerializer(list.GetType());

            // Create an instance of System.IO.TextReader 
            // to load the serialized data from disk
            if (File.Exists(fileWithPath))
            using (TextReader textReader = new StreamReader(fileWithPath))
            {
                // Assign the deserialized object to the new employeeList object
                list = (ItemDataList)serializer.Deserialize(textReader);
            }

            return list.Items;
        }

        public static string SaveTextureToFile(Texture2D texture)
        {
            
            string fileName = FilePathCheck($@"icons\{texture.name}.png");
            if (File.Exists(fileName))
                return fileName;

            Color32[] pixelBlock = null;
            try
            {
                pixelBlock = texture.GetPixels32();
            }
            catch (UnityException _e)
            {
                texture.filterMode = FilterMode.Point;
                RenderTexture rt = RenderTexture.GetTemporary(texture.width, texture.height);
                rt.filterMode = FilterMode.Point;
                RenderTexture.active = rt;
                Graphics.Blit(texture, rt);
                Texture2D img2 = new Texture2D(texture.width, texture.height);
                img2.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
                img2.Apply();
                RenderTexture.active = null;
                texture = img2;
                pixelBlock = texture.GetPixels32();
            }

            var bytes = texture.EncodeToPNG();
            using (var file = File.Create(fileName))
            {
                var binary = new BinaryWriter(file);
                binary.Write(bytes);
            }
            return fileName;
        }

        public static Texture2D LoadTextureFromFile(string fileName)
        {
            SRInfoHelper.Log("Loading " + fileName);

            string filePath = FilePathCheck($@"icons\{fileName}");
            SRInfoHelper.Log("Loading " + filePath);

            if (File.Exists(filePath))
            {
                SRInfoHelper.Log("File " + fileName + " exists");
                Vector2Int imgSize = ImageHeader.GetDimensions(filePath);
                SRInfoHelper.Log("Image size " + imgSize.x + " "+ imgSize.y);

                var bytes = File.ReadAllBytes(filePath);
                SRInfoHelper.Log("Loading bytes into image");
                Texture2D tmpTexture = new Texture2D(imgSize.x, imgSize.y);
                tmpTexture.LoadImage(bytes);
                SRInfoHelper.Log("Loaded bytes into image");
                tmpTexture.name = fileName.Replace(".png", "");
                return tmpTexture;
                //Texture2D texture = new Texture2D(tmpTexture.width, tmpTexture.height);
                //SRInfoHelper.Log("Loading bytes into image");
                //texture.LoadImage(bytes);
                //return texture;
            }
            else
            {
                return null;
            }
        }

        static public void SaveText(string text, string fileName = @"WriteLines.txt")
        {
            List<string> someList = new List<string>();

            someList.Add(text);

            // WriteAllLines creates a file, writes a collection of strings to the file,
            // and then closes the file.  You do NOT need to call Flush() or Close().
            System.IO.File.WriteAllText(Manager.GetPluginManager().PluginPath + @"\" + fileName, text);
        }

        static public string SaveList(List<string> stringsToSave, string fileNameWithPath = @"C:\Temp\WriteLines.txt")
        {
            // WriteAllLines creates a file, writes a collection of strings to the file,
            // and then closes the file.  You do NOT need to call Flush() or Close().
            System.IO.File.WriteAllLines(fileNameWithPath, stringsToSave.ToArray());

            //DirectoryInfo d = new DirectoryInfo(@".\");//Assuming Test is your Folder

            return fileNameWithPath;
        }

        // ReadAllLines loads all lines of text from a file
        static public List<string> LoadList(string fileNameWithPath = @"C:\Temp\WriteLines.txt")
        {
            List<string> list = new List<string>();

            if (System.IO.File.Exists(fileNameWithPath))
                list = System.IO.File.ReadAllLines(fileNameWithPath).ToList();

            return list;
        }

        static public string FilePathCheck(string fileName)
        {
            string fileWithPath = fileName;

            if (!fileWithPath.Contains(@":") || !fileWithPath.Contains(@"\"))
            {
                if (ExecPath.EndsWith(@"\"))
                    fileWithPath = ExecPath + fileWithPath;
                else
                    fileWithPath = ExecPath + @"\" + fileWithPath;
            }
            return fileWithPath;
        }
    }
}
