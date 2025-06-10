using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private Button playAgainBtn;

    // Start is called before the first frame update
    void Start()
    {
        if (deathPanel != null)
        {
            deathPanel.SetActive(false);
        }
        if (playAgainBtn != null)
        {
            playAgainBtn.onClick.AddListener(() => PlayAgain());
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ShowPlayAgainPanel()
    {
        deathPanel.SetActive (true);
    }

    public void PlayAgain()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
