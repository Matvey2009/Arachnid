using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class SaggingLineDrawer : MonoBehaviour
{
    public Material lineMaterial;
    public float lineWidth = 0.1f;
    public Color lineColor = Color.green;
    public Color invalidLineColor = Color.red;
    public LayerMask obstacleLayer;
    public float sagAmount = 1.0f; // Сила провисания
    public int resolution = 10;    // Количество сегментов линии
    
    private List<GameObject> lineObjects = new List<GameObject>();
    private Vector3? firstPoint = null;
    private LineRenderer previewLine;
    private bool isValidPlacement = true;

    void Start()
    {
        CreatePreviewLine();
    }

    void Update()
    {
        HandleInput();
        UpdatePreview();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (!firstPoint.HasValue)
                {
                    firstPoint = hit.point;
                }
                else if (!HasObstacleBetweenPoints(firstPoint.Value, hit.point))
                {
                    CreateSaggingLine(firstPoint.Value, hit.point);
                    firstPoint = null;
                }
            }
        }

        if (Input.GetMouseButtonDown(1)) RemoveLastLine();
        if (Input.GetKeyDown(KeyCode.C)) ClearAllLines();
    }

    void UpdatePreview()
    {
        if (!firstPoint.HasValue)
        {
            previewLine.positionCount = 0;
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            isValidPlacement = !HasObstacleBetweenPoints(firstPoint.Value, hit.point);
            UpdateSaggingLine(previewLine, firstPoint.Value, hit.point, isValidPlacement);
        }
    }

    bool HasObstacleBetweenPoints(Vector3 start, Vector3 end)
    {
        return Physics.Raycast(start, (end - start).normalized, 
                             Vector3.Distance(start, end), obstacleLayer);
    }

    void CreatePreviewLine()
    {
        GameObject previewObj = new GameObject("PreviewLine");
        previewLine = previewObj.AddComponent<LineRenderer>();
        ConfigureLineRenderer(previewLine);
    }

    void CreateSaggingLine(Vector3 start, Vector3 end)
    {
        GameObject lineObj = new GameObject("SaggingLine");
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lineObjects.Add(lineObj);
        
        ConfigureLineRenderer(lr);
        UpdateSaggingLine(lr, start, end, true);
    }

    void ConfigureLineRenderer(LineRenderer lr)
    {
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.material = lineMaterial ? lineMaterial : new Material(Shader.Find("Standard"));
        lr.startColor = lineColor;
        lr.endColor = lineColor;
        lr.useWorldSpace = true;
    }

    void UpdateSaggingLine(LineRenderer lr, Vector3 start, Vector3 end, bool valid)
    {
        lr.positionCount = resolution;
        Vector3[] points = new Vector3[resolution];
        
        for (int i = 0; i < resolution; i++)
        {
            float t = i / (float)(resolution - 1);
            Vector3 point = Vector3.Lerp(start, end, t);
            
            // Добавляем провисание по параболе
            float sagFactor = Mathf.Sin(t * Mathf.PI);
            point.y -= sagFactor * sagAmount;
            
            points[i] = point;
        }
        
        lr.SetPositions(points);
        lr.startColor = valid ? lineColor : invalidLineColor;
        lr.endColor = valid ? lineColor : invalidLineColor;
    }

    void RemoveLastLine()
    {
        if (lineObjects.Count > 0)
        {
            Destroy(lineObjects[lineObjects.Count - 1]);
            lineObjects.RemoveAt(lineObjects.Count - 1);
        }
    }

    void ClearAllLines()
    {
        foreach (GameObject line in lineObjects)
        {
            Destroy(line);
        }
        lineObjects.Clear();
    }
}