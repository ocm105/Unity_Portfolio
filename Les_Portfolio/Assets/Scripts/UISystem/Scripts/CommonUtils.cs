using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public static class CommonUtils
{
    public static string GetFullPath(Transform transform, string path = null)
    {
        Transform t = transform;
        string fullPath;
        if (path != null && path.StartsWith("/"))
        {
            fullPath = path;
        }
        else
        {
            fullPath = string.Empty;
            while (t != null)
            {
                if (string.IsNullOrEmpty(fullPath))
                    fullPath = t.name;
                else
                    fullPath = string.Concat(t.name, "/", fullPath);
                t = t.parent;
            }
            if (!string.IsNullOrEmpty(path))
            {
                fullPath += string.Concat("/", path);
            }
        }
        return fullPath;
    }

    static Transform FindTransform(Transform transform, string path)
    {
        Transform t = transform.Find(path);
        if (t == null)
        {
            // Debug.LogWarningFormat("Child not found : path={0}", GetFullPath(transform, path));
            return null;
        }

        return t;
    }

    public static GameObject Find(Transform transform, string path)
    {
        Transform t = FindTransform(transform, path);
        return t == null ? null : t.gameObject;
    }

    public static GameObject Find(Transform transform, string format, params object[] args)
    {
        return Find(transform, string.Format(format, args));
    }

    public static T Find<T>(Transform transform, string path) where T : Component
    {
        Transform t = FindTransform(transform, path);
        if (t == null) return null;

        T component = t.GetComponent<T>();
        if (component == null)
        {
            Debug.LogWarningFormat("{0} not found : path={1}", typeof(T).Name, GetFullPath(transform, path));
        }

        return component;
    }

    public static T Find<T>(Transform transform, string format, params object[] args) where T : Component
    {
        return Find<T>(transform, string.Format(format, args));
    }

    public static GameObject Find(this GameObject go, string path)
    {
        return Find(go.transform, path);
    }

    public static GameObject Find(this GameObject go, string format, params object[] args)
    {
        return Find(go.transform, string.Format(format, args));
    }

    public static T Find<T>(this GameObject go, string path) where T : Component
    {
        return Find<T>(go.transform, path);
    }

    public static T Find<T>(this GameObject go, string format, params object[] args) where T : Component
    {
        return Find<T>(go.transform, string.Format(format, args));
    }

    public static GameObject Find(Component component, string path)
    {
        return Find(component.transform, path);
    }

    public static GameObject Find(Component component, string format, params object[] args)
    {
        return Find(component.transform, string.Format(format, args));
    }

    public static T Find<T>(Component component, string path) where T : Component
    {
        return Find<T>(component.transform, path);
    }

    public static T Find<T>(Component component, string format, params object[] args) where T : Component
    {
        return Find<T>(component.transform, string.Format(format, args));
    }

    public static List<GameObject> FindAll(Transform transform, string path)
    {
        List<GameObject> all = new List<GameObject>();
        int index = path.LastIndexOf('/');
        string name = index < 0 ? path : path.Substring(index + 1);

        Transform found = FindTransform(transform, path);
        if (found != null)
        {
            Transform parent = found.parent;
            if (parent != null)
            {
                int childCount = parent.childCount;
                for (int i = 0; i < childCount; ++i)
                {
                    Transform child = parent.GetChild(i);
                    if (child.name == name)
                        all.Add(child.gameObject);
                }
            }
        }

        return all;
    }

    public static List<T> FindAll<T>(Transform transform, string path) where T : Component
    {
        List<T> all = new List<T>();
        foreach (GameObject go in FindAll(transform, path))
        {
            T component = go.GetComponent<T>();
            if (component != null)
                all.Add(component);
        }

        return all;
    }

    public static List<GameObject> FindAll(this GameObject go, string path)
    {
        return FindAll(go.transform, path);
    }

    public static List<T> FindAll<T>(this GameObject go, string path) where T : Component
    {
        return FindAll<T>(go.transform, path);
    }

    public static T FindInParents<T>(this GameObject go) where T : Component
    {
        if (go == null) return null;
        Transform t = go.transform;
        while (t != null)
        {
            T comp = t.gameObject.GetComponent<T>();
            if (comp != null)
                return comp;

            t = t.parent;
        }

        return null;
    }

    public static GameObject Instantiate(GameObject original, Transform parent, string name = null)
    {
        GameObject child = UnityEngine.Object.Instantiate(original);
        if (!string.IsNullOrEmpty(name))
            child.name = name;

        Transform t = child.transform;
        t.localRotation = Quaternion.identity;
        SetParent(t, parent);

        return child;
    }

    public static void SetParent(Transform t, Transform parent)
    {
        t.SetParent(parent);
        t.localScale = Vector3.one;
        t.gameObject.layer = parent.gameObject.layer;

        RectTransform rt = t.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchoredPosition3D = Vector3.zero;
        }
        else
        {
            t.localPosition = Vector3.zero;
        }
    }

    public static List<DateTime> MapToDateTimes(List<string> dateTimeStrings)
    {
        string[] formats = { "yyyyMMdd", "yyyy-MM-dd" };
        List<DateTime> values = new List<DateTime>();

        for (int i = 0; i < dateTimeStrings.Count; i++)
        {
            DateTime dat;
            if (DateTime.TryParseExact(dateTimeStrings[i], formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dat))
            {
                values.Add(dat);
            }
            else
            {
                Debug.LogError($"DateTime.Parse 실패({dateTimeStrings[i]})");
            }
        }

        return values;
    }

    public static T CheckOrCreateComponent<T>(this T targetT, Transform parentTr, string findName) where T : Component
    {
        if (targetT == null)
        {
            targetT = parentTr.FindOrCreateComponent<T>(findName);
            return targetT;
        }

        return targetT;
    }

    public static T FindOrCreateComponent<T>(this Transform parentTr, string findName) where T : Component
    {
        GameObject findObj = GameObject.Find(findName);
        if (findObj != null)
        {
            T findT = findObj.GetComponent<T>();
            if (findT != null)
                return findT;
            else
            {
                //Create
                GameObject go = new GameObject { name = findName };
                go.transform.SetParent(parentTr);
                return go.AddComponent<T>();
            }
        }
        else
        {
            //Create
            GameObject go = new GameObject { name = findName };
            go.transform.SetParent(parentTr);
            return go.AddComponent<T>();
        }
    }

    public static void StrechRectTransformToFullScreen(this RectTransform _mRect)
    {
        _mRect.anchoredPosition3D = Vector3.zero;
        _mRect.anchorMin = Vector2.zero;
        _mRect.anchorMax = Vector2.one;
        _mRect.pivot = new Vector2(0.5f, 0.5f);
        _mRect.sizeDelta = Vector2.zero;
    }

    // 바이트 배열을 String으로 변환 
    public static string ByteToString(byte[] strByte)
    {
        string str = Encoding.Default.GetString(strByte);
        return str;
    }

    // String을 바이트 배열로 변환 
    public static byte[] StringToByte(string str)
    {
        byte[] StrByte = Encoding.UTF8.GetBytes(str);
        return StrByte;
    }

    public static void SetLayerMask(GameObject go, int layerMask)
    {
        go.layer = layerMask;
        foreach (Transform t in go.GetComponentsInChildren<Transform>(true))
        {
            t.gameObject.layer = layerMask;
        }
    }

    public static int[,] ConvertTextAssetArray(TextAsset textAsset, int row, int col)
    {
        int[,] result = new int[row, col];
        string[] dataLines = textAsset.text.Split(new char[] { '\n' });
        for (int i = 0; i < dataLines.Length; i++)
        {
            var data = dataLines[i].Split(',');
            for (int j = 0; j < data.Length; j++)
            {
                result[i, j] = int.Parse(data[j]);
            }
        }

        return result;
    }

    public static void AddListener(this EventTrigger trigger, EventTriggerType triggerType, UnityAction<BaseEventData> callback)
    {
        if (trigger == null)
        {
            Debug.LogError("EventTrigger is NULL.");
            return;
        }

        EventTrigger.Entry entry = new();
        entry.eventID = triggerType;
        entry.callback.AddListener(callback);

        trigger.triggers.Add(entry);
    }

    public static void RemoveListener(this EventTrigger trigger, EventTriggerType triggerType, UnityAction<BaseEventData> callback)
    {
        if (trigger == null)
        {
            Debug.LogError("EventTrigger is NULL.");
            return;
        }

        EventTrigger.Entry entry = trigger.triggers.Find(e => e.eventID == triggerType);
        entry?.callback.RemoveListener(callback);
    }

    public static void RemoveAllListeners(this EventTrigger trigger, EventTriggerType triggerType)
    {
        if (trigger == null)
        {
            Debug.LogError("EventTrigger is NULL.");
            return;
        }

        EventTrigger.Entry entry = trigger.triggers.Find(e => e.eventID == triggerType);
        entry?.callback.RemoveAllListeners();
    }
}

