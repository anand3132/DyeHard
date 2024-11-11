using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{

    public static bool verbose = false;
    public static bool keepAlive = true;

    private static T _instance = null;
    public static T instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<T>();
                if (_instance == null)
                {
                    var singletonObj = new GameObject();
                    singletonObj.name = typeof(T).ToString();
                    _instance = singletonObj.AddComponent<T>();
                }
            }
            return _instance;
        }
    }

    static public bool isInstanceAlive
    {
        get { return _instance != null; }
    }

    public virtual void Awake()
    {
        if (_instance != null)
        {
            if (verbose)
                Debug.Log("SingleAccessPoint, Destroy duplicate instance " + name + " of " + instance.name);
            Destroy(gameObject);
            return;
        }

        _instance = GetComponent<T>();

        if (keepAlive)
        {
            DontDestroyOnLoad(gameObject);
        }

        if (_instance == null)
        {
            if (verbose)
                Debug.LogError("SingleAccessPoint<" + typeof(T).Name + "> Instance null in Awake");
            return;
        }

        if (verbose)
            Debug.Log("SingleAccessPoint instance found " + instance.GetType().Name);

    }

}

public class SingletonSimple<T> : MonoBehaviour
    where T : Component
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                var objs = FindObjectsOfType(typeof(T)) as T[];
                if (objs.Length > 0)
                    _instance = objs[0];
                if (objs.Length > 1)
                {
                    Debug.LogError("There is more than one " + typeof(T).Name + " in the scene.");
                }
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.hideFlags = HideFlags.HideAndDontSave;
                    _instance = obj.AddComponent<T>();
                }
            }
            return _instance;
        }
    }
}

public class SingletonPersistent<T> : MonoBehaviour
    where T : Component
{
    public static T Instance { get; private set; }
    public virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

public class SingletonSafe<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    // Lock object to ensure thread safety
    private static readonly object lockObject = new object();

    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                Debug.LogWarning($"[Singleton] Instance of {typeof(T)} already destroyed (application is quitting). Returning null.");
                return null;
            }

            lock (lockObject)
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();

                    if (instance == null)
                    {
                        GameObject singletonObject = new GameObject(typeof(T).Name);
                        instance = singletonObject.AddComponent<T>();
                        DontDestroyOnLoad(singletonObject);
                    }
                }

                return instance;
            }
        }
    }

    private static bool applicationIsQuitting = false;

    // This method is called when the application is about to quit
    private void OnApplicationQuit()
    {
        applicationIsQuitting = true;
    }

    // Optional: You can also reset the singleton instance during play mode (useful for testing)
    private void OnDestroy()
    {
        instance = null;
    }
}
