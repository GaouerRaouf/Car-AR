using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIManager : MonoBehaviour
{
    public GameObject currentPage;

    public void ChangePage(GameObject page)
    {
        currentPage.SetActive(false);
        currentPage = page;
        currentPage.SetActive(true);
    }

    public void SetCurrentPage(GameObject page)
    {
        currentPage = page;
        currentPage.SetActive(true);
    }

    public void OpenTabAdd(GameObject tab)
    {
        tab.SetActive(true);
    }

    public void CloseTabAdd(GameObject tab)
    {
        tab.SetActive(false);
    }

    public void TogglePage(GameObject go)
    {
        go.SetActive(!go.activeInHierarchy);
    }


}
