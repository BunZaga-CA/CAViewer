using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
    [SerializeField] private Image nftCoreEssence;
    [SerializeField] private TMPro.TextMeshProUGUI nftDivinity;
    [SerializeField] private TMPro.TextMeshProUGUI nftHouse;
    [SerializeField] private TMPro.TextMeshProUGUI nftPurity;

    [SerializeField] private ProGifPlayerHandler proGifPlayerHandler;
    [SerializeField] private ProGifPlayerRawImage proGifPlayerRawImage;
    
    [SerializeField] private SpriteDB spriteDB;

    [SerializeField] private PropertyControl propertyControl;
    
    private string videoURL;
    private string gifURL;

    private Vector3 toFrontGoal = new Vector3(0, 0, 0);
    private Vector3 toBackGoal = new Vector3(0, 180, 0);
    
    private const string DIVINITY_TEMPLATE = "Divinity: {0}";
    private const string PURITY_TEMPLATE = "Purity: {0}";

    private int loadedTotal = 0;
    private int loadedMask = 3;
    private PEData peData;

    private CardState lastCardState = CardState.ShowingFront;
    
    private void Awake()
    {
        NFTViewer.PEDataLoaded += OnPEDataLoaded;
        NFTViewer.CardStateChanged += OnCardStateChanged;
        
        ClearData();
    }
    
    private void OnCardButtonClicked()
    {
        switch (NFTViewer.CardState)
        {
            case CardState.Empty:
            case CardState.Fetching:
                return;
            
            case CardState.ShowingFront:
                NFTViewer.ChangeCardState(CardState.ToBack);
                break;
            
            case CardState.ShowingBack:
                NFTViewer.ChangeCardState(CardState.ToFront);
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
            
            case CardState.ToFront:
                if(cardButtonBack != null)
                   cardButtonBack.onClick.RemoveListener(OnCardButtonClicked);
                
                if (videoPlayer.isPaused || !videoPlayer.isPlaying)
                    videoPlayer.Play();

                cardRoot.DORotate(toFrontGoal, 1).OnComplete(() =>
                {
                    NFTViewer.ChangeCardState(CardState.ShowingFront);
                });
                break;
            
            case CardState.ShowingFront:
                if(cardButtonFront != null)
                    cardButtonFront.onClick.AddListener(OnCardButtonClicked);
                
                if (videoPlayer.isPaused || !videoPlayer.isPlaying)
                    videoPlayer.Play();

                        
                if (proGifPlayerRawImage != null)
                    proGifPlayerRawImage.Pause();

                lastCardState = CardState.ShowingFront;
                break;
            
            case CardState.ToBack:
                if(cardButtonFront != null)
                    cardButtonFront.onClick.RemoveListener(OnCardButtonClicked);

                if (proGifPlayerRawImage != null)
                    proGifPlayerRawImage.Resume();
                
                cardRoot.DORotate(toBackGoal, 1).OnComplete(() =>
                {
                    NFTViewer.ChangeCardState(CardState.ShowingBack);
                });
                break;
            
            case CardState.ShowingBack:
                if(cardButtonBack != null)
                    cardButtonBack.onClick.AddListener(OnCardButtonClicked);
                
                videoPlayer.Pause();
                
                if (proGifPlayerRawImage != null)
                    proGifPlayerRawImage.Resume();
                
                lastCardState = CardState.ShowingBack;
                break;
        }
    }
    
    private void OnPEDataLoaded(PEData peData)
    {
        this.peData = peData;
        
        videoURL = string.Format(NFTViewer.AnimationURI, peData.PicCode);
        gifURL = string.Format(NFTViewer.ThumbURI, peData.PicCode);
        
        videoPlayer.url = videoURL;
        videoPlayer.Prepare();

        if (proGifPlayerHandler != null)
        {
            proGifPlayerHandler.m_WebGifUrl = gifURL;
            proGifPlayerHandler.Play();
            StartCoroutine(CheckGifComplete());
        }
        
        StartCoroutine(CheckVideoComplete());
    }

    private IEnumerator CheckGifComplete()
    {
        while (proGifPlayerRawImage.IsLoadingComplete == false)
            yield return null;
        
        proGifPlayerRawImage.Pause();
        CheckDoneLoadingData(2);
    }

    private IEnumerator CheckVideoComplete()
    {
        while(!videoPlayer.isPrepared)
            yield return null;
        
        CheckDoneLoadingData(1);
    }
    

    private void CheckDoneLoadingData(int value)
    {
        loadedTotal |= value;
        if (loadedTotal < loadedMask)
            return;
        
        nftId.text = peData.Id.ToString();
        
        var house = Regex.Replace(peData.CoreEssence, @"\p{Cs}", "").Trim();
        
        nftFamily.text = peData.Family;
        nftCoreEssence.sprite = spriteDB.GetSprite(house);
        nftCoreEssence.gameObject.SetActive(true);
        
        if (nftHouse != null)
            nftHouse.text = house;

        nftDivinity.text = string.Format(DIVINITY_TEMPLATE, peData.Divinity);
        nftPurity.text = String.Format(PURITY_TEMPLATE, peData.Purity);
        
        propertyControl.SetProperties(peData);

        NFTViewer.ChangeCardState(lastCardState == CardState.ShowingFront ? CardState.ShowingFront : CardState.ShowingBack);
    }
    
    private void ClearData()
    {
        propertyControl.ClearData();
        videoURL = String.Empty;
        gifURL = String.Empty;
        loadedTotal = 0;
        peData = null;
        
        if( proGifPlayerRawImage != null)
            proGifPlayerRawImage.Clear();

        if(videoPlayer != null)
            videoPlayer.Stop();
        
        if(renderTexture != null)
            renderTexture.Release();
        
        if(nftId != null)
            nftId.text = string.Empty;
        
        if(nftFamily != null)
            nftFamily.text = string.Empty;

        if (nftCoreEssence != null)
        {
            nftCoreEssence.gameObject.SetActive(false);
            nftCoreEssence.sprite = null;
        }

        if(nftDivinity != null)
            nftDivinity.text = string.Empty;
        
        if(nftPurity != null)
            nftPurity.text = string.Empty;

        if (nftHouse != null)
            nftHouse.text = string.Empty;
    }
}