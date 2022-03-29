using UnityEngine;

[CreateAssetMenu(menuName = nameof(EssenceDB) + "/" + nameof(EssenceData))]
public class EssenceData : ScriptableObject
{
    public EssenceType EssenceType => essenceType;
    [SerializeField] private EssenceType essenceType;
    
    public Sprite EssenceSprite => essenceSprite;
    [SerializeField] private Sprite essenceSprite;
}
