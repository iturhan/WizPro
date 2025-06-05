// _Project/Scripts/Data/ScriptableObjects/ClassSO.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewClass", menuName = "WizardryPro/Character/Class")]
public class ClassSO : ScriptableObject {
    // ... (Basic Info, Stat Requirements, Progression ayný kalýr) ...
    [Header("Basic Info")]
    public string className;
    [TextArea(3, 5)]
    public string description;

    [Header("Stat Requirements")]
    public List<StatRequirement> statRequirements;

    [Header("Progression")]
    public int hpPerLevelBonus;
    public int stmPerLevelBonus;
    public int skillPointsToAllocatePerLevel = 3; // Seviye baþýna daðýtýlacak yetenek puaný (örnek)

    [Header("Spellcasting")]
    // Bu sýnýf hangi büyü okullarýndan/kitaplarýndan büyü öðrenebilir?
    public List<SpellSchoolType> learnableSpellSchools;

    [Header("Allowed Skills")]
    public List<SkillSO> allowedSkills;

    [Header("Starting Skill Points (Optional)")]
    public List<InitialSkillAllocation> startingSkillPoints;

    [Header("Equipment Proficiency (Placeholder)")]
    public List<string> allowedEquipmentTypes;

    [Header("Special Class Abilities")] // YENÝ EKLENDÝ
    public List<SpecialAbilitySO> classAbilities;

}

[System.Serializable]
public struct InitialSkillAllocation {
    public SkillSO skill; // Bu skill, yukarýdaki allowedSkills listesinde olmalý
    [Range(0, 100)] // Baþlangýç puaný sýnýrý
    public int points;
}

// StatRequirement struct'ý ayný kalabilir.
[System.Serializable]
public struct StatRequirement {
    public StatType stat; public int minValue;
}
