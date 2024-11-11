using System;
using System.Collections;
using System.Collections.Generic;
using CookApps.Playgrounds.Utility;
using Cysharp.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;
#if USE_UNITASK
using Cysharp.Threading.Tasks;
#endif

namespace ca.alpo.sim.Utility
{
    public struct SpriteAtlasRequest
    {
        public string AtlasKey;
        public string SpriteName;
        public int RequestKey;
    }

    public struct SpriteAtlasResult
    {
        public SpriteAtlas Atlas;
        public Sprite Sprite;
        public int RequestKey;
    }

    public class OnDemandAtlasManager : SingletonMonoBehaviour<OnDemandAtlasManager>
    {
        private class SpriteAtlasHolder
        {
            public string AtlasKey;
            public SpriteAtlas Atlas;
            public bool IsLoadingComplete;
        }

        private Dictionary<int, SpriteAtlasRequest> requestMap = new();
        private Dictionary<string, SpriteAtlasHolder> holderMap = new();

        private void OnEnable()
        {
            SpriteAtlasManager.atlasRequested += OnAtlasRequested;
        }

        private void OnDisable()
        {
            SpriteAtlasManager.atlasRequested -= OnAtlasRequested;
        }

#if USE_UNITASK
        private async void OnAtlasRequested(string key, Action<SpriteAtlas> callback)
        {
            var addr = ZString.Format("Addressable/Atlases/{0}.spriteatlasv2", key);
            var sa = await LoadAtlas(new SpriteAtlasRequest
            {
                AtlasKey = addr,
                SpriteName = string.Empty,
                RequestKey = 0,
            });
            callback.Invoke(sa.Atlas);
        }

        public async UniTask<SpriteAtlasResult> GetSpriteAsync(SpriteAtlasRequest request)
        {
            var result = await LoadAtlas(request);
            result.Sprite = result.Atlas.GetSprite(request.SpriteName);
            return result;
        }

        private async UniTask<SpriteAtlasResult> LoadAtlas(SpriteAtlasRequest request)
        {
            if (holderMap.TryGetValue(request.AtlasKey, out var context))
            {
                await UniTask.WaitUntil(() => context.IsLoadingComplete);
                return new SpriteAtlasResult
                {
                    Atlas = context.Atlas,
                    RequestKey = request.RequestKey,
                };
            }

            context = new SpriteAtlasHolder
            {
                AtlasKey = request.AtlasKey,
                IsLoadingComplete = false,
            };
            holderMap.Add(request.AtlasKey, context);

            var atlas = await Addressables.LoadAssetAsync<SpriteAtlas>(request.AtlasKey);
            context.Atlas = atlas;
            context.IsLoadingComplete = true;

            return new SpriteAtlasResult
            {
                Atlas = atlas,
                RequestKey = request.RequestKey,
            };
        }
#else
        public void GetAtlas(string key, Action<SpriteAtlas> callback)
        {
            OnAtlasRequested(key, atlas => { callback?.Invoke(atlas); });
        }

        private void OnAtlasRequested(string key, Action<SpriteAtlas> callback)
        {
            string addr = ZString.Format("Addressable/Atlases/{0}.spriteatlasv2", key);
            var request = new SpriteAtlasRequest
            {
                AtlasKey = addr,
                SpriteName = string.Empty,
                RequestKey = 0,
            };

            StartCoroutine(LoadAtlas(request, result => callback?.Invoke(result.Atlas)));
        }

        private IEnumerator LoadAtlas(SpriteAtlasRequest request, Action<SpriteAtlasResult> callback)
        {
            if (holderMap.TryGetValue(request.AtlasKey, out SpriteAtlasHolder context))
            {
                yield return new WaitUntil(() => context.IsLoadingComplete);
                callback?.Invoke(new SpriteAtlasResult
                {
                    Atlas = context.Atlas,
                    RequestKey = request.RequestKey,
                });
                yield break;
            }

            context = new SpriteAtlasHolder
            {
                AtlasKey = request.AtlasKey,
                IsLoadingComplete = false,
            };
            holderMap.Add(request.AtlasKey, context);

            AsyncOperationHandle<SpriteAtlas> h = Addressables.LoadAssetAsync<SpriteAtlas>(request.AtlasKey);
            while (!h.IsDone)
            {
                yield return null;
            }

            SpriteAtlas atlas = h.Result;
            context.Atlas = atlas;
            context.IsLoadingComplete = true;

            callback?.Invoke(new SpriteAtlasResult
            {
                Atlas = atlas,
                RequestKey = request.RequestKey,
            });

            Addressables.Release(h);
        }
#endif
    }
}
