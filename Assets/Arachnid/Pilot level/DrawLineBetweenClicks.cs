using System.Collections.Generic;
using UnityEngine;

public class PersistentLineDrawer : MonoBehaviour
{
    public Material lineMaterial; // Материал для линий
    public float lineWidth = 0.05f; // Толщина линии
    
    private List<GameObject> lineObjects = new List<GameObject>();
    private Vector3? startPoint = null;

    void Update()
    {
        // Левый клик - устанавливаем начальную точку
        if (Input.GetMouseButtonDown(0))
        {
            startPoint = GetMouseWorldPosition();
        }
        
        // Правый клик - рисуем линию от начальной до конечной точки
        if (Input.GetMouseButtonDown(1) && startPoint.HasValue)
        {
            Vector3 endPoint = GetMouseWorldPosition();
            CreateLine(startPoint.Value, endPoint);
            startPoint = null;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    private void CreateLine(Vector3 start, Vector3 end)
    {
        GameObject lineObject = new GameObject("Line");
        LineRenderer lr = lineObject.AddComponent<LineRenderer>();
        
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.material = lineMaterial != null ? lineMaterial : 
            new Material(Shader.Find("Unlit/Color")) { color = Color.red };
        
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        
        lineObjects.Add(lineObject);
    }

    // Метод для очистки всех линий
    private void ClearAllLines()
    {
        foreach (GameObject line in lineObjects)
        {
            Destroy(line);
        }
        lineObjects.Clear();
    }
}