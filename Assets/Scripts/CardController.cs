using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class CardController : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RenderTexture renderTexture;
    [SerializeField] private Button cardButtonFront;
    [SerializeField] private Button cardButtonBack;
    [SerializeField] private Transform cardRoot;

    [SerializeField] private TMPro.TextMeshProUGUI nftId;
    [SerializeField] private TMPro.TextMeshProUGUI nftFamily;
    [SerializeField] private TMPro.TextMeshProUGUI nftEssence;
    [SerializeField] private TMPro.TextMeshProUGUI nftDivinity;
    [SerializeField] private TMPro.TextMeshProUGUI nftPurity;
    
    private string loadedURL = "";

    private Vector3 toFrontGoal = new Vector3(0, 0, 0);
    private Vector3 toBackGoal = new Vector3(0, 180, 0);
    
    private const string DIVINITY_TEMPLATE = "Divinity: {0}";
    private const string PURITY_TEMPLATE = "Purity: {0}";
    
    private void Awake()
    {
        NFTViewer.PEDataLoaded += OnPEDataLoaded;
        NFTViewer.CardStateChanged += OnCardStateChanged;
        
        ClearData();
        
        if(cardButtonFront != null)
            cardButtonFront.onClick.AddListener(OnCardButtonClicked);
        
        if(cardButtonBack != null)
            cardButtonBack.onClick.AddListener(OnCardButtonClicked);
    }
    
    private void OnCardButtonClicked()
    {
        switch (NFTViewer.CardState)
        {
            case CardState.Empty:
            case CardState.Fetching:
                return;
            case CardState.ShowingFront:
            case CardState.ToFront:
                NFTViewer.CardState = CardState.ToBack;
                cardRoot.DORotate(toBackGoal, 0.5f).OnComplete(() =>
                {
                    NFTViewer.CardState = CardState.ShowingBack;
                });
                break;
            case CardState.ShowingBack:
            case CardState.ToBack:
                NFTViewer.CardState = CardState.ToFront;
                cardRoot.DORotate(toFrontGoal, 0.5f).OnComplete(() =>
                {
                    NFTViewer.CardState = CardState.ShowingFront;
                });
                break;
        }
    }
    
    private void OnCardStateChanged(CardState cardState)
    {
        if (videoPlayer == null)
            return;
        
        switch (cardState)
        {
            case CardState.Empty:
                ClearData();
                break;
            
            case CardState.Fetching:
                ClearData();
                break;
            
            case CardState.ShowingFront:
                videoPlayer.url = loadedURL;
                videoPlayer.Play();
                break;
        }
    }
    
    private void OnPEDataLoaded(PEData peData)
    {
        loadedURL = string.Format(NFTViewer.AnimationURI, peData.PicCode);
        NFTViewer.CardState = CardState.ShowingFront;
        nftId.text = peData.Id.ToString();
        nftFamily.text = peData.Family;
        nftEssence.text = peData.CoreEssence;
        nftDivinity.text = string.Format(DIVINITY_TEMPLATE, peData.Divinity);
        nftPurity.text = String.Format(PURITY_TEMPLATE, peData.Purity);
        OnCardStateChanged(NFTViewer.CardState);
    }

    private void ClearData()
    {
        loadedURL = String.Empty;
        
        if(videoPlayer != null)
            videoPlayer.Stop();
        
        if(renderTexture != null)
            renderTexture.Release();
        
        if(nftId != null)
            nftId.text = string.Empty;
        
        if(nftFamily != null)
            nftFamily.text = string.Empty;
        
        if(nftEssence != null)
            nftEssence.text = string.Empty;
        
        if(nftDivinity != null)
            nftDivinity.text = string.Empty;
        
        if(nftPurity != null)
            nftPurity.text = string.Empty;
    }
}