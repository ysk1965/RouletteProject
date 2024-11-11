using System.Collections.Generic;
using System.Threading.Tasks;
using CookApps.BM.TTT.Data;
using CookApps.BM.TTT.InGame.Object;
using CookApps.BM.TTT.UI;
using CookApps.Playgrounds.Scene;
using CookApps.Playgrounds.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using SceneManager = CookApps.Playgrounds.Scene.SceneManager;

namespace CookApps.BM.TTT.Scene
{
    [SceneDefine("Addrs/Scenes/Main.unity", LoadSceneMode.Single)]
    [SceneTransition(typeof(DefaultTransition))]
    public class Main : SceneBaseWithUI<MainUI>
    {
        // theme scene
        private SceneInstance _themeScene;

        // world object manager
        private WorldObjectManager _worldObjectManager;

        protected override void OnAwake()
        {
            base.OnAwake();
            IsLoadingComplete = false;
        }

        public override void OnTransitionComplete()
        {
            base.OnTransitionComplete();

            var userAssetData = UserDataManager.Instance.GetUserData<IUserAssetData>();
            UserAsset coinAsset = userAssetData.GetAsset(Data.ItemType.coin);
            Debug.Log(coinAsset.Amount);
        }

        public override async void OnOpen()
        {
            // ?
            LoadingProgress?.SetProgress(0.1f);
            // 무언가 로딩
            LoadingProgress?.SetProgress(0.2f);
            await LoadTheme();
            // ..~~~
            LoadingProgress?.SetProgress(0.4f);
            // 월드 오브젝트 매니저 실행
            _worldObjectManager = new WorldObjectManager(1, FindAllUpgradableObject);
            _worldObjectManager.Initialize();
            LoadingProgress?.SetProgress(1f);
            IsLoadingComplete = true;
        }

        private List<UpgradableObjectBase> FindAllUpgradableObject()
        {
            var upgradableObjectList = new List<UpgradableObjectBase>();
            UpgradableObjectBase[] worldObjects = FindObjectsOfType<UpgradableObjectBase>(true);
            foreach (UpgradableObjectBase worldObject in worldObjects)
            {
                upgradableObjectList.Add(worldObject);
            }

            return upgradableObjectList;
        }

        public override async void OnClose()
        {
            base.OnClose();
            AsyncOperationHandle<SceneInstance> handle = Addressables.UnloadSceneAsync(_themeScene);
            await handle.Task;
        }

        private async Task LoadTheme()
        {
            var themeAddrKey = "Addrs/Theme/0000.Tutorial.unity";
            AsyncOperationHandle<SceneInstance> sceneHandle =
                Addressables.LoadSceneAsync(themeAddrKey, LoadSceneMode.Additive);
            _themeScene = await sceneHandle.Task;
            // AsyncOperation oper = result.ActivateAsync();
            // while (!oper.isDone)
            // {
            //     await Task.Yield();
            // }

            UnityEngine.SceneManagement.SceneManager.SetActiveScene(_themeScene.Scene);

            Debug.Log("Theme Loaded!");
        }

        private async void ReloadThisScene()
        {
            await SceneManager.Instance.OpenSceneAsync<Main>();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (Input.GetKeyDown(KeyCode.R))
            {
                ReloadThisScene();
            }
        }
    }
}
