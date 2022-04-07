using System;
using UnityEngine;

[Serializable]
public class PrimeEternalData
{
    public int Divinity => divinity;
    [SerializeField] private int divinity;
    
    public int Purity => purity;
    [SerializeField] private int purity;

    public PropertyData Piercing => piercing;
    [SerializeField] private PropertyData piercing;

    public PropertyData Tail => tail;
    [SerializeField] private PropertyData tail;

    public PropertyData Halo => halo;
    [SerializeField] private PropertyData halo;

    public string PicCode => picCode;
    [SerializeField] private string picCode;

    public PropertyData HairStyle => hairstyle;
    [SerializeField] private PropertyData hairstyle;

    public PropertyData Claws => claws;
    [SerializeField] private PropertyData claws;

    public PropertyData Fangs => fangs;
    [SerializeField] private PropertyData fangs;

    public PropertyData Wings => wings;
    [SerializeField] private PropertyData wings;

    public EssenceType CoreEssence => coreEssence;
    [SerializeField] private EssenceType coreEssence;

    public int Id => id;
    [SerializeField] private int id;

    public string Family => family;
    [SerializeField] private string family;

    public PropertyData Horns => horns;
    [SerializeField] private PropertyData horns;

    public PropertyData WarPaint => warpaint;
    [SerializeField] private PropertyData warpaint;

    public PrimeEternalData(
        string family,
        EssenceType essence,
        int purity,
        int divinity,
        string picCode,
        int id,
        PropertyData claws,
        PropertyData fangs,
        PropertyData hair,
        PropertyData halo,
        PropertyData horns,
        PropertyData piercing,
        PropertyData tail,
        PropertyData paint, 
        PropertyData wings)
    {
        this.family = family;
        this.coreEssence = essence;
        this.purity = purity;
        this.divinity = divinity;
        this.picCode = picCode;
        this.id = id;
        this.claws = claws;
        this.fangs = fangs;
        this.hairstyle = hair;
        this.halo = halo;
        this.horns = horns;
        this.piercing = piercing;
        this.tail = tail;
        this.warpaint = paint;
        this.wings = wings;
    }
}