using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public enum CardState
{
    Empty,
    Fetching,
    ShowingFront,
    ShowingBack,
    ToFront,
    ToBack
}

public class NFTViewer : MonoBehaviour
{
    public static event Action<PEData> PEDataLoaded;
    public static event Action<CardState> CardStateChanged;
    
    public static CardState CardState = CardState.Empty;
    
    public static string ThumbURI = "https://champions.io/champions/nfts/art/{0}/thumbnail.gif";
    public static string AnimationURI = "https://champions.io/champions/nfts/art/{0}/animation.mp4";
    
    [SerializeField] private TextAsset nftDBRaw;
    
    [SerializeField] private Dictionary<int, PEData> nftDB = new Dictionary<int, PEData>();
    [SerializeField] private TMPro.TMP_InputField nftInput;
    [SerializeField] private Button fetchNFT;

    private void Awake()
    {
        var jsonString = nftDBRaw.text;
        Debug.Log(jsonString);
        var rawData = JsonHelper.FromJson<PEData>(jsonString);
        for (int i = 0, ilen = rawData.Length; i < ilen; i++)
        {
            nftDB.Add(rawData[i].Id, rawData[i]);
        }
        
        fetchNFT.onClick.AddListener(FetchData);
    }

    public void FetchData()
    {
        try
        {
            var nftId = Int32.Parse(nftInput.text);
            ChangeCardState(CardState.Fetching);

            if (nftDB.TryGetValue(nftId, out var peData))
                PEDataLoaded?.Invoke(peData);
        }
        catch
        {
        }
    }

    public static void ChangeCardState(CardState newState)
    {
        CardState = newState;
        CardStateChanged?.Invoke(newState);
    }

    public static PropertyData GetPropertyData(string stringIn)
    {
        var stringData = stringIn.Split();
        var data = new PropertyData();
        data.Rarity = Rarity.None;
        var strBuilder = new StringBuilder();

        for (int i = 0, ilen = stringData.Length; i < ilen; i++)
        {
            var remove = true;
            switch (stringData[i])
            {
                case "C":
                    data.Rarity = Rarity.Common;
                    break;
                case "R":
                    data.Rarity = Rarity.Rare;
                    break;
                case "E":
                    data.Rarity = Rarity.Epic;
                    break;
                case "D":
                    data.Rarity = Rarity.Divine;
                    break;
                
                case "🔮":
                    data.EssenceType = EssenceType.Arcane;
                    break;
                case "💀":
                    data.EssenceType = EssenceType.Death;
                    break;
                case "🌱":
                    data.EssenceType = EssenceType.Life;
                    break;
                case "-":
                    break;
                
                default:
                    remove = false;
                    break;
            }

            if (!remove)
            {
                if (strBuilder.Length > 0)
                    strBuilder.Append(" ");
                strBuilder.Append(stringData[i]);
            }
        }
        
        data.Value = strBuilder.ToString();
        return data;
    }
}


