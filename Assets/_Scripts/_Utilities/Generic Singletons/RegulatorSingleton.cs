using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Persistant Regulator singleton, will destroy any other older components of the same type if it finds some on awake
/// </summary>
public class RegulatorSingleton<T> : MonoBehaviour where T : Component
{
    protected static T instance;

    public static bool HasInstance => instance != null;
    public static T TryGetInstance => HasInstance ? instance : null;

    public float InitializationTime { get; private set; }

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<T>();
                if (instance == null)
                {
                    var gameObject = new GameObject($"{typeof(T).Name} - Auto Generated");
                    gameObject.hideFlags = HideFlags.HideAndDontSave;
                    instance = gameObject.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    /// <summary>
    /// Make sure to call base.Awake() in override if you need awake
    /// </summary>
    protected virtual void Awake()
    {
        InitializeSingleton();
    }

    protected virtual void InitializeSingleton()
    {
        if (!Application.isPlaying) return;
        InitializationTime = Time.time;
        DontDestroyOnLoad(gameObject);

        T[] oldInstances = FindObjectsByType<T>(FindObjectsSortMode.None);
        foreach (T old in oldInstances)
        {
            if(old.GetComponent<RegulatorSingleton<T>>().InitializationTime < InitializationTime)
            {
                Destroy(old.gameObject);
            }
        }

        if (instance == null)
        {
            instance = this as T;
        }
       
    }
}
