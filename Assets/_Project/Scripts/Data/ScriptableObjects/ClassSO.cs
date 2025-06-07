// _Project/Scripts/Data/ScriptableObjects/ClassSO.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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

    public StatRequirement? GetRequirementForStat(StatType stat) {
        // E�er gereksinim listesi hi� yoksa veya bo�sa, null d�nd�r.
        // Bu, art�k ge�erli bir i�lemdir ��nk� metodun imzas� '?' i�eriyor.
        if (statRequirements == null || statRequirements.Count == 0) {
            return null;
        }

        // LINQ'in FirstOrDefault metodu, aranan stat'� bulmaya �al���r.
        var requirement = statRequirements.FirstOrDefault(r => r.stat == stat);

        // �NEML� NOT: FirstOrDefault, bir 'struct' i�in e�le�me bulamazsa
        // null yerine "bo� bir struct" (t�m de�erleri 0 olan) d�nd�r�r.
        // Bu y�zden, d�nen struct'�n ger�ekten ge�erli bir gereksinim olup olmad���n�
        // minValue de�erine bakarak anl�yoruz. Hi�bir ge�erli gereksinim 0 olamaz.
        if (requirement.minValue == 0) {
            // Bu, "e�le�me bulunamad�" demektir, bu y�zden null d�nd�r�yoruz.
            return null;
        }

        // E�le�me bulundu, ge�erli gereksinimi d�nd�r.
        return requirement;
    }


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



