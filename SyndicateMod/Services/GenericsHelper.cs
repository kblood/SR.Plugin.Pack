using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;
using System.Collections;
using UnityEngine.Events;
using UnityEngine;
using Object = System.Object;

namespace SyndicateMod.Services
{
    public class GenericsHelper
    {
        public static Dictionary<string, Type> GetNamesAndTypes(Transform transform)
        {
            // get all public static properties of MyClass type
            PropertyInfo[] propertyInfos;
            Dictionary<string, Type> propertyNamesAndTypes = new Dictionary<string, Type>();

            List<string> output = new List<string>();

            TryGetStringList(ref output, 
                delegate
                {
                    output.Add("Getting properties from transform " + transform.name);

                    propertyInfos = transform.GetType().GetProperties();

                    foreach (var propertyInfo in propertyInfos)
                    {
                        //if (propertyInfo.GetGetMethod.GetParameters() != null && propertyInfo.GetMethod.GetParameters().Length > 0)
                        //    continue;

                        Type currentType = propertyInfo.PropertyType;

                        output.Add("Checking property " + propertyInfo.Name + " that is class type " + currentType);

                        if(propertyInfo.Name == "gameObject" || propertyInfo.Name == "transform" || propertyInfo.Name =="parent" || propertyInfo.Name == "root" || currentType == transform.GetType())
                        {
                            output[output.Count - 1] += ". Skipping recursive property.";
                            continue;
                        }

                        if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            currentType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
                        }

                        if (!currentType.Namespace.StartsWith("System"))
                        {
                            if (currentType.IsEnum)
                            {
                                propertyNamesAndTypes.Add(propertyInfo.Name, typeof(int));
                            }
                            else if (currentType.IsValueType && !currentType.IsEnum)
                            {
                                propertyNamesAndTypes.Add(propertyInfo.Name, typeof(string));
                            }
                            else
                            {
                                Dictionary<string, Type> subNamesAndTypes = GetNamesAndTypes(currentType).ToDictionary(p => propertyInfo.Name + "_" + p.Key, p => p.Value); // .Select(n => (propertyInfo.Name + "_" + n)).ToArray();
                                foreach (var d in subNamesAndTypes)
                                    propertyNamesAndTypes.Add(d.Key, d.Value);
                            }
                        }
                        else
                        {
                            propertyNamesAndTypes.Add(propertyInfo.Name, currentType);
                        }
                    }
                });

            // sort properties by name
            //Array.Sort(propertyInfos,
            //        delegate (PropertyInfo propertyInfo1, PropertyInfo propertyInfo2)
            //        { return propertyInfo1.Name.CompareTo(propertyInfo2.Name); });

            FileManager.SaveList(output, Manager.GetPluginManager().PluginPath + $@"\TransformInfo_{transform.name}.txt");

            return propertyNamesAndTypes;
        }

        public static void TryGetStringList(ref List<string> output, UnityAction action)
        {
            try
            {
                action.Invoke(); 
            }
            catch (Exception e)
            {
                output.Add(e.Message);
            }
        }

        public static Dictionary<string, string> GetNamesAndValuesAsString(Type type, System.Object obj)
        {
            SyndicateMod.ShowMessage("Test message");

            // get all public static properties of MyClass type
            PropertyInfo[] propertyInfos;

            propertyInfos = type.GetProperties();

            Dictionary<string, string> propertyValues = new Dictionary<string, string>();
            //List<object> propertyValues = new List<object>();

            SyndicateMod.ShowMessage("Test message 2 " + type.Name);

            SyndicateMod.ShowMessage("Test message 2.5 " + type.Name + " object" + (obj == null ? "is null" : "is not null"));
            SyndicateMod.ShowMessage("Test message 2.5 " + type.Name + " object" + (obj == null ? "is null" : "is not null") + " properties: " + propertyInfos.Count());

            foreach (var propertyInfo in propertyInfos)
            {
                SyndicateMod.ShowMessage("Test message 3. Property: ");
                //SyndicateMod.Get().setEntityInfo("Show Message", "Test message 3. Property: ");

                //if (propertyInfo.GetMethod.GetParameters() != null && propertyInfo.GetMethod.GetParameters().Length > 0)
                //    continue;
                if (propertyInfo.GetIndexParameters().Any())
                {
                    SyndicateMod.ShowMessage("Has parameters? ");
                    
                    SyndicateMod.ShowMessage("Has parameters : " + propertyInfo.GetIndexParameters().Count());

                    continue;
                }

                Type currentType = propertyInfo.PropertyType;
                if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    SyndicateMod.ShowMessage("Is Nullabe?");

                    currentType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);

                    SyndicateMod.ShowMessage("Underlying type is " + currentType.ToString());

                }

                if (!currentType.Namespace.StartsWith("System"))
                {
                    SyndicateMod.ShowMessage("Custom type? " + currentType.ToString());

                    if (currentType.IsEnum)
                    {
                        SyndicateMod.ShowMessage("Enum type");

                        if (propertyInfo.GetValue(obj, null) == null)
                            propertyValues.Add(propertyInfo.Name, "Null");
                        else
                        {
                            string val = propertyInfo.GetValue(obj, null).ToString();
                            propertyValues.Add(propertyInfo.Name, val);
                        }
                    }
                    else if (currentType.IsValueType && !currentType.IsEnum)
                    {
                        if (propertyInfo.GetValue(obj, null) == null)
                            propertyValues.Add(propertyInfo.Name, "Null");
                        else
                        {
                            string val = propertyInfo.GetValue(obj, null).ToString();
                            propertyValues.Add(propertyInfo.Name, val);
                        }
                    }
                    else
                    {
                        SyndicateMod.ShowMessage("Getting subtypes");

                        Dictionary<string, string> subValues = GetNamesAndValuesAsString(propertyInfo.PropertyType, propertyInfo.GetValue(obj, null));

                        foreach (var value in subValues)
                        {
                            if (value.Value == null)
                                propertyValues.Add(propertyInfo.Name + "_" + value.Key, "Null");
                            else
                                propertyValues.Add(propertyInfo.Name + "_" + value.Key, value.Value);

                        }
                        //propertyValues.AddRange(propertyInfo.Name, subValues);
                    }
                }
                else
                {
                    if (obj != null)
                    {
                        if(propertyInfo.PropertyType.GetInterface("IEnumerable`1") != null) // "IEnumerable`1"
                        {
                            FileManager.SaveText("Enumerating list " + propertyInfo.Name, "EnumeratingUpdate");
                            var collection = (IEnumerable)propertyInfo.GetValue(obj, null);

                            Dictionary<string, string> subValues = new Dictionary<string, string>();

                            int i = 0;
                            foreach(var x in collection)
                            {
                                foreach (var value2 in GetNamesAndValuesAsString(x.GetType(), x))
                                {
                                    if (value2.Value == null)
                                        propertyValues.Add(propertyInfo.Name + "_Collection_" + i+ "_" + value2.Key, "Null");
                                    else
                                        propertyValues.Add(propertyInfo.Name + "_Collection_" + i + "_" + value2.Key, value2.Value);
                                }
                                i++;
                            }
                        }
                        else
                        {
                            SyndicateMod.ShowMessage("Adding value to value list for " + propertyInfo.Name);

                            var value = propertyInfo.GetValue(obj, null);
                            if (value == null || (propertyInfo.PropertyType == typeof(DateTime) && value.ToString() == DateTime.MinValue.ToString()))
                                propertyValues.Add(propertyInfo.Name, "Null");
                            else
                                propertyValues.Add(propertyInfo.Name, value.ToString());
                        }
                    }
                    else
                    {
                        propertyValues.Add(propertyInfo.Name, "Null");
                    }
                }
            }
            return propertyValues;
        }

        public static Dictionary<string, Type> GetNamesAndTypes(Type type)
        {
            // get all public static properties of MyClass type
            PropertyInfo[] propertyInfos;

            propertyInfos = type.GetProperties();

            // sort properties by name
            //Array.Sort(propertyInfos,
            //        delegate (PropertyInfo propertyInfo1, PropertyInfo propertyInfo2)
            //        { return propertyInfo1.Name.CompareTo(propertyInfo2.Name); });

            Dictionary<string, Type> propertyNamesAndTypes = new Dictionary<string, Type>();

            foreach (var propertyInfo in propertyInfos)
            {
                //if (propertyInfo.GetGetMethod.GetParameters() != null && propertyInfo.GetMethod.GetParameters().Length > 0)
                //    continue;

                Type currentType = propertyInfo.PropertyType;
                if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    currentType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
                }

                if (propertyInfo.Name == "gameObject" || propertyInfo.Name == "transform" || propertyInfo.Name == "parent" || propertyInfo.Name == "root")
                {
                    continue;
                }

                if (!currentType.Namespace.StartsWith("System"))
                {
                    if (currentType.IsEnum)
                    {
                        propertyNamesAndTypes.Add(propertyInfo.Name, typeof(int));
                    }
                    else if (currentType.IsValueType && !currentType.IsEnum)
                    {
                        propertyNamesAndTypes.Add(propertyInfo.Name, typeof(string));
                    }
                    else
                    {
                        Dictionary<string, Type> subNamesAndTypes = GetNamesAndTypes(currentType).ToDictionary(p => propertyInfo.Name + "_" + p.Key, p => p.Value); // .Select(n => (propertyInfo.Name + "_" + n)).ToArray();
                        foreach (var d in subNamesAndTypes)
                            propertyNamesAndTypes.Add(d.Key, d.Value);
                    }
                }
                else
                {
                    propertyNamesAndTypes.Add(propertyInfo.Name, currentType);
                }
            }

            return propertyNamesAndTypes;
        }

        public static Dictionary<string, Object> GetNamesAndValues(Type type, Object obj)
        {
            // get all public static properties of MyClass type
            PropertyInfo[] propertyInfos;

            propertyInfos = type.GetProperties();

            Dictionary<string, Object> propertyValues = new Dictionary<string, object>();
            //List<object> propertyValues = new List<object>();

            foreach (var propertyInfo in propertyInfos)
            {
                //if (propertyInfo.GetMethod.GetParameters() != null && propertyInfo.GetMethod.GetParameters().Length > 0)
                //    continue;

                Type currentType = propertyInfo.PropertyType;
                if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    currentType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
                }

                if (!currentType.Namespace.StartsWith("System"))
                {
                    if (currentType.IsEnum)
                    {
                        if (propertyInfo.GetValue(obj, null) == null)
                            propertyValues.Add(propertyInfo.Name, Convert.DBNull);
                        else
                        {
                            int val = (int)propertyInfo.GetValue(obj, null);
                            propertyValues.Add(propertyInfo.Name, val);
                        }
                    }
                    else if (currentType.IsValueType && !currentType.IsEnum)
                    {
                        if (propertyInfo.GetValue(obj, null) == null)
                            propertyValues.Add(propertyInfo.Name, Convert.DBNull);
                        else
                        {
                            string val = propertyInfo.GetValue(obj, null).ToString();
                            propertyValues.Add(propertyInfo.Name, val);
                        }
                    }
                    else
                    {
                        Dictionary<string, Object> subValues = GetNamesAndValues(propertyInfo.PropertyType, propertyInfo.GetValue(obj, null));

                        foreach (var value in subValues)
                        {
                            if (value.Value == null)
                                propertyValues.Add(propertyInfo.Name + "_" + value.Key, Convert.DBNull);
                            else
                                propertyValues.Add(propertyInfo.Name + "_" + value.Key, value.Value);

                        }
                        //propertyValues.AddRange(propertyInfo.Name, subValues);
                    }
                }
                else
                {
                    if (obj != null)
                    {
                        var value = propertyInfo.GetValue(obj, null);
                        if (value == null || (propertyInfo.PropertyType == typeof(DateTime) && value.ToString() == DateTime.MinValue.ToString()))
                            propertyValues.Add(propertyInfo.Name, Convert.DBNull);
                        else
                            propertyValues.Add(propertyInfo.Name, value);
                    }
                    else
                    {
                        propertyValues.Add(propertyInfo.Name, Convert.DBNull);
                    }
                }
            }
            return propertyValues;
        }

        public static Dictionary<string, Type> GetNamesAndTypes(Type type, List<string> namespaceStartsWith)
        {
            // get all public static properties of MyClass type
            PropertyInfo[] propertyInfos;

            propertyInfos = type.GetProperties();

            // sort properties by name
            //Array.Sort(propertyInfos,
            //        delegate (PropertyInfo propertyInfo1, PropertyInfo propertyInfo2)
            //        { return propertyInfo1.Name.CompareTo(propertyInfo2.Name); });

            Dictionary<string, Type> propertyNamesAndTypes = new Dictionary<string, Type>();

            foreach (var propertyInfo in propertyInfos)
            {
                //if (propertyInfo.GetGetMethod.GetParameters() != null && propertyInfo.GetMethod.GetParameters().Length > 0)
                //    continue;

                Type currentType = propertyInfo.PropertyType;
                if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    currentType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
                }

                if (namespaceStartsWith.Where(ns => currentType.Namespace.StartsWith(ns)).Any() || currentType.Namespace.StartsWith("RPSOAPServiceReference"))
                {
                    if (currentType.IsEnum)
                    {
                        propertyNamesAndTypes.Add(propertyInfo.Name, typeof(int));
                    }
                    else if (currentType.IsValueType && !currentType.IsEnum)
                    {
                        propertyNamesAndTypes.Add(propertyInfo.Name, typeof(string));
                    }
                    else
                    {
                        Dictionary<string, Type> subNamesAndTypes = GetNamesAndTypes(currentType, namespaceStartsWith).ToDictionary(p => propertyInfo.Name + "_" + p.Key, p => p.Value); // .Select(n => (propertyInfo.Name + "_" + n)).ToArray();

                        //propertyNamesAndTypes.Concat(subNamesAndTypes).GroupBy(kvp => kvp.Key, propertyNamesAndTypes.Comparer).ToDictionary(grp => grp.Key, grp => grp.First(), propertyNamesAndTypes.Comparer);
                        //propertyNamesAndTypes.Union(subNamesAndTypes)
                        //    .GroupBy(kvp => kvp.Key)
                        //    .Select(grp => grp.FirstOrDefault())
                        //    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                        foreach (var d in subNamesAndTypes)
                            propertyNamesAndTypes.Add(d.Key, d.Value);
                        //propertyNamesAndTypes.AddRange(subNamesAndTypes);
                    }
                }
                else
                {
                    propertyNamesAndTypes.Add(propertyInfo.Name, currentType);
                }
            }

            return propertyNamesAndTypes;
        }

        public static Dictionary<string, Object> GetNamesAndValues(Type type, Object obj, List<string> namespaceStartsWith)
        {
            // get all public static properties of MyClass type
            PropertyInfo[] propertyInfos;

            propertyInfos = type.GetProperties();

            Dictionary<string, Object> propertyValues = new Dictionary<string, object>();
            //List<object> propertyValues = new List<object>();

            foreach (var propertyInfo in propertyInfos)
            {
                //if (propertyInfo.GetMethod.GetParameters() != null && propertyInfo.GetMethod.GetParameters().Length > 0)
                //    continue;

                Type currentType = propertyInfo.PropertyType;
                if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    currentType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
                }

                if (namespaceStartsWith.Where(ns => currentType.Namespace.StartsWith(ns)).Any() || currentType.Namespace.StartsWith("RPSOAPServiceReference"))
                {
                    if (currentType.IsEnum)
                    {
                        if (propertyInfo.GetValue(obj, null) == null)
                            propertyValues.Add(propertyInfo.Name, Convert.DBNull);
                        else
                        {
                            int val = (int)propertyInfo.GetValue(obj, null);
                            propertyValues.Add(propertyInfo.Name, val);
                        }
                    }
                    else if (currentType.IsValueType && !currentType.IsEnum)
                    {
                        if (propertyInfo.GetValue(obj, null) == null)
                            propertyValues.Add(propertyInfo.Name, Convert.DBNull);
                        else
                        {
                            string val = propertyInfo.GetValue(obj, null).ToString();
                            propertyValues.Add(propertyInfo.Name, val);
                        }
                    }
                    else
                    {
                        Dictionary<string, Object> subValues = GetNamesAndValues(propertyInfo.PropertyType, propertyInfo.GetValue(obj, null), namespaceStartsWith);

                        foreach (var value in subValues)
                        {
                            if (value.Value == null)
                                propertyValues.Add(propertyInfo.Name + "_" + value.Key, Convert.DBNull);
                            else
                                propertyValues.Add(propertyInfo.Name + "_" + value.Key, value.Value);

                        }
                        //propertyValues.AddRange(propertyInfo.Name, subValues);
                    }
                }
                else
                {
                    if (obj != null)
                    {
                        var value = propertyInfo.GetValue(obj, null);
                        if (value == null || (propertyInfo.PropertyType == typeof(DateTime) && value.ToString() == DateTime.MinValue.ToString()))
                            propertyValues.Add(propertyInfo.Name, Convert.DBNull);
                        else
                            propertyValues.Add(propertyInfo.Name, value);
                    }
                    else
                    {
                        propertyValues.Add(propertyInfo.Name, Convert.DBNull);
                    }
                }
            }
            return propertyValues;
        }
        /*
        public static IEnumerable<String> GetNames(IEnumerable<Object> objects, string nameProperty = "Name")
        {
            foreach (var instance in objects)
            {
                var type = instance.GetType();
                var property = type.GetProperty(nameProperty);
                yield return property.GetValue(instance, null) as string;
            }
        }

        public static string[] GetNames(Type type)
        {
            // get all public static properties of MyClass type
            PropertyInfo[] propertyInfos;

            //propertyInfos = type.GetProperties
            //    (BindingFlags.Public | 
            //    BindingFlags.Static);

            propertyInfos = type.GetProperties();
            //propertyInfos = type.GetProperties(BindingFlags.Default);

            // sort properties by name
            Array.Sort(propertyInfos,
                    delegate (PropertyInfo propertyInfo1, PropertyInfo propertyInfo2)
                    { return propertyInfo1.Name.CompareTo(propertyInfo2.Name); });

            List<string> propertyNames = new List<string>();

            foreach (var propertyInfo in propertyInfos)
            {
                if (propertyInfo.PropertyType.Namespace.StartsWith("RPSOAPServiceReference"))
                {
                    string[] subNames = GetNames(propertyInfo.PropertyType).Select(n => (propertyInfo.Name + "_" + n)).ToArray();
                    propertyNames.AddRange(subNames);
                }
                else
                {
                    propertyNames.Add(propertyInfo.Name);
                }
            }

            return propertyNames.ToArray();
        }

        public static string[] GetValuesAsStrings(Type type, Object obj)
        {
            // get all public static properties of MyClass type
            PropertyInfo[] propertyInfos;

            //propertyInfos = type.GetProperties
            //    (BindingFlags.Public | 
            //    BindingFlags.Static);

            propertyInfos = type.GetProperties();
            //propertyInfos = type.GetProperties(BindingFlags.Default);

            // sort properties by name

            //Array.Sort(propertyInfos,
            //        delegate (PropertyInfo propertyInfo1, PropertyInfo propertyInfo2)
            //        { return propertyInfo1.Name.CompareTo(propertyInfo2.Name); });

            List<string> propertyValues = new List<string>();

            foreach (var propertyInfo in propertyInfos)
            {
                if (propertyInfo.PropertyType.Namespace.StartsWith("RPSOAPServiceReference"))
                {
                    string[] subValues = GetValuesAsStrings(propertyInfo.PropertyType, propertyInfo.GetValue(obj, null)).ToArray();
                    propertyValues.AddRange(subValues);
                }
                else
                {
                    //DefinedTypes.FirstOrDefault(x => x.GetDeclaredProperty("functionCode") != null && 
                    //!x.IsAbstract && x.BaseType == typeof(MyClass) && 
                    //(byte)x.GetDeclaredProperty("functionCode").GetValue(Activator.CreateInstance(x.AsType()), null) == val);
                    if (obj != null)
                    {
                        var value = propertyInfo.GetValue(obj);
                        if (value == null)
                            propertyValues.Add(null);
                        else
                            propertyValues.Add(value.ToString());
                    }
                    else
                    {
                        propertyValues.Add(null);
                    }
                }
            }
            return propertyValues.ToArray();
        }

        public static object[] GetValues(Type type, Object obj)
        {
            // get all public static properties of MyClass type
            PropertyInfo[] propertyInfos;

            propertyInfos = type.GetProperties();

            List<object> propertyValues = new List<object>();

            foreach (var propertyInfo in propertyInfos)
            {
                if (propertyInfo.PropertyType.Namespace.StartsWith("RPSOAPServiceReference"))
                {
                    if (propertyInfo.PropertyType.IsEnum)
                    {
                        int val = (int)propertyInfo.GetValue(obj, null);
                        propertyValues.Add(val);
                    }
                    else if (propertyInfo.PropertyType.IsValueType && !propertyInfo.PropertyType.IsEnum)
                    {
                        string val = propertyInfo.GetValue(obj, null).ToString();
                        propertyValues.Add(val);
                    }
                    else
                    {
                        object[] subValues = GetValues(propertyInfo.PropertyType, propertyInfo.GetValue(obj, null)).ToArray();
                        propertyValues.AddRange(subValues);
                    }
                }
                else
                {
                    if (obj != null)
                    {
                        var value = propertyInfo.GetValue(obj);
                        if (value == null)
                            propertyValues.Add(null);
                        else
                            propertyValues.Add(value);
                    }
                    else
                    {
                        propertyValues.Add(null);
                    }
                }
            }
            return propertyValues.ToArray();
        }
        */
        private List<Type> _systemTypes;
        public List<Type> SystemTypes
        {
            get
            {
                if (_systemTypes == null)
                {
                    _systemTypes = Assembly.GetExecutingAssembly().GetType().Module.Assembly.GetExportedTypes().ToList();
                }
                return _systemTypes;
            }
        }

        //public static string ConvertToCsv<T>(IEnumerable<T> items)
        //{
        //    foreach (T item in items.Where(i => SystemTypes.Contains(i.GetType())))
        //    {
        //        // is system type
        //    }
        //}
    }
    public class GenericPropertyFinder<TModel> where TModel : class
    {
        public void PrintTModelPropertyAndValue(TModel tmodelObj)
        {
            //Getting Type of Generic Class Model
            Type tModelType = tmodelObj.GetType();

            //We will be defining a PropertyInfo Object which contains details about the class property 
            PropertyInfo[] arrayPropertyInfos = tModelType.GetProperties();

            //Now we will loop in all properties one by one to get value
            foreach (PropertyInfo property in arrayPropertyInfos)
            {
                Console.WriteLine("Name of Property is\t:\t" + property.Name);
                if (property.GetValue(tmodelObj, null) != null)
                    Console.WriteLine("Value of Property is\t:\t" + property.GetValue(tmodelObj, null).ToString());
                else
                    Console.WriteLine("Value of Property is\t:\t null");

                Console.WriteLine(Environment.NewLine);
            }
        }
    }
}
