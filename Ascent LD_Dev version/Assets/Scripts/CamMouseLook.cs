using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMouseLook : MonoBehaviour {

	public Vector2 mouseLook;
	public float sensitivity = 5.0f;

    [HideInInspector] GameObject character;
    [HideInInspector] PlayerController playerController;
    [HideInInspector] GameObject playerHolder;

	// Use this for initialization
	void Start () {
		character = this.transform.parent.gameObject;
        playerController = character.GetComponent<PlayerController>();
        playerHolder = GameObject.FindGameObjectWithTag("PlayerHolder");
    }

	// Update is called once per frame
	void Update () {
		Vector2 mouseDelta = new Vector2 (Input.GetAxisRaw ("Mouse X"), Input.GetAxisRaw ("Mouse Y"));
		//Debug.Log (mouseDelta);

		mouseDelta = Vector2.Scale (mouseDelta, new Vector2 (sensitivity , sensitivity ));
		mouseLook += mouseDelta;

		transform.localRotation = Quaternion.AngleAxis (-mouseLook.y, Vector3.right);
        character.transform.localRotation = Quaternion.identity;
        if (playerController.isAlenvers == true)
        {
            character.transform.localRotation = Quaternion.AngleAxis(-mouseLook.x, character.transform.up);
            /*if (playerController.inverseLook == true)
            {
               // mouseLook.x = 0;
                //mouseLook.x = -mouseLook.x;
                playerController.inverseLook = false;
            }*/
        }
        else
        {
            character.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, character.transform.up);
          /*  if (playerController.inverseLook == true)
            {
               // mouseLook.x = -mouseLook.x;
                //mouseLook.x = 0;
                playerController.inverseLook = false;
            }*/
        }


        // character.transform.localRotation = new Quaternion(mouseLook.x, character.transform.localRotation.y, -180, character.transform.localRotation.w);

        BlockView ();
	}



	void BlockView()
	{
		if (mouseLook.y > 90.0f) 
		{
			mouseLook.y = 90.0f;
			transform.localRotation = Quaternion.AngleAxis (-mouseLook.y, Vector3.right);
		}

		if (mouseLook.y < -70.0f) 
		{
			mouseLook.y = -70.0f;
			transform.localRotation = Quaternion.AngleAxis (-mouseLook.y, Vector3.right);
		}
	}
}
