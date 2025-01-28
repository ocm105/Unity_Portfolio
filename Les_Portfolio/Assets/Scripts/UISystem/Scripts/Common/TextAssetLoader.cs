using System.IO;
using UnityEngine;



namespace UISystem
{

    public class TextAssetLoader : SingletonMonoBehaviour<TextAssetLoader>
    {


        protected override void OnAwakeSingleton()
        {
            DontDestroyOnLoad(this);
        }

        public T LoadDataInfo<T>(string fileName, bool isUpdate = false)
        {
            object objData;
            string folder = "TextAsset";
            TextAsset textAsset = null;

            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                textAsset = Resources.Load<TextAsset>(Path.Combine(folder, Path.GetFileNameWithoutExtension(fileName)));
                objData = LitJson.JsonMapper.ToObject<T>(textAsset.text);
            }
            else
            {
                string folderPath = Path.Combine(Application.persistentDataPath, folder);
                string file = Path.Combine(folderPath, fileName);

                if (File.Exists(file))
                {
                    if (!isUpdate)
                    {
                        //file read
                        string data = ReadFile(file);
                        objData = LitJson.JsonMapper.ToObject<T>(data);
                        Debug.Log("TextAssetLoader : ReadOnly file data");
                    }
                    else
                    {
                        textAsset = Resources.Load<TextAsset>(Path.Combine(folder, Path.GetFileNameWithoutExtension(fileName)));
                        objData = LitJson.JsonMapper.ToObject<T>(textAsset.text);

                        if (!Directory.Exists(folderPath))
                            Directory.CreateDirectory(folderPath);

                        //file write
                        WriteFile(file, textAsset.text);
                        Debug.Log("TextAssetLoader : Update file data");
                    }
                }
                else
                {
                    textAsset = Resources.Load<TextAsset>(Path.Combine(folder, Path.GetFileNameWithoutExtension(fileName)));
                    objData = LitJson.JsonMapper.ToObject<T>(textAsset.text);

                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    //file write
                    WriteFile(file, textAsset.text);
                    Debug.Log("TextAssetLoader : Write new file data");
                }
            }

            return (T)objData;
        }

        public static string ReadFile(string fileName)
        {
            string data = "";
            using (Stream s = File.OpenRead(fileName))
            {
                using (StreamReader sr = new StreamReader(s, System.Text.Encoding.Default))
                {
                    data = sr.ReadToEnd();
                }
            }

            return data;
        }

        public static void WriteFile(string fileName, string data)
        {
            using (Stream s = File.OpenWrite(fileName))
            {
                using (StreamWriter sw = new StreamWriter(s, System.Text.Encoding.Default))
                {
                    sw.Write(data);
                }
            }
        }
    }
}
