using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using CookApps.Playgrounds.Utility;

public class AddressableManager : SingletonMonoBehaviour<AddressableManager>
{
    protected override void OnAwakeEvent()
    {
        Addressables.InitializeAsync(true);
    }

    public T Instantiate<T>(string key, Vector3 position = default, Quaternion rotation = default,
        Transform parent = default) where T : Object
    {
        var asset = LoadAssetAsync<T>(key);

        if (asset == null)
        {
            Debug.LogError($"Failed to load asset: {key}");
            return null;
        }

        return Instantiate(asset, position, rotation, parent);
    }

    private T LoadAssetAsync<T>(string key) where T : Object
    {
        AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);
        handle.WaitForCompletion();

        if (handle.Status == AsyncOperationStatus.Failed)
        {
            Debug.LogError($"Failed to load asset: {key}");
            return null;
        }

        return handle.Result;
    }

    private IList<T> LoadAssetsAsync<T>(string key) where T : Object
    {
        AsyncOperationHandle<IList<T>> handle = Addressables.LoadAssetsAsync<T>(key, null);
        handle.WaitForCompletion();
        return handle.Result;
    }
}
