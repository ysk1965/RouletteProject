using System.Collections;
using System.Collections.Generic;
using CookApps.Playgrounds.Utility;
using UnityEngine;

namespace CookApps.BM.TTT.Data
{
    public class GameSettingData : PersistentGameState
    {
        public static void InitializeGameSettingData()
        {
            bool isFirst = GameStateManager.Instance.GetState<GameSettingData>(false) == null;
            if (!isFirst)
            {
                return;
            }

            var setting = GameStateManager.Instance.GetState<GameSettingData>();
            setting.SetDefault();
            GameStateManager.Instance.SetState(setting);
            GameStateManager.Instance.Save();
        }

        private void SetDefault()
        {
            // Set default value
            IsBgmOn = true;
            IsSfxOn = true;
        }

        private bool isBgmOn = true;

        public bool IsBgmOn
        {
            get => isBgmOn;
            set =>
                // ???
                isBgmOn = value;
        }

        private bool isSfxOn = true;

        public bool IsSfxOn
        {
            get => isSfxOn;
            set =>
                // ???
                isSfxOn = value;
        }
    }
}
