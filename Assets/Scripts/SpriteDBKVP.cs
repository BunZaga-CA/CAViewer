using UnityEngine;

[CreateAssetMenu(menuName = "SpriteDB/"+nameof(SpriteDBKVP))]
public class SpriteDBKVP : ScriptableObject
{
    public Sprite Sprite => sprite;
    [SerializeField] private Sprite sprite;
}
