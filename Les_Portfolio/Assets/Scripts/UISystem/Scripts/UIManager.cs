using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UISystem
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; set; }
        public UIView CurrentView { get { return GetCurrentLayer(UILayerTypes.View) as UIView; } }
        public List<UILayer> CurrentPopups { get { return GetCurrentLayers(UILayerTypes.Popup); } }
        public List<UILayer> CurrentHeaders { get { return GetCurrentLayers(UILayerTypes.Header); } }
        public List<UILayer> CurrentFooters { get { return GetCurrentLayers(UILayerTypes.Footer); } }
        public bool Loading { get { return numLoading > 0; } }
        public int TargetFrameRate = 60;
        public string InitialView = null;

        protected virtual string PrefabRoot { get { return "Prefabs/"; } }
        protected Transform ViewRoot { get { return layerRoots[UILayerTypes.View]; } }
        static string initialViewName = null;
        protected static object viewParam = null;
        List<UILayer> activeLayers = new List<UILayer>();
        Dictionary<UILayerTypes, Transform> layerRoots = new Dictionary<UILayerTypes, Transform>();
        bool escapePressed = false;
        int numLoading = 0;


        protected virtual void OnAwakeUI() { }
        protected virtual void OnDestroyUI() { }
        protected virtual void OnUpdateUI() { }
        protected virtual void OnBeginLoading() { }
        protected virtual void OnEndLoading() { }
        internal void OnBeginLoading(UILayer layer)
        {
            if (++numLoading == 1)
                OnBeginLoading();
        }

        internal void OnEndLoading(UILayer layer)
        {
            if (--numLoading == 0)
                OnEndLoading();
        }

        void Awake()
        {
            Instance = this;

            UpdateTargetFrameRate();
            InitLayerRoots();
            CollectActiveLayers(UILayerTypes.Header);
            CollectActiveLayers(UILayerTypes.Footer);

            List<UIView> views = new List<UIView>();
            Transform viewRoot = layerRoots[UILayerTypes.View];
            int childCount = viewRoot.transform.childCount;
            for (int i = 0; i < childCount; ++i)
            {
                UIView view = viewRoot.GetChild(i).GetComponent<UIView>();
                if (view != null)
                    views.Add(view);
            }

            UIView initialView = null;
            string viewName = initialViewName ?? InitialView;
            object viewParam = UIManager.viewParam;

            if (string.IsNullOrEmpty(viewName))
            {
                initialView = views.Find(v => v.gameObject.activeSelf);
                if (initialView == null && views.Count > 0)
                    initialView = views[0];
            }
            else
            {
                initialView = View(viewName);
            }
            views.FindAll(v => v != initialView).ForEach(v => v.HideLayer());

            // DataHttp.OnEndDataSync += OnDataSync;

            OnAwakeUI();

            if (initialView == null)
            {
                Debug.LogWarning("No initial view");
            }
            else
            {
                initialView.gameObject.SetActive(true);
                initialView.SendMessage("Show", viewParam, SendMessageOptions.RequireReceiver);
            }

            UIManager.initialViewName = null;
            UIManager.viewParam = null;
        }

        public void InitLayerRoots()
        {
            foreach (UILayerTypes type in Enum.GetValues(typeof(UILayerTypes)))
            {
                if (type != UILayerTypes.Default)
                    layerRoots[type] = CreateMissingGameObject(type.ToString()).transform;
            }
        }

        protected Transform GetLayerRoot(UILayerTypes layerType)
        {
            return layerRoots[layerType];
        }

        void OnDestroy()
        {
            Instance = null;
            // DataHttp.OnEndDataSync -= OnDataSync;
            OnDestroyUI();
        }

        protected virtual void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                if (!escapePressed)
                {
                    escapePressed = true;
                    UILayer layer = activeLayers.FindLast(l => l.LayerType == UILayerTypes.Popup) ?? CurrentView;
                    if (layer != null)
                        layer.OnEscapePressed();
                }
            }
            else
            {
                escapePressed = false;
            }

            OnUpdateUI();
        }

        void CollectActiveLayers(UILayerTypes layerType)
        {
            Transform root = layerRoots[layerType];
            int childCount = root.transform.childCount;
            for (int i = 0; i < childCount; ++i)
            {
                Transform child = root.GetChild(i);
                if (child.gameObject.activeSelf)
                {
                    UILayer layer = child.GetComponent<UILayer>();
                    if (layer != null)
                    {
                        layer.ShowLayer();
                    }
                }
            }
        }

        protected GameObject CreateMissingGameObject(string name)
        {
            Transform t = transform.Find(name);
            if (t == null)
            {
                t = new GameObject(name).transform;
                CommonUtils.SetParent(t, transform);
            }
            return t.gameObject;
        }

        protected T CreateMissingLayer<T>(UILayerTypes type, string name) where T : Component
        {
            Transform root = layerRoots[type];
            Transform child = root.Find(name);
            if (child != null)
                return child.GetComponent<T>();

            GameObject go;
            try
            {
                go = Instantiate(AddressableManager.Instance.GetPopup(name));
            }
            catch (Exception)
            {
                string path = string.Concat(PrefabRoot, type.ToString(), "/", name);
                // Debug.Log("path = " + path);
                go = Instantiate(Resources.Load<GameObject>(path));
            }

            go.SetActive(false);
            go.name = name;
            T comp = go.GetComponent<T>();
            CommonUtils.SetParent(go.transform, root);
            return comp;
        }

        protected bool IsExists<T>(UILayerTypes type, string name) where T : Component
        {
            Transform root = layerRoots[type];
            Transform child = root.Find(name);
            return child != null && child.GetComponent<T>() != null;
        }

        public UILayer Header(string name)
        {
            return Header<UILayer>(name);
        }

        public T Header<T>() where T : UILayer
        {
            return Header<T>(typeof(T).Name);
        }

        public T Header<T>(string name) where T : UILayer
        {
            return CreateMissingLayer<T>(UILayerTypes.Header, name);
        }

        public UILayer Footer(string name)
        {
            return Footer<UILayer>(name);
        }

        public T Footer<T>() where T : UILayer
        {
            return Footer<T>(typeof(T).Name);
        }

        public T Footer<T>(string name) where T : UILayer
        {
            return CreateMissingLayer<T>(UILayerTypes.Footer, name);
        }

        public UIPopup Popup(string name)
        {
            return Popup<UIPopup>(name);
        }

        public T Popup<T>() where T : UIPopup
        {
            return Popup<T>(typeof(T).Name);
        }

        public T Popup<T>(string name) where T : UIPopup
        {
            return CreateMissingLayer<T>(UILayerTypes.Popup, name);
        }

        public T Popup<T>(bool topMost) where T : UIPopup
        {
            T popup = Popup<T>(typeof(T).Name);
            popup.TopMost = topMost;
            return popup;
        }

        public UIView View(string name)
        {
            return View<UIView>(name);
        }

        public T View<T>() where T : UIView
        {
            return View<T>(typeof(T).Name);
        }

        public T View<T>(string name) where T : UIView
        {
            return CreateMissingLayer<T>(UILayerTypes.View, name);
        }

        public bool IsViewLoaded(string name)
        {
            return IsViewLoaded<UIView>(name);
        }

        public bool IsViewLoaded<T>() where T : UIView
        {
            return IsViewLoaded<T>(typeof(T).Name);
        }

        public bool IsViewLoaded<T>(string name) where T : UIView
        {
            return IsExists<T>(UILayerTypes.View, name);
        }

        public bool IsPopupLoaded(string name)
        {
            return IsPopupLoaded<UIPopup>(name);
        }

        public bool IsPopupLoaded<T>() where T : UIPopup
        {
            return IsPopupLoaded<T>(typeof(T).Name);
        }

        public bool IsPopupLoaded<T>(string name) where T : UIPopup
        {
            return IsExists<T>(UILayerTypes.Popup, name);
        }

        public virtual void GoBack(object param = null)
        {
            Application.Quit();
        }

        public void LoadLevel(string levelName, object param = null)
        {
            viewParam = param;
            SceneManager.LoadScene(levelName);
        }

        public void LoadLevel(string levelName, string viewName, object param)
        {
            initialViewName = viewName;
            viewParam = param;
            SceneManager.LoadScene(levelName);
        }

        public AsyncOperation LoadLevelAsync(string levelName, string viewName, object param, LoadSceneMode mode = LoadSceneMode.Single)
        {
            initialViewName = viewName;
            viewParam = param;
            return SceneManager.LoadSceneAsync(levelName, mode);
        }

        public void SendMessageToLayers(string methodName, object value = null, SendMessageOptions options = SendMessageOptions.DontRequireReceiver)
        {
            activeLayers.ForEach(layer => layer.SendMessage(methodName, value, options));
        }

        private UILayer GetCurrentLayer(UILayerTypes type)
        {
            return activeLayers.FindLast(l => l.LayerType == type);
        }

        private List<UILayer> GetCurrentLayers(UILayerTypes type)
        {
            return activeLayers.FindAll(l => l.LayerType == type);
        }

        private void SetCurrentLayers(UILayerTypes type, string layerName)
        {
            List<string> layerNames = new List<string>();

            if (!string.IsNullOrEmpty(layerName))
                layerNames.AddRange(layerName.Split(','));

            layerNames = layerNames.ConvertAll<string>(l => l.Trim());
            layerNames.RemoveAll(l => string.IsNullOrEmpty(l));

            foreach (UILayer old in GetCurrentLayers(type))
            {
                if (old.name != layerName)
                    old.HideLayer();
            }

            foreach (string n in layerNames)
            {
                CreateMissingLayer<UILayer>(type, n).ShowLayer();
            }
        }

        protected virtual void OnShowPopup(UILayer popup, UILayer oldTop)
        {
        }

        protected virtual void OnHidePopup(UILayer popup, UILayer newTop)
        {
        }

        internal void ShowLayer(UILayer layer)
        {
            OnShowLayer(layer);
        }

        internal void HideLayer(UILayer layer)
        {
            OnHideLayer(layer);
        }

        public void UpdateTargetFrameRate()
        {
            int targetFrameRate = TargetFrameRate;
            foreach (var layer in activeLayers)
            {
                targetFrameRate = Mathf.Max(targetFrameRate, layer.TargetFrameRate);
            }
            if (Application.targetFrameRate != targetFrameRate)
                Application.targetFrameRate = targetFrameRate;
        }

        protected virtual void OnShowLayer(UILayer layer)
        {
            if (activeLayers.Contains(layer))
                activeLayers.Remove(layer);

            if (layer.LayerType == UILayerTypes.Popup)
            {
                UIPopup popup = layer as UIPopup;
                int topMostIndex = activeLayers.FindIndex(l => l.LayerType == UILayerTypes.Popup && (l as UIPopup).TopMost);
                if (!popup.TopMost && topMostIndex >= 0)
                {
                    activeLayers.Insert(topMostIndex, layer);
                    OnShowPopup(activeLayers.FindLast(l => l.LayerType == UILayerTypes.Popup), layer);
                }
                else
                {
                    UILayer old = activeLayers.FindLast(l => l.LayerType == UILayerTypes.Popup) ?? CurrentView;
                    activeLayers.Add(layer);
                    OnShowPopup(layer, old);
                }
            }
            else
            {
                activeLayers.Add(layer);
            }

            if (layer.LayerType == UILayerTypes.View)
            {
                UIView view = layer as UIView;
                //SetCurrentLayers(UILayerTypes.Background, view.Background);
                SetCurrentLayers(UILayerTypes.Header, view.Header);
                SetCurrentLayers(UILayerTypes.Footer, view.Footer);
                foreach (UILayer old in GetCurrentLayers(UILayerTypes.View))
                {
                    if (old != layer)
                        old.HideLayer();
                }

                UILayer popup = activeLayers.FindLast(l => l.LayerType == UILayerTypes.Popup);
                if (popup != null)
                    OnShowPopup(popup, layer);
            }

            UpdateTargetFrameRate();
            layer.gameObject.SetActive(true);
        }

        protected virtual void OnHideLayer(UILayer layer)
        {
            layer.gameObject.SetActive(false);
            activeLayers.Remove(layer);
            if (layer.LayerType == UILayerTypes.View && CurrentView == null)
            {
                //SetCurrentLayers(UILayerTypes.Background, null);
                SetCurrentLayers(UILayerTypes.Header, null);
                SetCurrentLayers(UILayerTypes.Footer, null);
            }

            if (layer.LayerType == UILayerTypes.Popup)
            {
                OnHidePopup(layer, activeLayers.FindLast(l => l.LayerType == UILayerTypes.Popup) ?? CurrentView);
            }

            UpdateTargetFrameRate();
        }

        private string SearchPrefabFilePath(string path, string fileName)
        {
            string prefabPath = "";

            DirectoryInfo dInfo = new DirectoryInfo(path);
            foreach (FileInfo f in dInfo.GetFiles("*.prefab"))
            {
                if (Path.GetFileNameWithoutExtension(f.Name) == fileName)
                {
                    prefabPath = Path.GetDirectoryName(f.FullName);
                    break;
                }
            }

            if (prefabPath == "")
            {
                foreach (DirectoryInfo d2Info in dInfo.GetDirectories())
                {
                    prefabPath = SearchPrefabFilePath(d2Info.FullName, fileName);
                    if (prefabPath != "")
                        break;
                }
            }

            return prefabPath;
        }

        public void AllHidePopup()
        {
            CurrentPopups.ForEach(p => p.HideLayer());
        }

        public bool IsExistAcivePopup()
        {
            //LoadingPopup 은 제외
            List<UILayer> acivePopups = CurrentPopups.FindAll(p => p != null && p.gameObject.activeInHierarchy);
            if (acivePopups.Count > 0)
            {
                acivePopups.RemoveAll(p => p.name == "LoadingPopup");
                return acivePopups.Count > 0;
            }

            return false;
        }

        public bool IsActivePopup(string name)
        {
            List<UILayer> acivePopups = CurrentPopups.FindAll(p => p != null && p.gameObject.activeInHierarchy);
            if (acivePopups.Count > 0)
            {
                return acivePopups.Find(f => f.name == name);
            }

            return false;
        }
    }
}