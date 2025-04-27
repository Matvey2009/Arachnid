using UnityEngine;

public class SpiderController : MonoBehaviour
{
    public float moveSpeed = 5f; // Скорость передвижения паука
    public float rotationSpeed = 500f; // Скорость поворота паука

    private Animator animator; // Ссылка на компонент Animator

    void Start()
    {
        // Получаем ссылку на компонент Animator
        animator = GetComponent<Animator>();

        // Проверяем, что Animator существует
        if (animator == null)
        {
            Debug.LogError("Animator component not found on the Spider!");
        }
    }

    void Update()
    {
        // Получаем ввод от пользователя
        float horizontalInput = Input.GetAxis("Horizontal"); // A/D или Стрелки влево/вправо
        float verticalInput = Input.GetAxis("Vertical");   // W/S или Стрелки вверх/вниз

        // Движение вперед/назад
        Vector3 moveDirection = -transform.forward * verticalInput; // Направление движения
        transform.position += moveDirection * moveSpeed * Time.deltaTime; // Перемещение

        // Поворот влево/вправо
        transform.Rotate(Vector3.up, horizontalInput * rotationSpeed * Time.deltaTime); // Вращение

        // Управление анимацией
        bool isMoving = Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(verticalInput) > 0.1f; // Движется ли паук? (определяем по вводу)
        animator.SetBool("IsMoving", isMoving); // Передаем значение в аниматор
    }
}
