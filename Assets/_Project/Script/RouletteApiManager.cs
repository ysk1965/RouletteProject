using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CookApps.TeamBattle;

public class RouletteApiManager : SingletonMonoBehaviour<RouletteApiManager>
{
    public List<Item> CachedItems => _cachedItems;
    private const string authKey = "AUTH_ROULETTE_SUNGRAK";
    private const string baseUrl = "https://api.sungrak.click/event-api/roulette";

    // 캐시할 데이터 구조
    private List<Item> _cachedItems;

    public void GetConsumeRouletteItem(Action<bool> onConsumeComplete)
    {
        GetRemainedItems(onConsumeComplete);
    }

    public void PostConsumeRouletteItem(Item selectedItem, Action<bool> onConsumeComplete)
    {
        StartCoroutine(ConsumeRouletteItem(selectedItem.name, onConsumeComplete));
    }

    // GET 요청: 남은 아이템 목록 가져오기
    private IEnumerator GetRemainedItems(Action<bool> onConsumeComplete)
    {
        string url = $"{baseUrl}/remained-items?authKey={authKey}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error fetching remained items: " + request.error);
                onConsumeComplete.Invoke(false);
            }
            else
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("Remained Items Response: " + jsonResponse);

                // JSON 응답을 객체로 변환하고 캐싱
                RemainedItemsResponse responseData = JsonUtility.FromJson<RemainedItemsResponse>(jsonResponse);
                _cachedItems = responseData.items;

                Debug.Log("Cached Items:");
                foreach (var item in _cachedItems)
                {
                    Debug.Log($"Item: {item.name}, Count: {item.count}, Weight: {item.weight}");
                }

                onConsumeComplete.Invoke(true);
            }
        }
    }

    public IEnumerator ConsumeRouletteItem(string itemName, Action<bool> onConsumeComplete)
    {
        string url = $"{baseUrl}/consume";

        // JSON 데이터 준비
        string jsonData = $"{{\"item_name\": \"{itemName}\"}}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("auth-key", authKey);
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error consuming roulette item: " + request.error);
                onConsumeComplete.Invoke(false);
            }
            else
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("Consume Response: " + jsonResponse);

                // JSON 응답을 객체로 변환하여 상태 확인
                ConsumeResponse response = JsonUtility.FromJson<ConsumeResponse>(jsonResponse);
                if (response.status == "success")
                {
                    Debug.Log("Item consumed successfully.");
                    onConsumeComplete.Invoke(true);
                }
                else if (response.status == "failed")
                {
                    Debug.Log("Failed to consume item: Out of stock.");
                    // 재고 없음 안내 처리 추가 가능
                    onConsumeComplete.Invoke(false);
                }
            }
        }
    }

    // 캐싱된 아이템 가져오기 (필요 시 호출)
    public List<Item> GetCachedItems()
    {
        return _cachedItems;
    }
}

[System.Serializable]
public class Item
{
    public string name;
    public int count;
    public int weight;
}

[System.Serializable]
public class RemainedItemsResponse
{
    public List<Item> items;
}

[System.Serializable]
public class ConsumeResponse
{
    public string status;
}
