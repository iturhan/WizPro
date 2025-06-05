using UnityEngine;
using UnityEngine.SceneManagement; // Sahne yönetimi için gerekli

public class MainMenuManager : MonoBehaviour {
    // Kahraman Oluþturma Sahnesinin adýný buraya yazýn (Build Settings'deki gibi)
    public string characterCreationSceneName = "CharacterCreationScene"; // Örnek isim
    // Ana Oyun Sahnesinin adýný buraya yazýn
    public string gameSceneName = "GameScene"; // Örnek isim
    // Oyun Yükleme Sahnesinin adýný buraya yazýn
    public string loadGameSceneName = "LoadGameScene"; // Örnek isim
    // Ayarlar Sahnesinin adýný buraya yazýn
    public string settingsSceneName = "SettingsScene"; // Örnek isim

    public void OnCreateCharacterClicked() {
        Debug.Log("Kahraman Oluþtur týklandý!");
        SceneManager.LoadScene(characterCreationSceneName);
    }

    public void OnStartGameClicked() {
        // Þimdilik doðrudan ana oyun sahnesine geçiyoruz.
        // Gelecekte: Kayýtlý oyun var mý kontrol et, yoksa Kahraman Oluþtur'a yönlendir veya
        // doðrudan yeni bir oyun baþlat (parti seçimi vb.).
        Debug.Log("Oyun sahnesine geçiliyor...");
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnLoadGameClicked() {
        Debug.Log("Oyun Yükle sahnesine geçiliyor...");
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnSettingsClicked() {
        Debug.Log("Ayarlar sahnesine geçiliyor...");
        SceneManager.LoadScene(settingsSceneName);
    }

    public void OnQuitGameClicked() {
        Debug.Log("Çýkýþ yapýlýyor...");
        Application.Quit();

        // Unity Editor'de Application.Quit() hemen çalýþmaz.
        // Oyun build alýndýðýnda çalýþacaktýr. Editor'de test için:
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
