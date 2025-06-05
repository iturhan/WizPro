// _Project/Scripts/Data/ScriptableObjects/ClassSO.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewClass", menuName = "WizardryPro/Character/Class")]
public class ClassSO : ScriptableObject {
    // ... (Basic Info, Stat Requirements, Progression ayn� kal�r) ...
    [Header("Basic Info")]
    public string className;
    [TextArea(3, 5)]
    public string description;

    [Header("Stat Requirements")]
    public List<StatRequirement> statRequirements;

    [Header("Progression")]
    public int hpPerLevelBonus;
    public int stmPerLevelBonus;
    public int skillPointsToAllocatePerLevel = 3; // Seviye ba��na da��t�lacak yetenek puan� (�rnek)

    [Header("Spellcasting")]
    // Bu s�n�f hangi b�y� okullar�ndan/kitaplar�ndan b�y� ��renebilir?
    public List<SpellSchoolType> learnableSpellSchools;

    [Header("Allowed Skills")]
    public List<SkillSO> allowedSkills;

    [Header("Starting Skill Points (Optional)")]
    public List<InitialSkillAllocation> startingSkillPoints;

    [Header("Equipment Proficiency (Placeholder)")]
    public List<string> allowedEquipmentTypes;

    [Header("Special Class Abilities")] // YEN� EKLEND�
    public List<SpecialAbilitySO> classAbilities;

}

[System.Serializable]
public struct InitialSkillAllocation {
    public SkillSO skill; // Bu skill, yukar�daki allowedSkills listesinde olmal�
    [Range(0, 100)] // Ba�lang�� puan� s�n�r�
    public int points;
}

// StatRequirement struct'� ayn� kalabilir.
[System.Serializable]
public struct StatRequirement {
    public StatType stat; public int minValue;
}
