using UnityEngine;

[CreateAssetMenu(menuName = nameof(EssenceDB) + "/" + nameof(EssenceData))]
public class EssenceData : ScriptableObject
{
    public EssenceType EssenceType => essenceType;
    [SerializeField] private EssenceType essenceType;
    
    public Sprite EssenceSpriteTop => essenceSpriteTop;
    [SerializeField] private Sprite essenceSpriteTop;
    public Sprite EssenceSpriteLeft => essenceSpriteLeft;
    [SerializeField] private Sprite essenceSpriteLeft;
    public Sprite EssenceSpriteRight => essenceSpriteRight;
    [SerializeField] private Sprite essenceSpriteRight;

    public Color EssenceColor => essenceColor;
    [SerializeField] private Color essenceColor;
    
    public Color EssenceColorDark => essenceColorDark;
    [SerializeField] private Color essenceColorDark;
}
