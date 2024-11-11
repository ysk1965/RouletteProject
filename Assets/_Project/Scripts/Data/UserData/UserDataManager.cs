using System;
using System.Collections.Generic;
using CookApps.LocalData;
using CookApps.Playgrounds.Utility;
using Cysharp.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CookApps.BM.TTT.Data
{
    public partial class UserDataManager : Singleton<UserDataManager>
    {
        private static readonly string FILE_NAME = "ttt_user_data.dat";

        private CookAppsLocalData _localData;

        private Dictionary<Type, IUserDataBase> _userDataMap = new();

        private UserData _userData;
        public UserData UserData => _userData;

        public void Init()
        {
            _localData = new CookAppsLocalData(GetKey());

            LoadUserData<UserAssetData>();

            bool isLoadSuccess = LoadUserData();
            if (isLoadSuccess == false) // 데이터가 없는 경우 - 새로 생성
            {
                CreateDefaultUserData();
            }
        }

        public void SaveUserData(Action callback = null)
        {
            CookAppsLocalData.EnumSaveResult enumSaveResult = _localData.Save(_userData, FILE_NAME);

            var resultMsg = "";
            switch (enumSaveResult)
            {
                case CookAppsLocalData.EnumSaveResult.SUCCESS:
                    resultMsg = ZString.Format("{0}", "Save Success!");
                    Debug.Log(resultMsg);
                    break;
                case CookAppsLocalData.EnumSaveResult.FAIL_UNKNOWN:
                    resultMsg = ZString.Format("{0}", "Save Failed Reason : Unknown.");
                    Debug.LogError(resultMsg);
                    break;
                case CookAppsLocalData.EnumSaveResult.FAIL_DISK_FULL:
                    resultMsg = ZString.Format("{0}", "Save Failed Reason : Disk Full.");
                    Debug.LogError(resultMsg);
                    break;
            }

            callback?.Invoke();
        }

        // 새 유저 데이터 생성
        private void CreateDefaultUserData()
        {
            CreateUserData();

            SaveUserData();
        }

        private void CreateUserData()
        {
            _userData = new UserData();

            _userData.UserName = Random.Range(10000, 100000).ToString();
            _userData.Level = 1;

            _userData.Coin = 10000;
            _userData.Dart = 10;
            _userData.Shield = 0;
        }

        private T LoadUserData<T>() where T : UserDataBase
        {
            Type type = typeof(T);
            if (!_localData.TryLoad(MakeFileName(type), out T a))
            {
                a = Activator.CreateInstance<T>();
            }

            Type baseType = typeof(IUserDataBase);
            Type[] interfaces = type.GetInterfaces();
            foreach (Type interfaceType in interfaces)
            {
                if (interfaceType == baseType)
                {
                    continue;
                }

                bool isTargetType = baseType.IsAssignableFrom(interfaceType);
                if (isTargetType)
                {
                    _userDataMap.Add(interfaceType, a);
                }
            }

            a.Initialize();
            return a;
        }

        public T GetUserData<T>() where T : IUserDataBase
        {
            Type type = typeof(T);
            if (_userDataMap.TryGetValue(type, out IUserDataBase userDataBase))
            {
                return (T) userDataBase;
            }

            return default;
        }

        private bool LoadUserData()
        {
            var resultMsg = "";
            if (_localData.TryLoad<UserData>(FILE_NAME, out _userData))
            {
                resultMsg = "로드완료";
                Debug.Log(resultMsg);
                return true;
            }
            else
            {
                resultMsg = "데이터가 null입니다.";
                Debug.Log(resultMsg);
                return false;
            }
        }

        private static string GetKey()
        {
            // encrypted with https://www.stringencrypt.com (v1.4.0) [C#]
            // key = "ttt123456789!@#~"
            var key =
                "\u5BF2\u5BCD\u5BD8\u5DFB\u5DFE\u5DC1\u5D24\u5A37\u5A2A\u5A3D\u5A20\u5A63\u5AAE\u5BA1\u5DB4\u5857";

            for (int EGuKt = 0, ZEDon = 0; EGuKt < 16; EGuKt++)
            {
                ZEDon = key[EGuKt];
                ZEDon--;
                ZEDon = (((ZEDon & 0xFFFF) >> 12) | (ZEDon << 4)) & 0xFFFF;
                ZEDon ^= 0x9586;
                ZEDon = ((ZEDon << 12) | ((ZEDon & 0xFFFF) >> 4)) & 0xFFFF;
                ZEDon--;
                ZEDon += EGuKt;
                ZEDon = ~ZEDon;
                ZEDon ^= 0x9088;
                ZEDon = ((ZEDon << 14) | ((ZEDon & 0xFFFF) >> 2)) & 0xFFFF;
                ZEDon -= EGuKt;
                ZEDon += 0xB68D;
                ZEDon = (((ZEDon & 0xFFFF) >> 1) | (ZEDon << 15)) & 0xFFFF;
                ZEDon += 0xB972;
                key = key.Substring(0, EGuKt) + (char) (ZEDon & 0xFFFF) + key.Substring(EGuKt + 1);
            }

            return key;
        }

        private static string MakeFileName(Type type)
        {
            return ZString.Format("{0}.dat", type.Name);
        }
    }
}
