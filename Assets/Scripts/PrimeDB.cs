using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = nameof(PrimeDB)+"/"+nameof(PrimeDB))]
public class PrimeDB : ScriptableObject
{
    [SerializeField] private TextAsset nftDBRaw;
    [SerializeField] private TextAsset distribution;
    
    [SerializeField] private PrimeEternalData[] peData;
    private Dictionary<int, PrimeEternalData> peDB = new Dictionary<int, PrimeEternalData>();

    private Dictionary<string, Dictionary<string, PropertyData>> propertyData =
        new Dictionary<string, Dictionary<string, PropertyData>>();

    public void GeneratePrimeDB()
    {
        peDB.Clear();
        for (int i = 0, ilen = peData.Length; i < ilen; i++)
        {
            peDB.Add(peData[i].Id, peData[i]);
        }
    }
    
#if UNITY_EDITOR
    public void GeneratePrimes()
    {
        var jsonString = nftDBRaw.text;
        var rawData = JsonHelper.FromJson<PEData>(jsonString);
        peData = new PrimeEternalData[rawData.Length];
        for (int i = 0, ilen = rawData.Length; i < ilen; i++)
        {
            var family = NFTViewer.GetPropertyName(rawData[i].Family);
            var essence = NFTViewer.GetEssenceType(rawData[i].CoreEssence);
            var purity = rawData[i].Purity;
            var divinity = rawData[i].Divinity;
            var picCode = rawData[i].PicCode;
            var id = rawData[i].Id;

            var piercing = NFTViewer.GetPropertyName(rawData[i].Piercing);
            var tail = NFTViewer.GetPropertyName(rawData[i].Tail);
            var halo = NFTViewer.GetPropertyName(rawData[i].Halo);
            var hair = NFTViewer.GetPropertyName(rawData[i].HairStyle);
            var claws = NFTViewer.GetPropertyName(rawData[i].Claws);
            var fangs = NFTViewer.GetPropertyName(rawData[i].Fangs);
            var wings = NFTViewer.GetPropertyName(rawData[i].Wings);
            var horns = NFTViewer.GetPropertyName(rawData[i].Horns);
            var paint = NFTViewer.GetPropertyName(rawData[i].WarPaint);

            peData[i] = new PrimeEternalData(
                family,
                essence,
                purity,
                divinity,
                picCode,
                id,
                propertyData["Claws"][claws],
                propertyData["Fangs"][fangs],
                propertyData["Hairstyle"][hair],
                propertyData["Halo"][halo],
                propertyData["Horns"][horns],
                propertyData["Piercing"][piercing],
                propertyData["Tail"][tail],
                propertyData["Warpaint"][paint],
                propertyData["Wings"][wings]
            );
        }
    }

    public void GenerateProperties()
    {
        var jsonString = distribution.text;
        var rawData = JsonHelper.FromJson<PropertyCategory>(jsonString);
        
        for (int i = 0, ilen = rawData.Length; i < ilen; ++i)
        {
            var category = rawData[i];
            //if (category.Name != "Claws")
            //    continue;

            var dir = Application.dataPath + "/DBStuff/Properties/" + category.Name;
            System.IO.Directory.CreateDirectory(dir);

            if (!propertyData.ContainsKey(category.Name))
                propertyData.Add(category.Name, new Dictionary<string, PropertyData>());

            var propertyDB = propertyData[category.Name];
            
            for(int j = 0, jlen = category.Properties.Count; j < jlen; ++j)
            {
                var property = ScriptableObject.CreateInstance<PropertyData>();
                var number = category.Properties[j].FIELD2.Trim(',', ' ');
                int count;
                Int32.TryParse(number, out count);
                float percent = (float)count / 7622;
                NFTViewer.SetPropertyData(category.Properties[j].FIELD1, property);
                property.name = property.Value;
                property.Percentage = Mathf.RoundToInt(percent * 100);
                
                if(!propertyDB.ContainsKey(property.Value))
                    propertyDB.Add(property.Value, property);
                
                AssetDatabase.CreateAsset(property, "Assets/DBStuff/Properties/" +category.Name + "/" + property.Value + ".asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                propertyDB[property.Value] = property;
            }
        }
    }
    #endif
    
    public PrimeEternalData GetPrimeData(int primeId)
    {
        if (peDB.TryGetValue(primeId, out var prime))
            return prime;
        
        return null;
    }
}

#if UNITY_EDITOR
[Serializable]
public class PropertyCategory
{
    public string Name => name;
    [SerializeField] private string name;

    public List<PropertyCount> Properties => properties;
    [SerializeField] private List<PropertyCount> properties;
    
    public static PropertyCategory CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<PropertyCategory>(jsonString);
    }
}

[Serializable]
public class PropertyCount
{
    public string FIELD1;
    public string FIELD2;
}
#endif