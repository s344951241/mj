using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class horMy : MonoBehaviour {

    public int vect = 0;//0左,1右,2上,3下
    private float pos;
    // Use this for initialization
    void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        switch (vect)
        {
            case 0:
                pos = transform.localPosition.x - transform.GetComponent<RectTransform>().rect.size.x/2;
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (transform.GetChild(i).gameObject.activeSelf)
                    {
                        transform.GetChild(i).GetComponent<RectTransform>().pivot = new Vector2(0, 1);
                        transform.GetChild(i).localPosition = new Vector3(pos, transform.GetChild(i).localPosition.y, 0);
                        pos += transform.GetChild(i).GetComponent<RectTransform>().rect.size.x;
                    }

                }
                break;

            case 1:
                pos = transform.localPosition.x + transform.GetComponent<RectTransform>().rect.size.x / 2;
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (transform.GetChild(i).gameObject.activeSelf)
                    {
                        transform.GetChild(i).GetComponent<RectTransform>().pivot = new Vector2(1, 1);
                        transform.GetChild(i).localPosition = new Vector3(pos, transform.GetChild(i).localPosition.y, 0);
                        pos -= transform.GetChild(i).GetComponent<RectTransform>().rect.size.x;
                    }

                }
                break;

            case 2:
                pos = transform.localPosition.y + transform.GetComponent<RectTransform>().rect.size.y / 2;
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (transform.GetChild(i).gameObject.activeSelf)
                    {
                        transform.GetChild(i).GetComponent<RectTransform>().pivot = new Vector2(1, 1);
                        transform.GetChild(i).localPosition = new Vector3(transform.GetChild(i).localPosition.x, pos, transform.GetChild(i).localPosition.z);
                        pos -= transform.GetChild(i).GetComponent<RectTransform>().rect.size.y;
                    }

                }
                break;

            case 3:
                pos = transform.localPosition.y - transform.GetComponent<RectTransform>().rect.size.y / 2;
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (transform.GetChild(i).gameObject.activeSelf)
                    {
                        transform.GetChild(i).GetComponent<RectTransform>().pivot = new Vector2(1, 0);
                        transform.GetChild(i).localPosition = new Vector3(transform.GetChild(i).localPosition.x, pos, transform.GetChild(i).localPosition.z);
                        pos += transform.GetChild(i).GetComponent<RectTransform>().rect.size.y;
                    }

                }
                break;
        }
	}
}
