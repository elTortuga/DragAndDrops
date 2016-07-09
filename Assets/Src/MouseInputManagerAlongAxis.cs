using UnityEngine;
using System.Collections;

public class MouseInputManagerAlongAxis : MonoBehaviour {
	public Camera mainCamera;
	private bool draggingItem = false;
	private bool ignoreInput = false;
	private GameObject draggedObject;
	private Vector3 adjacentVector;
	private float initialRayCameraOriginX;

	void Update () {
		if (HasInput){
			if (draggingItem) {
//				Drag();
//				DragAlongAxisX();
//				DragAlongAxisY();
				DragAlongAxisZ();
			} else {
				Pickup();
			}
		}
		else {
			ignoreInput = false;
			if (draggingItem)
				DropItem();
		}	
	}

	void Pickup() {
		if (!ignoreInput) {
			Ray rayCamera = mainCamera.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] touches = Physics.RaycastAll(rayCamera.origin, rayCamera.direction, 20.0f);
			if (touches.Length > 0) {
				var hit = touches[0];
				if (hit.transform != null && touches[0].transform.gameObject.GetComponent("Draggable") != null) {
					draggingItem = true;
					draggedObject = hit.transform.gameObject;
					draggedObject.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
//					this.SetAdjacentVector(rayCamera);
//					this.SetAdjacentVectorAlongAxisX(rayCamera);
//					this.SetAdjacentVectorAlongAxisY(rayCamera);
					this.SetAdjacentVectorAlongAxisZ(rayCamera);
				}
			} else {
				this.ignoreInput = true;
			}
		}
	}

	void Drag() {
		Ray rayCamera = mainCamera.ScreenPointToRay(Input.mousePosition);
		float angleBetweenMouseDirectionAndCameraFoward = Mathf.Deg2Rad * Vector3.Angle(mainCamera.transform.forward, rayCamera.direction);
		Vector3 draggedObjectVectorFromCamera = rayCamera.direction.normalized * (adjacentVector.magnitude / Mathf.Cos(angleBetweenMouseDirectionAndCameraFoward));
		draggedObject.transform.position = rayCamera.origin + draggedObjectVectorFromCamera;
	}

	void DragAlongAxisX() {
		Ray rayCamera = mainCamera.ScreenPointToRay(Input.mousePosition);
		Vector3 rayCameraDirectionAlongDraggedObjectXZPlane = new Vector3(rayCamera.direction.x, 0.0f, rayCamera.direction.z);
		float angleBetweenMouseDirectionAndCameraFoward = Mathf.Deg2Rad * Vector3.Angle(mainCamera.transform.forward, rayCameraDirectionAlongDraggedObjectXZPlane);
		Vector3 draggedObjectVectorFromCamera = rayCameraDirectionAlongDraggedObjectXZPlane.normalized * (adjacentVector.magnitude / Mathf.Cos(angleBetweenMouseDirectionAndCameraFoward));
		draggedObject.transform.position = new Vector3(rayCamera.origin.x, draggedObject.transform.position.y, rayCamera.origin.z) + draggedObjectVectorFromCamera;
	}

	void DragAlongAxisY() {
		Ray rayCamera = mainCamera.ScreenPointToRay(Input.mousePosition);
		Vector3 rayCameraDirectionAlongDraggedObjectYZPlane = new Vector3(0.0f, rayCamera.direction.y, rayCamera.direction.z);
		float angleBetweenMouseDirectionAndCameraFoward = Mathf.Deg2Rad * Vector3.Angle(mainCamera.transform.forward, rayCameraDirectionAlongDraggedObjectYZPlane);
		Vector3 draggedObjectVectorFromCamera = rayCameraDirectionAlongDraggedObjectYZPlane.normalized * (adjacentVector.magnitude / Mathf.Cos(angleBetweenMouseDirectionAndCameraFoward));
		draggedObject.transform.position = new Vector3(draggedObject.transform.position.x, rayCamera.origin.y, rayCamera.origin.z) + draggedObjectVectorFromCamera;
	}

	void DragAlongAxisZ() {
		Ray rayCamera = mainCamera.ScreenPointToRay(Input.mousePosition);
		Vector3 rayCameraDirectionAlongDraggedObjectXZPlane = new Vector3(rayCamera.direction.x, 0.0f, rayCamera.direction.z);
		float angleBetweenMouseDirectionAndCameraFoward = Vector3.Angle(mainCamera.transform.right, rayCameraDirectionAlongDraggedObjectXZPlane);
		Debug.Log("Drag Angle: " + (angleBetweenMouseDirectionAndCameraFoward));
		float adjacentVectorMagnitudeOffset = rayCamera.origin.x - initialRayCameraOriginX;
		if (angleBetweenMouseDirectionAndCameraFoward > 90.0f) {
			angleBetweenMouseDirectionAndCameraFoward = 180.0f - angleBetweenMouseDirectionAndCameraFoward;
			Vector3 draggedObjectVectorFromCamera = rayCameraDirectionAlongDraggedObjectXZPlane.normalized * ((adjacentVector.magnitude + adjacentVectorMagnitudeOffset) / Mathf.Cos(Mathf.Deg2Rad * angleBetweenMouseDirectionAndCameraFoward));
			draggedObject.transform.position = new Vector3(rayCamera.origin.x, draggedObject.transform.position.y, rayCamera.origin.z) + draggedObjectVectorFromCamera;
		} else {
			Vector3 draggedObjectVectorFromCamera = rayCameraDirectionAlongDraggedObjectXZPlane.normalized * ((adjacentVector.magnitude - adjacentVectorMagnitudeOffset) / Mathf.Cos(Mathf.Deg2Rad * angleBetweenMouseDirectionAndCameraFoward));
			draggedObject.transform.position = new Vector3(rayCamera.origin.x, draggedObject.transform.position.y, rayCamera.origin.z) + draggedObjectVectorFromCamera;
		}
		Debug.Log("WorldToScreen: " + mainCamera.WorldToScreenPoint(draggedObject.transform.position) +
			"\nMouse Postion: " + Input.mousePosition);
	}

	void DropItem() {
		draggingItem = false;
		draggedObject.transform.localScale = new Vector3(1, 1, 1);
	}

	private bool HasInput {
		get {
			return Input.GetMouseButton(0);
		}
	}

	void SetAdjacentVector(Ray rayCamera) {
		Vector3 toObjectVector = draggedObject.transform.position - rayCamera.origin;
		float angleBetweenInitialHitAndFoward = Vector3.Angle(mainCamera.transform.forward, toObjectVector);
		adjacentVector = mainCamera.transform.forward.normalized * (toObjectVector.magnitude * Mathf.Cos(Mathf.Deg2Rad * angleBetweenInitialHitAndFoward));
	}

	void SetAdjacentVectorAlongAxisX(Ray rayCamera) {
		Vector3 rayCameraOriginPositionAtXZPlaneOfDraggedObject = new Vector3(rayCamera.origin.x, draggedObject.transform.position.y, rayCamera.origin.z);
		Vector3 toObjectVector = draggedObject.transform.position - rayCameraOriginPositionAtXZPlaneOfDraggedObject;
		float angleBetweenInitialHitAndFoward = Vector3.Angle(mainCamera.transform.forward, toObjectVector);
		adjacentVector = mainCamera.transform.forward.normalized * (toObjectVector.magnitude * Mathf.Cos(Mathf.Deg2Rad * angleBetweenInitialHitAndFoward));
	}

	void SetAdjacentVectorAlongAxisY(Ray rayCamera) {
		Vector3 rayCameraOriginPositionAtYZPlaneOfDraggedObject = new Vector3(draggedObject.transform.position.x, rayCamera.origin.y, rayCamera.origin.z);
		Vector3 toObjectVector = draggedObject.transform.position - rayCameraOriginPositionAtYZPlaneOfDraggedObject;
		float angleBetweenInitialHitAndFoward = Vector3.Angle(mainCamera.transform.forward, toObjectVector);
		adjacentVector = mainCamera.transform.forward.normalized * (toObjectVector.magnitude * Mathf.Cos(Mathf.Deg2Rad * angleBetweenInitialHitAndFoward));
	}

	void SetAdjacentVectorAlongAxisZ(Ray rayCamera) {  //Needs initial x position to use for an offset variable during drag.
		Vector3 rayCameraOriginPositionAtXZPlaneOfDraggedObject = new Vector3(rayCamera.origin.x, draggedObject.transform.position.y, rayCamera.origin.z);
		initialRayCameraOriginX = rayCamera.origin.x;
		Vector3 toObjectVector = draggedObject.transform.position - rayCameraOriginPositionAtXZPlaneOfDraggedObject;
		float angleBetweenInitialHitAndFoward = Vector3.Angle(mainCamera.transform.right, toObjectVector);
		Debug.Log("Set Angle: " + angleBetweenInitialHitAndFoward);
		if (angleBetweenInitialHitAndFoward > 90.0f) {
			angleBetweenInitialHitAndFoward = 180.0f - angleBetweenInitialHitAndFoward;
			adjacentVector = (-1.0f * mainCamera.transform.right.normalized) * (toObjectVector.magnitude * Mathf.Cos(Mathf.Deg2Rad * angleBetweenInitialHitAndFoward));
		} else {
			adjacentVector = (mainCamera.transform.right.normalized) * (toObjectVector.magnitude * Mathf.Cos(Mathf.Deg2Rad * angleBetweenInitialHitAndFoward));
		}

	}

	//	void DragOrPickup () {  						//Based on http://unity.grogansoft.com/drag-and-drop/
	//		if (ignoreInput == false) {
	//			rayCamera = camera.ScreenPointToRay(Input.mousePosition);
	//			var inputPoistion = rayCamera.origin;
	//			if (draggingItem) {
	//				draggedObject.transform.position = inputPoistion + touchOffset;
	//			} else {
	//				RaycastHit[] touches = Physics.RaycastAll(rayCamera.origin, rayCamera.direction, 10.0f);
	//				if (touches.Length > 0) {
	//					var hit = touches[0];
	//					if (hit.transform != null) {
	//						draggingItem = true;
	//						draggedObject = hit.transform.gameObject;
	//						touchOffset = (Vector3)hit.transform.position - inputPoistion;
	//						draggedObject.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
	//					}
	//				} else {
	//					this.ignoreInput = true;
	//				}
	//			}
	//		}
	//	}
}