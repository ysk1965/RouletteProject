using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.U2D;

[CreateAssetMenu(fileName = "SpriteAtlasSO", menuName = "ScriptableObjects/SpriteAtlasSO", order = 1)]
public class SpriteAtlasSO : ScriptableObject
{
    [SerializedDictionary("AtlasName", "SpriteAtlas")]
    public SerializedDictionary<string, SpriteAtlas> SpriteAtlasDic = new();

    public Sprite GetSprite(string atlasName, string spriteName)
    {
        if (SpriteAtlasDic.TryGetValue(atlasName, out var atlas))
        {
            return atlas.GetSprite(spriteName);
        }

        return null;
    }
}
