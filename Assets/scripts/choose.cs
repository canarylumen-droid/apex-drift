using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class choose : MonoBehaviour
{
	public static int num;
	public RCC_CarControllerV3 [] car;
	public GameObject trans;
	public GameObject otherUIElement;
	public GameObject[] otherUIElements;
	private int mult;

	private GameObject carCamera;
	void Start () {
		mult=SceneManager.GetActiveScene().buildIndex; 
		for (int i = 0; i < otherUIElements.Length; i++)
		{
			otherUIElements[i].gameObject.SetActive(false);
		}
	}
	public void Choose(int n){
		// Check both legacy score AND new coin system for car unlock
		bool canUnlock = false;

		// Legacy check
		if (PlayerPrefs.GetInt ("score") >= n * 1000)
			canUnlock = true;

		// New coin-based unlock check
		if (CurrencyManager.Instance != null && n < GameConfig.CAR_UNLOCK_COSTS.Length)
		{
			if (CurrencyManager.Instance.Coins >= GameConfig.CAR_UNLOCK_COSTS[n])
				canUnlock = true;
		}

		if (!canUnlock)
			return;

		num = n;
		GameObject.FindObjectOfType<RCC_Camera> ().SetPlayerCar (car [n].gameObject);

		for (int i = 0; i < 5; i++) {
			if (n == i) {
				car [i].gameObject.GetComponent<RCC_CarControllerV3> ().steerHelperStrength =.55f ;

				// Apply car upgrades from shop
				if (CarUpgradeData.Instance != null)
				{
					RCC_CarControllerV3 ctrl = car[i].gameObject.GetComponent<RCC_CarControllerV3>();
					float speedMult = CarUpgradeData.Instance.GetStatMultiplier(n, CarUpgradeData.StatType.Speed);
					float handlingMult = CarUpgradeData.Instance.GetStatMultiplier(n, CarUpgradeData.StatType.Handling);
					ctrl.maxspeed *= speedMult;
					ctrl.steerHelperStrength *= handlingMult;
				}

				continue;
			}
			car [i].gameObject.AddComponent<RCC_AICarController> ();
			car [i].gameObject.GetComponent<RCC_AICarController> ().wideRayLength = 30;
			car [i].gameObject.GetComponent<RCC_AICarController> ().tightRayLength = 50;
			car [i].gameObject.GetComponent<RCC_AICarController> ().limitSpeed =true;
			car [i].gameObject.GetComponent<RCC_CarControllerV3> ().steerHelperStrength += 0.05f*(mult-1)+0.015f*i;
			car [i].gameObject.GetComponent<RCC_AICarController> ().maximumSpeed = 150+(mult-1)*18;
			car [i].gameObject.GetComponent<RCC_AICarController> ().nextWaypointPassRadius = 30;
		}
		car [0].gameObject.transform.position = car [n].gameObject.transform.position;
		car [n].gameObject.transform.position = trans.transform.position;
		for (int i = 0; i < otherUIElements.Length; i++) {
			otherUIElements [i].gameObject.SetActive (true);
		}

		otherUIElement.gameObject.SetActive (false);
  
	}

}

