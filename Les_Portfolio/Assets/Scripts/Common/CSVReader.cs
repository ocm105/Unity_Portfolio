using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SimpleJSONUtil;
using System.Text;

public static class CSVBuilder
{
    public static string GetCsvLine(params object[] args)
    {
        StringBuilder builder = new StringBuilder();

        for (int i = 0; i < args.Length; i++)
        {
            builder.Append(args[i]);

            if (i != args.Length - 1)
            {
                builder.Append(',');
            }
        }

        return builder.ToString();
    }

    public static string GetVector2(Vector2 value)
    {
        return string.Format("\"({0}, {1})\"", value.x, value.y);
    }

    public static string GetVector2List(List<Vector2> valueList)
    {
        string value = string.Empty;
        for (int i = 0; i < valueList.Count; i++)
        {
            value += CSVBuilder.GetVector2(valueList[i]);

            if (i != valueList.Count - 1)
            {
                value += ",";
            }
        }

        value = value.Replace("\"", "");
        value = value.Insert(0, "\"{");
        value = value.Insert(value.Length, "}\"");

        return value;
    }

    public static string GetVector3List(List<Vector3> valueList)
    {
        string value = string.Empty;
        for (int i = 0; i < valueList.Count; i++)
        {
            value += CSVBuilder.GetVector3(valueList[i]);

            if (i != valueList.Count - 1)
            {
                value += ",";
            }
        }

        value = value.Replace("\"", "");
        value = value.Insert(0, "\"{");
        value = value.Insert(value.Length, "}\"");

        return value;
    }

    public static string GetVector3(Vector3 value)
    {
        return string.Format("\"({0}, {1}, {2})\"", value.x, value.y, value.z);
    }

    public static string GetQuaternion(Quaternion value)
    {
        return string.Format("\"({0}, {1}, {2})\"", value.x, value.y, value.z);
    }

    public static string GetColor(Color color)
    {
        return string.Format("\"<{0}, {1}, {2}, {3}>\"", color.r, color.g, color.b, color.a);
    }

    public static string GetString(string value)
    {
        return string.Format("\"{0}\"", value);
    }

    public static string GetFloat3(Vector3 value)
    {
        return string.Format("{0}, {1}, {2}", value.x, value.y, value.z);
    }

    public static string GetInt5(int[] value)
    {
        return string.Format("{0}, {1}, {2}, {3}, {4}", value[0], value[1], value[2], value[3], value[4]);
    }
}

public class CSVReader
{

    public static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    public static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    public static char[] TRIM_CHARS = { '\"', '{', '}' };
    static System.Globalization.CultureInfo CULTURE_INFO = System.Globalization.CultureInfo.CurrentCulture;

    public static List<Dictionary<string, object>> Read(string file)
    {
        var list = new List<Dictionary<string, object>>();
        TextAsset data = Resources.Load(file) as TextAsset;

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);

        if (lines.Length <= 1) return list;

        var header = Regex.Split(lines[0], SPLIT_RE);
        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;

            var entry = new Dictionary<string, object>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                object finalvalue = value;
                int n;
                float f;
                if (int.TryParse(value, out n))
                {
                    finalvalue = n;
                }
                else if (float.TryParse(value, out f))
                {
                    finalvalue = f;
                }
                entry[header[j]] = finalvalue;
            }
            list.Add(entry);
        }
        return list;
    }

    public struct FieldTypeActionData
    {
        public Type mType;
        public string mFieldName;
        public Action<Type, string, string, JSONNode> mAction;
    }

    public static Dictionary<int, T> ReadFromResource<T>(string data)
    {
        // TextAsset data = Resources.Load(data) as TextAsset;
        if (data == null) return null;
        string[] lines = Regex.Split(data, LINE_SPLIT_RE);
        int lineCount = lines.Length;
        if (lineCount <= 1) return null;

        Dictionary<int, T> dictionary = new Dictionary<int, T>();

        string headLine = lines[0];

        List<string> header = GetColumn(headLine);
        List<string> values;

        List<FieldTypeActionData> fieldTypeList = GetFieldTypeList(typeof(T), header);
        int headerCount = header.Count;

        for (int i = 1; i < lines.Length; i++)
        {
            string key, value;

            JSONNode js = JSON.Parse("{}");

            values = GetColumn(lines[i]);

            int keyIndex = 0;
            int valueCount = values.Count;

            if (values.Count >= headerCount)
            {
                for (int j = 0; j < headerCount; j++)
                {
                    if (fieldTypeList[j].mAction == null)
                    {
                        continue;
                    }

                    key = header[j];
                    value = values[j];

                    //value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                    value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\n", "\n").Replace("\\", "");

                    fieldTypeList[j].mAction(fieldTypeList[j].mType, key, value, js);

                    if (j == 0)
                    {
                        keyIndex = js[key].AsInt;
                        if (keyIndex == 0)
                        {
                            Debug.Log("Invalid TableIndex(0)!!! : " + data);
                            return dictionary;
                        }
                    }
                }

                value = js.ToString();
                dictionary[keyIndex] = JsonUtility.FromJson<T>(value);
            }
            else
            {
                // 컬럼 매칭 실패
                //Debug.LogError(file + " /// " + "Error : " + headerCount + " /// " + values.Count);
            }
        }

        fieldTypeList.Clear();

        fieldTypeList = null;
        return dictionary;
    }

    public static Dictionary<int, T> ReadFromFolderResource<T>(string file)
    {
        string data = File.ReadAllText(file + ".csv");
        if (data == null) return null;
        string[] lines = Regex.Split(data, LINE_SPLIT_RE);
        int lineCount = lines.Length;
        if (lineCount <= 1) return null;

        Dictionary<int, T> dictionary = new Dictionary<int, T>();

        string headLine = lines[0];

        List<string> header = GetColumn(headLine);
        List<string> values;

        List<FieldTypeActionData> fieldTypeList = GetFieldTypeList(typeof(T), header);
        int headerCount = header.Count;

        for (int i = 1; i < lines.Length; i++)
        {
            string key, value;

            JSONNode js = JSON.Parse("{}");

            values = GetColumn(lines[i]);

            int keyIndex = 0;
            int valueCount = values.Count;

            if (values.Count >= headerCount)
            {
                for (int j = 0; j < headerCount; j++)
                {
                    if (fieldTypeList[j].mAction == null)
                    {
                        continue;
                    }

                    key = header[j];
                    value = values[j];

                    //value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                    value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\n", "\n").Replace("\\", "");

                    fieldTypeList[j].mAction(fieldTypeList[j].mType, key, value, js);

                    if (j == 0)
                    {
                        keyIndex = js[key].AsInt;
                        if (keyIndex == 0)
                        {
                            Debug.Log("Invalid TableIndex(0)!!! : " + file);
                            return dictionary;
                        }
                    }
                }

                value = js.ToString();
                dictionary[keyIndex] = JsonUtility.FromJson<T>(value);
            }
            else
            {
                // 컬럼 매칭 실패
                //Debug.LogError(file + " /// " + "Error : " + headerCount + " /// " + values.Count);
            }
        }

        fieldTypeList.Clear();

        fieldTypeList = null;
        return dictionary;
    }

    public static string[] ReadCSV(string file)
    {
        StreamReader streamReader = null;
        try
        {
            streamReader = new StreamReader(file, System.Text.Encoding.UTF8, true);
        }
        catch (Exception e)
        {
            Debug.Log(e + " : File not exist!!!");
            return null;
        }
        if (streamReader == null)
        {
            Debug.LogErrorFormat("CSVUtil.Read() : {0} file read error!", file);
            return null;
        }
        string streamText = streamReader.ReadToEnd();
        streamReader.Close();

        string[] lines = Regex.Split(streamText, LINE_SPLIT_RE);

        return lines;
    }

    public static List<string> GetColumn(string line)
    {
        List<string> column = new List<string>();
        char splitArray = '\"';
        char splitComma = ',';
        char splitBar = '|';

        if (line.Contains("\"") == false)
        {
            return new List<string>(line.Split(','));
        }
        else
        {
            int startCommaIndex = 0;
            int startArrayIndex = 0;
            bool checkArray = false;

            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == splitArray)
                {
                    // check array
                    if (startArrayIndex == 0)
                    {
                        checkArray = true;
                        startArrayIndex = i;
                    }
                    else
                    {
                        checkArray = false;
                    }
                }
                else if (line[i] == splitBar)
                {
                    startCommaIndex = i + 1;
                    break;
                }
                else if (checkArray == false && line[i] == splitComma)
                {
                    // add array
                    if (startArrayIndex != 0)
                    {
                        column.Add(line.Substring(startArrayIndex, i - startArrayIndex));
                        startArrayIndex = 0;
                        startCommaIndex = i + 1;

                        continue;
                    }

                    // add column
                    column.Add(line.Substring(startCommaIndex, i - startCommaIndex));
                    startCommaIndex = i + 1;
                }
            }

            // end column
            column.Add(line.Substring(startCommaIndex));
        }

        return column;
    }

    public static List<FieldTypeActionData> GetFieldTypeList(Type targetType, List<string> headerList)
    {
        List<FieldTypeActionData> result = new List<FieldTypeActionData>();
        FieldTypeActionData data;

        System.Reflection.FieldInfo[] fieldInfoList = targetType.GetFields();
        List<string> memberFieldNames = new List<string>();
        List<Type> memberFieldTypes = new List<Type>();

        for (int i = 0; i < fieldInfoList.Length; i++)
        {
            // 필드명
            memberFieldNames.Add(fieldInfoList[i].Name);
            // 필드 타입
            memberFieldTypes.Add(fieldInfoList[i].FieldType);
        }

        for (int i = 0; i < headerList.Count; i++)
        {
            int index = memberFieldNames.FindIndex(c => c.ToLower() == headerList[i].ToLower());
            bool checkField = index != -1;

            // 해당 필드의 타입 설정
            if (checkField == true)
            {
                data = new FieldTypeActionData();
                data.mType = memberFieldTypes[index];
                data.mFieldName = memberFieldNames[index];
                AddTypeAction(data.mType, ref data.mAction);
                result.Add(data);
            }
            else
            {
                data = new FieldTypeActionData();
                result.Add(data);
            }
        }

        return result;
    }

    public static void AddTypeAction(Type fieldType, ref Action<Type, string, string, JSONNode> actionList)
    {
        if (fieldType == typeof(int))
        {
            actionList = SetInt;
        }
        else if (fieldType == typeof(long))
        {
            actionList = SetLong;
        }
        else if (fieldType == typeof(byte))
        {
            actionList = SetByte;
        }
        else if (fieldType == typeof(string))
        {
            actionList = SetString;
        }
        else if (fieldType == typeof(float))
        {
            actionList = SetFloat;
        }
        else if (fieldType.IsArray)
        {
            if (fieldType == typeof(Vector2[]))
            {
                actionList = SetVector2Array;
            }
            else if (fieldType == typeof(int[]))
            {
                actionList = SetIntArray;
            }
            else if (fieldType == typeof(float[]))
            {
                actionList = SetFloatArray;
            }
            else if (fieldType.GetElementType() != null && fieldType.GetElementType().IsEnum)
            {
                actionList = SetEnumArray;
            }
            else
            {
                actionList = SetDefaultArray;
            }
        }
        else if (fieldType.IsEnum)
        {
            actionList = SetEnum;
        }
        else if (fieldType == typeof(bool))
        {
            actionList = SetBool;
        }
        else if (fieldType == typeof(Vector3) || fieldType == typeof(Vector2))
        {
            actionList = SetVector;
        }
        else if (fieldType == typeof(double))
        {
            actionList = SetDouble;
        }
        else if (fieldType == typeof(short))
        {
            actionList = SetShort;
        }
        else if (fieldType == typeof(ushort))
        {
            actionList = SetUshort;
        }
        else
        {
            actionList = null;
            Debug.LogError("정의되지 않은 타입입니다.");
        }
    }

    private static void SetString(Type fieldType, string key, string value, JSONNode node)
    {
        node[key] = value;
    }

    private static void SetInt(Type fieldType, string key, string value, JSONNode node)
    {
        int result = 0;
        int.TryParse(value, out result);
        node[key].AsInt = result;
    }

    private static void SetLong(Type fieldType, string key, string value, JSONNode node)
    {
        long result = 0;
        long.TryParse(value, out result);
        node[key].AsLong = result;
    }

    private static void SetBool(Type fieldType, string key, string value, JSONNode node)
    {
        bool result = false;
        bool.TryParse(value, out result);
        node[key].AsBool = result;
    }

    private static void SetDouble(Type fieldType, string key, string value, JSONNode node)
    {
        double result = 0.0f;
        double.TryParse(value, System.Globalization.NumberStyles.Float, CULTURE_INFO, out result);
        node[key].AsDouble = result;
    }

    private static void SetByte(Type fieldType, string key, string value, JSONNode node)
    {
        byte result = 0;
        byte.TryParse(value, out result);
        node[key].AsByte = result;
    }

    private static void SetFloat(Type fieldType, string key, string value, JSONNode node)
    {
        float result = 0.0f;
        float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CurrentCulture, out result);
        node[key].AsFloat = result;
    }

    private static void SetUshort(Type fieldType, string key, string value, JSONNode node)
    {
        ushort result = 0;
        ushort.TryParse(value, out result);
        node[key].AsUshort = result;
    }

    private static void SetShort(Type fieldType, string key, string value, JSONNode node)
    {
        short result = 0;
        short.TryParse(value, out result);
        node[key].AsShort = result;
    }

    private static void SetEnum(Type fieldType, string key, string value, JSONNode node)
    {
        if (Enum.IsDefined(fieldType, value) == true)
        {
            node[key].AsInt = (int)Enum.Parse(fieldType, value);
        }
    }

    private static void SetVector(Type fieldType, string key, string value, JSONNode node)
    {
        string[] elements = value.TrimStart('(').TrimEnd(')').Split(',');
        switch (elements.Length)
        {
            case 2:
                node[key].AsJson = new Vector2(ToFloat(elements[0]), ToFloat(elements[1]));
                break;
            case 3:
                node[key].AsJson = new Vector3(ToFloat(elements[0]), ToFloat(elements[1]), ToFloat(elements[2]));
                break;
            case 4:
                node[key].AsJson = new Vector4(ToFloat(elements[0]), ToFloat(elements[1]), ToFloat(elements[2]), ToFloat(elements[3]));
                break;
        }
    }

    private static void SetIntArray(Type fieldType, string key, string value, JSONNode node)
    {
        string[] elements = value.TrimStart('{').TrimEnd('}').Split(',');
        if (elements.Length > 0)
        {
            int n;
            int[] arr = new int[elements.Length];
            for (int k = 0; k < elements.Length; ++k)
            {
                if (int.TryParse(elements[k], out n))
                    arr[k] = n;
            }
            node[key].AsJson = new IntArray(arr);

            int startIndex = node[key].Value.IndexOf('[');
            int endIndex = node[key].Value.IndexOf(']') - startIndex;
            node[key].Value = node[key].Value.Substring(startIndex, endIndex + 1);
        }
    }

    private static void SetFloatArray(Type fieldType, string key, string value, JSONNode node)
    {
        string[] elements = value.TrimStart('{').TrimEnd('}').Split(',');
        if (elements.Length > 0)
        {
            float f;
            float[] arr = new float[elements.Length];
            for (int k = 0; k < elements.Length; ++k)
            {
                if (float.TryParse(elements[k], out f))
                    arr[k] = f;
            }
            node[key].AsJson = new FloatArray(arr);

            int startIndex = node[key].Value.IndexOf('[');
            int endIndex = node[key].Value.IndexOf(']') - startIndex;
            node[key].Value = node[key].Value.Substring(startIndex, endIndex + 1);
        }
    }

    private static void SetEnumArray(Type fieldType, string key, string value, JSONNode node)
    {
        string[] elements = value.TrimStart('{').TrimEnd('}').Split(',');
        if (elements.Length > 0)
        {
            int[] arr = new int[elements.Length];
            string elementValue;
            for (int k = 0; k < elements.Length; ++k)
            {
                elementValue = elements[k].Trim();
                if (Enum.IsDefined(fieldType.GetElementType(), elementValue) == true)
                    arr[k] = (int)Enum.Parse(fieldType.GetElementType(), elementValue);
            }
            node[key].AsJson = new IntArray(arr);

            int startIndex = node[key].Value.IndexOf('[');
            int endIndex = node[key].Value.IndexOf(']') - startIndex;
            node[key].Value = node[key].Value.Substring(startIndex, endIndex + 1);
        }
    }

    private static void SetDefaultArray(Type fieldType, string key, string value, JSONNode node)
    {
        string[] elements = value.TrimStart('{').TrimEnd('}').Split(',');
        if (elements.Length > 0)
        {
            node[key].AsJson = new StringArray(elements);

            int startIndex = node[key].Value.IndexOf('[');
            int endIndex = node[key].Value.IndexOf(']') - startIndex;
            node[key].Value = node[key].Value.Substring(startIndex, endIndex + 1);
        }
    }

    private static void SetVector2Array(Type fieldType, string key, string value, JSONNode node)
    {
        string[] elements = value.Replace("(", "").Replace(")", "").TrimStart('{').TrimEnd('}').Split(',');
        int length = elements.Length / 2;
        Vector2[] vectorList = new Vector2[length];

        int xCount = 0;
        int yCount = 0;

        for (int i = 0; i < elements.Length; i++)
        {
            if (i % 2 == 0)
            {
                vectorList[xCount].x = ToFloat(elements[i]);
                xCount++;
            }
            else
            {
                vectorList[yCount].y = ToFloat(elements[i]);
                yCount++;
            }
        }

        node[key].AsJson = new Vector2Array(vectorList);


        int startIndex = node[key].Value.IndexOf('[');
        int endIndex = node[key].Value.IndexOf(']') - startIndex;
        node[key].Value = node[key].Value.Substring(startIndex, endIndex + 1);
    }

    public static float ToFloat(string s)
    {
        float f;
        return float.TryParse(s, out f) ? f : 0.0f;
    }

    public static byte ToByte(string s)
    {
        byte c;
        return byte.TryParse(s, out c) ? c : (byte)0;
    }
}