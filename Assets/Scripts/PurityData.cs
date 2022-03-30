using System;
using UnityEngine;

[CreateAssetMenu(menuName = nameof(PurityDB) + "/" + nameof(PurityData))]
public class PurityData : ScriptableObject
{
    public Sprite PuritySprite => puritySprite;
    [SerializeField] private Sprite puritySprite;
    
    public Sprite PurityGlowSprite => purityGlowSprite;
    [SerializeField] private Sprite purityGlowSprite;
}
