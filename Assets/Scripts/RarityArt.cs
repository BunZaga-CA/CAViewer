using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Rarity
{
    None,
    Common,
    Rare,
    Epic,
    Divine
}

[CreateAssetMenu(menuName = "RarityDB/"+nameof(RarityArt))]
public class RarityArt : ScriptableObject
{
    public Rarity Rarity => rarity;
    [SerializeField] private Rarity rarity;

    public Sprite PropertySprite => propertySprite;
    [SerializeField] private Sprite propertySprite;

    public Color PropertyColor => propertyColor;
    [SerializeField] private Color propertyColor;
    
    public Color PropertyColorDark => propertyColorDark;
    [SerializeField] private Color propertyColorDark;
}
