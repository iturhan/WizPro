using UnityEngine;
using TMPro;

/// <summary>
/// UI'da bir yetenek kategorisi ba�l���n� ve o kategoriye ait puanlar� y�netir.
/// Bu script, CategoryHeader_Template prefab'�n�n ana objesine eklenmelidir.
/// </summary>
public class CategoryDisplay : MonoBehaviour {
    [Header("UI References")]
    [Tooltip("Kategori ad�n� g�steren TextMeshPro objesi")]
    public TextMeshProUGUI categoryNameText;

    [Tooltip("Bu kategori i�in harcanacak puanlar� g�steren TextMeshPro objesi")]
    public TextMeshProUGUI pointsText;

    // Bu script'in Unity Edit�r'�nde elle atanmas�na gerek yoktur.
    // SkillSelectionController bu referans� kod ile ayarlayacakt�r.
    [HideInInspector]
    public SkillSelectionController mainController;

    private SkillCategory category;
    private int pointsToSpend;
    private int initialPoints;

    /// <summary>
    /// Bu kategori ekran�n� ba�lang�� de�erleriyle kurar.
    /// </summary>
    /// <param name="targetCategory">Bu UI eleman�n�n temsil etti�i kategori (�rn: Weaponry).</param>
    /// <param name="startingPoints">Bu kategori i�in harcanabilecek ba�lang�� puan�.</param>
    /// <param name="ownerController">Bu UI eleman�n� y�neten ana kontrolc�.</param>
    public void Setup(SkillCategory targetCategory, int startingPoints, SkillSelectionController ownerController) {
        this.category = targetCategory;
        this.initialPoints = startingPoints;
        this.pointsToSpend = startingPoints;
        this.mainController = ownerController;

        categoryNameText.text = this.category.ToString();
        UpdatePointsText();
    }

    /// <summary>
    /// Bu kategoride harcanacak puan olup olmad���n� kontrol eder.
    /// </summary>
    public bool CanSpendPoint() {
        return pointsToSpend > 0;
    }

    /// <summary>
    /// Bu kategoride bir puan�n geri al�n�p al�namayaca��n� kontrol eder.
    /// Yani, ba�lang��ta verilen puandan daha az puana d���l�p d���lmedi�ini kontrol eder.
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
            mainController.OnPointChanged(); // Ana kontrolc�y� durum de�i�ikli�inden haberdar et.
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
    /// Puan metnini g�nceller.
    /// </summary>
    private void UpdatePointsText() {
        pointsText.text = $"Puan: {pointsToSpend}";
    }
}