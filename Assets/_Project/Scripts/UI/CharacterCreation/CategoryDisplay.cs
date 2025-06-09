using UnityEngine;
using TMPro;

/// <summary>
/// UI'da bir yetenek kategorisi baþlýðýný ve o kategoriye ait puanlarý yönetir.
/// Bu script, CategoryHeader_Template prefab'ýnýn ana objesine eklenmelidir.
/// </summary>
public class CategoryDisplay : MonoBehaviour {
    [Header("UI References")]
    [Tooltip("Kategori adýný gösteren TextMeshPro objesi")]
    public TextMeshProUGUI categoryNameText;

    [Tooltip("Bu kategori için harcanacak puanlarý gösteren TextMeshPro objesi")]
    public TextMeshProUGUI pointsText;

    // Bu script'in Unity Editör'ünde elle atanmasýna gerek yoktur.
    // SkillSelectionController bu referansý kod ile ayarlayacaktýr.
    [HideInInspector]
    public SkillSelectionController mainController;

    private SkillCategory category;
    private int pointsToSpend;
    private int initialPoints;

    /// <summary>
    /// Bu kategori ekranýný baþlangýç deðerleriyle kurar.
    /// </summary>
    /// <param name="targetCategory">Bu UI elemanýnýn temsil ettiði kategori (örn: Weaponry).</param>
    /// <param name="startingPoints">Bu kategori için harcanabilecek baþlangýç puaný.</param>
    /// <param name="ownerController">Bu UI elemanýný yöneten ana kontrolcü.</param>
    public void Setup(SkillCategory targetCategory, int startingPoints, SkillSelectionController ownerController) {
        this.category = targetCategory;
        this.initialPoints = startingPoints;
        this.pointsToSpend = startingPoints;
        this.mainController = ownerController;

        categoryNameText.text = this.category.ToString();
        UpdatePointsText();
    }

    /// <summary>
    /// Bu kategoride harcanacak puan olup olmadýðýný kontrol eder.
    /// </summary>
    public bool CanSpendPoint() {
        return pointsToSpend > 0;
    }

    /// <summary>
    /// Bu kategoride bir puanýn geri alýnýp alýnamayacaðýný kontrol eder.
    /// Yani, baþlangýçta verilen puandan daha az puana düþülüp düþülmediðini kontrol eder.
    /// </summary>
    public bool CanRefundPoint() {
        return pointsToSpend < initialPoints;
    }

    /// <summary>
    /// Bu kategoriden bir puan harcar.
    /// </summary>
    public void SpendPoint() {
        if (CanSpendPoint()) {
            pointsToSpend--;
            UpdatePointsText();
            mainController.OnPointChanged(); // Ana kontrolcüyü durum deðiþikliðinden haberdar et.
        }
    }

    /// <summary>
    /// Bu kategoriye bir puan iade eder.
    /// </summary>
    public void RefundPoint() {
        if (CanRefundPoint()) {
            pointsToSpend++;
            UpdatePointsText();
            mainController.OnPointChanged();
        }
    }

    /// <summary>
    /// Puan metnini günceller.
    /// </summary>
    private void UpdatePointsText() {
        pointsText.text = $"Puan: {pointsToSpend}";
    }
}