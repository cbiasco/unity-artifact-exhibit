using System.Collections;
using System.Collections.Generic;

using UnityEngine;
#if UNITY_5_5_OR_NEWER
    using UnityEngine.AI;
#endif


public class ForwardMovement : MonoBehaviour {
	[Tooltip("Handles input that isn't eaten by raycasting.")]
	private GvrInputHandler inputHandler;
	[Tooltip("Movement speed of the player using the Forward Movement script.")]
	public float speed = 1.0f;
	[Tooltip("Player's height from the NavMesh floor (should probably be somewhere else).")]
	public float playerHeight = 4f;

	[Tooltip("Setting to true sets the movement control to tapping on and off instead of holding a touch.")]
	public bool tapToMove = false;
	[Tooltip("Allows movement regardless of looking at interactable objects.")]
	public bool useRawInputs = false;

	private Camera playerCamera;
	private bool moveActive = false;

	// Use this for initialization
	void Start () {
		this.inputHandler = GvrInputHandler.FindInputHandler();
		if (!this.inputHandler) {
			Debug.LogError("Must have a GvrInputHandler in scene for GVR usage!");
		}
		else {
			if (this.useRawInputs) {
				if (this.tapToMove) {
					this.inputHandler.onTapRaw += FlipMovement;
				}
				else {
					this.inputHandler.onHoldDownRaw += StartMovement;
					this.inputHandler.onHoldUpRaw += StopMovement;
				}
			}
			else {
				if (this.tapToMove) {
					this.inputHandler.onTap += FlipMovement;
				}
				else {
					this.inputHandler.onHoldDown += StartMovement;
					this.inputHandler.onHoldUp += StopMovement;
				}
			}
		}
		this.playerCamera = Camera.main;

        /*RaycastHit hit;
        if (!Physics.Raycast(this.gameObject.transform.position, new Vector3(0, -1, 0), out hit))
            Physics.Raycast(this.gameObject.transform.position, new Vector3(0, 1, 0), out hit);
        */

        NavMeshHit meshHit;
        bool validNavMeshLocation = NavMesh.SamplePosition(
            this.gameObject.transform.position,
            out meshHit,
            this.playerHeight,
            NavMesh.AllAreas);

        if (validNavMeshLocation) {
            this.gameObject.transform.position = new Vector3(
                this.gameObject.transform.position.x,
                meshHit.position.y + this.playerHeight,
                this.gameObject.transform.position.z);
        }
        else {
            this.gameObject.transform.position = new Vector3(
                this.gameObject.transform.position.x,
                this.gameObject.transform.position.y + this.playerHeight,
                this.gameObject.transform.position.z);
        }
    }

	private void OnDestroy() {
		if (this.inputHandler) {
			if (this.useRawInputs) {
				if (this.tapToMove) {
					this.inputHandler.onTapRaw -= FlipMovement;
				}
				else {
					this.inputHandler.onHoldDownRaw -= StartMovement;
					this.inputHandler.onHoldUpRaw -= StopMovement;
				}
			}
			else {
				if (this.tapToMove) {
					this.inputHandler.onTap -= FlipMovement;
				}
				else {
					this.inputHandler.onHoldDown -= StartMovement;
					this.inputHandler.onHoldUp -= StopMovement;
				}
			}
		}
	}

	private void FlipMovement() {
		this.moveActive = !this.moveActive;
	}

	private void StartMovement() {
		this.moveActive = true;
	}

	private void StopMovement() {
		this.moveActive = false;
	}

    // Finds the appropriate Next position according to the touch movement
    private Vector3 QueryNextPosition(Vector3 forwardDirection) {
        bool validNavMeshLocation = false;

        // Determine the movement direction
        Vector3 movementDirection = forwardDirection;
        movementDirection.y = 0;
        movementDirection.Normalize(); // may want to try skipping normalizing
        movementDirection *= this.speed * 4 * Time.deltaTime;

        Vector3 navMeshPosition = this.gameObject.transform.position - new Vector3(0, this.playerHeight, 0);
        /*RaycastHit hit;
        if (Physics.Raycast(this.gameObject.transform.position, new Vector3(0, -1, 0), out hit)) {
            navMeshPosition = hit.point;
        }
        else {
            Debug.Log("Can't hit NavMesh with downward raycast!");
            return new Vector3(0, 0, 0);
        }*/

        NavMeshHit meshHit;
        validNavMeshLocation = NavMesh.SamplePosition(
            navMeshPosition + movementDirection,
            out meshHit,
            this.playerHeight,
            NavMesh.AllAreas);

        if (validNavMeshLocation)
        {
            return meshHit.position;
        }
        else
        {
            NavMeshHit edgeHit;
            bool foundEdge = NavMesh.FindClosestEdge(
                navMeshPosition + movementDirection,
                out edgeHit,
                NavMesh.AllAreas);
            if (foundEdge)
            {
                return edgeHit.position;
            }
        }
        Debug.Log("Totally invalid NavMesh location!");
        return new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update () {
		if (this.moveActive && this.playerCamera)
        {
            this.gameObject.transform.position = QueryNextPosition(this.playerCamera.transform.forward);
            this.gameObject.transform.position += new Vector3(0, this.playerHeight, 0);
        }
	}

	private bool GetInputHandler() {
		GameObject player = Camera.main.gameObject.transform.parent.gameObject;
		this.inputHandler = player.GetComponentInChildren<GvrInputHandler>();
		return this.inputHandler;
	}
}
