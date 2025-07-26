using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using System.Xml.Serialization;
using SRMod.DTOs;
using System.Drawing;

namespace SRMod.Services
{
    public class FileManager
    {
        public static string ExecPath
        {
            get
            {
                return Environment.CurrentDirectory;
            }
        }

        static public List<SerializableEnemyEntry> LoadEnemyDataXML(string fileName, string filePath = @"C:\Temp\")
        {
            string fileWithPath = fileName;

            if (!fileWithPath.Contains(@":") && !fileWithPath.Contains(@"\"))
            {
                if (filePath.EndsWith(@"\"))
                    fileWithPath = filePath + fileWithPath;
                else
                    fileWithPath = filePath + @"\" + fileWithPath;
            }

            List<SerializableEnemyEntry> list = new List<SerializableEnemyEntry>();

            // Create an instance of System.Xml.Serialization.XmlSerializer
            XmlSerializer serializer = new XmlSerializer(list.GetType());

            try
            {
                // Create an instance of System.IO.TextReader to load the serialized data
                using (TextReader textReader = new StreamReader(fileWithPath))
                {
                    // Assign the deserialized object to the list
                    list = (List<SerializableEnemyEntry>)serializer.Deserialize(textReader);
                }
            }
            catch (FileNotFoundException)
            {
                // Return empty list if file doesn't exist
                return list;
            }

            return list;
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

        static public void SaveAsXML<T>(T data, string fileName, string filePath = @"C:\Temp\")
        {
            // Create an instance of System.Xml.Serialization.XmlSerializer
            XmlSerializer serializer = new XmlSerializer(data.GetType());

            // Create an instance of System.IO.TextWriter 
            // to save the serialized object to disk
            using (TextWriter textWriter = new StreamWriter($@"{filePath}{fileName}"))
            {
                // Serialize the employeeList object
                serializer.Serialize(textWriter, data);
            }
        }

        static public void SaveAsXML(List<TranslationElementDTO> data, string fileName, string filePath = @"C:\Temp\")
        {
            TranslationsDTO list = new TranslationsDTO() { Translations = data };

            // Create an instance of System.Xml.Serialization.XmlSerializer
            XmlSerializer serializer = new XmlSerializer(list.GetType());

            // Create an instance of System.IO.TextWriter 
            // to save the serialized object to disk
            using (TextWriter textWriter = new StreamWriter($@"{filePath}{fileName}"))
            {
                // Serialize the employeeList object
                serializer.Serialize(textWriter, list);
            }
        }

        static public void SaveAsXML(List<SerializableItemData> data, string fileName, string filePath = @"C:\Temp\")
        {
            //ItemDataList list = new ItemDataList();

            //list.Items = data;

            // Create an instance of System.Xml.Serialization.XmlSerializer
            XmlSerializer serializer = new XmlSerializer(data.GetType());

            // Create an instance of System.IO.TextWriter 
            // to save the serialized object to disk
            using (TextWriter textWriter = new StreamWriter($@"{filePath}{fileName}"))
            {
                // Serialize the employeeList object
                serializer.Serialize(textWriter, data);
            }
        }

        static public List<TranslationElementDTO> LoadTranslationsXML(string fileName, string filePath = @"C:\Temp\")
        {
            string fileWithPath = fileName;

            if (!fileWithPath.Contains(@":") && !fileWithPath.Contains(@"\"))
            {
                if (filePath.EndsWith(@"\"))
                    fileWithPath = filePath + fileWithPath;
                else
                    fileWithPath = filePath + @"\" + fileWithPath;
            }

            TranslationsDTO list = new TranslationsDTO();

            // Create an instance of System.Xml.Serialization.XmlSerializer
            XmlSerializer serializer = new XmlSerializer(list.GetType());

            // Create an instance of System.IO.TextReader 
            // to load the serialized data from disk
            using (TextReader textReader = new StreamReader(fileWithPath))
            {
                // Assign the deserialized object to the new employeeList object
                list = (TranslationsDTO)serializer.Deserialize(textReader);
            }

            return list.Translations;
        }

        static public List<SerializableItemData> LoadItemDataXML(string fileName, string filePath = @"C:\Temp\")
        {
            string fileWithPath = fileName;

            if(!fileWithPath.Contains(@":") && !fileWithPath.Contains(@"\"))
            {
                if(filePath.EndsWith(@"\"))
                    fileWithPath = filePath + fileWithPath;
                else
                    fileWithPath = filePath + @"\" + fileWithPath;
            }

            List<SerializableItemData> list = new List<SerializableItemData>();

            // Create an instance of System.Xml.Serialization.XmlSerializer
            XmlSerializer serializer = new XmlSerializer(list.GetType());

            // Create an instance of System.IO.TextReader 
            // to load the serialized data from disk
            using (TextReader textReader = new StreamReader(fileWithPath))
            {
                // Assign the deserialized object to the new employeeList object
                list = (List<SerializableItemData>)serializer.Deserialize(textReader);
            }

            return list;
        }

        static public SerializableQuestManager LoadQuestDataXML(string fileName, string filePath = @"C:\Temp\")
        {
            string fileWithPath = fileName;

            if (!fileWithPath.Contains(@":") && !fileWithPath.Contains(@"\"))
            {
                if (filePath.EndsWith(@"\"))
                    fileWithPath = filePath + fileWithPath;
                else
                    fileWithPath = filePath + @"\" + fileWithPath;
            }

            SerializableQuestManager questManager = new SerializableQuestManager();

            if (!File.Exists(fileWithPath))
            {
                return questManager;
            }

            try
            {
                // Create an instance of System.Xml.Serialization.XmlSerializer
                XmlSerializer serializer = new XmlSerializer(typeof(SerializableQuestManager));

                // Create an instance of System.IO.TextReader to load the serialized data
                using (TextReader textReader = new StreamReader(fileWithPath))
                {
                    // Assign the deserialized object to the quest manager
                    questManager = (SerializableQuestManager)serializer.Deserialize(textReader);
                }
            }
            catch (Exception)
            {
                // Return empty quest manager if deserialization fails
                return new SerializableQuestManager();
            }

            return questManager;
        }

        public static string SaveTextureToFile(Texture2D texture)
        {
            string fileName = Manager.GetPluginManager().PluginPath + @"\" + texture.name + ".png";
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
                UnityEngine.Graphics.Blit(texture, rt);
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

            string filePath = ExecPath + @"\" + fileName;
            //string filePath = Manager.GetPluginManager().PluginPath + @"\" + fileName;

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

        // Load a texture image from its name
        public static Image LoadImageFromFile(string textureName)
        {
            SRInfoHelper.Log("Loading " + textureName);

            string filePath = ExecPath + @"\icons\" + textureName + ".png";
            //string filePath = Manager.GetPluginManager().PluginPath + @"\" + fileName;

            if (File.Exists(filePath))
            {
                return Image.FromFile(filePath);
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

        static public string Test(string saveText = "", string fileNameWithPath = @"C:\Temp\WriteLines.txt")
        {
            // Example #1: Write an array of strings to a file.
            // Create a string array that consists of three lines.
            List<string> lines = new List<string> { "First line", "Second line", "Third line" };

            if(!string.IsNullOrEmpty(saveText))
            {
                lines.Add(saveText);
            }

            // WriteAllLines creates a file, writes a collection of strings to the file,
            // and then closes the file.  You do NOT need to call Flush() or Close().
            //System.IO.File.WriteAllLines(@"C:\Users\Public\TestFolder\WriteLines.txt", lines);
            System.IO.File.WriteAllLines(fileNameWithPath, lines.ToArray());

            var linesList = lines.ToList();

            DirectoryInfo d = new DirectoryInfo(@".\");//Assuming Test is your Folder

            //DirectoryInfo d2 = new DirectoryInfo(@".\");//Assuming Test is your Folder

            //System.IO.File.WriteAllLines(d.FullName + "\test.txt", lines.ToArray());

            return d.FullName;
        }

        static public List<string> JSONSaver(System.Object obj)
        {
            List<string> output = new List<string>();

            try
            {
                string json = JsonUtility.ToJson(obj);
                output.Add(json);
                System.IO.File.WriteAllText(Manager.GetPluginManager().PluginPath + @"\" + obj.GetType().ToString(), json);
            }
            catch(Exception e)
            {
                output.Add(e.Message);
            }

            return output;
        }

        static public T JSONLoad<T>()
        {
            string json = System.IO.File.ReadAllText(Manager.GetPluginManager().PluginPath + @"\" + typeof(T));

            T obj = JsonUtility.FromJson<T>(json);

            return obj;
        }

        //static public string SaveObjectAsJson(object dataObject, string fileName = "data", string fileExtension = "json")
        //{
        //    //File.Create($@"{fileName}.{fileExtension}");
        //    // @"C:\Temp\WriteLines.txt"
        //    try
        //    {
        //        //using (StreamWriter file = File.CreateText($@"{fileName}.{fileExtension}"))
        //        using (StreamWriter file = File.CreateText(@"C:\Temp\WriteLines.txt"))
        //        {
        //            JsonSerializer serializer = new JsonSerializer();
        //            //serialize object directly into file stream
        //            serializer.Serialize(file, dataObject);
        //        }
        //        return "OK";
        //    }
        //    catch (Exception e)
        //    {
        //        return e.Message;
        //    }
        //}

        //static public T LoadFileAsClassObject<T>(string fileName)
        //{
        //    try
        //    {
        //        using (StreamReader file = new StreamReader($"{fileName}"))
        //        {
        //            string json = file.ReadToEnd();
        //            T arc = JsonConvert.DeserializeObject<T>(json);

        //            return arc;
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        return default(T);
        //        //throw e;
        //    }
        //}

        static public string[] GetJsonFileNames()
        {
            DirectoryInfo d = new DirectoryInfo(@".\");//Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles("*.json"); //Getting Text files

            return Files.Select(f => f.FullName).ToArray();
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

        static public SerializableQuestManager LoadQuestDataXML(string fileName, string filePath = @"C:\Temp\")
        {
            try
            {
                string fileWithPath;
                if (fileName.Contains(@":") && fileName.Contains(@"\"))
                {
                    fileWithPath = fileName;
                }
                else
                {
                    if (!filePath.EndsWith(@"\"))
                        filePath += @"\";
                    fileWithPath = filePath + fileName;
                }
                
                if (File.Exists(fileWithPath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(SerializableQuestManager));
                    using (StreamReader textReader = new StreamReader(fileWithPath))
                    {
                        var questData = (SerializableQuestManager)serializer.Deserialize(textReader);
                        return questData;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Failed to load quest data: {ex.Message}", "Error", 
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            return new SerializableQuestManager();
        }
    }
}
