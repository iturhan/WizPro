// _Project/Scripts/Data/ScriptableObjects/SpecialAbilitySO.cs (Yeni bir dosya oluþturun)
using UnityEngine;

public enum SpecialAbilityParameterType // Yeteneðin türüne göre parametreleri ayýrt etmek için
{
    None,
    SkillBonus,
    ResistanceBonus,
    StatBasedEffect, // Knock-out þansý STR'ye baðlý olabilir vs.
    Regeneration,
    SpecificMechanic // BreatheAcid, CheatDeath gibi özel þeyler
}

[CreateAssetMenu(fileName = "NewSpecialAbility", menuName = "WizardryPro/Character/Special Ability")]
public class SpecialAbilitySO : ScriptableObject {
    [Header("Core Info")]
    public SpecialAbilityType uniqueID; // Yukarýdaki enum'dan seçilecek
    public string abilityName; // Kullanýcý arayüzünde gösterilecek ad
    [TextArea(3, 10)]
    public string description; // Detaylý açýklama
    public Sprite icon; // Opsiyonel: Yetenek için bir ikon

    [Header("Categorization & Parameters")]
    public SpecialAbilityParameterType parameterType = SpecialAbilityParameterType.None;

    // Parametreler (türüne göre doldurulacak)
    [Header("Skill Bonus (If parameterType is SkillBonus)")]
    public SkillSO skillToBonus;
    public int skillBonusAmount;

    [Header("Resistance Bonus (If parameterType is ResistanceBonus)")]
    public MagicRealm realmToBonus; // Tümüne bonus için özel bir deðer veya ayrý bir boolean
    public bool bonusToAllRealms = false;
    public int resistanceBonusValue;

    [Header("Regeneration (If parameterType is Regeneration)")]
    public bool regeneratesHP = false;
    public bool regeneratesStamina = false;
    public float regenerationRatePerTurnOrSecond = 1.0f; // Ya da saniyede/turda yüzde

    // Diðer özel parametreler buraya eklenebilir
    // Örn: Fighter_MayKnockOutOpponents için:
    // public float knockOutBaseChance;
    // public StatType knockOutInfluencingStat;

    // Örn: Faerie_BonusToBaseArmorClass için:
    // public int acBonusValue;
}
