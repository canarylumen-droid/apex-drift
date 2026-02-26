using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class obj_tracker: MonoBehaviour {
	public GameObject checkpoints;
	public GameObject otherUI;

	private int i=0;
	private GameObject checkp;
	public Text lapLabel;
	public Text mainlabel;
	private int lap=1;
	private int pos;
	public GameObject[] otherUIElements;
	private string tagi;
	private int bonus;

	// Revive system
	[Header("Revive")]
	public GameObject revivePanel;      // Panel with "Watch Ad to Revive" button
	public Button reviveButton;
	private int lastCheckpointIndex = 0;
	private bool hasRevived = false;

	Color gold =new Color32(0xD4,0xAF,0x37,0xFF);
	Color silver =new Color32(0xC0,0xC0,0xC0,0xFF);
	Color bronze =new Color32(0xCD,0x7F,0x32,0xFF);

	void Start(){
		tagi = "montacar" + (choose.num +1).ToString ("0");
		lapLabel.text=lap.ToString("0")+"/2";
		checkp= checkpoints.transform.GetChild (i).gameObject;
		this.transform.position = checkp.transform.position;
		this.transform.rotation = checkp.transform.rotation;

		// Hide revive panel at start
		if (revivePanel != null)
			revivePanel.SetActive(false);

		// Wire revive button
		if (reviveButton != null)
			reviveButton.onClick.AddListener(OnReviveClicked);

		// Listen for revive reward
		RewardedAdController.RewardGranted += OnRewardGranted;

		// Hide banner during racing
		if (AdManager.Instance != null)
			AdManager.Instance.HideBanner();
	}

	void OnDestroy()
	{
		RewardedAdController.RewardGranted -= OnRewardGranted;
	}

	 void OnTriggerEnter (Collider collision){
		if (i > 32 && collision.gameObject.tag == tagi){
			bonus=100*SceneManager.GetActiveScene().buildIndex; 
			i =-1;
			lap++;

			// Notify RaceTimer of lap complete
			if (RaceTimer.Instance != null)
				RaceTimer.Instance.OnLapComplete(lap - 1);

			if (lap == 3) {
				Time.timeScale = 0.7f;
				AudioListener.volume = 0.3f;
				pos = checkpoints.GetComponent<positioncheck> ().currentpos;
				otherUI.gameObject.SetActive (true);
				GameObject.FindObjectOfType<positioncheck>().car[choose.num].AddComponent<RCC_AICarController>();

				// Stop race timer
				if (RaceTimer.Instance != null)
					RaceTimer.Instance.StopTimer();

				// Award coins via CurrencyManager
				if (CurrencyManager.Instance != null)
					CurrencyManager.Instance.AwardRaceReward(pos);

				// Show RaceResults screen (if available)
				RaceResults results = FindObjectOfType<RaceResults>();
				if (results != null)
					results.ShowResults(pos);

				// Show banner after race ends
				if (AdManager.Instance != null)
					AdManager.Instance.ShowBanner();

				// Try interstitial at race end
				if (AdManager.Instance != null)
					AdManager.Instance.ShowInterstitialIfReady();

				// Also keep legacy score system working
				switch (pos) {
				case 1:
					mainlabel.GetComponent<Text> ().color = gold;
					mainlabel.text = "CONGRATULATIONS!\n\n1st Place!\n\n+" + bonus.ToString("0") + " pts";
					PlayerPrefs.SetInt ("score", PlayerPrefs.GetInt ("score")+bonus);
					break;
				case 2:
					mainlabel.GetComponent<Text> ().color = silver;
					mainlabel.text = "WELL DONE!\n\n2nd Place!\n\n+" + (bonus/2).ToString("0") + " pts";
					PlayerPrefs.SetInt ("score", PlayerPrefs.GetInt ("score")+bonus/2);
					break;
				case 3:
					mainlabel.GetComponent<Text> ().color = bronze;
					mainlabel.text = "NOT BAD!\n\n3rd Place!\n\n+" + (bonus/4).ToString("0") + " pts";
					PlayerPrefs.SetInt ("score", PlayerPrefs.GetInt ("score") +bonus/4);
					break;
				case 4:
					mainlabel.GetComponent<Text> ().color = Color.red;
					mainlabel.text = "4th Place\n\nBetter luck next time!";

					// Offer revive for 4th/5th place
					ShowReviveOption();
					break;
				case 5:
					mainlabel.GetComponent<Text> ().color = Color.red;
					mainlabel.text = "5th Place\n\nBetter luck next time!";

					// Offer revive for 4th/5th place
					ShowReviveOption();
					break;
			
				}
				for (int i = 0; i < otherUIElements.Length; i++)
				{
					otherUIElements[i].gameObject.SetActive(false);
				}
			}
			lapLabel.text=lap.ToString("0")+"/2";
		}
		if (collision.gameObject.tag == tagi) {
			i++;
			lastCheckpointIndex = i; // Save for revive

			checkp = checkpoints.transform.GetChild (i).gameObject;
			this.transform.position = checkp.transform.position;
			this.transform.rotation = checkp.transform.rotation;

		}


	}

	// ===== REVIVE SYSTEM =====

	void ShowReviveOption()
	{
		if (hasRevived) return; // Only one revive per race
		if (RewardedAdController.Instance == null) return;
		if (!RewardedAdController.Instance.CanWatchRewardedAd()) return;

		if (revivePanel != null)
			revivePanel.SetActive(true);
	}

	void OnReviveClicked()
	{
		if (RewardedAdController.Instance != null)
			RewardedAdController.Instance.ShowRewardedAd("revive");
	}

	void OnRewardGranted(string rewardType)
	{
		if (rewardType != "revive") return;
		if (hasRevived) return;

		hasRevived = true;

		// Hide revive panel
		if (revivePanel != null)
			revivePanel.SetActive(false);

		// Reset race state — go back to last checkpoint, reset lap to 2
		Time.timeScale = 1f;
		AudioListener.volume = 1f;
		otherUI.gameObject.SetActive(false);

		// Restart race timer
		if (RaceTimer.Instance != null)
			RaceTimer.Instance.ResumeTimer();

		// Move player car back to last checkpoint
		lap = 2; // Reset to last lap
		i = lastCheckpointIndex;
		checkp = checkpoints.transform.GetChild(i).gameObject;
		this.transform.position = checkp.transform.position;
		this.transform.rotation = checkp.transform.rotation;

		lapLabel.text = lap.ToString("0") + "/2";

		// Re-enable HUD elements
		for (int j = 0; j < otherUIElements.Length; j++)
		{
			otherUIElements[j].gameObject.SetActive(true);
		}

		// Remove AI from player car (was added at race end)
		RCC_AICarController aiComp = GameObject.FindObjectOfType<positioncheck>().car[choose.num].GetComponent<RCC_AICarController>();
		if (aiComp != null)
			Destroy(aiComp);

		// Re-enable player control
		GameObject.FindObjectOfType<positioncheck>().car[choose.num].GetComponent<RCC_CarControllerV3>().canControl = true;

		// Hide banner during racing again
		if (AdManager.Instance != null)
			AdManager.Instance.HideBanner();

		Debug.Log("[Revive] Player revived at checkpoint " + lastCheckpointIndex);
	}
}

