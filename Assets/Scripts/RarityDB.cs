using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = nameof(EssenceDB) + "/" + nameof(EssenceDB))]
public class RarityDB : ScriptableObject
{
    [SerializeField] private RarityArt[] rarity;
    private Dictionary<Rarity, RarityArt> rarityDB = new Dictionary<Rarity, RarityArt>();
    
    public RarityArt GetRarity(Rarity key)
    {
        if(rarityDB is {Count: 0})
            for (int i = 0, ilen = rarity.Length; i < ilen; i++)
                rarityDB.Add(rarity[i].Rarity, rarity[i]);
        
        return rarityDB.TryGetValue(key, out var rarityArt) ? rarityArt : null;
    }
}
