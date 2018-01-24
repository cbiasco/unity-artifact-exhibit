using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GazePickup : MonoBehaviour {
	[Tooltip("Handles input that isn't eaten by raycasting.")]
	private GvrInputHandler inputHandler;
	[Tooltip("Centers the object to your gaze on Pickup.")]
	public bool centerOnPickup = false;
	[Tooltip("Tap to pick up the object; false requires a held touch.")]
	public bool tapToPickup = false;

	private bool grabbed = false;
	private Camera playerCamera;
	private Transform parent;
	
	// Use this for initialization
	void Start () {
		this.inputHandler = GvrInputHandler.FindInputHandler();
		if (!this.inputHandler) {
			Debug.LogError("Must have a GvrInputHandler in scene for GVR usage!");
		}
		else {
			if (tapToPickup) {
				this.inputHandler.onTapRaw += TapPickup;
			}
			else {
				this.inputHandler.onHoldDownRaw += Pickup;
				this.inputHandler.onHoldUpRaw += Drop;
			}
		}
		this.playerCamera = Camera.main;
		this.parent = this.gameObject.transform.parent;

		// Create an EventTrigger component if one does not exist to intercept click events
		if (!this.gameObject.GetComponent<EventTrigger>()) {
			EventTrigger et = this.gameObject.AddComponent<EventTrigger>();
		}
	}

	private void OnDestroy() {
		if (this.inputHandler) {
			if (tapToPickup) {
				this.inputHandler.onTapRaw -= TapPickup;
			}
			else {
				this.inputHandler.onHoldDownRaw -= Pickup;
				this.inputHandler.onHoldUpRaw -= Drop;
			}
		}
	}

	private void Pickup() {
		if (this.inputHandler.ModulePointerPressedObject() == this.gameObject) {
			this.grabbed = true;

			if (this.centerOnPickup) {
				float distanceFromCamera = (this.gameObject.transform.position - this.playerCamera.transform.position).magnitude;
				this.gameObject.transform.position = this.playerCamera.transform.position +
					this.playerCamera.transform.forward * distanceFromCamera;
			}

			this.gameObject.transform.SetParent(this.playerCamera.transform);
		}
	}

	private void Drop() {
		this.grabbed = false;
		this.gameObject.transform.SetParent(parent);
	}

	private void TapPickup() {
		if (!this.grabbed && this.inputHandler.ModulePointerCurrentRaycastObject() == this.gameObject) {
			this.grabbed = true;

			if (this.centerOnPickup) {
				float distanceFromCamera = (this.gameObject.transform.position - this.playerCamera.transform.position).magnitude;
				this.gameObject.transform.position = this.playerCamera.transform.position +
					this.playerCamera.transform.forward * distanceFromCamera;
			}

			this.gameObject.transform.SetParent(this.playerCamera.transform);
		}
		else {
			this.grabbed = false;
			this.gameObject.transform.SetParent(parent);
		}
	}
}
