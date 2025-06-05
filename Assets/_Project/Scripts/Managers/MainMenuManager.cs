using UnityEngine;
using UnityEngine.SceneManagement; // Sahne y�netimi i�in gerekli

public class MainMenuManager : MonoBehaviour {
    // Kahraman Olu�turma Sahnesinin ad�n� buraya yaz�n (Build Settings'deki gibi)
    public string characterCreationSceneName = "CharacterCreationScene"; // �rnek isim
    // Ana Oyun Sahnesinin ad�n� buraya yaz�n
    public string gameSceneName = "GameScene"; // �rnek isim
    // Oyun Y�kleme Sahnesinin ad�n� buraya yaz�n
    public string loadGameSceneName = "LoadGameScene"; // �rnek isim
    // Ayarlar Sahnesinin ad�n� buraya yaz�n
    public string settingsSceneName = "SettingsScene"; // �rnek isim

    public void OnCreateCharacterClicked() {
        Debug.Log("Kahraman Olu�tur t�kland�!");
        SceneManager.LoadScene(characterCreationSceneName);
    }

    public void OnStartGameClicked() {
        // �imdilik do�rudan ana oyun sahnesine ge�iyoruz.
        // Gelecekte: Kay�tl� oyun var m� kontrol et, yoksa Kahraman Olu�tur'a y�nlendir veya
        // do�rudan yeni bir oyun ba�lat (parti se�imi vb.).
        Debug.Log("Oyun sahnesine ge�iliyor...");
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnLoadGameClicked() {
        Debug.Log("Oyun Y�kle sahnesine ge�iliyor...");
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnSettingsClicked() {
        Debug.Log("Ayarlar sahnesine ge�iliyor...");
        SceneManager.LoadScene(settingsSceneName);
    }

    public void OnQuitGameClicked() {
        Debug.Log("��k�� yap�l�yor...");
        Application.Quit();

        // Unity Editor'de Application.Quit() hemen �al��maz.
        // Oyun build al�nd���nda �al��acakt�r. Editor'de test i�in:
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
