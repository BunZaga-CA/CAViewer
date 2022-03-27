using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PEData
{
    public int Divinity => divinity;
    [SerializeField] private int divinity;
    
    public int Purity => purity;
    [SerializeField] private int purity;

    public string Piercing => piercing;
    [SerializeField] private string piercing;

    public string Tail => tail;
    [SerializeField] private string tail;

    public string Halo => halo;
    [SerializeField] private string halo;

    public string PicCode => picCode;
    [SerializeField] private string picCode;

    public string HairStyle => hairstyle;
    [SerializeField] private string hairstyle;

    public string Claws => claws;
    [SerializeField] private string claws;

    public string Fangs => fangs;
    [SerializeField] private string fangs;

    public string Wings => wings;
    [SerializeField] private string wings;

    public string CoreEssence => coreEssense;
    [SerializeField] private string coreEssense;

    public int Id => id;
    [SerializeField] private int id;

    public string Family => family;
    [SerializeField] private string family;

    public string Horns => horns;
    [SerializeField] private string horns;

    public string WarPaint => warpaint;
    [SerializeField] private string warpaint;

    public static PEData CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<PEData>(jsonString);
    }
}