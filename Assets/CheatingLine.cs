using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatingLine : MonoBehaviour
{
    public LineRenderer lineRenderer;
    private List<LineRenderer> lineToEnemys;
    private List<Transform> enemyList = new List<Transform>();
    private Transform playerTransform;
    private bool isCheating = false;


    private IEnumerator Start()
    {
        // ⚠ CHỜ CHO PLAYER ĐƯỢC SPAWN
        yield return new WaitUntil(() => GameObject.FindGameObjectWithTag("Player") != null);

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        lineToEnemys = new List<LineRenderer>();
        enemyList = new List<Transform>();

        // Load tất cả enemy
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject obj in objs)
        {
            Transform tf = obj.transform;
            enemyList.Add(tf);

            LineRenderer lr = Instantiate(lineRenderer);
            lr.positionCount = 2;
            lineToEnemys.Add(lr);
        }
    }


    private void Update()
    {
        if (playerTransform == null) return; // Player chưa có thì ko chạy

        if (Input.GetKeyDown(KeyCode.P))
        {
            isCheating = !isCheating;
            if (!isCheating)
            {
                DisableLine();
            }
        }

        if (isCheating)
        {
            for (int i = 0; i < enemyList.Count; i++)
            {
                SetLine(lineToEnemys[i], enemyList[i].position);
            }
        }
    }


    private void SetLine(LineRenderer l, Vector3 enemyPosition)
    {
        l.SetPosition(0, playerTransform.position);
        l.SetPosition(1, enemyPosition);
    }


    private void DisableLine()
    {
        foreach (LineRenderer lr in lineToEnemys)
        {
            lr.SetPosition(0, Vector3.zero);
            lr.SetPosition(1, Vector3.zero);
        }
    }
}
