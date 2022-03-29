using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = nameof(EssenceDB)+"/"+nameof(EssenceDB))]
public class EssenceDB : ScriptableObject
{
    [SerializeField] private EssenceData[] essenceData;
    private Dictionary<EssenceType, EssenceData> essenceDB = new Dictionary<EssenceType, EssenceData>();
    
    public EssenceData GetEssence(EssenceType essenceType)
    {
        if(essenceDB is {Count: 0})
            for (int i = 0, ilen = essenceData.Length; i < ilen; i++)
                essenceDB.Add(essenceData[i].EssenceType, essenceData[i]);
        
        return essenceDB.TryGetValue(essenceType, out var sprite) ? sprite : null;
    }
}
