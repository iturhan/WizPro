// _Project/Scripts/Data/ScriptableObjects/SpecialAbilitySO.cs (Yeni bir dosya olu�turun)
using UnityEngine;

public enum SpecialAbilityParameterType // Yetene�in t�r�ne g�re parametreleri ay�rt etmek i�in
{
    None,
    SkillBonus,
    ResistanceBonus,
    StatBasedEffect, // Knock-out �ans� STR'ye ba�l� olabilir vs.
    Regeneration,
    SpecificMechanic // BreatheAcid, CheatDeath gibi �zel �eyler
}

[CreateAssetMenu(fileName = "NewSpecialAbility", menuName = "WizardryPro/Character/Special Ability")]
public class SpecialAbilitySO : ScriptableObject {
    [Header("Core Info")]
    public SpecialAbilityType uniqueID; // Yukar�daki enum'dan se�ilecek
    public string abilityName; // Kullan�c� aray�z�nde g�sterilecek ad
    [TextArea(3, 10)]
    public string description; // Detayl� a��klama
    public Sprite icon; // Opsiyonel: Yetenek i�in bir ikon

    [Header("Categorization & Parameters")]
    public SpecialAbilityParameterType parameterType = SpecialAbilityParameterType.None;

    // Parametreler (t�r�ne g�re doldurulacak)
    [Header("Skill Bonus (If parameterType is SkillBonus)")]
    public SkillSO skillToBonus;
    public int skillBonusAmount;

    [Header("Resistance Bonus (If parameterType is ResistanceBonus)")]
    public MagicRealm realmToBonus; // T�m�ne bonus i�in �zel bir de�er veya ayr� bir boolean
    public bool bonusToAllRealms = false;
    public int resistanceBonusValue;

    [Header("Regeneration (If parameterType is Regeneration)")]
    public bool regeneratesHP = false;
    public bool regeneratesStamina = false;
    public float regenerationRatePerTurnOrSecond = 1.0f; // Ya da saniyede/turda y�zde

    // Di�er �zel parametreler buraya eklenebilir
    // �rn: Fighter_MayKnockOutOpponents i�in:
    // public float knockOutBaseChance;
    // public StatType knockOutInfluencingStat;

    // �rn: Faerie_BonusToBaseArmorClass i�in:
    // public int acBonusValue;
}
