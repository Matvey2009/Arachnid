using UnityEngine;

public class WebControl : MonoBehaviour
{
    public GameObject webAnchorPrefab;
    public GameObject webLinePrefab;
    public float webLineWidth = 0.05f;
    public int lineResolution = 10; // Количество сегментов линии
    public float sagAmount = 1f;    // Сила провисания
    public float webStartOffset = 10f; // Смещение начала паутины от центра паука

    private LineRenderer _currentWebLine;
    private GameObject _currentWebAnchor;
    private bool _isFirstWebCreated = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            CreateWeb();
        }

        if (_isFirstWebCreated && _currentWebLine != null && _currentWebAnchor != null)
        {
            UpdateWebLine();
        }
    }

    void CreateWeb()
    {
        // Создаем якорь паутины в текущей позиции паука
        GameObject anchor = Instantiate(webAnchorPrefab, transform.position, Quaternion.identity);

        // Если это первая паутина
        if (!_isFirstWebCreated)
        {
            // Сохраняем якорь и создаем линию паутины
            _currentWebAnchor = anchor;
            _currentWebLine = Instantiate(webLinePrefab).GetComponent<LineRenderer>();
            _currentWebLine.startWidth = webLineWidth;
            _currentWebLine.endWidth = webLineWidth;
            _currentWebLine.positionCount = lineResolution + 1; // Устанавливаем количество точек

            UpdateWebLine(); // Вызываем UpdateWebLine сразу после создания
            _isFirstWebCreated = true; // отмечаем, что первая паутина создана
        }
        else
        {
            // Если это вторая паутина, создаем только якорь
            // Останавливаем обновление линии первой паутины, отсоединяя её от паука
            _currentWebLine = null;
            _currentWebAnchor = null;
            _isFirstWebCreated = false;
        }
    }

    // Обновляем линию паутины между пауком и якорем
    void UpdateWebLine()
    {
        if (_currentWebLine != null && _currentWebAnchor != null)
        {
            // Определяем точки кривой Безье
            Vector3 startPoint = transform.TransformPoint(new Vector3(0, 2*webStartOffset, 2*webStartOffset)); // webStartOffset по оси YZ
            Vector3 endPoint = _currentWebAnchor.transform.position;
            Vector3 controlPoint = (startPoint + endPoint) * 0.5f;  // Середина между точками
            controlPoint.y -= sagAmount; // Смещаем контрольную точку вниз для создания провисания

            // Заполняем LineRenderer точками кривой Безье
            for (int i = 0; i <= lineResolution; i++)
            {
                float t = (float)i / lineResolution;
                Vector3 point = CalculateBezierPoint(t, startPoint, controlPoint, endPoint);
                _currentWebLine.SetPosition(i, point);
            }
        }
    }

    // Функция для вычисления точки на кривой Безье
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
