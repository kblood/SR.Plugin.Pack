using SyndicateMod.DTOs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using SystemLanguage = UnityEngine.SystemLanguage;
using Language = TextManager.Language;

namespace SyndicateMod.Services
{

    public class SRMapper
    {
        public static Dictionary<SystemLanguage, Language> SystemLangaugeMap;
        public static Dictionary<Language, CultureInfo> LanguageCultureInfo;
        public static Language[] NotLanguages;
        public static CultureInfo DefaultCultureInfo;

        public static ItemData Map(ItemManager.ItemData itemData)
        {
            var mappedData = new ItemData();

            return mappedData;
        }

        public static GameObjectDTO Map(UnityEngine.GameObject gameObject)
        {
            var mappedData = new GameObjectDTO();

            SRInfoHelper.Log("Mapping gameobject " + gameObject.name);

            var goDTO = SRMapper.ReflectionObjectBuilder<DTOs.GameObjectDTO>(gameObject);

            SRInfoHelper.Log("Mapping " + gameObject.name);


            List<UnityEngine.Component> components = new List<UnityEngine.Component>();
            components.AddRange(gameObject.transform.GetComponents(typeof(UnityEngine.Component)));

            goDTO.transform.Components = components;

            goDTO.transform.Children = new List<DTOs.GameObjectDTO>();
            foreach (UnityEngine.Transform t in gameObject.transform)
            {
                try
                {
                    goDTO.transform.Children.Add(Map(t.gameObject));
                }
                catch (Exception e)
                {
                    SRInfoHelper.Log("Exception thrown: " + e.Message + " Inner: "+ e.InnerException);

                }
            }

            return mappedData;
        }

        static public void LanguageMapper()
        {
            Dictionary<UnityEngine.SystemLanguage, Language> dictionary = new Dictionary<SystemLanguage, TextManager.Language>();
            dictionary.Add(SystemLanguage.English, Language.EN);
            dictionary.Add(SystemLanguage.Czech, Language.CZ);
            dictionary.Add(SystemLanguage.French, Language.FR);
            dictionary.Add(SystemLanguage.German, Language.GE);
            dictionary.Add(SystemLanguage.Italian, Language.IT);
            dictionary.Add(SystemLanguage.Russian, Language.RU);
            dictionary.Add(SystemLanguage.Spanish, Language.SP);

            DefaultCultureInfo = new CultureInfo("en-US");
            Dictionary<Language, CultureInfo> dictionary2 = new Dictionary<Language, CultureInfo>();
            dictionary2.Add(Language.EN, DefaultCultureInfo);
            dictionary2.Add(Language.CZ, new CultureInfo("cs-CZ"));
            dictionary2.Add(Language.FR, new CultureInfo("fr-FR"));
            dictionary2.Add(Language.GE, new CultureInfo("de-DE"));
            dictionary2.Add(Language.IT, new CultureInfo("it-IT"));
            dictionary2.Add(Language.RU, new CultureInfo("ru-RU"));
            dictionary2.Add(Language.SP, new CultureInfo("ru-RU"));
            LanguageCultureInfo = dictionary2;
            NotLanguages = new TextManager.Language[]
            {
                TextManager.Language.id,
                TextManager.Language.NOTES,
                TextManager.Language.MAX
            };
        }

        /// <summary>
        /// A generic object builder that takes the properties from the reader and puts into a
        /// new object of the same type as the destination object
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        static public T ReflectionObjectBuilder<T>(Object obj)
        {
            //Type sourceType = source.GetType();
            Type destinationType = typeof(T);

            T result = (T)Activator.CreateInstance(destinationType);

            // columns is a list of attribute names in the reader
            var sourceFields = obj.GetType().GetFields();
            var sourceProps = obj.GetType().GetProperties();

            var destProps = destinationType.GetProperties();
            var destFields = destinationType.GetFields();

            SRInfoHelper.Log("Mapping to " + destinationType +" : "+ destProps.Count() + " and mapping from " + obj.GetType() + " " + sourceProps.Count());

            // We get properties from the destination type instead of the source type
            foreach (PropertyInfo pi in destProps)
            {
                // set pDest to the currently instanced property type
                PropertyInfo pDest = destinationType.GetProperty(pi.Name);
                var pSource = sourceProps.Where(s => s.Name == pi.Name).FirstOrDefault();
                var fSource = sourceFields.Where(s => s.Name == pi.Name).FirstOrDefault();

                if(pSource == null && fSource == null)
                {
                    SRInfoHelper.Log("No source found for propety " + pi.Name + "Skipping");
                    continue;
                }

                if (pSource != null)
                    SRInfoHelper.Log("Property name " + pi.Name + " destination property name " + pSource?.Name);
                else
                    SRInfoHelper.Log("Property name " + pi.Name + " destination field name " + fSource?.Name);

                if (pDest != null && pDest.CanWrite)
                {
                    if(pDest.PropertyType.FullName == "SyndicateMod.DTOs.Sprite")
                    {
                        //SRInfoHelper.Log("Mapping sprite");
                        DTOs.Sprite destSprite = (DTOs.Sprite)pDest.GetValue(result, null);
                        UnityEngine.Sprite sourceSprite = new UnityEngine.Sprite();

                        if (pSource != null)
                            sourceSprite = (UnityEngine.Sprite)pSource.GetValue(obj, null);
                        else if (fSource != null)
                            sourceSprite = (UnityEngine.Sprite)fSource.GetValue(obj);
                        else
                            continue;

                        if (sourceSprite == null || sourceSprite.name == null || sourceSprite.name == "")
                            continue;

                        SRInfoHelper.Log("Mapping sprite " + sourceSprite.name);

                        DTOs.Sprite mappedSprite = ReflectionObjectBuilder<DTOs.Sprite>(sourceSprite);

                        //SRInfoHelper.Log("Mapped sprite");

                        if (sourceSprite.texture != null)
                        {
                            FileManager.SaveTextureToFile(sourceSprite.texture);
                            mappedSprite.textureName = sourceSprite.texture.name + ".png";
                        }
                        if (sourceSprite.associatedAlphaSplitTexture != null)
                        {
                            FileManager.SaveTextureToFile(sourceSprite.associatedAlphaSplitTexture);
                            mappedSprite.associatedAlphaSplitTextureName = sourceSprite.associatedAlphaSplitTexture.name + ".png";
                        }
                        SRInfoHelper.Log("Saved textures");

                        pDest.SetValue(result, mappedSprite, null);
                        continue;
                    }

                    bool isCollection = pDest.PropertyType.GetInterfaces()
                        .Any(x => x == typeof(IEnumerable));

                    if (isCollection && pDest.PropertyType.FullName != "System.String")
                    {
                        Type itemType = pDest.PropertyType;
                        if (itemType.IsArray)
                            itemType = itemType.GetElementType();
                        //if (pSource.FieldType.GetGenericArguments().Any())
                        //    itemType = pSource.FieldType.GetGenericArguments().FirstOrDefault();
                        
                        SRInfoHelper.Log("Found collection " + pDest.PropertyType + " named " + pDest.Name + " namespace is " + itemType.FullName);

                        if (itemType.FullName.Contains("Syndicate"))
                        {
                            List<DTOs.ModifierData5L> modifiers = new List<DTOs.ModifierData5L>();

                            if(fSource != null)
                                foreach (var item in (IEnumerable)fSource.GetValue(obj))
                                {
                                    var mappedItem = ReflectionObjectBuilder<DTOs.ModifierData5L>(item);
                                    modifiers.Add(mappedItem);
                                }
                            else if(pSource != null)
                                foreach (var item in (IEnumerable)pSource.GetValue(obj, null))
                                {
                                    var mappedItem = ReflectionObjectBuilder<DTOs.ModifierData5L>(item);
                                    modifiers.Add(mappedItem);
                                }
                            result.SetMemberValue(pi.Name, modifiers.ToArray());
                        }
                        else if (itemType.FullName == "ModifierData5L")
                        {
                            List<ModifierData5L> modifiers = new List<ModifierData5L>();

                            if (fSource != null)
                                foreach (var item in (IEnumerable)fSource.GetValue(obj))
                                {
                                    var mappedItem = ReflectionObjectBuilder<ModifierData5L>(item);
                                    modifiers.Add(mappedItem);
                                }
                            else
                                foreach (var item in (IEnumerable)pSource.GetValue(obj, null))
                                {
                                    var mappedItem = ReflectionObjectBuilder<ModifierData5L>(item);
                                    modifiers.Add(mappedItem);
                                }
                            result.SetMemberValue(pi.Name, modifiers.ToArray());
                        }
                        else
                        {
                            SRInfoHelper.Log("Processing collection " + pDest.PropertyType);
                            if (fSource != null)
                                result.SetMemberValue(pi.Name, fSource.GetValue(obj));
                            else if (pSource != null)
                                result.SetMemberValue(pi.Name, pSource.GetValue(obj, null));
                        }
                        //result.SetMemberValue(pi.Name, obj.GetMemberValue(pi.Name));
                    }
                    // Make sure the current property name can be found in the reader
                    else
                    {
                        string value = "";
                        string type = "";
                        if(pSource != null)
                        {
                            type = pSource.PropertyType.FullName;
                            value = pSource.GetValue(obj, null).ToString();
                            SRInfoHelper.Log("Found " + type + " named " + pDest.Name + " with value " + value);

                            result.SetMemberValue(pi.Name, pSource.GetValue(obj, null));
                        }
                        else if(fSource != null)
                        {
                            type = fSource.FieldType.FullName;
                            value = fSource.GetValue(obj).ToString();
                            SRInfoHelper.Log("Found " + type + " named " + pDest.Name + " with value " + value);

                            result.SetMemberValue(pi.Name, fSource.GetValue(obj));
                        }
                    }
                }
            }

            // We get properties from the destination type instead of the source type
            foreach (FieldInfo pi in destFields)
            {
                // set pDest to the currently instanced property type
                FieldInfo fDest = destinationType.GetField(pi.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                var pSource = sourceProps.Where(s => s.Name == pi.Name).FirstOrDefault();
                var fSource = sourceFields.Where(s => s.Name == pi.Name).FirstOrDefault();

                if (pSource != null)
                    SRInfoHelper.Log("Field name " + pi.Name + " destination name " + pSource?.Name);
                else
                    SRInfoHelper.Log("Field name " + pi.Name + " destination name " + fSource?.Name);

                if (pSource == null && fSource == null)
                {
                    SRInfoHelper.Log("No source found for field " + pi.Name + "Skipping");
                    continue;
                }

                if (fDest != null && fDest.IsPublic)
                {
                    if (fDest.FieldType.FullName == "UnityEngine.Sprite")
                    {
                        //SRInfoHelper.Log("Mapping sprite");
                        UnityEngine.Sprite destSprite = (UnityEngine.Sprite)fDest.GetValue(result);
                        Sprite sourceSprite = new Sprite();

                        if (pSource != null)
                            sourceSprite = (Sprite)pSource.GetValue(obj, null);
                        else if (fSource != null)
                            sourceSprite = (Sprite)fSource.GetValue(obj);
                        else
                            continue;

                        SRInfoHelper.Log("Mapping sprite " + sourceSprite);
                        if (sourceSprite == null || string.IsNullOrEmpty(sourceSprite.textureName))
                            continue;

                        UnityEngine.Sprite mappedSprite = ReflectionObjectBuilder<UnityEngine.Sprite>(sourceSprite);

                        SRInfoHelper.Log("Loading textures");

                        UnityEngine.Texture2D texture = null;
                        UnityEngine.Texture2D associatedAlphaSplitTexture = null;

                        if (!string.IsNullOrEmpty(sourceSprite.textureName))
                        {
                            texture = FileManager.LoadTextureFromFile(sourceSprite.textureName);

                            SRInfoHelper.Log("Loaded texture " + texture.name + " size " + texture.height + "x" + texture.width);
                        }
                        if (!string.IsNullOrEmpty(sourceSprite.associatedAlphaSplitTextureName))
                        {
                            associatedAlphaSplitTexture = FileManager.LoadTextureFromFile(sourceSprite.associatedAlphaSplitTextureName);
                            //mappedSprite.associatedAlphaSplitTexture.Resize(texture.width, texture.height);
                            SRInfoHelper.Log("Loaded alpha split texture " + associatedAlphaSplitTexture.name + " size " + associatedAlphaSplitTexture.height + "x" + associatedAlphaSplitTexture.width);

                            //mappedSprite.associatedAlphaSplitTexture.SetPixels(associatedAlphaSplitTexture.GetPixels());
                        }
                        //SRInfoHelper.Log("Creating Texture " + mappedSprite.rect + " " +mappedSprite.pivot);

                        var createdSprite = UnityEngine.Sprite.Create(texture, new UnityEngine.Rect(0, 0, texture.width, texture.height), new UnityEngine.Vector2(16, 16), 100);
                        //createdSprite.textureRect.position = mappedSprite.textureRect.position;
                        SRInfoHelper.Log("Loaded textures");

                        fDest.SetValue(result, createdSprite);
                        continue;
                    }

                    bool isCollection = fDest.FieldType.GetInterfaces()
                        .Any(x => x == typeof(IEnumerable));

                    if (isCollection && fDest.FieldType.FullName != "System.String")
                    {
                        Type itemType = fDest.FieldType;
                        if (itemType.IsArray)
                            itemType = itemType.GetElementType();
                        //if (pSource.FieldType.GetGenericArguments().Any())
                        //    itemType = pSource.FieldType.GetGenericArguments().FirstOrDefault();

                        SRInfoHelper.Log("Found collection " + fDest.FieldType + " named " + fDest.Name + " namespace is " + itemType.FullName);

                        if (itemType.FullName.Contains("Syndicate"))
                        {
                            List<DTOs.ModifierData5L> modifiers = new List<DTOs.ModifierData5L>();

                            if (fSource != null)
                                foreach (var item in (IEnumerable)fSource.GetValue(obj))
                                {
                                    var mappedItem = ReflectionObjectBuilder<DTOs.ModifierData5L>(item);
                                    modifiers.Add(mappedItem);
                                }
                            else
                                foreach (var item in (IEnumerable)pSource.GetValue(obj, null))
                                {
                                    var mappedItem = ReflectionObjectBuilder<DTOs.ModifierData5L>(item);
                                    modifiers.Add(mappedItem);
                                }

                            fDest.SetValue(result, modifiers.ToArray());
                        }
                        else if(itemType.FullName == "ModifierData5L")
                        {
                            List<ModifierData5L> modifiers = new List<ModifierData5L>();

                            if (fSource != null)
                                foreach (var item in (IEnumerable)fSource.GetValue(obj))
                                {
                                    var mappedItem = ReflectionObjectBuilder<ModifierData5L>(item);
                                    modifiers.Add(mappedItem);
                                }
                            else
                                foreach (var item in (IEnumerable)pSource.GetValue(obj, null))
                                {
                                    var mappedItem = ReflectionObjectBuilder<ModifierData5L>(item);
                                    modifiers.Add(mappedItem);
                                }
                            fDest.SetValue(result, modifiers.ToArray());
                            //result.SetMemberValue(pi.Name, modifiers.ToArray());
                        }
                        else
                        {
                            if(pSource != null)
                                fDest.SetValue(result, pSource.GetValue(obj, null));
                            else if (fSource != null)
                                fDest.SetValue(result, fSource.GetValue(obj));
                        }
                        //result.SetMemberValue(pi.Name, obj.GetMemberValue(pi.Name));
                    }
                    // Make sure the current property name can be found in the reader
                    else
                    {
                        string value;
                        string type;
                        if (pSource != null)
                        {
                            type = pSource.PropertyType.FullName;
                            value = pSource.GetValue(obj, null).ToString();
                            SRInfoHelper.Log("Found type of " + type + " named " + fDest.Name + " with value " + value);

                            fDest.SetValue(result, pSource.GetValue(obj, null));
                        }
                        else if (fSource != null)
                        {
                            type = fSource.FieldType.FullName;
                            value = fSource.GetValue(obj).ToString();
                            SRInfoHelper.Log("Found " + type + " named " + fDest.Name + " with value " + value);
                            result.SetMemberValue(pi.Name, fSource.GetValue(obj));
                        }
                    }
                }
            }

            return result;
        }
    }
}
