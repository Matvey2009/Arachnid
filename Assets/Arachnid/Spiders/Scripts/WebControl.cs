using UnityEngine;

public class WebControl : MonoBehaviour
{
    public GameObject webAnchorPrefab;
    public GameObject webLinePrefab;
    public float webLineWidth = 0.05f;
    public int lineResolution = 10;
    public float sagAmount = 1f;
    public float webStartOffset = 10f;
    
    [Header("Resource Settings")]
    public int maxSpiderSilk = 100; // Максимальное количество ресурса
    public int webCost = 20;        // Стоимость одной паутины
    private int currentSpiderSilk;  // Текущее количество ресурса

    private LineRenderer _currentWebLine;
    private GameObject _currentWebAnchor;
    private bool _isFirstWebCreated = false;

    void Start()
    {
        currentSpiderSilk = maxSpiderSilk; // Инициализируем с максимальным запасом
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryCreateWeb();
        }

        if (_isFirstWebCreated && _currentWebLine != null && _currentWebAnchor != null)
        {
            UpdateWebLine();
        }
    }

    void TryCreateWeb()
    {
        // Проверяем, достаточно ли ресурса
        if (currentSpiderSilk >= webCost)
        {
            CreateWeb();
            currentSpiderSilk -= webCost;
            Debug.Log($"Паутина создана! Осталось шелка: {currentSpiderSilk}/{maxSpiderSilk}");
        }
        else
        {
            Debug.LogWarning($"Недостаточно шелка! Нужно: {webCost}, есть: {currentSpiderSilk}");
        }
    }

    void CreateWeb()
    {
        GameObject anchor = Instantiate(webAnchorPrefab, transform.position, Quaternion.identity);

        if (!_isFirstWebCreated)
        {
            _currentWebAnchor = anchor;
            _currentWebLine = Instantiate(webLinePrefab).GetComponent<LineRenderer>();
            _currentWebLine.startWidth = webLineWidth;
            _currentWebLine.endWidth = webLineWidth;
            _currentWebLine.positionCount = lineResolution + 1;

            UpdateWebLine();
            _isFirstWebCreated = true;
        }
        else
        {
            _currentWebLine = null;
            _currentWebAnchor = null;
            _isFirstWebCreated = false;
        }
    }

    // Метод для пополнения ресурса (можно вызывать из других скриптов)
    public void AddSpiderSilk(int amount)
    {
        currentSpiderSilk = Mathf.Min(currentSpiderSilk + amount, maxSpiderSilk);
        Debug.Log($"Шелк пополнен! Теперь: {currentSpiderSilk}/{maxSpiderSilk}");
    }

    void UpdateWebLine()
    {
        if (_currentWebLine != null && _currentWebAnchor != null)
        {
            Vector3 startPoint = transform.TransformPoint(new Vector3(0, 2*webStartOffset, 2*webStartOffset));
            Vector3 endPoint = _currentWebAnchor.transform.position;
            Vector3 controlPoint = (startPoint + endPoint) * 0.5f;
            controlPoint.y -= sagAmount;

            for (int i = 0; i <= lineResolution; i++)
            {
                float t = (float)i / lineResolution;
                Vector3 point = CalculateBezierPoint(t, startPoint, controlPoint, endPoint);
                _currentWebLine.SetPosition(i, point);
            }
        }
    }

    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;

        return p;
    }
}