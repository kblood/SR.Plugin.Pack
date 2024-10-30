using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using SRMod.Services;

namespace SRMod.Services
{
    struct ObjMaterial
    {
        public string name;
        public string textureName;
    }

    public class MeshHelper
    {
        private static int vertexOffset = 0;
        private static int normalOffset = 0;
        private static int uvOffset = 0;

        //User should probably be able to change this. It is currently left as an excercise for
        //the reader.
        public static string targetFolder = "ExportedObj";

        public static Component[] GetMeshfiltersFromTransforms(GameObject[] gameObjects)
        {
            List<Component> meshfilter = new List<Component>();

            foreach (GameObject go in gameObjects)
            {
                meshfilter.AddRange(go.transform.GetComponentsInChildren<MeshFilter>());
            }

            return meshfilter.ToArray();
        }

        public static void ExportSelectionToSeparate(GameObject[] gameObjects)
        {
            targetFolder = targetFolder + Path.PathSeparator + "export";

            if (!CreateTargetFolder())
                return;

            int exportedObjects = 0;

            for (int i = 0; i < gameObjects.Length; i++)
            {
                Component[] meshfilter = gameObjects[i].GetComponentsInChildren(typeof(MeshFilter));

                for (int m = 0; m < meshfilter.Length; m++)
                {
                    exportedObjects++;
                    MeshToFile((MeshFilter)meshfilter[m], targetFolder, gameObjects[i].name + "_" + i + "_" + m);
                }
            }
        }

        public static void ExportSelectionToSeparate(Component[] meshfilters)
        {
            targetFolder = Path.Combine(targetFolder, "export");

            if (!CreateTargetFolder())
                return;

            int exportedObjects = 0;

            for (int m = 0; m < meshfilters.Length; m++)
            {
                exportedObjects++;
                MeshToFile((MeshFilter)meshfilters[m], targetFolder, meshfilters[m].gameObject.name + "_" + m);
            }
        }

        public static void ExportAllToSingle(Component[] meshfilter)
        {
            if (!CreateTargetFolder())
                return;

            //Component[] meshfilter = t.GetComponentsInChildren(typeof(MeshFilter));

            ArrayList mfList = new ArrayList();
            int exportedObjects = 0;

            for (int m = 0; m < meshfilter.Length; m++)
            {
                exportedObjects++;
                mfList.Add(meshfilter[m]);
            }

            if (exportedObjects > 0)
            {
                MeshFilter[] mf = new MeshFilter[mfList.Count];

                for (int i = 0; i < mfList.Count; i++)
                {
                    mf[i] = (MeshFilter)mfList[i];
                }

                string filename = meshfilter.First().gameObject.name + "_" + exportedObjects;

                int stripIndex = filename.LastIndexOf(Path.PathSeparator);

                if (stripIndex >= 0)
                    filename = filename.Substring(stripIndex + 1).Trim();

                MeshesToFile(mf, targetFolder, filename);

                SRInfoHelper.Log("Objects exported. Exported " + exportedObjects + " objects to " + filename);
            }
            else
                SRInfoHelper.Log("Objects not exported. Make sure at least some of your selected objects have mesh filters!");
        }

        public static string MeshToString(MeshFilter mf)
        {
            Mesh m = mf.mesh;
            Material[] mats = mf.gameObject.GetComponent<Renderer>().sharedMaterials;

            StringBuilder sb = new StringBuilder();

            sb.Append("g ").Append(mf.name).Append("\n");
            foreach (Vector3 v in m.vertices)
            {
                sb.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, v.z));
            }
            sb.Append("\n");
            foreach (Vector3 v in m.normals)
            {
                sb.Append(string.Format("vn {0} {1} {2}\n", v.x, v.y, v.z));
            }
            sb.Append("\n");
            foreach (Vector3 v in m.uv)
            {
                sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
            }
            for (int material = 0; material < m.subMeshCount; material++)
            {
                sb.Append("\n");
                sb.Append("usemtl ").Append(mats[material].name).Append("\n");
                sb.Append("usemap ").Append(mats[material].name).Append("\n");

                int[] triangles = m.GetTriangles(material);
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
                        triangles[i] + 1, triangles[i + 1] + 1, triangles[i + 2] + 1));
                }
            }
            return sb.ToString();
        }

        public static void MeshToFile(MeshFilter mf, string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.Write(MeshToString(mf));
            }
        }

        private static void MeshToFile(MeshFilter mf, string folder, string filename)
        {
            Dictionary<string, ObjMaterial> materialList = PrepareFileWrite();

            using (StreamWriter sw = new StreamWriter(folder + Path.PathSeparator + filename + ".obj"))
            {
                sw.Write("mtllib ./" + filename + ".mtl\n");

                sw.Write(MeshToString(mf, materialList));
            }

            MaterialsToFile(materialList, folder, filename);
        }

        private static void MeshesToFile(MeshFilter[] mf, string folder, string filename)
        {
            Dictionary<string, ObjMaterial> materialList = PrepareFileWrite();

            using (StreamWriter sw = new StreamWriter(folder + Path.PathSeparator + filename + ".obj"))
            {
                sw.Write("mtllib ./" + filename + ".mtl\n");

                for (int i = 0; i < mf.Length; i++)
                {
                    sw.Write(MeshToString(mf[i], materialList));
                }
            }

            MaterialsToFile(materialList, folder, filename);
        }

        private static bool CreateTargetFolder()
        {
            try
            {
                System.IO.Directory.CreateDirectory(targetFolder);
            }
            catch
            {

                return false;
            }

            return true;
        }

        private static void Clear()
        {
            vertexOffset = 0;
            normalOffset = 0;
            uvOffset = 0;
        }

        private static Dictionary<string, ObjMaterial> PrepareFileWrite()
        {
            Clear();

            return new Dictionary<string, ObjMaterial>();
        }

        private static string MeshToString(MeshFilter mf, Dictionary<string, ObjMaterial> materialList)
        {
            Mesh m = mf.sharedMesh;
            Material[] mats = mf.GetComponent<Renderer>().sharedMaterials;

            StringBuilder sb = new StringBuilder();

            sb.Append("g ").Append(mf.name).Append("\n");
            foreach (Vector3 lv in m.vertices)
            {
                Vector3 wv = mf.transform.TransformPoint(lv);

                //This is sort of ugly - inverting x-component since we're in
                //a different coordinate system than "everyone" is "used to".
                sb.Append(string.Format("v {0} {1} {2}\n", -wv.x, wv.y, wv.z));
            }
            sb.Append("\n");

            foreach (Vector3 lv in m.normals)
            {
                Vector3 wv = mf.transform.TransformDirection(lv);

                sb.Append(string.Format("vn {0} {1} {2}\n", -wv.x, wv.y, wv.z));
            }
            sb.Append("\n");

            foreach (Vector3 v in m.uv)
            {
                sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
            }

            for (int material = 0; material < m.subMeshCount; material++)
            {
                sb.Append("\n");
                sb.Append("usemtl ").Append(mats[material].name).Append("\n");
                sb.Append("usemap ").Append(mats[material].name).Append("\n");

                //See if this material is already in the materiallist.
                try
                {
                    ObjMaterial objMaterial = new ObjMaterial();

                    objMaterial.name = mats[material].name;

                    

                    if (mats[material].mainTexture)
                    {
                        objMaterial.textureName = mats[material].mainTexture.name; //AssetDatabase.GetAssetPath(mats[material].mainTexture);
                        
                        // Save the texture somehow? Add to some texture database?
                        Texture objTexture;
                        if(false)
                            objTexture = mats[material].GetTexture(objMaterial.textureName);
                    }
                    else
                        objMaterial.textureName = null;

                    materialList.Add(objMaterial.name, objMaterial);
                }
                catch (ArgumentException)
                {
                    //Already in the dictionary
                }


                int[] triangles = m.GetTriangles(material);
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    //Because we inverted the x-component, we also needed to alter the triangle winding.
                    sb.Append(string.Format("f {1}/{1}/{1} {0}/{0}/{0} {2}/{2}/{2}\n",
                        triangles[i] + 1 + vertexOffset, triangles[i + 1] + 1 + normalOffset, triangles[i + 2] + 1 + uvOffset));
                }
            }

            vertexOffset += m.vertices.Length;
            normalOffset += m.normals.Length;
            uvOffset += m.uv.Length;

            return sb.ToString();
        }

        private static void MaterialsToFile(Dictionary<string, ObjMaterial> materialList, string folder, string filename)
        {
            using (StreamWriter sw = new StreamWriter(folder + Path.PathSeparator + filename + ".mtl"))
            {
                foreach (KeyValuePair<string, ObjMaterial> kvp in materialList)
                {
                    sw.Write("\n");
                    sw.Write("newmtl {0}\n", kvp.Key);
                    sw.Write("Ka  0.6 0.6 0.6\n");
                    sw.Write("Kd  0.6 0.6 0.6\n");
                    sw.Write("Ks  0.9 0.9 0.9\n");
                    sw.Write("d  1.0\n");
                    sw.Write("Ns  0.0\n");
                    sw.Write("illum 2\n");

                    if (kvp.Value.textureName != null)
                    {
                        string destinationFile = kvp.Value.textureName;

                        int stripIndex = destinationFile.LastIndexOf(Path.PathSeparator);

                        if (stripIndex >= 0)
                            destinationFile = destinationFile.Substring(stripIndex + 1).Trim();

                        string relativeFile = destinationFile;

                        destinationFile = folder + Path.PathSeparator + destinationFile;

                        Debug.Log("Copying texture from " + kvp.Value.textureName + " to " + destinationFile);

                        try
                        {
                            //Copy the source file
                            File.Copy(kvp.Value.textureName, destinationFile);
                        }
                        catch
                        {

                        }
                        sw.Write("map_Kd {0}", relativeFile);
                    }
                    sw.Write("\n\n\n");
                }
            }
        }

    }
}