// _Project/Scripts/Data/ScriptableObjects/ClassSO.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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

    public StatRequirement? GetRequirementForStat(StatType stat) {
        // Eðer gereksinim listesi hiç yoksa veya boþsa, null döndür.
        // Bu, artýk geçerli bir iþlemdir çünkü metodun imzasý '?' içeriyor.
        if (statRequirements == null || statRequirements.Count == 0) {
            return null;
        }

        // LINQ'in FirstOrDefault metodu, aranan stat'ý bulmaya çalýþýr.
        var requirement = statRequirements.FirstOrDefault(r => r.stat == stat);

        // ÖNEMLÝ NOT: FirstOrDefault, bir 'struct' için eþleþme bulamazsa
        // null yerine "boþ bir struct" (tüm deðerleri 0 olan) döndürür.
        // Bu yüzden, dönen struct'ýn gerçekten geçerli bir gereksinim olup olmadýðýný
        // minValue deðerine bakarak anlýyoruz. Hiçbir geçerli gereksinim 0 olamaz.
        if (requirement.minValue == 0) {
            // Bu, "eþleþme bulunamadý" demektir, bu yüzden null döndürüyoruz.
            return null;
        }

        // Eþleþme bulundu, geçerli gereksinimi döndür.
        return requirement;
    }


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



