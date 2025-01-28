#region Header
/**
 * JsonMapper.cs
 *   JSON to .Net object and object to JSON conversions.
 *
 * The authors disclaim copyright to this source code. For more details, see
 * the COPYING file included with this distribution.
 **/
#endregion


using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;


namespace UISystem.LitJson
{
    public interface IVariantTypeDictionary
    {
        Type GetElementType(object key);
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class JsonProperty : Attribute
    {
        public string Name;

        public JsonProperty()
        {
        }

        public JsonProperty(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class JsonIgnore : Attribute
    {
        public JsonIgnore()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field)]
    public class JsonCamelCase : Attribute
    {
        public JsonCamelCase()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class JsonDynamic : Attribute
    {
        public string PropertyName;

        public JsonDynamic(string propertyName)
        {
            PropertyName = propertyName;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class JsonAutoDetect : Attribute
    {
        public bool Field = true;
        public bool Property = true;
        public bool Static = true;

        public JsonAutoDetect()
        {
        }
    }

    public enum JsonEnumOptions
    {
        UseUnderlyingType,
        UseToString,
        UseAlias
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class JsonEnum : Attribute
    {
        public JsonEnumOptions Option = JsonEnumOptions.UseToString;

        public JsonEnum()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class JsonEnumAliasAttribute : Attribute
    {
        public string Alias { get; private set; }

        public JsonEnumAliasAttribute(string alias)
        {
            Alias = alias;
        }

        static string GetAlias(object enumValue)
        {
            JsonEnumAliasAttribute attr = Attribute.GetCustomAttribute(enumValue.GetType().GetField(enumValue.ToString()), typeof(JsonEnumAliasAttribute)) as JsonEnumAliasAttribute;
            return attr == null ? null : attr.Alias;
        }

        static Dictionary<Type, Dictionary<string, object>> toEnum = new Dictionary<Type, Dictionary<string, object>>();
        static Dictionary<Type, Dictionary<object, string>> toAlias = new Dictionary<Type, Dictionary<object, string>>();
        static readonly object catchLock = new Object();

        static Dictionary<string, object> GetToEnumDictionary(Type type)
        {
            Dictionary<string, object> cache = null;
            lock (catchLock)
            {
                if (toEnum.TryGetValue(type, out cache))
                    return cache;
            }
            cache = new Dictionary<string, object>();
            foreach (object enumValue in Enum.GetValues(type))
            {
                cache.Add(GetAlias(enumValue) ?? enumValue.ToString(), enumValue);
            }
            lock (catchLock)
            {
                toEnum[type] = cache;
            }
            return cache;
        }

        static Dictionary<object, string> GetToAliasDictionary(Type type)
        {
            Dictionary<object, string> cache = null;
            lock (catchLock)
            {
                if (toAlias.TryGetValue(type, out cache))
                    return cache;
            }
            cache = new Dictionary<object, string>();
            foreach (object enumValue in Enum.GetValues(type))
            {
                cache.Add(enumValue, GetAlias(enumValue) ?? enumValue.ToString());
            }
            lock (catchLock)
            {
                toAlias[type] = cache;
            }
            return cache;
        }

        public static string ToAlias(Type type, object enumValue)
        {
            string alias = null;
            GetToAliasDictionary(type).TryGetValue(enumValue, out alias);
            return alias;
        }

        public static object Parse(Type type, string code)
        {
            object value = null;
            GetToEnumDictionary(type).TryGetValue(code, out value);
            return value;
            //			foreach (object enumValue in Enum.GetValues(type))
            //			{
            //				if (GetAlias(enumValue) == code)
            //					return enumValue;
            //			}
            //			return null;
        }
    }

    internal struct PropertyMetadata
    {
        public string Name;
        public MemberInfo Info;
        public bool IsField;
        public Type Type;
        public PropertyInfo DynamicType;
        public JsonEnumOptions? EnumOption;
    }

    internal struct ArrayMetadata
    {
        private Type element_type;
        private bool is_array;
        private bool is_list;

        public Type ElementType
        {
            get
            {
                if (element_type == null)
                    return typeof(JsonData);

                return element_type;
            }

            set { element_type = value; }
        }

        public bool IsArray
        {
            get { return is_array; }
            set { is_array = value; }
        }

        public bool IsList
        {
            get { return is_list; }
            set { is_list = value; }
        }
    }


    internal struct ObjectMetadata
    {
        private Type key_type;
        private Type element_type;
        private bool is_dictionary;
        private bool is_variant_type_dictionary;

        private IDictionary<string, PropertyMetadata> properties;

        public Type KeyType
        {
            get
            {
                if (key_type == null)
                    return typeof(string);

                return key_type;
            }

            set { key_type = value; }
        }

        public Type ElementType
        {
            get
            {
                if (element_type == null)
                    return typeof(JsonData);

                return element_type;
            }

            set { element_type = value; }
        }

        public Type GetElementType(object instance, object key)
        {
            Type elementType;
            if (is_variant_type_dictionary)
            {
                elementType = (instance as IVariantTypeDictionary).GetElementType(key);
                if (elementType != null) return elementType;
            }
            return ElementType;
        }

        public bool IsDictionary
        {
            get { return is_dictionary; }
            set { is_dictionary = value; }
        }

        public bool IsVariantTypeDictionary
        {
            get { return is_variant_type_dictionary; }
            set { is_variant_type_dictionary = value; }
        }

        public IDictionary<string, PropertyMetadata> Properties
        {
            get { return properties; }
            set { properties = value; }
        }
    }


    internal delegate void ExporterFunc(object obj, JsonWriter writer);
    public delegate void ExporterFunc<T>(T obj, JsonWriter writer);

    internal delegate object ImporterFunc(object input);
    public delegate TValue ImporterFunc<TJson, TValue>(TJson input);

    public delegate IJsonWrapper WrapperFactory();


    public class JsonMapper
    {
        #region Fields
        private static int max_nesting_depth;

        private static IFormatProvider datetime_format;

        private static IDictionary<Type, ExporterFunc> base_exporters_table;
        private static IDictionary<Type, ExporterFunc> custom_exporters_table;

        private static IDictionary<Type, IDictionary<Type, ImporterFunc>> base_importers_table;
        private static IDictionary<Type, IDictionary<Type, ImporterFunc>> custom_importers_table;

        private static IDictionary<Type, ArrayMetadata> array_metadata;
        private static readonly object array_metadata_lock = new Object();

        private static IDictionary<Type, IDictionary<Type, MethodInfo>> conv_ops;
        private static readonly object conv_ops_lock = new Object();

        private static IDictionary<Type, ObjectMetadata> object_metadata;
        private static readonly object object_metadata_lock = new Object();

        private static IDictionary<Type, IList<PropertyMetadata>> type_properties;
        private static readonly object type_properties_lock = new Object();

        private static JsonWriter static_writer;
        private static readonly object static_writer_lock = new Object();

        private static JsonEnumOptions json_enum_option = JsonEnumOptions.UseToString;
        #endregion


        #region Constructors
        static JsonMapper()
        {
            max_nesting_depth = 100;

            array_metadata = new Dictionary<Type, ArrayMetadata>();
            conv_ops = new Dictionary<Type, IDictionary<Type, MethodInfo>>();
            object_metadata = new Dictionary<Type, ObjectMetadata>();
            type_properties = new Dictionary<Type, IList<PropertyMetadata>>();

            static_writer = new JsonWriter();

            datetime_format = DateTimeFormatInfo.InvariantInfo;

            base_exporters_table = new Dictionary<Type, ExporterFunc>();
            custom_exporters_table = new Dictionary<Type, ExporterFunc>();

            base_importers_table = new Dictionary<Type,
            IDictionary<Type, ImporterFunc>>();
            custom_importers_table = new Dictionary<Type,
            IDictionary<Type, ImporterFunc>>();

            RegisterBaseExporters();
            RegisterBaseImporters();
        }
        #endregion


        #region Private Methods
        private static void AddArrayMetadata(Type type)
        {
            if (array_metadata.ContainsKey(type))
                return;

            ArrayMetadata data = new ArrayMetadata();

            data.IsArray = type.IsArray;

            if (type.GetInterface("System.Collections.IList", true) != null)
                data.IsList = true;

            foreach (PropertyInfo p_info in type.GetProperties())
            {
                if (p_info.Name != "Item")
                    continue;

                ParameterInfo[] parameters = p_info.GetIndexParameters();

                if (parameters.Length != 1)
                    continue;

                if (parameters[0].ParameterType == typeof(int))
                    data.ElementType = p_info.PropertyType;
            }

            lock (array_metadata_lock)
            {
                try
                {
                    array_metadata.Add(type, data);
                }
                catch (ArgumentException)
                {
                    return;
                }
            }
        }

        private static void AddObjectMetadata(Type type)
        {
            if (object_metadata.ContainsKey(type))
                return;

            ObjectMetadata data = new ObjectMetadata();

            bool isCamelCase = type.GetCustomAttributes(typeof(JsonCamelCase), true).Length > 0;

            bool autoDetectField = true;
            bool autoDetectProperty = true;
            bool autoDetectStatic = true;
            JsonAutoDetect[] autoDetects = (JsonAutoDetect[])type.GetCustomAttributes(typeof(JsonAutoDetect), true);
            if (autoDetects.Length > 0)
            {
                JsonAutoDetect autoDetect = autoDetects[0];
                autoDetectField = autoDetect.Field;
                autoDetectProperty = autoDetect.Property;
                autoDetectStatic = autoDetect.Static;
            }

            if (type.GetInterface("System.Collections.IDictionary", true) != null)
            {
                data.IsDictionary = true;
                MethodInfo addMethod = type.GetMethod("Add");
                data.KeyType = addMethod.GetParameters()[0].ParameterType;
                data.ElementType = addMethod.GetParameters()[1].ParameterType;
                if (data.ElementType == typeof(object))
                {
                    data.ElementType = null;
                }
                data.IsVariantTypeDictionary = typeof(IVariantTypeDictionary).IsAssignableFrom(type);
            }

            data.Properties = new Dictionary<string, PropertyMetadata>();

            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            if (autoDetectStatic)
                flags |= BindingFlags.Static;

            foreach (PropertyInfo p_info in type.GetProperties(flags))
            {
                if (p_info.Name == "Item")
                {
                    ParameterInfo[] parameters = p_info.GetIndexParameters();

                    if (parameters.Length != 1)
                        continue;

                    if (parameters[0].ParameterType == typeof(string))
                        data.ElementType = p_info.PropertyType;

                    continue;
                }
                //bool isPublic = p_info.CanWrite && p_info.GetSetMethod(true).IsPublic;
                bool isPublic = p_info.CanRead && p_info.GetGetMethod(true).IsPublic;
                AddTypePropertiesTo(type, p_info, isPublic, isCamelCase, data.Properties, autoDetectProperty);
            }

            foreach (FieldInfo f_info in type.GetFields(flags))
            {
                bool isPublic = f_info.IsPublic;
                AddTypePropertiesTo(type, f_info, isPublic, isCamelCase, data.Properties, autoDetectField);
            }

            lock (object_metadata_lock)
            {
                try
                {
                    object_metadata.Add(type, data);
                }
                catch (ArgumentException)
                {
                    return;
                }
            }
        }

        private static PropertyMetadata? CreatePropertyMetadata(Type type, MemberInfo info, bool isPublic, bool isCamelCase, bool autoDetect)
        {
            PropertyMetadata p_data = new PropertyMetadata();
            p_data.Name = info.Name;
            p_data.Info = info;
            if (info is FieldInfo)
            {
                p_data.IsField = true;
                p_data.Type = (info as FieldInfo).FieldType;
            }
            else if (info is PropertyInfo)
            {
                p_data.IsField = false;
                p_data.Type = (info as PropertyInfo).PropertyType;
            }
            else
            {
                return null;
            }

            bool isJsonProperty = isPublic && autoDetect;
            foreach (Attribute attr in info.GetCustomAttributes(true))
            {
                if (attr is JsonProperty)
                {
                    string name = (attr as JsonProperty).Name;
                    if (!string.IsNullOrEmpty(name))
                    {
                        p_data.Name = (attr as JsonProperty).Name;
                        isCamelCase = false;
                    }
                    isJsonProperty = true;
                }
                else if (attr is JsonDynamic)
                {
                    p_data.Type = null;
                    p_data.DynamicType = type.GetProperty((attr as JsonDynamic).PropertyName,
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                }
                else if (attr is JsonCamelCase)
                {
                    isCamelCase = true;
                }
                else if (attr is JsonIgnore)
                {
                    return null;
                }
                else if (attr is JsonEnum)
                {
                    p_data.EnumOption = (attr as JsonEnum).Option;
                }
                //				else if (attr is UnityEngine.SerializeField)
                //				{
                //					isJsonProperty = true;
                //				}
                //				else if (attr is NonSerializedAttribute)
                //				{
                //					return null;
                //				}
            }
            if (!isJsonProperty)
            {
                return null;
            }
            if (isCamelCase)
            {
                p_data.Name = char.ToLower(p_data.Name[0]) + p_data.Name.Substring(1);
            }
            return p_data;
        }

        private static void AddTypePropertiesTo(Type type, MemberInfo info, bool isPublic, bool isCamelCase, IList<PropertyMetadata> props, bool autoDetect)
        {
            PropertyMetadata? p_data = CreatePropertyMetadata(type, info, isPublic, isCamelCase, autoDetect);
            if (p_data.HasValue)
                props.Add(p_data.Value);
        }

        private static void AddTypePropertiesTo(Type type, MemberInfo info, bool isPublic, bool isCamelCase, IDictionary<string, PropertyMetadata> props, bool autoDetect)
        {
            PropertyMetadata? p_data = CreatePropertyMetadata(type, info, isPublic, isCamelCase, autoDetect);
            if (p_data.HasValue)
                props.Add(p_data.Value.Name, p_data.Value);
        }

        private static void AddTypeProperties(Type type)
        {
            if (type_properties.ContainsKey(type))
                return;

            IList<PropertyMetadata> props = new List<PropertyMetadata>();
            bool isCamelCase = type.GetCustomAttributes(typeof(JsonCamelCase), true).Length > 0;

            bool autoDetectField = true;
            bool autoDetectProperty = true;
            bool autoDetectStatic = true;
            JsonAutoDetect[] autoDetects = (JsonAutoDetect[])type.GetCustomAttributes(typeof(JsonAutoDetect), true);
            if (autoDetects.Length > 0)
            {
                JsonAutoDetect autoDetect = autoDetects[0];
                autoDetectField = autoDetect.Field;
                autoDetectProperty = autoDetect.Property;
                autoDetectStatic = autoDetect.Static;
            }

            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            if (autoDetectStatic)
                flags |= BindingFlags.Static;

            foreach (PropertyInfo p_info in type.GetProperties(flags))
            {
                if (p_info.Name == "Item")
                    continue;
                bool isPublic = p_info.CanRead && p_info.GetGetMethod(true).IsPublic;
                AddTypePropertiesTo(type, p_info, isPublic, isCamelCase, props, autoDetectProperty);
            }

            foreach (FieldInfo f_info in type.GetFields(flags))
            {
                bool isPublic = f_info.IsPublic;
                AddTypePropertiesTo(type, f_info, isPublic, isCamelCase, props, autoDetectField);
            }

            lock (type_properties_lock)
            {
                try
                {
                    type_properties.Add(type, props);
                }
                catch (ArgumentException)
                {
                    return;
                }
            }
        }

        private static MethodInfo GetConvOp(Type t1, Type t2)
        {
            lock (conv_ops_lock)
            {
                if (!conv_ops.ContainsKey(t1))
                    conv_ops.Add(t1, new Dictionary<Type, MethodInfo>());
            }

            if (conv_ops[t1].ContainsKey(t2))
                return conv_ops[t1][t2];

            MethodInfo op = t1.GetMethod(
                "op_Implicit", new Type[] { t2 });

            lock (conv_ops_lock)
            {
                try
                {
                    conv_ops[t1].Add(t2, op);
                }
                catch (ArgumentException)
                {
                    return conv_ops[t1][t2];
                }
            }

            return op;
        }

        private static object ReadValue(Type inst_type, JsonReader reader, JsonEnumOptions enumOption)
        {
            bool isNullable = false;
            reader.Read();

            if (reader.Token == JsonToken.ArrayEnd)
                return null;

            if (inst_type.IsGenericType &&
                inst_type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                inst_type = inst_type.GetGenericArguments()[0];
                isNullable = true;
            }

            if (reader.Token == JsonToken.Null)
            {
                if (!inst_type.IsClass && !isNullable)
                    throw new JsonException(String.Format(
                        "Can't assign null to an instance of type {0}",
                        inst_type));
                return null;
            }

            if (reader.Token == JsonToken.Double ||
                reader.Token == JsonToken.Int ||
                reader.Token == JsonToken.Long ||
                reader.Token == JsonToken.String ||
                reader.Token == JsonToken.Boolean)
            {

                Type json_type = reader.Value.GetType();

                if (inst_type.IsAssignableFrom(json_type))
                    return reader.Value;

                // If there's a custom importer that fits, use it
                if (custom_importers_table.ContainsKey(json_type) &&
                    custom_importers_table[json_type].ContainsKey(
                    inst_type))
                {

                    ImporterFunc importer =
                        custom_importers_table[json_type][inst_type];

                    return importer(reader.Value);
                }

                // Maybe there's a base importer that works
                if (base_importers_table.ContainsKey(json_type) &&
                    base_importers_table[json_type].ContainsKey(
                    inst_type))
                {

                    ImporterFunc importer =
                        base_importers_table[json_type][inst_type];

                    return importer(reader.Value);
                }

                // Maybe it's an enum
                if (inst_type.IsEnum)
                {
                    try
                    {
                        if (reader.Token == JsonToken.String)
                        {
                            if (enumOption == JsonEnumOptions.UseAlias)
                                return JsonEnumAliasAttribute.Parse(inst_type, (string)reader.Value);
                            else
                                return Enum.Parse(inst_type, (string)reader.Value);
                        }
                        else
                        {
                            return Enum.ToObject(inst_type, reader.Value);
                        }
                    }
                    catch
                    {
                        throw new JsonException(String.Format(
                            "Can't parse enum value '{0}' (type {1})",
                            reader.Value, inst_type));
                    }
                }

                // Try using an implicit conversion operator
                MethodInfo conv_op = GetConvOp(inst_type, json_type);

                if (conv_op != null)
                    return conv_op.Invoke(null,
                                           new object[] { reader.Value });

                // No luck
                throw new JsonException(String.Format(
                    "Can't assign value '{0}' (type {1}) to type {2}",
                    reader.Value, json_type, inst_type));
            }

            object instance = null;

            if (reader.Token == JsonToken.ArrayStart)
            {

                AddArrayMetadata(inst_type);
                ArrayMetadata t_data = array_metadata[inst_type];

                if (!t_data.IsArray && !t_data.IsList)
                    throw new JsonException(String.Format(
                        "Type {0} can't act as an array",
                        inst_type));

                IList list;
                Type elem_type;

                if (!t_data.IsArray)
                {
                    list = (IList)Activator.CreateInstance(inst_type);
                    elem_type = t_data.ElementType;
                }
                else
                {
                    list = new ArrayList();
                    elem_type = inst_type.GetElementType();
                }

                while (true)
                {
                    object item = ReadValue(elem_type, reader, enumOption);
                    if (item == null && reader.Token == JsonToken.ArrayEnd)
                        break;

                    list.Add(item);
                }

                if (t_data.IsArray)
                {
                    int n = list.Count;
                    instance = Array.CreateInstance(elem_type, n);

                    for (int i = 0; i < n; i++)
                        ((Array)instance).SetValue(list[i], i);
                }
                else
                    instance = list;

            }
            else if (reader.Token == JsonToken.ObjectStart)
            {

                instance = Activator.CreateInstance(inst_type);
                ReadObjectTo(inst_type, reader, instance, enumOption);
            }

            return instance;
        }

        private static void ReadObjectTo(Type inst_type, JsonReader reader, object instance, JsonEnumOptions enumOption)
        {
            AddObjectMetadata(inst_type);
            ObjectMetadata t_data = object_metadata[inst_type];

            while (true)
            {
                reader.Read();

                if (reader.Token == JsonToken.ObjectEnd)
                    break;

                string property = (string)reader.Value;

                if (t_data.Properties.ContainsKey(property))
                {
                    PropertyMetadata prop_data =
                        t_data.Properties[property];

                    JsonEnumOptions currEnumOption = enumOption;
                    if (prop_data.EnumOption.HasValue)
                        currEnumOption = prop_data.EnumOption.Value;

                    Type propType = prop_data.Type;
                    if (propType == null)
                    {
                        propType = (Type)prop_data.DynamicType.GetValue(instance, null);
                        if (propType == null)
                        {
                            propType = typeof(object);
                        }
                    }

                    if (prop_data.IsField)
                    {
                        ((FieldInfo)prop_data.Info).SetValue(
                            instance, ReadValue(propType, reader, currEnumOption));
                    }
                    else
                    {
                        PropertyInfo p_info =
                            (PropertyInfo)prop_data.Info;

                        if (p_info.CanWrite)
                            p_info.SetValue(
                                instance,
                                ReadValue(propType, reader, currEnumOption),
                                null);
                        else
                            ReadValue(propType, reader, currEnumOption);
                    }

                }
                else
                {
                    if (!t_data.IsDictionary)
                    {
                        if (!reader.SkipNonMembers)
                        {
                            throw new JsonException(String.Format(
                                "The type {0} doesn't have the " +
                                "property '{1}'",
                                inst_type, property));
                        }
                        else
                        {
                            ReadSkip(reader);
                            continue;
                        }
                    }
                    object key = property;

                    if (t_data.KeyType.IsEnum)
                    {
                        if (enumOption == JsonEnumOptions.UseAlias)
                            key = JsonEnumAliasAttribute.Parse(t_data.KeyType, property);
                        else
                            key = Enum.Parse(t_data.KeyType, property);
                    }
                    else if (t_data.KeyType != typeof(string))
                    {
                        key = Convert.ChangeType(key, Type.GetTypeCode(t_data.KeyType));
                    }

                    ((IDictionary)instance).Add(
                        key,
                        ReadValue(t_data.GetElementType(instance, key), reader, enumOption));
                }

            }
        }

        private static IJsonWrapper ReadValue(WrapperFactory factory,
                                               JsonReader reader)
        {
            reader.Read();

            if (reader.Token == JsonToken.ArrayEnd ||
                reader.Token == JsonToken.Null)
                return null;

            IJsonWrapper instance = factory();

            if (reader.Token == JsonToken.String)
            {
                instance.SetString((string)reader.Value);
                return instance;
            }

            if (reader.Token == JsonToken.Double)
            {
                instance.SetDouble((double)reader.Value);
                return instance;
            }

            if (reader.Token == JsonToken.Int)
            {
                instance.SetInt((int)reader.Value);
                return instance;
            }

            if (reader.Token == JsonToken.Long)
            {
                instance.SetLong((long)reader.Value);
                return instance;
            }

            if (reader.Token == JsonToken.Boolean)
            {
                instance.SetBoolean((bool)reader.Value);
                return instance;
            }

            if (reader.Token == JsonToken.ArrayStart)
            {
                instance.SetJsonType(JsonType.Array);

                while (true)
                {
                    IJsonWrapper item = ReadValue(factory, reader);
                    if (item == null && reader.Token == JsonToken.ArrayEnd)
                        break;

                    ((IList)instance).Add(item);
                }
            }
            else if (reader.Token == JsonToken.ObjectStart)
            {
                instance.SetJsonType(JsonType.Object);

                while (true)
                {
                    reader.Read();

                    if (reader.Token == JsonToken.ObjectEnd)
                        break;

                    string property = (string)reader.Value;

                    ((IDictionary)instance)[property] = ReadValue(
                        factory, reader);
                }

            }

            return instance;
        }

        private static void ReadSkip(JsonReader reader)
        {
            ToWrapper(
                delegate { return new JsonMockWrapper(); }, reader);
        }

        private static void RegisterBaseExporters()
        {
            base_exporters_table[typeof(byte)] =
            delegate (object obj, JsonWriter writer)
            {
                writer.Write(Convert.ToInt32((byte)obj));
            };

            base_exporters_table[typeof(char)] =
            delegate (object obj, JsonWriter writer)
            {
                writer.Write(Convert.ToString((char)obj));
            };

            base_exporters_table[typeof(DateTime)] =
            delegate (object obj, JsonWriter writer)
            {
                writer.Write(((DateTime)obj).ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'"));
                //writer.Write (Convert.ToString ((DateTime) obj,
                //                                datetime_format));
            };

            base_exporters_table[typeof(decimal)] =
            delegate (object obj, JsonWriter writer)
            {
                writer.Write((decimal)obj);
            };

            base_exporters_table[typeof(sbyte)] =
            delegate (object obj, JsonWriter writer)
            {
                writer.Write(Convert.ToInt32((sbyte)obj));
            };

            base_exporters_table[typeof(short)] =
            delegate (object obj, JsonWriter writer)
            {
                writer.Write(Convert.ToInt32((short)obj));
            };

            base_exporters_table[typeof(ushort)] =
            delegate (object obj, JsonWriter writer)
            {
                writer.Write(Convert.ToInt32((ushort)obj));
            };

            base_exporters_table[typeof(uint)] =
            delegate (object obj, JsonWriter writer)
            {
                writer.Write(Convert.ToUInt64((uint)obj));
            };

            base_exporters_table[typeof(ulong)] =
            delegate (object obj, JsonWriter writer)
            {
                writer.Write((ulong)obj);
            };
        }

        private static void RegisterBaseImporters()
        {
            ImporterFunc importer;

            importer = delegate (object input)
            {
                return Convert.ToByte((int)input);
            };
            RegisterImporter(base_importers_table, typeof(int),
                              typeof(byte), importer);

            importer = delegate (object input)
            {
                return Convert.ToUInt64((int)input);
            };
            RegisterImporter(base_importers_table, typeof(int),
                              typeof(ulong), importer);

            importer = delegate (object input)
            {
                return Convert.ToSByte((int)input);
            };
            RegisterImporter(base_importers_table, typeof(int),
                              typeof(sbyte), importer);

            importer = delegate (object input)
            {
                return Convert.ToInt16((int)input);
            };
            RegisterImporter(base_importers_table, typeof(int),
                              typeof(short), importer);

            importer = delegate (object input)
            {
                return Convert.ToUInt16((int)input);
            };
            RegisterImporter(base_importers_table, typeof(int),
                              typeof(ushort), importer);

            importer = delegate (object input)
            {
                return Convert.ToUInt32((int)input);
            };
            RegisterImporter(base_importers_table, typeof(int),
                              typeof(uint), importer);

            importer = delegate (object input)
            {
                return Convert.ToSingle((int)input);
            };
            RegisterImporter(base_importers_table, typeof(int),
                              typeof(float), importer);

            importer = delegate (object input)
            {
                return Convert.ToDouble((int)input);
            };
            RegisterImporter(base_importers_table, typeof(int),
                              typeof(double), importer);

            importer = delegate (object input)
            {
                return Convert.ToDecimal((double)input);
            };
            RegisterImporter(base_importers_table, typeof(double),
                              typeof(decimal), importer);


            importer = delegate (object input)
            {
                return Convert.ToUInt32((long)input);
            };
            RegisterImporter(base_importers_table, typeof(long),
                              typeof(uint), importer);

            importer = delegate (object input)
            {
                return Convert.ToChar((string)input);
            };
            RegisterImporter(base_importers_table, typeof(string),
                              typeof(char), importer);

            importer = delegate (object input)
            {
                return Convert.ToDateTime((string)input, datetime_format);
            };
            RegisterImporter(base_importers_table, typeof(string),
                              typeof(DateTime), importer);

            importer = delegate (object input)
            {
                DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                return epoch.AddMilliseconds((long)input).ToLocalTime();
            };
            RegisterImporter(base_importers_table, typeof(long),
                              typeof(DateTime), importer);

            importer = delegate (object input)
            {
                return Convert.ToInt64((int)input);
            };
            RegisterImporter(base_importers_table, typeof(int),
                              typeof(long), importer);

            importer = delegate (object input)
            {
                return Convert.ToSingle((double)input);
            };
            RegisterImporter(base_importers_table, typeof(double),
                              typeof(float), importer);
        }

        private static void RegisterImporter(
            IDictionary<Type, IDictionary<Type, ImporterFunc>> table,
            Type json_type, Type value_type, ImporterFunc importer)
        {
            if (!table.ContainsKey(json_type))
                table.Add(json_type, new Dictionary<Type, ImporterFunc>());

            table[json_type][value_type] = importer;
        }

        private static void WriteValue(object obj, JsonWriter writer,
                                        bool writer_is_private,
            int depth, JsonEnumOptions enumOption)
        {
            if (depth > max_nesting_depth)
                throw new JsonException(
                    String.Format("Max allowed object depth reached while " +
                               "trying to export from type {0}",
                               obj.GetType()));

            if (obj == null)
            {
                writer.Write(null);
                return;
            }

            if (obj is IJsonWrapper)
            {
                if (writer_is_private)
                    writer.TextWriter.Write(((IJsonWrapper)obj).ToJson());
                else
                    ((IJsonWrapper)obj).ToJson(writer);

                return;
            }

            if (obj is String)
            {
                writer.Write((string)obj);
                return;
            }

            if (obj is Double)
            {
                writer.Write((double)obj);
                return;
            }

            if (obj is Single)
            {
                writer.Write((float)obj);
                return;
            }

            if (obj is Int32)
            {
                writer.Write((int)obj);
                return;
            }

            if (obj is Boolean)
            {
                writer.Write((bool)obj);
                return;
            }

            if (obj is Int64)
            {
                writer.Write((long)obj);
                return;
            }

            if (obj is Array)
            {
                writer.WriteArrayStart();

                foreach (object elem in (Array)obj)
                    WriteValue(elem, writer, writer_is_private, depth + 1, enumOption);

                writer.WriteArrayEnd();

                return;
            }

            if (obj is IList)
            {
                writer.WriteArrayStart();
                foreach (object elem in (IList)obj)
                    WriteValue(elem, writer, writer_is_private, depth + 1, enumOption);
                writer.WriteArrayEnd();

                return;
            }

            if (obj is IDictionary)
            {
                writer.WriteObjectStart();
                foreach (DictionaryEntry entity in (IDictionary)obj)
                {
                    writer.WritePropertyName(entity.Key.ToString());
                    WriteValue(entity.Value, writer, writer_is_private,
                        depth + 1, enumOption);
                }
                writer.WriteObjectEnd();

                return;
            }

            Type obj_type = obj.GetType();

            // See if there's a custom exporter for the object
            if (custom_exporters_table.ContainsKey(obj_type))
            {
                ExporterFunc exporter = custom_exporters_table[obj_type];
                exporter(obj, writer);

                return;
            }

            // If not, maybe there's a base exporter
            if (base_exporters_table.ContainsKey(obj_type))
            {
                ExporterFunc exporter = base_exporters_table[obj_type];
                exporter(obj, writer);

                return;
            }

            // Last option, let's see if it's an enum
            if (obj is Enum)
            {
                switch (enumOption)
                {
                    case JsonEnumOptions.UseToString:
                        writer.Write(obj.ToString());
                        break;
                    case JsonEnumOptions.UseUnderlyingType:
                        Type e_type = Enum.GetUnderlyingType(obj_type);

                        if (e_type == typeof(long)
                            || e_type == typeof(uint)
                            || e_type == typeof(ulong))
                            writer.Write((ulong)obj);
                        else
                            writer.Write((int)obj);
                        break;
                    case JsonEnumOptions.UseAlias:
                        writer.Write(JsonEnumAliasAttribute.ToAlias(obj.GetType(), obj));
                        break;
                }
                return;
            }

            // Okay, so it looks like the input should be exported as an
            // object
            AddTypeProperties(obj_type);
            IList<PropertyMetadata> props = type_properties[obj_type];

            writer.WriteObjectStart();
            foreach (PropertyMetadata p_data in props)
            {
                JsonEnumOptions currEnumOption = enumOption;
                if (p_data.EnumOption.HasValue)
                    currEnumOption = p_data.EnumOption.Value;
                if (p_data.IsField)
                {
                    writer.WritePropertyName(p_data.Name);
                    WriteValue(((FieldInfo)p_data.Info).GetValue(obj),
                        writer, writer_is_private, depth + 1, currEnumOption);
                }
                else
                {
                    PropertyInfo p_info = (PropertyInfo)p_data.Info;
                    if (p_info.CanRead)
                    {
                        writer.WritePropertyName(p_data.Name);
                        WriteValue(p_info.GetValue(obj, null),
                            writer, writer_is_private, depth + 1, currEnumOption);
                    }
                }
            }
            writer.WriteObjectEnd();
        }
        #endregion

        public static JsonEnumOptions JsonEnumOption
        {
            get { return json_enum_option; }
            set { json_enum_option = value; }
        }

        public static string ToJson(object obj, bool prettyPrint = false)
        {
            lock (static_writer_lock)
            {
                static_writer.Reset();
                static_writer.PrettyPrint = prettyPrint;

                WriteValue(obj, static_writer, true, 0, JsonEnumOption);

                return static_writer.ToString();
            }
        }

        public static void ToJson(object obj, JsonWriter writer)
        {
            WriteValue(obj, writer, false, 0, JsonEnumOption);
        }

        public static JsonData ToObject(JsonReader reader)
        {
            return (JsonData)ToWrapper(
                delegate { return new JsonData(); }, reader);
        }

        public static JsonData ToObject(TextReader reader)
        {
            JsonReader json_reader = new JsonReader(reader);

            return (JsonData)ToWrapper(
                delegate { return new JsonData(); }, json_reader);
        }

        public static JsonData ToObject(string json)
        {
            return (JsonData)ToWrapper(
                delegate { return new JsonData(); }, json);
        }

        public static T ToObject<T>(JsonReader reader)
        {
            return (T)ReadValue(typeof(T), reader, JsonEnumOption);
        }

        public static T ToObject<T>(TextReader reader)
        {
            JsonReader json_reader = new JsonReader(reader);

            return (T)ReadValue(typeof(T), json_reader, JsonEnumOption);
        }

        public static T ToObject<T>(string json)
        {
            JsonReader reader = new JsonReader(json);

            return (T)ReadValue(typeof(T), reader, JsonEnumOption);
        }

        public static object ToObject(TextReader reader, Type type)
        {
            JsonReader json_reader = new JsonReader(reader);

            return ReadValue(type, json_reader, JsonEnumOption);
        }

        public static object ToObject(string json, Type type)
        {
            JsonReader reader = new JsonReader(json);

            return ReadValue(type, reader, JsonEnumOption);
        }

        public static T ReadObjectTo<T>(TextReader reader, T instance) where T : class
        {
            return ReadObjectTo(new JsonReader(reader), instance);
        }

        public static T ReadObjectTo<T>(string json, T instance) where T : class
        {
            return ReadObjectTo(new JsonReader(json), instance);
        }

        static T ReadObjectTo<T>(JsonReader json_reader, T instance) where T : class
        {
            json_reader.Read();
            switch (json_reader.Token)
            {
                case JsonToken.ObjectStart:
                    ReadObjectTo(typeof(T), json_reader, instance, JsonEnumOption);
                    return instance;
                case JsonToken.None:
                    return null;
                default:
                    throw new JsonException("Invalid token : " + json_reader.Token.ToString());
            }
        }

        public static IJsonWrapper ToWrapper(WrapperFactory factory,
                                              JsonReader reader)
        {
            return ReadValue(factory, reader);
        }

        public static IJsonWrapper ToWrapper(WrapperFactory factory,
                                              string json)
        {
            JsonReader reader = new JsonReader(json);

            return ReadValue(factory, reader);
        }

        public static void RegisterExporter<T>(ExporterFunc<T> exporter)
        {
            ExporterFunc exporter_wrapper =
            delegate (object obj, JsonWriter writer)
            {
                exporter((T)obj, writer);
            };

            custom_exporters_table[typeof(T)] = exporter_wrapper;
        }

        public static void RegisterImporter<TJson, TValue>(
            ImporterFunc<TJson, TValue> importer)
        {
            ImporterFunc importer_wrapper =
            delegate (object input)
            {
                return importer((TJson)input);
            };

            RegisterImporter(custom_importers_table, typeof(TJson),
                              typeof(TValue), importer_wrapper);
        }

        public static void UnregisterExporters()
        {
            custom_exporters_table.Clear();
        }

        public static void UnregisterImporters()
        {
            custom_importers_table.Clear();
        }
    }
}
