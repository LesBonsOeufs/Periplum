using UnityEngine;

public abstract class AndroidAARCaller : MonoBehaviour
{
    private AndroidJavaObject pluginInstance;

    protected abstract string PluginFullName { get; }

    protected void Awake()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Initialize(PluginFullName);
#endif
    }

    private void Initialize(string pluginName)
    {
        pluginInstance = new AndroidJavaObject(pluginName).GetStatic<AndroidJavaObject>("Companion");

        if (pluginInstance == null)
            Debug.LogError("No plugin instance");
    }

    public void Call(string methodName, params object[] args)
    {
        Call<object>(methodName, args);
    }

    public void Call<T>(string methodName, params T[] args)
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