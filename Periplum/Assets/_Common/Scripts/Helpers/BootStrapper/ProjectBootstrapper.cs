#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
//using UnityEngine.AddressableAssets;

public static class ProjectBootstrapper
{
    public const string prefabName = "---BOOTSTRAPPER---";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Execute()
    {
        Object.DontDestroyOnLoad(Object.Instantiate(Resources.Load(prefabName)));
    }

    // If using Addressable
    /*
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void ExecuteB()
    {
        Object.DontDestroyOnLoad(Addressables.InstantiateAsync(prefabName).WaitForCompletion());
    }
    */

#if UNITY_EDITOR
    [MenuItem("WorldGame/ShowBootstrap")]
    public static void OpenBootStrapProperties() => OpenPropertiesWindowOf(Resources.Load(prefabName));
    public static void OpenPropertiesWindowOf(Object pObject) => EditorUtility.OpenPropertyEditor(pObject);
#endif
}




