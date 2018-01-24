using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class GvrInputHandler : MonoBehaviour {
	[Tooltip("Holds pointer data that is checked to determine blocked input.")]
	private GvrPointerInputModule inputModule;
	private static GvrInputHandler handler;

	private bool unfoundInputModule = false;

	[Tooltip("Amount of time given before a tap turns into a hold.")]
	public float tapDuration = 0.25f;

	private bool pressedDown = false;
	private float downTime = 0.0f;
	private bool held = false;
	private bool blocked = false;
	private bool firstFrame = false;
	private float firstFrameTime = -1.0f;

	private void Awake() {
		if (GvrInputHandler.handler) {
			Debug.Log("Warning! Multiple input handlers detected in scene, there should only be one!");
		}
		else {
			GvrInputHandler.handler = this;
		}
	}

	private void Start() {
		this.inputModule = GvrPointerInputModule.FindInputModule();
		if (!this.inputModule) {
			Debug.Log("Cannot find GvrPointerInputModule in scene on Start!");
		}
	}

	public static GvrInputHandler FindInputHandler() {
		return GvrInputHandler.handler;
	}

	// Regular inputs: these only happen when GVR doesn't eat the input first
	public delegate void OnTap();
	public OnTap onTap;

	public delegate void OnHoldDown();
	public OnHoldDown onHoldDown;

	public delegate void OnHoldUp();
	public OnHoldUp onHoldUp;

	// Raw inputs: these always occur, even when GVR eats the input
	public delegate void OnTapRaw();
	public OnTap onTapRaw;

	public delegate void OnHoldDownRaw();
	public OnHoldDown onHoldDownRaw;

	public delegate void OnHoldUpRaw();
	public OnHoldUp onHoldUpRaw;


	public GameObject ModulePointerPressedObject() {
		if (!this.inputModule)
			return null;
		return this.inputModule.Impl.CurrentEventData.pointerPress;
	}

	public GameObject ModulePointerCurrentRaycastObject() {
		if (!this.inputModule)
			return null;
		return this.inputModule.Impl.CurrentEventData.pointerCurrentRaycast.gameObject;
	}


	private bool BlockedInput() {
		if (!this.inputModule || this.pressedDown)
			return false;

		return this.inputModule.Impl.IsPointerOverGameObject(0) &&
			this.inputModule.Impl.CurrentEventData.pointerEnter.layer != SortingLayer.NameToID("Pointer Non-Blocking");
	}

	// Processes input based on running through editor or smartphone
	// Will be used for more complex inputs, such as taps and held touches
	private void ProcessInput() {
		// If in editor
		if (Application.isEditor) {
			this.firstFrame = Input.GetMouseButtonDown(0);

			this.pressedDown = Input.GetMouseButton(0);
		}
		else {
			this.firstFrame = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;

			this.pressedDown = Input.touchCount > 0;
		}
	}


	// Update is called once per frame
	void Update() {
		if (!this.inputModule) {
			this.inputModule = GvrPointerInputModule.FindInputModule();
			if (!unfoundInputModule && !this.inputModule) {
				Debug.LogError("Must have a GvrPointerInputModule in scene for GVR usage!");
				this.unfoundInputModule = true;
			}
			else if (this.inputModule) {
				Debug.Log("Found a GvrPointerInputModule on Update.");
			}
		}

		if (BlockedInput()) {
			this.blocked = true;
		}

		ProcessInput();

		// Set downtime
		if (pressedDown) {
			this.downTime += Time.deltaTime;

			if (firstFrame)
				this.firstFrameTime = this.downTime;
			else
				this.firstFrameTime = -1;
		}

		// Processed inputs
		if (!blocked) {
			if (pressedDown) {
				if (!this.held && this.downTime > 0 && this.downTime > this.tapDuration)
					if (onHoldDown != null) onHoldDown();
			}
			else if ((this.downTime > 0 && this.downTime <= this.tapDuration) || this.firstFrameTime == this.downTime) {
				if (onTap != null) onTap();
			}
			else if (this.downTime > 0 && this.downTime > this.tapDuration) {
				if (onHoldUp != null) onHoldUp();
			}
		}

		if (!pressedDown)
			blocked = false;

		// Raw inputs
		if (pressedDown) {
			if (!this.held && this.downTime > 0 && this.downTime > this.tapDuration) {
				if (onHoldDownRaw != null) onHoldDownRaw();
				this.held = true;
			}
		}
		else if ((this.downTime > 0 && this.downTime <= this.tapDuration) || this.firstFrameTime == this.downTime) {
			if (onTapRaw != null) onTapRaw();
			this.downTime = 0;
			this.firstFrameTime = -1;
		}
		else if (this.downTime > 0 && this.downTime > this.tapDuration) {
			if (onHoldUpRaw != null) onHoldUpRaw();
			this.downTime = 0;
			this.held = false;
		}
	}
}
