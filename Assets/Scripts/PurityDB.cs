using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = nameof(PurityDB) + "/" + nameof(PurityDB))]
public class PurityDB : ScriptableObject
{
    [SerializeField] private PurityData[] purity;
    private Dictionary<int, PurityData> purityDB = new Dictionary<int, PurityData>();
    
    public PurityData GetPurityData(int key)
    {
        if(purityDB is {Count: 0})
            for (int i = 0, ilen = purity.Length; i < ilen; i++)
                purityDB.Add(i+1, purity[i]);
        
        return purityDB.TryGetValue(key, out var purityData) ? purityData : null;
    }
}
