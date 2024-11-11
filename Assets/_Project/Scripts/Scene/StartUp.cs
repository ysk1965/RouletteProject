using ca.alpo.sim.Utility;
using CookApps.BM.TTT.Data;
using CookApps.Playgrounds.Scene;
using CookApps.Playgrounds.Utility;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CookApps.BM.TTT.Scene
{
    public class StartUp : SceneBase
    {
        private async void Awake()
        {
            await UniTask.Yield();

            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            GameSettingData.InitializeGameSettingData();
            AddressableManager.Init(true);
            OnDemandAtlasManager.Init(true);
            CoroutineTaskManager.Init(true);
            TaskScheduler.Init(true);
            SpecDataManager.Instance.LoadSpecData();

            await UniTask.WaitForSeconds(1f);

#if USE_SRDEBUG
            SRDebug.Init();
#endif

            UserDataManager.Instance.Init();

            SceneManager.Initialize(this);
        }

        public override void OnOpen()
        {
            MoveToNextScene();
        }

        private void MoveToNextScene()
        {
            SceneManager.Instance.OpenSceneAsync<Main>();
        }
    }
}
