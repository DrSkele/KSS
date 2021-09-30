﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Component
{
	private static T instance;

	public static T Instance
	{
		get
		{
			if (IsQuit)
			{
				throw new System.ObjectDisposedException($"Instance was called when the application quit\nTo avoid error, add 'if ({typeof(T).Name}.IsQuit) return;'");
			}
			if (instance == null)
			{
				instance = FindObjectOfType<T>();
				if (instance == null)
				{
					GameObject obj = new GameObject();
					obj.name = typeof(T).Name;
					instance = obj.AddComponent<T>();
				}
			}
			return instance;
		}
	}

	public static bool IsQuit;

	private void OnApplicationQuit()
	{
		IsQuit = true;
	}
	private void OnDestroy()
	{
		if (instance != null && instance == (this as T))
			instance = null;
	}
	private void Awake()
    {
		if (instance == null || instance == this)
		{
			instance = this as T;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}

		_Awake();
    }

    protected virtual void _Awake()
	{
		
	}
}
