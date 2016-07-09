using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MouseInputManager : MonoBehaviour {
	public Camera mainCamera;
	private bool draggingItem = false;
	private bool ignoreInput = false;
	private GameObject draggedObject;
	private Vector3 adjacentVector;

	void Update () {
		if (HasInput){
			if (draggingItem) {
				Drag();
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
			RaycastHit[] touches = Physics.RaycastAll(rayCamera.origin, rayCamera.direction, 10.0f);
			if (touches.Length > 0) {
				var hit = touches[0];
				if (hit.transform != null && touches[0].transform.gameObject.GetComponent("Draggable") != null) {
					draggingItem = true;
					draggedObject = hit.transform.gameObject;
					draggedObject.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
					this.SetAdjacentVector(rayCamera);
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