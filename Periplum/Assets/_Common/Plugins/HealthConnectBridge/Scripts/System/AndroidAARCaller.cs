using UnityEngine;

public abstract class AndroidAARCaller : MonoBehaviour
{
    protected AndroidJavaObject pluginInstance;

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
}