using System.Collections.Generic;
using UnityEngine;

public class CheatLineRenderer : MonoBehaviour
{
    public bool showLines = false;
    public Material lineMaterial;
    public float lineWidth = 0.03f;

    private List<LineRenderer> lines = new List<LineRenderer>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            showLines = !showLines;

            if (!showLines)
                ClearLines();
        }

        if (showLines)
            DrawLines();
    }

    void DrawLines()
    {
        ClearLines();

        GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");

        foreach (GameObject zombie in zombies)
        {
            LineRenderer lr = new GameObject("CheatLine").AddComponent<LineRenderer>();
            lr.material = lineMaterial;
            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;

            lr.startColor = Color.red;
            lr.endColor = Color.red;

            lr.positionCount = 2;
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, zombie.transform.position);

            lines.Add(lr);
        }
    }

    void ClearLines()
    {
        foreach (LineRenderer lr in lines)
            if (lr != null) Destroy(lr.gameObject);

        lines.Clear();
    }
}