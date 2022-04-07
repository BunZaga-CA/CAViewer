using UnityEngine;

[CreateAssetMenu(menuName = nameof(PrimeDB)+"/"+nameof(PropertyData))]
public class PropertyData : ScriptableObject
{
    public EssenceType EssenceType;
    public Rarity Rarity;
    public string Value;
    public int Percentage;
}
