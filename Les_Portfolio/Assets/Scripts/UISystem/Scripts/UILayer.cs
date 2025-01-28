using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace UISystem
{
    public abstract class UILayer : MonoBehaviour
    {
        public abstract UILayerTypes LayerType { get; }
        public bool DestroyOnHide = false;
        public int TargetFrameRate = -1;
        public bool Loading
        {
            get { return loading; }
            set
            {
                if (loading != value)
                {
                    loading = value;
                    if (value)
                        OnBeginLoading();
                    else
                        OnEndLoading();
                }
            }
        }

        List<Action> onEnableActions = new List<Action>();
        bool loading = false;
        bool first = true;


        protected virtual void OnAwakeLayer() { }
        protected virtual void OnEnableLayer() { }
        protected virtual void OnDisableLayer() { }
        protected virtual void OnFirstShow() { }
        protected virtual void OnShow() { }
        protected virtual void OnHide() { }
        protected virtual void OnBeginLoading()
        {
            UIManager.Instance.OnBeginLoading(this);
        }
        protected virtual void OnEndLoading()
        {
            UIManager.Instance.OnEndLoading(this);
        }



        void Awake()
        {
            OnAwakeLayer();
        }

        void OnEnable()
        {
            OnEnableLayer();
            onEnableActions.ForEach(a => a());
            onEnableActions.Clear();
        }

        void OnDisable()
        {
            Loading = false;
            OnDisableLayer();
            onEnableActions.Clear();
        }

        protected internal virtual void OnEscapePressed()
        {
        }

        protected internal virtual void ShowLayer()
        {
            if (first)
            {
                first = false;
                OnFirstShow();
            }

            OnShow();

            if (UIManager.Instance != null)
                UIManager.Instance.ShowLayer(this);
            else
                gameObject.SetActive(true);
        }

        protected internal void HideLayer()
        {
            if (UIManager.Instance != null)
                UIManager.Instance.HideLayer(this);
            else
                gameObject.SetActive(false);

            OnHide();

            if (DestroyOnHide)
            {
                Destroy(gameObject);
            }
        }

        public void ExecWhenActive(Action action)
        {
            if (gameObject.activeInHierarchy)
                action();
            else
                onEnableActions.Add(action);
        }

        public void StartCoroutineWhenActive(string methodName)
        {
            ExecWhenActive(() => StartCoroutine(methodName));
        }

        public void StartCoroutineWhenActive(string methodName, object value)
        {
            ExecWhenActive(() => StartCoroutine(methodName, value));
        }

        public void StartCoroutineWhenActive(IEnumerator routine)
        {
            ExecWhenActive(() => StartCoroutine(routine));
        }

        public GameObject Find(string path)
        {
            return CommonUtils.Find(transform, path);
        }

        public GameObject Find(string format, params object[] args)
        {
            return CommonUtils.Find(transform, format, args);
        }

        public T Find<T>(string path) where T : Component
        {
            return CommonUtils.Find<T>(transform, path);
        }

        public T Find<T>(string format, params object[] args) where T : Component
        {
            return CommonUtils.Find<T>(transform, format, args);
        }
    }
}
