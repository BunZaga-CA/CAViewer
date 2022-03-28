using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class PropertyControl : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI idText;
    [SerializeField] private TMPro.TextMeshProUGUI familyText;
    [SerializeField] private TMPro.TextMeshProUGUI houseText;
    [SerializeField] private TMPro.TextMeshProUGUI divinityText;
    [SerializeField] private TMPro.TextMeshProUGUI purityText;
    [SerializeField] private TMPro.TextMeshProUGUI clawsText;
    [SerializeField] private TMPro.TextMeshProUGUI fangsText;
    [SerializeField] private TMPro.TextMeshProUGUI hairText;
    [SerializeField] private TMPro.TextMeshProUGUI haloText;
    [SerializeField] private TMPro.TextMeshProUGUI hornsText;
    [SerializeField] private TMPro.TextMeshProUGUI piercingText;
    [SerializeField] private TMPro.TextMeshProUGUI tailText;
    [SerializeField] private TMPro.TextMeshProUGUI warPaintText;
    [SerializeField] private TMPro.TextMeshProUGUI wingsText;

    private const string DIVINITY = "Divinity:{0}";
    private const string PURITY = "Purity:{0}";
    
    public void SetProperties(PEData peData)
    {
        familyText.text = peData.Family;
        houseText.text = Regex.Replace(peData.CoreEssence, @"\p{Cs}", "").Trim();
        divinityText.text = string.Format(DIVINITY, peData.Divinity);
        purityText.text = string.Format(PURITY, peData.Purity);
        
        idText.text = peData.Id.ToString();
        clawsText.text = Regex.Replace(peData.Claws, @"\p{Cs}", "").Trim();
        fangsText.text = Regex.Replace(peData.Fangs, @"\p{Cs}", "").Trim();
        hairText.text = Regex.Replace(peData.HairStyle, @"\p{Cs}", "").Trim();
        haloText.text = Regex.Replace(peData.Halo, @"\p{Cs}", "").Trim();
        hornsText.text = Regex.Replace(peData.Horns, @"\p{Cs}", "").Trim();
        piercingText.text = Regex.Replace(peData.Piercing, @"\p{Cs}", "").Trim();
        tailText.text = Regex.Replace(peData.Tail, @"\p{Cs}", "").Trim();
        warPaintText.text = Regex.Replace(peData.WarPaint, @"\p{Cs}", "").Trim();
        wingsText.text = Regex.Replace(peData.Wings, @"\p{Cs}", "").Trim();
    }

    public void ClearData()
    {
        familyText.text = string.Empty;
        houseText.text = string.Empty;
        divinityText.text = string.Empty;
        purityText.text = string.Empty;
        
        idText.text = string.Empty;
        clawsText.text = string.Empty;
        fangsText.text = string.Empty;
        hairText.text = string.Empty;
        haloText.text = string.Empty;
        hornsText.text = string.Empty;
        piercingText.text = string.Empty;
        tailText.text = string.Empty;
        warPaintText.text = string.Empty;
        wingsText.text = string.Empty;
    }
}
