using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

public class CardController : MonoBehaviour
{
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

    [SerializeField] private Slider[] mp4LoadingBar;
    [SerializeField] private Slider[] gifLoadingBar;
    [SerializeField] private GameObject[] fetching;
    [SerializeField] private GameObject[] cardParts;
    [SerializeField] private GameObject[] frames;
    [SerializeField] private Renderer[] lightHouseColorRenderers;
    [SerializeField] private Image[] lightHouseColorImages;
    [SerializeField] private Image[] darkHouseColorImages;
    [SerializeField] private TMPro.TextMeshProUGUI[] lightHouseColorText;
    [SerializeField] private TMPro.TextMeshProUGUI[] darkHouseColorText;

    [SerializeField] private VideoController videoController;

    public static bool PrimeMindView { get; private set; }
    [SerializeField] private bool primeMindView = false;
    
    private Vector3 toFrontGoal = new Vector3(0, 180, 0);
    private Vector3 toBackGoal = new Vector3(0, -180, 0);
    
    private static readonly string DIVINITY_TEMPLATE = "Divinity: {0}";
    private static readonly string PURITY_TEMPLATE = "Purity: {0}";

    private int loadedTotal = 0;
    private int loadedMask = 3;
    private PrimeEternalData peData;

    private CardState lastCardState = CardState.ShowingFront;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    private static readonly int DiffuseColor = Shader.PropertyToID("_DiffuseColor");

    private Coroutine gifLoader = null;

    private void Awake()
    {
        PrimeMindView = primeMindView;
        NFTViewer.PEDataLoaded += OnPEDataLoaded;
        NFTViewer.CardStateChanged += OnCardStateChanged;
        
        ClearData();
        DisableCardParts();
    }

    public void DisableCardParts()
    {
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
        if (videoController == null)
            return;
        
        switch (cardState)
        {
            case CardState.Empty:
                ClearData();
                DisableCardParts();
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
                
                if (!videoController.isPlaying)
                    videoController.StartVideo();
                
                if (videoController.isPaused)
                    videoController.ResumeVideo();

                cardRoot.DORotate(toFrontGoal, 1, RotateMode.LocalAxisAdd).OnComplete(() =>
                {
                    NFTViewer.ChangeCardState(CardState.ShowingFront);
                });
                break;
            
            case CardState.ShowingFront:
                if(cardButtonFront != null)
                    cardButtonFront.onClick.AddListener(OnCardButtonClicked);
                
                if (!videoController.isPlaying)
                    videoController.StartVideo();
                
                if (videoController.isPaused)
                    videoController.ResumeVideo();
                
                if (proGifPlayerRawImage != null)
                    if(primeMindView)
                        proGifPlayerRawImage.Resume();
                    else 
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
                
                videoController.PauseVideo();
                
                if (proGifPlayerRawImage != null)
                    proGifPlayerRawImage.Resume();
                
                lastCardState = CardState.ShowingBack;
                break;
        }
    }
    
    public static IEnumerator CheckInternetConnection(Action<bool> syncResult)
    {
        const string echoServer = "https://www.cacodex.com";

        bool result;
        using (var request = UnityWebRequest.Head(echoServer))
        {
            request.timeout = 5;
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();
            
            result = request.result != UnityWebRequest.Result.ConnectionError;
        }
        syncResult(result);
    }
    
    private void OnPEDataLoaded(PrimeEternalData peData)
    {
        this.peData = peData;
        StartCoroutine(CheckInternetConnection(AfterPingInternet));
    }

    private void AfterPingInternet(bool hasInternet)
    {
        if (!hasInternet)
        {
            HandleOfflineMode();
            return;
        }
        
        var videoURL = string.Format(NFTViewer.AnimationURI, peData.PicCode);
        var gifURL = string.Format(NFTViewer.ThumbURI, peData.PicCode);

        videoController.VideoLoadComplete += OnVideoLoadComplete;
        videoController.VideoLoadError += OnVideoLoadError;
        videoController.VideoLoadProgress += OnMp4ProgressUpdate;
        
        videoController.FetchURL(videoURL);
        
        if (proGifPlayerHandler != null)
        {
            proGifPlayerHandler.m_WebGifUrl = gifURL;
            proGifPlayerHandler.Play();
            gifLoader = StartCoroutine(CheckGifComplete());
        }
    }

    private void OnMp4ProgressUpdate(float progress)
    {
        if (mp4LoadingBar == null)
            return;
        
        for (int i = 0, ilen = mp4LoadingBar.Length; i < ilen; ++i)
        {
            if (mp4LoadingBar[i] == null)
                continue;
            mp4LoadingBar[i].value = progress;
        }
    }
    
    private void OnGifProgressUpdate(float progress)
    {
        if (gifLoadingBar == null)
            return;
        
        for (int i = 0, ilen = gifLoadingBar.Length; i < ilen; ++i)
        {
            if (gifLoadingBar[i] == null)
                continue;
            gifLoadingBar[i].value = progress;
        }
    }
    
    private void HandleOfflineMode()
    {
        CheckDoneLoadingData(1);
        CheckDoneLoadingData(2);
    }
    
    private IEnumerator CheckGifComplete()
    {
        proGifPlayerRawImage.SetLoadingCallback(OnGifProgressUpdate);
        while (proGifPlayerRawImage.IsLoadingComplete == false)
            yield return null;

        proGifPlayerRawImage.Pause();
        proGifPlayerRawImage.SetLoadingCallback(null);
        CheckDoneLoadingData(2);
    }

    private void OnVideoLoadComplete()
    {
        videoController.VideoLoadComplete -= OnVideoLoadComplete;
        videoController.VideoLoadError -= OnVideoLoadError;
        videoController.VideoLoadProgress -= OnMp4ProgressUpdate;
        CheckDoneLoadingData(1);
    }

    private void OnVideoLoadError()
    {
        videoController.VideoLoadComplete -= OnVideoLoadComplete;
        videoController.VideoLoadError -= OnVideoLoadError;
        videoController.VideoLoadProgress -= OnMp4ProgressUpdate;
        proGifPlayerRawImage.SetLoadingCallback(null);
        CheckDoneLoadingData(1);
        CheckDoneLoadingData(2);
        StopCoroutine(gifLoader);
    }
    
    private void CheckDoneLoadingData(int value)
    {
        loadedTotal |= value;
        if (loadedTotal < loadedMask)
            return;
        
        nftId.text = peData.Id.ToString();

        var houseProperty = peData.CoreEssence;
        
        nftFamily.text = peData.Family;
        var essence = essenceDB.GetEssence(houseProperty);
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

        if(videoController != null)
            videoController.StopVideo();

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
        
        for (int i = 0, ilen = mp4LoadingBar.Length; i < ilen; ++i)
        {
            if (mp4LoadingBar[i] == null)
                continue;
            mp4LoadingBar[i].value = 0;
        }
        
        for (int i = 0, ilen = gifLoadingBar.Length; i < ilen; ++i)
        {
            if (gifLoadingBar[i] == null)
                continue;
            gifLoadingBar[i].value = 0;
        }
    }
}