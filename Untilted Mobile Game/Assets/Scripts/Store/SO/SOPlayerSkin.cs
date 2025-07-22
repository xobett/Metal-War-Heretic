using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/Player Skin/Create Skin", fileName = "New Skin")]
public class SOPlayerSkin : ScriptableObject
{
    public string skinName;

    public int cost;

    public bool isPurchased;
    public bool isEquipped;

    public Material skinMTL;
}