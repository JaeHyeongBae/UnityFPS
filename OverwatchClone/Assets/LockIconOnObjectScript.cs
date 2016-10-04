using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class LockIconOnObjectScript : MonoBehaviour
{
    public Canvas lockCanvasPrefab;
    public Transform CanvasCenter;
    public Text ultGuageText;
    public Transform[] enemy = new Transform[3];
    public Transform UIcamera;

    Canvas[] lockCanvas = new Canvas[3];

    float ultTimer = 0;
    bool ultimate = false;
    int ultGuage = 0;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) ultGuage += 10; // 시연용 게이지 채우기
        if (ultGuage >= 100) ultGuage = 100;
        ultGuageText.text = ultGuage.ToString() + "%";
        if (Input.GetKeyDown(KeyCode.Q) && ultGuage >= 100)
        {
            ultGuage = 0;
            for (int i = 0; i < 3; i++)
            {
                lockCanvas[i] = Instantiate(lockCanvasPrefab); 
            }
            ultimate = true;
        }
        if (ultimate)
        {
            tacticalVisorOn();
            if (ultTimer < 6.0) ultTimer += Time.deltaTime;
            else
            {
                ultimate = false;
                ultTimer = 0;
                tacticalVisorOff();
            }
        }

    }

    void tacticalVisorOff()
    {
        for (int i = 0; i < 3; i++)
        {
            Destroy(lockCanvas[i].gameObject); 
        }
    }

    void tacticalVisorOn()
    {
        float[] enemyOffset = { 0.0f, 0.0f, 0.0f };
        int maxIndex = -1;
        for (int i = 0; i < 3; i++)
        {
            bool enabled = false;
            Vector3 toObject = enemy[i].position - UIcamera.position;
            RaycastHit hitCanvas = new RaycastHit();
            RaycastHit hitPlayer = new RaycastHit();
            int uiLayerMask = 1 << 5;

            lockCanvas[i].transform.rotation = UIcamera.rotation;

            if (Physics.Raycast(UIcamera.position, toObject, out hitCanvas, 100.0f, uiLayerMask))
            {
                if (Physics.Raycast(UIcamera.position, toObject, out hitPlayer, 100.0f, ~uiLayerMask))
                {
                    if (hitPlayer.transform.tag == "Player")
                    {
                        enabled = true;
                        lockCanvas[i].transform.localScale = new Vector3(1, 1, 1);
                        lockCanvas[i].transform.position = hitCanvas.point;
                    }
                    else
                    {
                        enabled = false;
                        lockCanvas[i].transform.localScale = Vector3.zero;
                    }
                }
            }
            else
            {
                enabled = false;
                lockCanvas[i].transform.localScale = Vector3.zero;
            }

            if (enabled == true)
            {
                enemyOffset[i] = (CanvasCenter.position - lockCanvas[i].transform.position).magnitude;
            }
            else enemyOffset[i] = 0;
        }

        if (enemyOffset[1] > enemyOffset[0]) maxIndex = 1;
        else maxIndex = 0;

        if (enemyOffset[2] > enemyOffset[maxIndex]) maxIndex = 2;
        if (enemyOffset[maxIndex] == 0) maxIndex = -1;
    }
}
