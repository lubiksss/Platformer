using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public SoundManager soundManager;
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    // Start is called before the first frame update
    public int Health;
    public PlayerMove player;
    public GameObject[] stages;
    public Image[] UIhealth;
    public Text UIPoint;
    public Text UIStage;
    public GameObject UIButton;

    void Update()
    {
        UIPoint.text = (totalPoint + stagePoint).ToString();
    }

    public void NextStage()
    {
        //Change Stage
        if (stageIndex < stages.Length - 1)
        {
            stages[stageIndex].SetActive(false);
            stageIndex++;
            player.transform.position = new Vector3(0, 3, 0);
            stages[stageIndex].SetActive(true);
            UIStage.text = "STAGE " + (stageIndex + 1);
        }
        else
        {
            Time.timeScale = 0;
            Text btnText = UIButton.GetComponentInChildren<Text>();
            btnText.text = "Game Clear";
            UIButton.SetActive(true);
            soundManager.source.clip = soundManager.audioFinish;
            soundManager.source.Play();
        }
        //Calculate Point
        totalPoint += stagePoint;
        stagePoint = 0;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (Health > 1)
            {
                //Player Reposition
                collision.attachedRigidbody.velocity = Vector2.zero;
                collision.transform.position = new Vector3(0, 3, 0);
            }
            //Health Down
            HealthDown();
        }
    }
    public void HealthDown()
    {
        if (Health > 1)
        {
            Health--;
            UIhealth[Health].color = new Color(1, 1, 1, 0.4f);
        }
        else
        {
            Health--;
            UIhealth[Health].color = new Color(1, 1, 1, 0.4f);
            //Player Die Effect
            player.OnDie();
            //Retry Button UI
            UIButton.SetActive(true);
        }
    }
    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
