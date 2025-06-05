// _Project/Scripts/Data/Enums/SpecialAbilityEnums.cs (Yeni bir dosya oluþturun)

public enum SpecialAbilityType {
    None,

    // Fighter Abilities
    Fighter_StaminaRegeneration,
    Fighter_MayKnockOutOpponents,
    Fighter_BerserkAttack,
    Fighter_CloseCombatSkillBonus,

    // Valkyrie Abilities
    Valkyrie_CheatDeath,
    Valkyrie_PolearmsSkillBonus,

    // Lord Abilities
    Lord_HealthRegeneration,
    Lord_DualWeaponsSkillBonus,

    // Ranger Abilities
    Ranger_RangedCriticals,
    Ranger_SearchesAllTheTime,
    Ranger_RangedCombatSkillBonus,
    Ranger_CanCastAlchSpellsWhenSilenced,

    // Samurai Abilities
    Samurai_Fearless,
    Samurai_LightningStrike,
    Samurai_SwordSkillBonus,

    // Monk Abilities
    Monk_NaturalDamageResistance,
    Monk_EffectiveWhileBlind,
    Monk_MartialArtsSkillBonus,

    // Ninja Abilities
    Ninja_ThrownCriticals,
    Ninja_ThrownAutoPenetrate,
    Ninja_CriticalStrikeSkillBonus,
    Ninja_CanCastAlchSpellsWhenSilenced, // Ranger'dan farklý olabilir

    // Rogue Abilities
    Rogue_AttacksByBackstabbing,
    Rogue_LocksAndTrapsSkillBonus,

    // Priest Abilities
    Priest_PrayForMiracle,
    Priest_DispelUndead,
    Priest_DivinitySkillBonus,

    // Alchemist Abilities
    Alchemist_MakesPotionsWhileCamping,
    Alchemist_CanCastAlchSpellsWhenSilenced,
    Alchemist_AlchemySkillBonus,

    // Bishop Abilities
    Bishop_RemoveCursedItems,
    Bishop_DispelUndead, // Priest'ten farklý olabilir
    Bishop_ArtifactsSkillBonus,

    // Mage Abilities
    Mage_BonusToAllResistances,
    Mage_WizardrySkillBonus,

    // Psionic Abilities
    Psionic_Fearless,
    Psionic_MentalConditionImmunity,
    Psionic_PsionicsSkillBonus,

    // --- Race Abilities ---
    // Human: None (Enum'a eklemeye gerek yok veya Dummy_Human_Ability eklenebilir)
    // Elf: None

    Dwarf_NaturalDamageResistance,

    // Gnomes: None
    // Hobbit: None

    Faerie_BonusToBaseArmorClass,
    Faerie_UnusualStartingEquipment,
    Faerie_ReducedCarryingCapacity,
    Faerie_EquippableItemWeightLimit,
    Faerie_FasterMagicPowerRecovery,

    Lizardman_SlowerMagicPowerRecovery,

    Dracon_BreatheAcid
}
