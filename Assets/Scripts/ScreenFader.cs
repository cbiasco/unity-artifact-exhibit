using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenFader : MonoBehaviour {
	//[Tooltip("The speed at which the fade in/out happens by default; 1.0 is instantly.")]
	//public float defaultFadeRate = 0.02f;

	public enum Transition {
		IN,
		OUT
	}

	private IEnumerator coroutine;
	private GameObject fadeBox;
	private Camera playerCamera;

	// Events
	public delegate void OnStart();
	public OnStart onStart;

	public delegate void OnComplete();
	public OnComplete onComplete;

	private void Start() {
		this.playerCamera = Camera.main;
	}

	public void UpdatePositionToCamera() {
		if (this.fadeBox) {
			this.fadeBox.transform.position = this.playerCamera.transform.position
				+ playerCamera.transform.forward * .09f;
			this.fadeBox.transform.rotation = this.playerCamera.transform.rotation;
		}
	}

	public void StartFade(Transition t, float rate = 0.02f, float step = 0.01f) {
		bool fadingOut = t == Transition.OUT;
		if (!this.fadeBox) {
			this.fadeBox = new GameObject("Fade Box");
			SpriteRenderer sr = this.fadeBox.AddComponent<SpriteRenderer>();
			sr.sprite = Resources.Load<Sprite>("Black");
			sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0);
			UpdatePositionToCamera();
			this.fadeBox.transform.parent = this.playerCamera.transform;
		}

		coroutine = Fade(fadingOut, rate, step);
		StartCoroutine(coroutine);
	}

	private IEnumerator Fade(bool fadingOut, float rate, float step) {
		if (this.onStart != null)
			this.onStart();

		SpriteRenderer sr = this.fadeBox.GetComponent<SpriteRenderer>();
		float f, g;
		int s;
		Color c;

		if (fadingOut) {
			g = 0;
			s = 1;
		}
		else {
			g = 1;
			s = -1;
		}

		for (f = 0.0f; f <= 1.0f; f += rate) {
			c = sr.color;
			c.a = g + f * s;
			sr.color = c;
			yield return new WaitForSeconds(step);
		}

		c = sr.color;
		c.a = f;
		sr.color = c;
		if (!fadingOut)
			Destroy(this.fadeBox);
		if (this.onComplete != null)
			this.onComplete();
	}
}
