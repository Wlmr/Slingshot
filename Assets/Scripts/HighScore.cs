using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class HighScore : MonoBehaviour {

    public GameObject fuel;
    public GameObject inGameScore;
    public GameObject highScoreScore;
    public GameObject highScoreText;

	// Use this for initialization
	void Start () {
        highScoreText.GetComponent<Text>().text = "high score:";
        if (PlayerPrefs.HasKey("highscore")){
            highScoreScore.GetComponent<Text>().text = PlayerPrefs.GetInt("highscore").ToString();
        }else {
            PlayerPrefs.SetInt("highscore", 0);
            highScoreScore.GetComponent<Text>().text = "0";
        }
	}

    public void HighScoreUpdater() {
        highScoreScore.GetComponent<Text>().text = PlayerPrefs.GetInt("highscore").ToString();
    }
	
	// Update is called once per frame

    public void CongratulateNewHighScore(bool boolean) {
        if (boolean == true) {
            highScoreText.GetComponent<Text>().text = "NEW HIGH SCORE!!!!!";
            Show(true);
        }else {
            Show(false);
            highScoreText.GetComponent<Text>().text = "high score:";

        }
    }

    public void Show(bool boolean) {
        fuel.SetActive(!boolean);
        inGameScore.SetActive(!boolean);
        highScoreScore.SetActive(boolean);
        highScoreText.SetActive(boolean);

    }
}
