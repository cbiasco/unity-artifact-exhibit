using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPoint : MonoBehaviour {
	private GameObject onSphere;
	private GameObject offSphere;

	public static GameObject currentPoint;
	private static bool multipleStartPointsDetected = false;
	private static bool teleporting = false;

	[Tooltip("If set to true, player's position is set to this point's position at the start " +
			 "of the simulation. \n" +
			 "NOTE: Multiple starting points will be arbitrarily resolved and a warning will be thrown!")]
	public bool startingPoint = false;

	private ScreenFader fader;

	// Use this for initialization
	void Start () {
		for (int i = 0; i < this.transform.childCount; i++) {
			if (this.transform.GetChild(i).gameObject.name == "OnSphere") {
				this.onSphere = this.transform.GetChild(i).gameObject;
			}
			else if (this.transform.GetChild(i).gameObject.name == "OffSphere") {
				this.offSphere = this.transform.GetChild(i).gameObject;
			}
		}

		fader = Camera.main.GetComponent<ScreenFader>();
		if (!fader) {
			Debug.LogWarning("No Screen Fader on main camera! Attaching one to it.");
			fader = Camera.main.gameObject.AddComponent<ScreenFader>();
		}

		if (!TeleportPoint.currentPoint && this.startingPoint) {
			TeleportPoint.currentPoint = this.gameObject;
			PointUnselected();
			this.gameObject.SetActive(false);
			Camera.main.transform.parent.transform.position = this.transform.position;
		}
		else if (!TeleportPoint.multipleStartPointsDetected && TeleportPoint.currentPoint && this.startingPoint) {
			Debug.LogWarning("Multiple starting points! Starting point will be chosen arbitrarily.");
			TeleportPoint.multipleStartPointsDetected = true;
		}
	}

	public void PointSelected() {
		this.onSphere.SetActive(true);
		this.offSphere.SetActive(false);
	}

	public void PointUnselected() {
		this.onSphere.SetActive(false);
		this.offSphere.SetActive(true);
	}

	public void StartTeleport() {
		if (TeleportPoint.teleporting)
			return;

		TeleportPoint.teleporting = true;
		this.fader.onComplete += TeleportToPoint;
		this.fader.StartFade(ScreenFader.Transition.OUT, 0.1f);
	}

	private void TeleportToPoint() {
		this.fader.onComplete -= TeleportToPoint;

		if (TeleportPoint.currentPoint)
			TeleportPoint.currentPoint.SetActive(true);

		TeleportPoint.currentPoint = this.gameObject;
		PointUnselected();
		this.gameObject.SetActive(false);
		Camera.main.transform.parent.transform.position = this.transform.position;

		TeleportPoint.teleporting = false;
		this.fader.StartFade(ScreenFader.Transition.IN, 0.1f);
	}

	void Update() {
		this.transform.Rotate(0, .3f, 0);
	}
}
