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
    public static event Action<PrimeEternalData> PEDataLoaded;
    public static event Action<CardState> CardStateChanged;
    
    public static CardState CardState = CardState.Empty;
    
    public static string ThumbURI = "https://champions.io/champions/nfts/art/{0}/thumbnail.gif";
    public static string AnimationURI = "https://champions.io/champions/nfts/art/{0}/animation.mp4";
    
    //[SerializeField] private Dictionary<int, PEData> nftDB = new Dictionary<int, PEData>();
    [SerializeField] private PrimeDB primeDB;
    [SerializeField] private TMPro.TMP_InputField nftInput;
    [SerializeField] private TMPro.TextMeshProUGUI messageText;
    [SerializeField] private GameObject messageWindow;
    [SerializeField] private Button messageButton;
    
    private void Awake()
    {
        primeDB.GeneratePrimeDB();
        
        nftInput.onEndEdit.AddListener(FetchData);
        nftInput.onSelect.AddListener(HideMessageInternal);
        
        ShowMessage += ShowMessageInternal;
        HideMessage += HideMessageInternal;
        
        ShowMessage(TextMessages.MustEnterAValueTxt);
        
        messageButton.onClick.AddListener(HideMessageNoText);
    }

    public void FetchData(string nftText)
    {
        if (string.IsNullOrEmpty(nftText))
        {
            ChangeCardState(CardState.Empty);
            ShowMessage(TextMessages.MustEnterAValueTxt);
            return;
        }

        try
        {
            if (CardState == CardState.Fetching)
                return;
            
            var nftId = Int32.Parse(nftText);

            if (nftId < 1 || nftId > 7622)
            {
                ChangeCardState(CardState.Empty);
                ShowMessage(string.Format(TextMessages.NoCardExistsTxt, nftId));
                return;
            }
            
            ChangeCardState(CardState.Fetching);
            var peData = primeDB.GetPrimeData(nftId);
            if (peData != null)
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

    
    public static PropertyData SetPropertyData(string stringIn, PropertyData data)
    {
        var stringData = stringIn.Split();
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

    public static EssenceType GetEssenceType(string stringIn)
    {
        var stringData = stringIn.Split();
        for (int i = 0, ilen = stringData.Length; i < ilen; i++)
        {
            switch (stringData[i])
            {
                case "🔮":
                    return EssenceType.Arcane;
                case "💀":
                    return EssenceType.Death;
                case "🌱":
                    return EssenceType.Life;
            }
        }

        return EssenceType.None;
    }
    
    public static string GetPropertyName(string stringIn)
    {
        var stringData = stringIn.Split();
        var strBuilder = new StringBuilder();

        for (int i = 0, ilen = stringData.Length; i < ilen; i++)
        {
            var remove = true;
            switch (stringData[i])
            {
                case "🔮":
                    break;
                case "💀":
                    break;
                case "🌱":
                    break;
                case "-":
                    break;
                case "C":
                    break;
                case "R":
                    break;
                case "E":
                    break;
                case "D":
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
        
        //data.Value = strBuilder.ToString();
        return strBuilder.ToString();
    }

    public static Action<string> ShowMessage;
    public static Action<string> HideMessage;
    private void ShowMessageInternal(string message)
    {
        messageText.text = message;
        messageWindow.SetActive(true);
    }

    private void HideMessageNoText()
    {
        HideMessageInternal(string.Empty);
    }
    private void HideMessageInternal(string value)
    {
        messageText.text = string.Empty;
        messageWindow.SetActive(false);
    }
}


