using UnityEngine;
using System.Collections;

public class CameraZoom : MonoBehaviour {
	
	private float fovSense = 30f;
	private int maxView = 70;
	private int minView = 20;

	private float ySense = 3f;
	private int maxY = 15;
	private int minY = 10;	


	private float smooth = 5.0f;

	public float dragSpeed = -10;
	public int minX = -892;
	public int maxX = 1111;
	public int minZ = -880;
	public int maxZ = 1145;
	
	public int bottomMargin = 80; // if you have some icons at the bottom (like an RPG game) this will help preventing the drag action at the bottom
	
	public float orthZoomStep = 10.0f;
	public int orthZoomMaxSize = 500;
	public int orthZoomMinSize = 300;
	
	private bool orthographicView = false;
	private Vector3 dragOrigin;

	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	void Awake()
	{
		//Vector3 move = new Vector3(transform.position.x, -3, transform.position.z);
		//transform.Translate(move, Space.World);
	}
	
	// Update is called once per frame
	void Update () {
		//moveCamera();
		zoomFOV();
		//zoomY();
	}
	
	void zoomFOV()
	{

		if (Input.GetAxis("Mouse ScrollWheel") != 0) {
			//Debug.Log("Camera.main.fieldOfView: " + transform.position.y + " : " + Camera.main.fieldOfView);

			if (Camera.main.fieldOfView <= maxView && Camera.main.fieldOfView >= minView) {
				float changeVal = (-Input.GetAxis("Mouse ScrollWheel") * fovSense);
				//Camera.main.fieldOfView += changeVal;

				Mathf.Lerp(Camera.main.fieldOfView, Camera.main.fieldOfView += changeVal, Time.deltaTime*smooth);

				//camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, zoom, Time.deltaTime*smooth);

			}

			if (Camera.main.fieldOfView < minView) Camera.main.fieldOfView = minView;
			if (Camera.main.fieldOfView > maxView) Camera.main.fieldOfView = maxView;
		}

	}
	
	void zoomY()
	{

		if (Input.GetAxis("Mouse ScrollWheel") != 0) {
			
			//Debug.Log("Camera.main.fieldOfView: " + transform.position.y + " : " + Camera.main.fieldOfView);
			
			//if (transform.position.y <= maxY && transform.position.y >= minY) {
				float changeVal = (Input.GetAxis("Mouse ScrollWheel") * ySense);
				//Camera.main.fieldOfView += changeVal;
				
				Vector3 move = new Vector3(0, 0, changeVal);
				transform.Translate(move, Space.World);
			//}
		}

		//if (transform.position.y < minView) transform.position.y = minView;
		//if (transform.position.y > maxView) transform.position.y = maxView;

	}		
	
	void moveCamera()
	{
		if (Input.GetMouseButtonDown(0))
		{    
			dragOrigin = Input.mousePosition;
			return;
		}
		
		if (!Input.GetMouseButton(0)) return;
		
		if(dragOrigin.y <= bottomMargin) return;
		
		Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
		Vector3 move = new Vector3(pos.x * dragSpeed, 0, pos.y * dragSpeed);
		
		if(move.x > 0)
		{
			if(!isWithinRightBorder())
				move.x =0;
		}
		else
		{
			if(!isWithinLeftBorder())
				move.x=0;
		}
		
		if(move.z > 0)
		{
			if(!isWithinTopBorder())
				move.z=0;
		}
		else
		{
			if(!isWithinBottomBorder())
				move.z=0;
		}
		
		
		transform.Translate(move, Space.World);
	}

	
	bool isWithinBorders()
	{
		return ( isWithinLeftBorder() && isWithinBottomBorder() && isWithinRightBorder() && isWithinTopBorder() );
	}
	
	bool isWithinLeftBorder()
	{
		Vector3 currentTopLeftGlobal = Camera.main.ScreenToWorldPoint(new Vector3(0,0,0));
		if(currentTopLeftGlobal.x > minX)
			return true;
		else
			return false;
		
	}
	
	bool isWithinRightBorder()
	{
		Vector3 currentBottomRightGlobal = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width,0,0));
		if(currentBottomRightGlobal.x < maxX)
			return true;
		else
			return false;
	}
	
	bool isWithinTopBorder()
	{
		Vector3 currentTopLeftGlobal = Camera.main.ScreenToWorldPoint(new Vector3(0,Screen.height,0));
		if(currentTopLeftGlobal.z < maxZ)
			return true;
		else
			return false;
	}
	
	bool isWithinBottomBorder()
	{
		Vector3 currentBottomRightGlobal = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width,0,0));
		if(currentBottomRightGlobal.z > minZ)
			return true;
		else
			return false;
	}
}