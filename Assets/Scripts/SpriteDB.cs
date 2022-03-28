using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SpriteDB/"+nameof(SpriteDB))]
public class SpriteDB : ScriptableObject
{
    [SerializeField] private SpriteDBKVP[] spriteDbkvps;
    private Dictionary<string, Sprite> spriteDB = new Dictionary<string, Sprite>();
    
    public Sprite GetSprite(string key)
    {
        if(spriteDB is {Count: 0})
            for (int i = 0, ilen = spriteDbkvps.Length; i < ilen; i++)
                spriteDB.Add(spriteDbkvps[i].name, spriteDbkvps[i].Sprite);
        
        return spriteDB.TryGetValue(key, out var sprite) ? sprite : null;
    }
}
