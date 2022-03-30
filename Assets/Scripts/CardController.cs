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
    [SerializeField] private Image nftCoreEssenceTop;
    [SerializeField] private Image nftCoreEssenceLeft;
    [SerializeField] private Image nftCoreEssenceRight;
    [SerializeField] private Image nftPurityGlow;
    [SerializeField] private Image nftPurity;

    [SerializeField] private ProGifPlayerHandler proGifPlayerHandler;
    [SerializeField] private ProGifPlayerRawImage proGifPlayerRawImage;
    
    [SerializeField] private EssenceDB essenceDB;
    [SerializeField] private PurityDB purityDB;

    [SerializeField] private PropertyControl propertyControl;

    [SerializeField] private GameObject[] fetching;
    [SerializeField] private GameObject[] cardParts;
    [SerializeField] private GameObject[] frames;
    [SerializeField] private Renderer[] lightHouseColorRenderers;
    [SerializeField] private Image[] lightHouseColorImages;
    [SerializeField] private Image[] darkHouseColorImages;
    [SerializeField] private TMPro.TextMeshProUGUI[] lightHouseColorText;
    [SerializeField] private TMPro.TextMeshProUGUI[] darkHouseColorText;
    
    private string videoURL;
    private string gifURL;

    private Vector3 toFrontGoal = new Vector3(0, 180, 0);
    private Vector3 toBackGoal = new Vector3(0, -180, 0);
    
    private static readonly string DIVINITY_TEMPLATE = "Divinity: {0}";
    private static readonly string PURITY_TEMPLATE = "Purity: {0}";

    private int loadedTotal = 0;
    private int loadedMask = 3;
    private PEData peData;

    private CardState lastCardState = CardState.ShowingFront;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    private static readonly int DiffuseColor = Shader.PropertyToID("_DiffuseColor");

    private void Awake()
    {
        NFTViewer.PEDataLoaded += OnPEDataLoaded;
        NFTViewer.CardStateChanged += OnCardStateChanged;
        
        ClearData();
        for (int i = 0, ilen = cardParts.Length; i < ilen; i++)
        {
            cardParts[i].SetActive(false);
        }
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
                for(int i = 0, ilen = fetching.Length; i < ilen ;i++)
                {
                    fetching[i].SetActive(true);
                }
                break;
            
            case CardState.ToFront:
                if(cardButtonBack != null)
                   cardButtonBack.onClick.RemoveListener(OnCardButtonClicked);
                
                if (videoPlayer.isPaused || !videoPlayer.isPlaying)
                    videoPlayer.Play();

                cardRoot.DORotate(toFrontGoal, 1, RotateMode.LocalAxisAdd).OnComplete(() =>
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
                
                cardRoot.DORotate(toBackGoal, 1, RotateMode.LocalAxisAdd).OnComplete(() =>
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

        var houseProperty = NFTViewer.GetPropertyData(peData.CoreEssence);
        
        nftFamily.text = peData.Family;
        var essence = essenceDB.GetEssence(houseProperty.EssenceType);
        nftCoreEssenceTop.sprite = essence.EssenceSpriteTop;
        nftCoreEssenceTop.gameObject.SetActive(true);
        
        nftCoreEssenceLeft.sprite = essence.EssenceSpriteLeft;
        nftCoreEssenceLeft.gameObject.SetActive(true);
        
        nftCoreEssenceRight.sprite = essence.EssenceSpriteRight;
        nftCoreEssenceRight.gameObject.SetActive(true);

        if (peData.Purity > 0)
        {
            var purityData = purityDB.GetPurityData(peData.Purity);
            if (purityData != null)
            {
                nftPurity.sprite = purityData.PuritySprite;
                nftPurityGlow.sprite = purityData.PurityGlowSprite;
                nftPurityGlow.color = essence.EssenceColor;


                if (nftPurity != null)
                    nftPurity.gameObject.SetActive(true);

                if (nftPurityGlow != null)
                    nftPurityGlow.gameObject.SetActive(true);
            }
        }


        propertyControl.SetProperties(peData);

        for(int i = 0, ilen = fetching.Length; i < ilen ;i++)
        {
            fetching[i].SetActive(false);
        }
        
        for(int i = 0, ilen = frames.Length; i < ilen ;i++)
        {
            frames[i].SetActive(true);
        }

        for (int i = 0, ilen = lightHouseColorRenderers.Length; i < ilen; ++i)
        {
            lightHouseColorRenderers[i].material.SetColor(DiffuseColor, essence.EssenceColor);
            lightHouseColorRenderers[i].material.SetColor(EmissionColor, essence.EssenceColor);
        }

        for (int i = 0, ilen = lightHouseColorImages.Length; i < ilen; ++i)
            lightHouseColorImages[i].color = essence.EssenceColor;
        
        for (int i = 0, ilen = darkHouseColorImages.Length; i < ilen; ++i)
            darkHouseColorImages[i].color = essence.EssenceColorDark;
        
        for (int i = 0, ilen = lightHouseColorText.Length; i < ilen; ++i)
            lightHouseColorText[i].color = essence.EssenceColor;
        
        for (int i = 0, ilen = darkHouseColorText.Length; i < ilen; ++i)
            darkHouseColorText[i].color = essence.EssenceColorDark;

        NFTViewer.ChangeCardState(lastCardState == CardState.ShowingFront ? CardState.ShowingFront : CardState.ShowingBack);
    }
    
    private void ClearData()
    {
        propertyControl.ClearData();
        videoURL = String.Empty;
        gifURL = String.Empty;
        loadedTotal = 0;
        peData = null;
        
        for (int i = 0, ilen = cardParts.Length; i < ilen; i++)
        {
            cardParts[i].SetActive(true);
        }
        
        for(int i = 0, ilen = fetching.Length; i < ilen ;i++)
        {
            fetching[i].SetActive(false);
        }

        for(int i = 0, ilen = frames.Length; i < ilen ;i++)
        {
            frames[i].SetActive(false);
        }
        
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

        if (nftCoreEssenceTop != null)
        {
            nftCoreEssenceTop.gameObject.SetActive(false);
            nftCoreEssenceTop.sprite = null;
        }
        
        if (nftCoreEssenceLeft != null)
        {
            nftCoreEssenceLeft.gameObject.SetActive(false);
            nftCoreEssenceLeft.sprite = null;
        }
        
        if (nftCoreEssenceRight != null)
        {
            nftCoreEssenceRight.gameObject.SetActive(false);
            nftCoreEssenceRight.sprite = null;
        }
        
        if(nftPurity != null)
            nftPurity.gameObject.SetActive(false);
        
        if(nftPurityGlow != null)
            nftPurityGlow.gameObject.SetActive(false);
    }
}