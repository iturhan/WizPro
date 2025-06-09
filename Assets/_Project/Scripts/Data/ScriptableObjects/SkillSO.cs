// _Project/Scripts/Data/ScriptableObjects/SkillSO.cs
using UnityEngine;

// SkillCategory enum'ýný SkillSO.cs'in dýþýna, MagicEnums.cs veya CharacterEnums.cs'e taþýyabilirsiniz
// public enum SkillCategory { Weaponry, Physical, Academia, Expert, Magic }

public enum MagicSkillType // Büyü yeteneðinin özel tipi
{
    NotMagic,
    Mastery, // Wizardry, Divinity, Psionics, Alchemy
    Realm    // Fire Magic, Air Magic, etc.
}

[CreateAssetMenu(fileName = "NewSkill", menuName = "WizardryPro/Character/Skill")]
public class SkillSO : ScriptableObject {
    [Header("Basic Info")]
    public string skillName;
    [TextArea(5, 10)]
    public string description;
    public SkillCategory category; // Weaponry, Physical, Academia, Expert
    public Sprite icon;

    [Header("Progression")]
    public int maxSkillPoints = 100;

    [Header("Magic Skill Specifics")]
    public MagicSkillType magicSkillType = MagicSkillType.NotMagic;
    // Eðer Mastery ise, hangi okula ait?
    public SpellSchoolType associatedMasterySchool = SpellSchoolType.None;
    // Eðer Realm ise, hangi büyü alanýna ait?
    public MagicRealm associatedMagicRealm = MagicRealm.None;

    [Header("Expert Skill Info")]
    public bool isExpertSkill = false;
    [TextArea(3, 5)]
    public string unlockPrerequisitesDescription;
}