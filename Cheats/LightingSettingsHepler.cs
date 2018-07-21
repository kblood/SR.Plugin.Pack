//using System;
//using System.Reflection;
//using UnityEditor;
//using UnityEngine;
//using Object = UnityEngine.Object;


//public static class LightingSettingsHepler
//{
//    public static void SetIndirectResolution(float val)
//    {
//        SetFloat("m_LightmapEditorSettings.m_Resolution", val);
//    }

//    public static void SetAmbientOcclusion(float val)
//    {
//        SetFloat("m_LightmapEditorSettings.m_CompAOExponent", val);
//    }

//    public static void SetBakedGiEnabled(bool enabled)
//    {
//        SetBool("m_GISettings.m_EnableBakedLightmaps", enabled);
//    }

//    public static void SetFinalGatherEnabled(bool enabled)
//    {
//        SetBool("m_LightmapEditorSettings.m_FinalGather", enabled);
//    }

//    public static void SetFinalGatherRayCount(int val)
//    {
//        SetInt("m_LightmapEditorSettings.m_FinalGatherRayCount", val);
//    }

//    public static void SetFloat(string name, float val)
//    {
//        ChangeProperty(name, property => property.floatValue= val);
//    }

//    public static void SetInt(string name, int val)
//    {
//        ChangeProperty(name, property => property.intValue = val);
//    }

//    public static void SetBool(string name, bool val)
//    {
//        ChangeProperty(name, property => property.boolValue = val);
//    }

//    public static void ChangeProperty(string name, Action<SerializedProperty> changer)
//    {
//        var lightmapSettings = getLighmapSettings();
//        var prop = lightmapSettings.FindProperty(name);
//        if (prop != null)
//        {
//            changer(prop);
//            lightmapSettings.ApplyModifiedProperties();
//        }
//        else Debug.LogError("lighmap property not found: " + name);
//    }

//    static SerializedObject getLighmapSettings()
//    {
//        var getLightmapSettingsMethod = typeof(LightmapEditorSettings).GetMethod("GetLightmapSettings", BindingFlags.Static | BindingFlags.NonPublic);
//        var lightmapSettings = getLightmapSettingsMethod.Invoke(null, null) as Object;
//        return new SerializedObject(lightmapSettings);
//    }

//    public static void Test()
//    {
//        SetBakedGiEnabled(true);
//        SetIndirectResolution(1.337f);
//        SetAmbientOcclusion(1.337f);
//        SetFinalGatherEnabled(true);
//        SetFinalGatherRayCount(1337);
//    }
//}
