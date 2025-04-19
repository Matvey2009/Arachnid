using UnityEngine;
using UnityEngine.UI;

public class TextureChanger : MonoBehaviour
{
    public GameObject targetObject; // Объект, чью текстуру будем менять
    public Material[] materials;    // Массив материалов для смены
    public Button changeButton;     // Ссылка на кнопку
    private int currentMaterialIndex = 0;

    void Start()
    {
        // Назначаем метод на клик кнопки
        changeButton.onClick.AddListener(ChangeTexture);
    }

    public void ChangeTexture()
    {
        if (materials.Length == 0 || targetObject == null) return;
        
        // Переключаем индекс материала
        currentMaterialIndex = (currentMaterialIndex + 1) % materials.Length;
        
        // Применяем новый материал
        Renderer renderer = targetObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = materials[currentMaterialIndex];
        }
    }
    
}