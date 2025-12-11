using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatingLine : MonoBehaviour
{
    private List<Outline> enemyOutlines;
    private bool isCheating;
    private void Start()
    {
        enemyOutlines = new List<Outline>();
        isCheating = false;
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject obj in objs)
        {
            Transform tf = obj.transform;
            Outline t = tf.GetComponent<Outline>();
            enemyOutlines.Add(t);
            t.enabled = false;
        }
        SetOutine(isCheating);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            isCheating = !isCheating;
            SetOutine(isCheating);
        }
    }
    private void SetOutine(bool set)
    {
        foreach (Outline ol in enemyOutlines)
        {
            ol.enabled = set;
        }
    }
}
