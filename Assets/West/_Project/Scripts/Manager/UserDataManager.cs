using System;
using System.Collections;
using System.Collections.Generic;
using CookApps.LocalData;
using UnityEngine;

public partial class UserDataManager : Singleton<UserDataManager>
{
    private static readonly string FILE_NAME = "gowest_user_data.dat";

    private CookAppsLocalData _localData;

    private UserData _userData;
    public UserData UserData => _userData;

    public void Init()
    {
        _localData = new CookAppsLocalData(GetKey());

        bool isLoadSuccess = LoadUserData();
        if (isLoadSuccess) // 데이터가 있는 경우
        {
            //...
        }
        else // 데이터가 없는 경우 - 새로 생성
        {
            CreateDefaultUserData();
        }
    }

    // 새 유저 데이터 생성
    public void CreateDefaultUserData()
    {
        _userData = new UserData();
        _userData.PlayerName = "Susan";
        _userData.PlayerID = "1";

        _userData.RankPoint = 0;
        _userData.TutorialPlayCount = 1;

        _userData.Gold = 0;
        _userData.Jewel = 0;

        _userData.MainCharacterID = 1001;
        AddCharacter(_userData.MainCharacterID, false);

        SaveUserData();
    }

    public void SaveUserData()
    {
        CookAppsLocalData.EnumSaveResult enumSaveResult = _localData.Save(_userData, FILE_NAME);

        var resultMsg = "";
        switch (enumSaveResult)
        {
            case CookAppsLocalData.EnumSaveResult.SUCCESS:
                resultMsg = "저장 완료!";
                Debug.Log(resultMsg);
                break;

            case CookAppsLocalData.EnumSaveResult.FAIL_UNKNOWN:
            case CookAppsLocalData.EnumSaveResult.FAIL_DISK_FULL:
                resultMsg = $"저장에 실패했습니다. 이유 : {enumSaveResult}. 디바이스의 디스크 공간을 확보하세요.";
                Debug.LogError(resultMsg);
                break;
        }
    }

    public bool LoadUserData()
    {
        var resultMsg = "";
        if (_localData.TryLoad(FILE_NAME, out _userData))
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
    public static string GetKey()
    {
        // encrypted with https://www.stringencrypt.com (v1.4.0) [C#]
        // key = "gowest123!@#$%^&"
        String key = "\u9F11\u1F2B\u9F28\u1F2B\u9F21\u9F2A\u9F01\u9F00\u9F07\u1EFE\u1F1B\u1F0A\u1F06\u1F06\u1F1E\u9F0A";

        for (int szhed = 0, lzdfu = 0; szhed < 16; szhed++)
        {
            lzdfu = key[szhed];
            lzdfu += 0x3892;
            lzdfu ^= szhed;
            lzdfu ^= 0x124E;
            lzdfu ++;
            lzdfu -= szhed;
            lzdfu -= 0xA7BF;
            lzdfu ^= 0xF500;
            lzdfu -= 0x6AFC;
            lzdfu = (((lzdfu & 0xFFFF) >> 15) | (lzdfu << 1)) & 0xFFFF;
            lzdfu ^= szhed;
            key = key.Substring(0, szhed) + (char)(lzdfu & 0xFFFF) + key.Substring(szhed + 1);
        }

        return key;
    }
}
