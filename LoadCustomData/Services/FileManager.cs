using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using System.Xml;
using SRMod.DTOs;

namespace SRMod.Services
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

        public static bool SaveData(byte[] Data, string FileName)
        {
            if (FileName == null) FileName = "C:\\temp\\TestFileDoc.xml";
            return SaveDataInternal(Data, FileName);
        }
        
        public static bool SaveData(byte[] Data)
        {
            return SaveDataInternal(Data, "C:\\temp\\TestFileDoc.xml");
        }
        
        private static bool SaveDataInternal(byte[] Data, string FileName)
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
            
            try
            {
                // Use proper XML serialization for .NET Framework 4.5.1
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                using (var streamWriter = new StreamWriter(fileWithPath))
                {
                    serializer.Serialize(streamWriter, data);
                }
                SRInfoHelper.Log("Successfully saved XML file: " + fileWithPath);
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log("Error saving XML file " + fileWithPath + ": " + ex.Message);
                // Fallback to simple text serialization
                using (TextWriter textWriter = new StreamWriter(fileWithPath))
                {
                    textWriter.WriteLine("<" + data.GetType().Name + ">");
                    textWriter.WriteLine(data.ToString());
                    textWriter.WriteLine("</" + data.GetType().Name + ">");
                }
            }
        }

        //static public void SaveAsXML(List<ItemData> data, string fileName)
        //{
        //    string fileWithPath = FilePathCheck(fileName);

        //    ItemDataList list = new ItemDataList();

        //    list.Items = data;

        //    // Create an instance of System.Xml.Serialization.XmlSerializer
        //    XmlSerializer serializer = new XmlSerializer(list.GetType());

        //    // Create an instance of System.IO.TextWriter 
        //    // to save the serialized object to disk
        //    using (TextWriter textWriter = new StreamWriter(fileWithPath))
        //    {
        //        // Serialize the employeeList object
        //        serializer.Serialize(textWriter, list);
        //    }
        //}

        static public List<TranslationElementDTO> LoadTranslationsXML(string fileName)
        {
            string fileWithPath = FilePathCheck(fileName);
            SRInfoHelper.Log("Loading " + fileWithPath);
            
            if (!File.Exists(fileWithPath))
                return new List<TranslationElementDTO>();

            try
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<TranslationElementDTO>));
                using (var streamReader = new StreamReader(fileWithPath))
                {
                    return (List<TranslationElementDTO>)serializer.Deserialize(streamReader);
                }
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log("Error loading translations XML: " + ex.Message);
                return new List<TranslationElementDTO>();
            }
        }



        //static public List<ItemData> LoadXML(string fileName)
        //{
        //    string fileWithPath = FilePathCheck(fileName);
        //    SRInfoHelper.Log("Loading " + fileWithPath);
        //    ItemDataList list = new ItemDataList();

        //    // Create an instance of System.Xml.Serialization.XmlSerializer
        //    XmlSerializer serializer = new XmlSerializer(list.GetType());

        //    // Create an instance of System.IO.TextReader 
        //    // to load the serialized data from disk
        //    if (File.Exists(fileWithPath))
        //    using (TextReader textReader = new StreamReader(fileWithPath))
        //    {
        //        // Assign the deserialized object to the new employeeList object
        //        list = (ItemDataList)serializer.Deserialize(textReader);
        //    }

        //    return list.Items;
        //}

        public static string SaveTextureToFile(Texture2D texture)
        {
            string fileName = FilePathCheck(string.Format("icons\\{0}.png", texture.name));
            if (File.Exists(fileName))
                return fileName;

            try
            {
                var bytes = texture.EncodeToPNG();
                using (var file = File.Create(fileName))
                {
                    var binary = new BinaryWriter(file);
                    binary.Write(bytes);
                }
                return fileName;
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log("Error saving texture: " + ex.Message);
                return "";
            }
        }

        public static Texture2D LoadTextureFromFile(string textureName)
        {
            string filePath = FilePathCheck(string.Format("icons\\{0}.png", textureName));
            
            if (File.Exists(filePath))
            {
                try
                {
                    var bytes = File.ReadAllBytes(filePath);
                    Texture2D tmpTexture = new Texture2D(2, 2); // Will be resized by LoadImage
                    tmpTexture.LoadImage(bytes);
                    tmpTexture.name = textureName.Replace(".png", "");
                    return tmpTexture;
                }
                catch (Exception ex)
                {
                    SRInfoHelper.Log("Error loading texture: " + ex.Message);
                    return null;
                }
            }
            
            return null;
        }

        static public void SaveText(string text, string fileName)
        {
            if (fileName == null) fileName = "WriteLines.txt";
            SaveTextInternal(text, fileName);
        }
        
        static public void SaveText(string text)
        {
            SaveTextInternal(text, "WriteLines.txt");
        }
        
        static private void SaveTextInternal(string text, string fileName)
        {
            List<string> someList = new List<string>();

            someList.Add(text);

            // WriteAllLines creates a file, writes a collection of strings to the file,
            // and then closes the file.  You do NOT need to call Flush() or Close().
            System.IO.File.WriteAllText(Manager.GetPluginManager().PluginPath + @"\" + fileName, text);
        }

        static public string SaveList(List<string> stringsToSave, string fileNameWithPath)
        {
            if (fileNameWithPath == null) fileNameWithPath = "C:\\Temp\\WriteLines.txt";
            return SaveListInternal(stringsToSave, fileNameWithPath);
        }
        
        static public string SaveList(List<string> stringsToSave)
        {
            return SaveListInternal(stringsToSave, "C:\\Temp\\WriteLines.txt");
        }
        
        static private string SaveListInternal(List<string> stringsToSave, string fileNameWithPath)
        {
            // WriteAllLines creates a file, writes a collection of strings to the file,
            // and then closes the file.  You do NOT need to call Flush() or Close().
            System.IO.File.WriteAllLines(fileNameWithPath, stringsToSave.ToArray());

            //DirectoryInfo d = new DirectoryInfo(@".\");//Assuming Test is your Folder

            return fileNameWithPath;
        }

        // ReadAllLines loads all lines of text from a file
        static public List<string> LoadList(string fileNameWithPath)
        {
            if (fileNameWithPath == null) fileNameWithPath = "C:\\Temp\\WriteLines.txt";
            return LoadListInternal(fileNameWithPath);
        }
        
        static public List<string> LoadList()
        {
            return LoadListInternal("C:\\Temp\\WriteLines.txt");
        }
        
        static private List<string> LoadListInternal(string fileNameWithPath)
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

        internal static SerializableQuestManager LoadQuestDataXML(string fileName)
        {
            string fileWithPath = FilePathCheck(fileName);
            SRInfoHelper.Log("Loading " + fileWithPath);
            
            if (!File.Exists(fileWithPath))
                return null;

            try
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(SerializableQuestManager));
                using (var streamReader = new StreamReader(fileWithPath))
                {
                    return (SerializableQuestManager)serializer.Deserialize(streamReader);
                }
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log("Error loading quest data XML: " + ex.Message);
                return new SerializableQuestManager();
            }
        }
    }
}
