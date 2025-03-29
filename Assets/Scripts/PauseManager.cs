using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public interface IPausable
{
    void Pause();
    void Unpause();
}


public class PauseManager : MonoBehaviour
{
    private List<IPausable> pausables = new List<IPausable>();
    public UnityEvent RescanEvent;

    void Start()
    {
        pausables.Clear();
        IPausable[] foundPausables = FindObjectsOfType<MonoBehaviour>(true) as IPausable[];
        pausables.AddRange(foundPausables);
    }

    public void PauseAll()
    {
        foreach (IPausable pausable in pausables)
        {
            pausable.Pause();
        }
    }

    public void UnpauseAll()
    {
        foreach (IPausable pausable in pausables)
        {
            pausable.Unpause();
        }
    }


    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
