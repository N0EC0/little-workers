using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    [Header("Main UI")]
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text timerText;

    [Header("Workers")]
    [SerializeField] private TMP_Text worker1Text;
    [SerializeField] private TMP_Text worker2Text;

    [Header("Happiness Bars")]
    [SerializeField] private Slider worker1HappinessBar;
    [SerializeField] private Slider worker2HappinessBar;

    [SerializeField] private Image worker1Fill;
    [SerializeField] private Image worker2Fill;

    [SerializeField] private Worker worker1;
    [SerializeField] private Worker worker2;

    [Header("Results Screen")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TMP_Text resultTitle;
    [SerializeField] private TMP_Text resultMoney;
    [SerializeField] private TMP_Text resultHappiness;


    void Start()
    {
        resultPanel.SetActive(false);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnShiftEnded += ShowResults;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        // Money
        moneyText.text = "Money: " + Mathf.FloorToInt(GameManager.Instance.Money) + "$ / " + Mathf.FloorToInt(GameManager.Instance.Quota) + "$";

        // Timer
        timerText.text = "Time: " + Mathf.CeilToInt(GameManager.Instance.TimeReamaining);

        // Worker 1
        worker1Text.text = "Worker 1\n" + worker1.CurrentState;

        // Worker 2
        worker2Text.text = "Worker 2\n" + worker2.CurrentState;

        worker1HappinessBar.value = worker1.Happiness;
        worker2HappinessBar.value = worker2.Happiness;

        UpdateBarColor(worker1Fill, worker1.Happiness);
        UpdateBarColor(worker2Fill, worker2.Happiness);
    }


    void UpdateBarColor(Image fill, float happiness)
    {
        if (happiness > 60f)
        {
            fill.color = Color.greenYellow;
        }
        else if (happiness > 30f)
        {
            fill.color = Color.yellow;
        }
        else
        {
            fill.color = Color.red;
        }
    }


    void ShowResults()
    {
        // Pause the game
        //Time.timeScale = 0f;

        resultPanel.SetActive(true);
        float averageHappiness = (worker1.Happiness + worker2.Happiness) / 2f;

        if (GameManager.Instance.Money >= GameManager.Instance.Quota)
        {
            resultTitle.text = "QUOTA REACHED!";
        }
        else
        {
            resultTitle.text = "QUOTA FAILED!";
        }
        resultMoney.text = "Money: " + Mathf.FloorToInt(GameManager.Instance.Money) + "$ / " + Mathf.FloorToInt(GameManager.Instance.Quota) + "$";
        resultHappiness.text = "Average Happiness: " + Mathf.FloorToInt(averageHappiness);

        worker1.StopAllAudio();
        worker2.StopAllAudio();
        Time.timeScale = 0f;

    }


    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
