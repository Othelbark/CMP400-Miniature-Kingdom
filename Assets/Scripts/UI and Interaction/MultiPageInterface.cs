using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiPageInterface : MonoBehaviour
{

    public GameObject[] pages;

    public GameObject next;
    public GameObject back;
    public Text pageDisplay; 

    protected int pageIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject page in pages)
        {
            page.SetActive(false);
        }

        pages[pageIndex].SetActive(true);

        back.SetActive(false);

        if (pages.Length > 1)
        {
            next.SetActive(true);
        }
        else
        {
            next.SetActive(false);
        }

        pageDisplay.text = (pageIndex + 1) + "/" + pages.Length;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Next()
    {
        if (pageIndex + 1 < pages.Length)
        {
            pages[pageIndex].SetActive(false);
            pageIndex++;
            pages[pageIndex].SetActive(true);

            back.SetActive(true);

            if (pages.Length - 1 > pageIndex)
            {
                next.SetActive(true);
            }
            else
            {
                next.SetActive(false);
            }

            pageDisplay.text = (pageIndex + 1) + "/" + pages.Length;
        }
    }

    public void Back()
    {
        if (pageIndex - 1 >= 0)
        {
            pages[pageIndex].SetActive(false);
            pageIndex--;
            pages[pageIndex].SetActive(true);

            next.SetActive(true);

            if (pageIndex > 0)
            {
                back.SetActive(true);
            }
            else
            {
                back.SetActive(false);
            }

            pageDisplay.text = (pageIndex + 1) + "/" + pages.Length;
        }
    }
}
