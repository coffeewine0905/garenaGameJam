using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CardAbilityData", menuName = "ScriptableObjects/CardAbilityData")]
public class CardAbilityData : ScriptableObject
{
    public int ID;
    public string Name;
    public string Description;
    public float showDelay = 1f;
    public Sprite Image;
}
