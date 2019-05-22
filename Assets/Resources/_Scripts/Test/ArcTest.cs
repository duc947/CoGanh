using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcTest : MonoBehaviour
{
    public Transform startPoint, endPoint;
    public Vector3 minPoint;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    public void startMove() {
        StartCoroutine(move());
         
    }
    IEnumerator move() {
        float count = 0.0f;
        int dem = 0;
        
        while (count < 1.0f) {
            dem++;
            Vector3 m1 = Vector3.Lerp(startPoint.localPosition, startPoint.localPosition + minPoint, count );
            Vector3 m2 = Vector3.Lerp(startPoint.localPosition + minPoint, endPoint.localPosition, count );
            yield return new WaitForFixedUpdate();
            count += 1.0f *Time.deltaTime*dem/10;
            transform.localPosition = Vector3.Lerp(m1, m2, count);
        }
    }
}
