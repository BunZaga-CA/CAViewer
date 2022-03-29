using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EssenceType
{
    None,
    Arcane,
    Death,
    Life
}

public class PropertyData
{
    public EssenceType EssenceType;
    public Rarity Rarity;
    public string Value;
}
