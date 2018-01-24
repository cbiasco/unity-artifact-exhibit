using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour {
	private int currentSceneIdx = 0;
	private int loadedSceneIdx = -1;
	private bool optionSelected = false;

	private ScreenFader fader;

	private static AsyncOperation loadSceneOp;

	private void Start() {
		currentSceneIdx = SceneManager.GetActiveScene().buildIndex;

		fader = Camera.main.GetComponent<ScreenFader>();
		if (!fader) {
			Debug.LogWarning("No Screen Fader on main camera! Attaching one to it.");
			fader = Camera.main.gameObject.AddComponent<ScreenFader>();
		}
	}

	public void BackToFirst() {
		if (currentSceneIdx == 0 || optionSelected)
			return;

		loadedSceneIdx = 0;
		optionSelected = true;

		prepareSwap();
	}

	public void Next() {
		if (optionSelected)
			return;

		loadedSceneIdx = (currentSceneIdx + 1) % SceneManager.sceneCountInBuildSettings;
		optionSelected = true;

		prepareSwap();
	}

	public void Previous() {
		if (optionSelected)
			return;

		loadedSceneIdx = (currentSceneIdx - 1 < 0) ? SceneManager.sceneCountInBuildSettings - 1 : currentSceneIdx - 1;
		optionSelected = true;

		prepareSwap();
	}

	private void prepareSwap() {
		// NOTE: An EmulatorConfig error will pop up on moving between scenes since
		// another class tries to instance it in a subthread.
		// The current fix for this is to add DontDestroyOnLoad to the instance creation code
		// inside of GoogleVR\Scripts\Controller\Internal\Emulator\EmulatorConfig.cs

		fader.onComplete += SwapScene;
		fader.StartFade(ScreenFader.Transition.OUT);
	}

	private void SwapScene() {
		fader.onComplete -= SwapScene;

		SceneManager.LoadSceneAsync(loadedSceneIdx);
		currentSceneIdx = loadedSceneIdx;
		loadedSceneIdx = -1;
		optionSelected = false;
	}
}
