using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
	private static T instance = null;
	public static T Instance
    {
        get
        {
			if (instance == null)
            {
            	instance = GameObject.FindObjectOfType(typeof(T)) as T;
                if (instance == null)
                {
                    GameObject go = new GameObject(typeof(T).Name);
                    instance = go.AddComponent<T>();
                    GameObject parent = GameObject.Find("Boot");
                    if (parent == null)
                    {
                        parent = new GameObject("Boot");
                        GameObject.DontDestroyOnLoad(parent);
                    }
                    if (parent != null)
                    {
                        go.transform.parent = parent.transform;
                    }
                }
            }
            return instance;
        }
    }

    /*
     * 没有任何实现的函数，用于保证MonoSingleton在使用前已创建
     */
    public void Startup()
    {

    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
        }
        DontDestroyOnLoad(gameObject);
        Init();
    }
 
    protected virtual void Init()
    {

    }

    public void DestroySelf()
    {
        Dispose();
        MonoSingleton<T>.instance = null;
        UnityEngine.Object.Destroy(gameObject);
    }

    public virtual void Dispose()
    {

    }
}