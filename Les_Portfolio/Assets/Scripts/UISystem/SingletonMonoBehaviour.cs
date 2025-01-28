using UnityEngine;
using System;


namespace UISystem
{
	public class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
	{
		public static T Instance
		{
			get
			{
				CreateMissing();
				return instance;
			}
		}

		public static bool HasInstance { get { return instance != null; } }
		public static bool IsLoadCompleted { get { return Instance.loaded; } }

		public static void CreateMissing()
		{
			if (instance == null)
			{
				Type type = typeof(T);
				instance = FindObjectOfType(type) as T;
				if (instance == null)
				{
					GameObject go = new GameObject(type.Name);
					instance = go.AddComponent<T>();
				}
				instance.Initialize();
			}
		}

		public static void DestroySingleton()
		{
			if (instance != null)
			{
				if (instance.name == typeof(T).Name)
				{
					if (Application.isEditor)
						DestroyImmediate(instance.gameObject);
					else
						Destroy(instance.gameObject);
				}
				else
				{
					if (Application.isEditor)
						DestroyImmediate(instance);
					else
						Destroy(instance);
				}
			}
		}

		private static T instance = null;
		private bool initialized = false;
		private bool loaded = false;

		protected virtual void OnInitialize() { }
		protected virtual void OnAwakeSingleton() { }
		protected virtual void OnDestroySingleton() { }

		void Initialize()
		{
			if (!initialized)
			{
				initialized = true;
				OnInitialize();
			}
		}

		void Awake()
		{
			if (instance == null)
			{
				instance = this as T;
			}
			else if (instance != this)
			{
				Destroy(this.gameObject);
				return;
			}
			Initialize();
			OnAwakeSingleton();
			loaded = true;
		}

		void OnDestroy()
		{
			if (instance == this)
			{
				instance = null;
				OnDestroySingleton();
			}
		}
	}
}