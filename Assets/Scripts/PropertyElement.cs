using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PropertyElement : MonoBehaviour
{
    public static Color DEFAULT_WHITE = Color.white;
    
    [SerializeField] private TMPro.TextMeshProUGUI value;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private RarityDB rarityDB;
    [SerializeField] private EssenceDB essenceDB;
    [SerializeField] private Image essenceImage;
    [SerializeField] private Image essenceMatch;

    public void SetProperty(PropertyData data, EssenceType coreType)
    {
        value.text = data.Value;
        if (data.Rarity != null)
        {
            var rarityArt = rarityDB.GetRarity(data.Rarity);
            if (rarityArt != null)
            {
                backgroundImage.color = rarityArt.PropertyColor;
                essenceImage.gameObject.SetActive(data.EssenceType != EssenceType.None);
                var essenceData = essenceDB.GetEssence(data.EssenceType);
                if(essenceData != null)
                    essenceImage.sprite = essenceData.EssenceSprite;
                    
                essenceMatch.gameObject.SetActive(data.EssenceType == coreType);
            }
        }
    }

    public void ClearProperty()
    {
        value.text = string.Empty;
        backgroundImage.color = DEFAULT_WHITE;
        essenceImage.gameObject.SetActive(false);
        essenceMatch.gameObject.SetActive(false);
    }
}
