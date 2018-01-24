using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GazeRotator : MonoBehaviour {

    [System.Serializable]
    public enum RotationCenter
    {
        Pivot,
        Center
    }

    [Tooltip("Determines the location of the pivot of the rotation.")]
    public RotationCenter rotationCenter = RotationCenter.Center;

    private Vector3 centerOffset;

    [Space]

    [Tooltip("Handles input that isn't eaten by raycasting.")]
	private GvrInputHandler inputHandler;
	[Tooltip("The object's rotation speed depends on the gaze's distance from the object center. \n" +
			 "Setting this to false makes gaze \"grab\" the object to be pulled manually.")]
	public bool autoRotate = true;
	[Tooltip("Rotation persists after the object is released.")]
	[ConditionalHide("autoRotate", true, false)]
	public bool rotationalInertia = true;
	[Tooltip("Head tilting rotates the object along the Z-axis.")]
	public bool headTiltRotation = true;
	[Tooltip("Rotation speed multiplier.")]
	public float rotationSpeed = 1.0f;

	struct Rotation {
		public Vector3 axis;
		public float magnitude;
	}

	private Plane raycastSurface;
	private Vector3 grabPoint;
	private Camera playerCamera;

	private Rotation rotation;
	private Rotation tiltRotation;
	private float previousTilt = 0;

	private bool rotateActive = false;

	// Use this for initialization
	void Start () {
		this.inputHandler = GvrInputHandler.FindInputHandler();
		if (!this.inputHandler) {
			Debug.LogError("Must have a GvrInputHandler in scene for GVR usage!");
		}
		else {
			this.inputHandler.onHoldDownRaw += StartRotating;
			this.inputHandler.onHoldUpRaw += StopRotating;
		}
		this.playerCamera = Camera.main;

        // Get the center offset of the models based on the model bounds
        GetCenterOffset();

        // Create the plane that allows for rotation when looking off of the object
        Vector3 planeNormal = this.playerCamera.transform.position - this.gameObject.transform.TransformPoint(this.centerOffset);
		planeNormal.Normalize();
		this.raycastSurface = new Plane(planeNormal, this.gameObject.transform.TransformPoint(this.centerOffset));

		// Create an EventTrigger component if one does not exist to intercept click events
		if (!this.gameObject.GetComponent<EventTrigger>()) {
			EventTrigger et = this.gameObject.AddComponent<EventTrigger>();
		}
	}

	private void OnDestroy() {
		if (this.inputHandler) {
			this.inputHandler.onHoldDownRaw -= StartRotating;
			this.inputHandler.onHoldUpRaw -= StopRotating;
		}
	}

	private void StartRotating() {
		// Check if this object is what we're looking at
		if (this.inputHandler.ModulePointerPressedObject() == this.gameObject) {
			this.rotateActive = true;
		}
		else return;

		// Set up rotation based on gaze point
		if (this.autoRotate)
			this.grabPoint = this.gameObject.transform.TransformPoint(this.centerOffset);
		else {
			// Correct the plane normal first to get an accurate grab point
			Vector3 planeNormal = this.playerCamera.transform.position - this.gameObject.transform.TransformPoint(this.centerOffset);
			planeNormal.Normalize();
			this.raycastSurface.SetNormalAndPosition(planeNormal, this.gameObject.transform.TransformPoint(this.centerOffset));

			Ray r = new Ray(this.playerCamera.transform.position, this.playerCamera.transform.forward);
			float t;
			this.raycastSurface.Raycast(r, out t);
			this.grabPoint = r.GetPoint(t);

			this.previousTilt = this.playerCamera.transform.rotation.z * 100;
		}
	}

	private void StopRotating() {
		this.rotateActive = false;
	}

	// Update is called once per frame
	void Update () {
		if (this.rotateActive) {
			Vector3 planeNormal = this.playerCamera.transform.position - this.gameObject.transform.TransformPoint(this.centerOffset);
			planeNormal.Normalize();
			this.raycastSurface.SetNormalAndPosition(planeNormal, this.gameObject.transform.TransformPoint(this.centerOffset));

			float t;
			Ray r = new Ray(this.playerCamera.transform.position, this.playerCamera.transform.forward);
			if (this.raycastSurface.Raycast(r, out t)) {
				Vector3 hitPoint = r.GetPoint(t);
				Vector3 hitRadius = hitPoint - this.grabPoint;
				if (!this.autoRotate)
					this.grabPoint = hitPoint;
				
				// Take cross product of gaze point vector and the view vector for the rotation axis
				Vector3 rotationAxis = Vector3.Cross(hitRadius, this.playerCamera.transform.forward);
				this.rotation.axis = rotationAxis;

				// Rotate game object around rotation axis based on magnitude of gaze point vector
				float mult;
				if (this.autoRotate)
					mult = 1.5f;
				else
					mult = 50f;
				this.rotation.magnitude = hitRadius.magnitude * mult * this.rotationSpeed;
				if (this.rotationSpeed < 0) {
					this.rotation.magnitude *= -1;
					this.rotation.axis *= -1;
				}

				if (this.rotation.magnitude > 1 || (this.autoRotate && this.rotation.magnitude > .01))
                    RotateObject(this.rotation.axis, this.rotation.magnitude);
			}

			if (headTiltRotation) {
				if (this.autoRotate)
                    RotateObject(this.gameObject.transform.TransformPoint(this.centerOffset) - this.playerCamera.transform.position,
                        this.playerCamera.transform.rotation.z * 2);
				else {
					this.tiltRotation.axis = this.gameObject.transform.TransformPoint(this.centerOffset) - this.playerCamera.transform.position;
					this.tiltRotation.magnitude = this.playerCamera.transform.rotation.z * 100 - this.previousTilt;
					this.previousTilt = this.playerCamera.transform.rotation.z * 100;
                    RotateObject(this.tiltRotation.axis, this.tiltRotation.magnitude);
				}
			}
		}
		else if (!this.autoRotate && this.rotationalInertia) {
			if (this.rotation.magnitude > 1)
                RotateObject(this.rotation.axis, this.rotation.magnitude);
			if (Mathf.Abs(this.tiltRotation.magnitude) > .01)
                RotateObject(this.tiltRotation.axis, this.tiltRotation.magnitude);
		}
	}

    private void RotateObject(Vector3 axis, float magnitude)
    {
        if (rotationCenter == RotationCenter.Pivot)
        {
            this.transform.Rotate(axis, magnitude, Space.World);
        }
        else
        {
            this.transform.RotateAround(this.gameObject.transform.TransformPoint(this.centerOffset), axis, magnitude);
        }
    }

    private void GetCenterOffset()
    {
        this.centerOffset = GetTotalBounds(this.gameObject).center / this.gameObject.transform.localScale.x;
    }

    private Bounds GetTotalBounds(GameObject go, bool checkForSkinnedRenderers = true)
    {
        bool foundRenderer = false;
        Bounds bounds = new Bounds();
        MeshRenderer[] mrs = go.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mr in mrs)
        {
            Vector3 centerRelativeToParent = mr.bounds.center - mr.transform.position;
            Vector3 parentTranslation = (mr.transform.parent != this.transform.parent) ? mr.transform.parent.rotation* Vector3.Scale(mr.transform.lossyScale, mr.transform.localPosition) : Vector3.zero;
            Vector3 size = mr.bounds.extents * 2f;

            bounds.Encapsulate(new Bounds(centerRelativeToParent + 
                               parentTranslation,
                               size));
            foundRenderer = true;
        }
        if (checkForSkinnedRenderers)
        {
            SkinnedMeshRenderer[] smrs = go.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer smr in smrs)
            {
                Vector3 centerRelativeToParent = smr.bounds.center - smr.transform.position;
                Vector3 parentTranslation = (smr.transform.parent != this.transform.parent) ? smr.transform.parent.rotation * Vector3.Scale(smr.transform.lossyScale, smr.transform.localPosition) : Vector3.zero;
                Vector3 size = smr.bounds.extents * 2f;

                bounds.Encapsulate(new Bounds(centerRelativeToParent +
                                   parentTranslation,
                                   size));
                foundRenderer = true;
            }
        }

        if (!foundRenderer)
        {
            Debug.Log("Warning! " + go.name + " has no MeshRenderers in its heirarchy!");
        }

        return bounds;
    }
}
