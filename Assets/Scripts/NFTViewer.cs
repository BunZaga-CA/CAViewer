using System;
using System.Collections.Generic;
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
            Debug.Log("Starting to fetch data");
            CardState = CardState.Fetching;
            CardStateChanged?.Invoke(CardState);

            if (nftDB.ContainsKey(nftId) && nftDB.TryGetValue(nftId, out var peData))
            {
                PEDataLoaded?.Invoke(peData);
            }
        }
        catch
        {
        }
    }
}


