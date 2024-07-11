using UnityEngine;

public abstract class AndroidAARCaller : MonoBehaviour
{
    private AndroidJavaClass unityClass;
    private AndroidJavaObject unityActivity;
    private AndroidJavaObject pluginInstance;

    protected void Awake()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Initialize("com.gabrielbernabeu.hcwforunity.Plugin");
#endif
    }

    private void Initialize(string pluginName)
    {
        unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        unityActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
        pluginInstance = new AndroidJavaObject(pluginName).GetStatic<AndroidJavaObject>("Companion");

        if (pluginInstance == null)
        {
            Debug.Log("Plugin instance error");
        }
    }

    public void Call(string methodName, params object[] args)
    {
        Call<object>(methodName, args);
    }

    public void Call<T>(string methodName, params object[] args)
    {
        if (pluginInstance != null)
        {
            Debug.Log("Plugin exists!");
            pluginInstance.Call(methodName, args);
        }
        else
            Debug.LogError("Plugin is null!");
    }
}