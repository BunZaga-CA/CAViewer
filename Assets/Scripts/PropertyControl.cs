using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class PropertyControl : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI idText;
    [SerializeField] private Image coreEssenceImageTop;
    [SerializeField] private Image coreEssenceImageLeft;
    [SerializeField] private Image coreEssenceImageRight;
    [SerializeField] private TMPro.TextMeshProUGUI familyText;
    [SerializeField] private TMPro.TextMeshProUGUI houseText;
    [SerializeField] private TMPro.TextMeshProUGUI divinityText;
    [SerializeField] private TMPro.TextMeshProUGUI purityText;
    [SerializeField] private PropertyElement claws;
    [SerializeField] private PropertyElement fangs;
    [SerializeField] private PropertyElement hair;
    [SerializeField] private PropertyElement halo;
    [SerializeField] private PropertyElement horns;
    [SerializeField] private PropertyElement piercing;
    [SerializeField] private PropertyElement tail;
    [SerializeField] private PropertyElement warPaint;
    [SerializeField] private PropertyElement wings;
    [SerializeField] private EssenceDB essenceDB;
    
    private const string DIVINITY = "D:{0}";
    private const string PURITY = "P:{0}";
    
    public void SetProperties(PEData peData)
    {
        familyText.text = peData.Family;
        var coreEssence = NFTViewer.GetPropertyData(peData.CoreEssence);
        var essenceData = essenceDB.GetEssence(coreEssence.EssenceType);
        coreEssenceImageTop.sprite = essenceData.EssenceSpriteTop;
        coreEssenceImageTop.gameObject.SetActive(true);
        coreEssenceImageLeft.sprite = essenceData.EssenceSpriteLeft;
        coreEssenceImageLeft.gameObject.SetActive(true);
        coreEssenceImageRight.sprite = essenceData.EssenceSpriteRight;
        coreEssenceImageRight.gameObject.SetActive(true);
        houseText.text = coreEssence.Value;
        divinityText.text = string.Format(DIVINITY, peData.Divinity);
        purityText.text = string.Format(PURITY, peData.Purity);
        idText.text = peData.Id.ToString();
        
        claws.SetProperty(NFTViewer.GetPropertyData(peData.Claws), coreEssence.EssenceType);
        fangs.SetProperty(NFTViewer.GetPropertyData(peData.Fangs), coreEssence.EssenceType);
        hair.SetProperty(NFTViewer.GetPropertyData(peData.HairStyle), coreEssence.EssenceType);
        halo.SetProperty(NFTViewer.GetPropertyData(peData.Halo), coreEssence.EssenceType);
        horns.SetProperty(NFTViewer.GetPropertyData(peData.Horns), coreEssence.EssenceType);
        piercing.SetProperty(NFTViewer.GetPropertyData(peData.Piercing), coreEssence.EssenceType);
        tail.SetProperty(NFTViewer.GetPropertyData(peData.Tail), coreEssence.EssenceType);
        warPaint.SetProperty(NFTViewer.GetPropertyData(peData.WarPaint), coreEssence.EssenceType);
        wings.SetProperty(NFTViewer.GetPropertyData(peData.Wings), coreEssence.EssenceType);
    }

    public void ClearData()
    {
        familyText.text = string.Empty;
        houseText.text = string.Empty;
        divinityText.text = string.Empty;
        purityText.text = string.Empty;
        idText.text = string.Empty;
        
        coreEssenceImageTop.sprite = null;
        coreEssenceImageTop.gameObject.SetActive(false);
        coreEssenceImageLeft.sprite = null;
        coreEssenceImageLeft.gameObject.SetActive(false);
        coreEssenceImageRight.sprite = null;
        coreEssenceImageRight.gameObject.SetActive(false);
        
        claws.ClearProperty();
        fangs.ClearProperty();
        hair.ClearProperty();
        halo.ClearProperty();
        horns.ClearProperty();
        piercing.ClearProperty();
        tail.ClearProperty();
        warPaint.ClearProperty();
        wings.ClearProperty();
    }
}
