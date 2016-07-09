using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MouseInputManager : MonoBehaviour {
	public Camera camera;
	public Text mousePositionText;
	public Text screenHeightandWidthText;
	private bool draggingItem = false;
	private bool ignoreInput = false;
	private GameObject draggedObject;
	private Vector3 touchOffset;
	private Vector3 adjacentVector;
	private float angleBetweenInitialHitAndFoward;
	private Ray rayCamera;
	private Vector3 inputPoistion;
	private RaycastHit raycastHit;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update () {
		mousePositionText.text = Input.mousePosition.ToString();
		screenHeightandWidthText.text = "Height: " + Screen.height.ToString() + "\nWidth: " + Screen.width.ToString();
		if (HasInput){
//			DragOrPickup();
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
			rayCamera = camera.ScreenPointToRay(Input.mousePosition);
			inputPoistion = rayCamera.origin;
			RaycastHit[] touches = Physics.RaycastAll(rayCamera.origin, rayCamera.direction, 10.0f);
			if (touches.Length > 0) {
				var hit = touches[0];
				if (hit.transform != null) {
					draggingItem = true;
					draggedObject = hit.transform.gameObject;
					draggedObject.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);

					Vector3 toObjectVector = draggedObject.transform.position - camera.transform.position;

					angleBetweenInitialHitAndFoward = Vector3.Angle(camera.transform.forward, toObjectVector);
					Debug.DrawRay(camera.transform.position, toObjectVector, Color.magenta, 3.0f, false);
					adjacentVector = camera.transform.forward;

					adjacentVector = adjacentVector.normalized * (toObjectVector.magnitude * Mathf.Cos(Mathf.Deg2Rad * this.angleBetweenInitialHitAndFoward));

					adjacentVector += camera.transform.position;
					Debug.Log(adjacentVector);
					Debug.Log("hypotnuse: " + toObjectVector.magnitude +
						"\nadjacent:  " + (toObjectVector.magnitude * Mathf.Cos(Mathf.Deg2Rad * angleBetweenInitialHitAndFoward)) +
						"\nangle:     " + angleBetweenInitialHitAndFoward);
				}
			} else {
				this.ignoreInput = true;
			}
		}
	}

	void Drag() {
		rayCamera = camera.ScreenPointToRay(Input.mousePosition);
		inputPoistion = rayCamera.origin;
		float angleBetweenMouseDirectionAndCameraFoward = Mathf.Deg2Rad * Vector3.Angle(camera.transform.forward, rayCamera.direction);
		Vector3 draggedObjectVectorFromCamera = rayCamera.direction.normalized * (adjacentVector.magnitude / Mathf.Cos(angleBetweenMouseDirectionAndCameraFoward));
		draggedObject.transform.position = camera.transform.position + draggedObjectVectorFromCamera;
//		draggedObject.transform.position = inputPoistion + touchOffset;
		Debug.Log("angle: " + angleBetweenMouseDirectionAndCameraFoward +
			"\nhypotnuse vector: " + draggedObjectVectorFromCamera +
			"\nadjavcent vector: " + adjacentVector);
	}

//	void DragOrPickup () {
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

	void DropItem() {
		draggingItem = false;
		draggedObject.transform.localScale = new Vector3(1, 1, 1);
	}

	private bool HasInput {
		get {
			return Input.GetMouseButton(0);
		}
	}

	void OnDrawGizmos() {
		if (draggingItem){
			Gizmos.DrawCube(rayCamera.origin, Vector3.one * .10f );
			Gizmos.DrawSphere(camera.transform.position, 1.0f);
			if (adjacentVector != null){
				Debug.DrawRay(camera.transform.position, this.adjacentVector-camera.transform.position, Color.blue, 0.2f, false );
			}
			Debug.Log("camera position: " + camera.transform.position);
			Debug.Log("draggedObject position: " + draggedObject.transform.position);
		}
	}
}