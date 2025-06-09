// _Project/Scripts/Data/ScriptableObjects/GameAssetHolderSO.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameAssetHolder", menuName = "WizardryPro/Singletons/Game Asset Holder")]
public class GameAssetHolderSO : ScriptableObject {
    [Header("Character Creation Assets")]
    public List<RaceSO> allRaces;
    public List<ClassSO> allClasses;

    // Gelecekte buraya tüm SpellSO'lar, ItemSO'lar vb. de eklenebilir.
    [Header("Spell Data")]
    public List<SpellSO> allSpells;

    [Header("Skill Data")]
    public List<SkillSO> allSkills;

}
