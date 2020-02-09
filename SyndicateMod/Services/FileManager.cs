using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

namespace SyndicateMod.Services
{
    public class FileManager
    {
        string fileName = "MyFile.txt";

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

        static public void SaveText(string text, string fileName = @"WriteLines.txt")
        {
            List<string> someList = new List<string>();

            someList.Add(text);

            // WriteAllLines creates a file, writes a collection of strings to the file,
            // and then closes the file.  You do NOT need to call Flush() or Close().
            //System.IO.File.WriteAllLines(@"C:\Users\Public\TestFolder\WriteLines.txt", lines);
            System.IO.File.WriteAllText(Manager.GetPluginManager().PluginPath + @"\" + fileName, text);

            //DirectoryInfo d = new DirectoryInfo(@".\");//Assuming Test is your Folder

            //DirectoryInfo d2 = new DirectoryInfo(@".\");//Assuming Test is your Folder

            //System.IO.File.WriteAllLines(d.FullName + "\test.txt", lines.ToArray());

            //return d.FullName;
        }

        static public string SaveList(List<string> stringsToSave, string fileNameWithPath = @"C:\Temp\WriteLines.txt")
        {
            // WriteAllLines creates a file, writes a collection of strings to the file,
            // and then closes the file.  You do NOT need to call Flush() or Close().
            //System.IO.File.WriteAllLines(@"C:\Users\Public\TestFolder\WriteLines.txt", lines);
            System.IO.File.WriteAllLines(fileNameWithPath, stringsToSave.ToArray());

            DirectoryInfo d = new DirectoryInfo(@".\");//Assuming Test is your Folder

            //DirectoryInfo d2 = new DirectoryInfo(@".\");//Assuming Test is your Folder

            //System.IO.File.WriteAllLines(d.FullName + "\test.txt", lines.ToArray());

            return d.FullName;
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
    }
}
