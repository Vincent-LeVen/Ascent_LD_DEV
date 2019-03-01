using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
	public CamMouseLook cam;
	Text timer;
    [HideInInspector] float timeLastDeath = 0f;

    [HideInInspector] public bool isAlive = true;
	[HideInInspector]public bool canMove = true;
	private float justDied = 0f;

    [SerializeField] GameObject cameraBase;

    [HideInInspector] public Vector3 spawnPoint;

    [SerializeField] float airStraffe = 2.0f;
    [SerializeField] float walkSpeed = 10.0f;
    [SerializeField] float runSpeed = 30.0f;
	[HideInInspector] public float speed;
	private float fadeSpeed = 0.5f;

    [SerializeField] float wallJumpForce = 10.0f;
    [SerializeField] float jumpForce = 10.0f;
    [SerializeField] float jumpForceFade = 10.0f;
    [SerializeField] float maxTimeJump = 0.2f;
	private float actualTimeJump = 1.5f;
	private bool jumped = false;
	private bool doubleWalljumpCounter = false;

	private Vector3 previousPosition;
    [SerializeField] float airBufferDivider = 180f;

    [SerializeField] float groundSpeed = 30.0f;
    public float wallJumpReduction = 10.0f;
    [SerializeField] float speedDivisionStraffe = 2.0f;
	Rigidbody rbPlayer;

	[Range (0.0f, 1.0f)]public float angleAttaque = 0.5f;

	public bool onGround = false;

	private AudioSource source;
	[SerializeField] AudioClip deathSound;
	[SerializeField] AudioClip walkSound;
	[SerializeField] AudioClip walkSound2;
	[SerializeField] AudioClip walkSound3;
	[SerializeField] AudioClip jumpSound;
	[SerializeField] AudioClip landSound;
    [SerializeField] AudioClip spawnSound;

	private float walksoundPlayed;
	private float timeBetweenSteps;

    private bool ChangeGravity;

    [SerializeField] float reverseGravityValue = 9.81f;
    [SerializeField] float gravityPowersDuration = 0.5f;
    [SerializeField] float worldGravity = -30f;

    private Transform myTransform;
    public bool isAlenvers = false;
    [HideInInspector] public GameObject playerHolder;
    [HideInInspector] public bool inverseLook = false;

    public bool isAttachedToOne = false;
    public bool isAttachedToCrusher = false;
    public bool callDeath = false;

    public bool slideFirstFrame = true;
    public bool isSliding;
    private Vector3 slideVector;
    private bool wasOnGround;
    private bool powerIsOnCd;
    int fallStartingH;
    int fallLandingH;
    [SerializeField] private int maxFallHeight = 50;

    private int fallCounter = 0;
    private Quaternion targetRotation;

    private Vector3 jumpStartPosition;
    [SerializeField] private bool alternateAirBuffer = false;

    [SerializeField] private CapsuleCollider myCapsuleCollider5H;

    public bool respawnUpsideDown = false;
    private bool fallingCheck;
    public float checkpointDirection;
    private bool dieOnLanding = false;

    // Use this for initialization
    void Start () 
	{
        Physics.gravity = new Vector3(0, worldGravity, 0);
        source = GetComponent<AudioSource>();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		rbPlayer = GetComponent<Rigidbody> ();
		speed = walkSpeed;
		spawnPoint = transform.position;
		previousPosition = transform.position;
		//particuleSystem = GetComponent<ParticleSystem> ();
		timeLastDeath = Time.time;
		canMove = true;
		cameraBase.SetActive (true);
		//model3D.SetActive (true);
		Time.timeScale = 1;
		source.PlayOneShot (spawnSound, 1.0f);
        ChangeGravity = false;
        myTransform = GetComponent<Transform>();
        playerHolder = GameObject.FindGameObjectWithTag("PlayerHolder");


        targetRotation = transform.rotation;
    }

	void Update ()
	{
		if (canMove) 
		{
			Jumping ();

			WallJumping ();

            if (Input.GetKey(KeyCode.C) && onGround)
            {
                isSliding = true;
            } else if (!slideFirstFrame && isSliding == true && !Input.GetKey(KeyCode.C))
            {
                isSliding = false;
                if (isAlenvers)
                {
                    cameraBase.transform.position = new Vector3(cameraBase.transform.position.x, cameraBase.transform.position.y - 1.5f, cameraBase.transform.position.z);
                }
                else
                {
                    cameraBase.transform.position = new Vector3(cameraBase.transform.position.x, cameraBase.transform.position.y + 1.5f, cameraBase.transform.position.z);
                    Debug.Log("+from releasing c");
                }
                myCapsuleCollider5H.center = new Vector3(0,2.5f,0);
                myCapsuleCollider5H.height = 5;
                slideFirstFrame = true;
            }

           if (Input.GetKeyDown(KeyCode.R) || callDeath)
            {
                Death();
            }
		}
        if (!isAlive)
        {
            Debug.Log(Time.time - justDied);
        }
		if (Time.time - justDied > 0.1f && !isAlive) 
		{
            Debug.Log("Respawn");
			source.PlayOneShot (spawnSound, 1.0f);
			transform.position = spawnPoint;
            fallStartingH = (int)transform.position.y;
      /*  if (isAlenvers)
        {
            playerHolder.transform.position = this.transform.position;
            transform.position = playerHolder.transform.position;
            playerHolder.transform.eulerAngles = new Vector3(playerHolder.transform.rotation.x, playerHolder.transform.rotation.eulerAngles.y, -180);
            isAlenvers = false;
        } */
            cameraBase.SetActive (true);
			timeLastDeath = Time.time;
			cam.mouseLook = new Vector2 (checkpointDirection, 0f);
			justDied = 0f;
			isAlive = true;
			canMove = true;
		}

        //gravity control
         /* if (Input.GetKeyDown(KeyCode.E) && ChangeGravity == false || Input.GetButtonDown("Fire1") && ChangeGravity == false)
        {
            if ((fallStartingH - fallLandingH) < maxFallHeight)
            {
                fallStartingH = (int)transform.position.y;
            }
            if (isAlenvers == false)
            {
                Physics.gravity = new Vector3(0, reverseGravityValue, 0);
            } else
            {
                Physics.gravity = new Vector3(0, -reverseGravityValue, 0);
            }
            StartCoroutine(GravityTimeController());
            ChangeGravity = true;
        } */

        if (Input.GetKeyDown(KeyCode.A) && !ChangeGravity && !isAlenvers && !powerIsOnCd && !isSliding)
        {
            Physics.gravity = new Vector3(0, -worldGravity, 0);                   
            isAlenvers = true;
            powerIsOnCd = true;
            StartCoroutine(PlayerRotaterDelayer());    
        }
        else if (Input.GetKeyDown(KeyCode.A) && !ChangeGravity && isAlenvers && !powerIsOnCd && !isSliding)
        {
            powerIsOnCd = true;
            isAlenvers = false;
            ResetGravity();
            StartCoroutine(PlayerRotaterDelayer());                  
        }
                
    }

    IEnumerator PlayerRotaterDelayer()
    {
        fallCounter = 0;
        playerHolder.transform.position = this.transform.position;
        transform.position = playerHolder.transform.position;
        playerHolder.transform.eulerAngles = new Vector3(playerHolder.transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y , playerHolder.transform.rotation.eulerAngles.z); 
        cam.mouseLook.x = 0;
        if (isAlenvers)
          {
              for (int i = 0; i < 185; i = i +5)
              {
                    playerHolder.transform.position = this.transform.position;
                    transform.position = playerHolder.transform.position;
                if (i < 165)
                {
                    playerHolder.transform.eulerAngles = new Vector3(playerHolder.transform.rotation.x, playerHolder.transform.rotation.eulerAngles.y, -i);
                    yield return new WaitForSecondsRealtime(0.0001f);
                }
                else
                {
                    i = i - 1;
                    playerHolder.transform.eulerAngles = new Vector3(playerHolder.transform.rotation.x, playerHolder.transform.rotation.eulerAngles.y, -i);
                    yield return new WaitForSecondsRealtime(0.0001f);
                }
              }
              
        }
          else
          {
              for (int j = 180; j > -5; j = j - 5)
              {
                    playerHolder.transform.position = this.transform.position;
                    transform.position = playerHolder.transform.position;
                if (j > 15)
                {
                    playerHolder.transform.eulerAngles = new Vector3(playerHolder.transform.rotation.x, playerHolder.transform.rotation.eulerAngles.y, -j);
                    yield return new WaitForSecondsRealtime(0.0001f);
                }
                else
                {
                    j = j + 1;
                    playerHolder.transform.eulerAngles = new Vector3(playerHolder.transform.rotation.x, playerHolder.transform.rotation.eulerAngles.y, -j);
                    yield return new WaitForSecondsRealtime(0.0001f);
                }
              }
            
        }
        fallStartingH = (int)transform.position.y;
        if (isAlenvers)
        {        
            rbPlayer.velocity = new Vector3(rbPlayer.velocity.x, 12f, rbPlayer.velocity.z);          
        }
        else
        {
            rbPlayer.velocity = new Vector3(rbPlayer.velocity.x, -12f, rbPlayer.velocity.z);       
        }
        StopCoroutine(PlayerRotaterDelayer());
    }

    IEnumerator GravityTimeController()
    {
        yield return new WaitForSecondsRealtime(gravityPowersDuration);
        ChangeGravity = false;
        ResetGravity();
        StopCoroutine(GravityTimeController());
    }

	void FixedUpdate ()
	{
        if (powerIsOnCd || fallingCheck ) {
        fallCounter = fallCounter + 1;
            if (fallCounter >= 180)
            {
                fallCounter = 0;              
                Death();
            }
        } else { fallCounter = 0;
            fallingCheck = false;
        }

		GroundChecking ();
        IsRunning ();

		if (canMove)
        {
			Moving ();
		}
	}

    private void ResetGravity()
    {
        if (isAlenvers == false)
        {
            Physics.gravity = new Vector3(0, worldGravity, 0);
        }
        else
        {
            Physics.gravity = new Vector3(0, -worldGravity, 0);
        }
    }

	private void OnCollisionEnter (Collision coll)
	{
		if (coll.gameObject.tag == "KillZone" && isAlive) 
		{
            Death();
		}
	}

    private void Death() {
        callDeath = false;
        powerIsOnCd = false;
        fallCounter = 0;

        if (!slideFirstFrame)
        {       
                if (isAlenvers)
                {
                    cameraBase.transform.position = new Vector3(cameraBase.transform.position.x, cameraBase.transform.position.y - 1.5f, cameraBase.transform.position.z);
                }
                else
                {
                    cameraBase.transform.position = new Vector3(cameraBase.transform.position.x, cameraBase.transform.position.y + 1.5f, cameraBase.transform.position.z);
                Debug.Log("+from death");
            }
            myCapsuleCollider5H.center = new Vector3(0, 2.5f, 0);
            myCapsuleCollider5H.height = 5;
            slideFirstFrame = true;
        }
        if (respawnUpsideDown)
        {
            playerHolder.transform.eulerAngles = new Vector3(playerHolder.transform.rotation.x, playerHolder.transform.rotation.eulerAngles.y, -180);
            isAlenvers = true;
        } else
        {
            playerHolder.transform.eulerAngles = new Vector3(playerHolder.transform.rotation.x, playerHolder.transform.rotation.eulerAngles.y, 0);
            isAlenvers = false;
        }

        fallingCheck = false;
        isSliding = false;
        source.PlayOneShot(deathSound, 1.0f);
        rbPlayer.velocity = Vector3.zero;
        justDied = Time.time;
        isAlive = false;
        canMove = false;
        cameraBase.SetActive(false);
        ResetGravity();
    }

    private void IsRunning()
	{
		if (Input.GetKey (KeyCode.LeftShift)) 
		{
			speed = runSpeed;
			timeBetweenSteps = 0.12f;
		} 
		else 
		{
			speed = walkSpeed;
			timeBetweenSteps = 0.15f;
		}
	}

	private void GroundChecking()
	{
		RaycastHit hit;
		Vector3 physicsCentre = this.transform.position + this.GetComponent<CapsuleCollider> ().center;		

		if (onGround) 
		{
			wasOnGround = true;  
		} 
		else 
		{
			wasOnGround = false;
            fallLandingH = (int)transform.position.y;
            if ((fallStartingH - fallLandingH) >= 1.5 * maxFallHeight)
            {
                dieOnLanding = true;
            }
        }

		float j = 0.0f;
        for (int i = 0; i < 16; i++)
        {
            if (isAlenvers == true)
            {
                Vector3 rayStartPoint = physicsCentre + new Vector3((Mathf.Cos(j * (Mathf.PI / 8)) * 0.42f), 0f, (Mathf.Sin(j * (Mathf.PI / 8)) * 0.42f));
                Debug.DrawRay(rayStartPoint, Vector3.up * 0.6f, Color.red, 0.25f);

                if (Physics.Raycast(rayStartPoint, Vector3.up, out hit, 0.6f))
                {
                    if (hit.transform.gameObject.tag != "Player")
                    {
                        onGround = true;
                        break;
                    }
                }
                else
                {
                    onGround = false;
                }

                j += 1.0f;
            } else if (isAlenvers == false){

                Vector3 rayStartPoint = physicsCentre + new Vector3((Mathf.Cos(j * (Mathf.PI / 8)) * 0.42f), 0f, (Mathf.Sin(j * (Mathf.PI / 8)) * 0.42f));
                Debug.DrawRay(rayStartPoint, Vector3.down * 0.6f, Color.red, 0.25f);

                if (Physics.Raycast(rayStartPoint, Vector3.down, out hit, 0.6f)) {
                    if (hit.transform.gameObject.tag != "Player") {
                        onGround = true;
                        break;
                    }
                }
                else
                {
                    onGround = false;
                }
                j += 1.0f;
            }
        }
		if (!wasOnGround && onGround) 
		{
			source.PlayOneShot (landSound, 0.8f);
            fallLandingH = (int)transform.position.y;
            powerIsOnCd = false;
            if ((fallStartingH - fallLandingH) >= maxFallHeight || dieOnLanding)
            {
                dieOnLanding = false;
                Death();
            }
		}
        else if (wasOnGround && !onGround)
        {
            fallStartingH = (int)transform.position.y;
            fallingCheck = true;
        } 
	}

	private void Moving ()
	{
		float translation = 0f;
		float straffe = 0f;
        
		if (onGround) 
		{
            fallingCheck = false;
            translation = Input.GetAxisRaw ("Vertical") * ((speed * fadeSpeed) * groundSpeed);
			straffe = Input.GetAxisRaw ("Horizontal") * (((speed * fadeSpeed)*groundSpeed) / speedDivisionStraffe);
			translation *= Time.deltaTime;
			straffe *= Time.deltaTime;
            if (isSliding)
            {
                if (slideFirstFrame)
                {
                    Vector3 force = new Vector3(0.0f, 0.0f, ((speed * fadeSpeed) * groundSpeed) * 0.037f);
                    force = transform.localToWorldMatrix.MultiplyVector(force);
                    slideVector = force;
                    rbPlayer.AddForce(force, ForceMode.VelocityChange);
                    slideFirstFrame = false;

                    if (isAlenvers)
                    {
                        cameraBase.transform.position = new Vector3(cameraBase.transform.position.x, cameraBase.transform.position.y + 1.5f, cameraBase.transform.position.z);
                    }
                    else
                    {
                       cameraBase.transform.position = new Vector3(cameraBase.transform.position.x, cameraBase.transform.position.y - 1.5f, cameraBase.transform.position.z);                                                                     
                    }
                    myCapsuleCollider5H.center = new Vector3(0, 1f, 0);
                    myCapsuleCollider5H.height = 2;
                } else
                {
                    slideVector = slideVector * 0.987f;
                    rbPlayer.AddForce(slideVector, ForceMode.VelocityChange);
                }
            }
            else
            {               
                Vector3 force = new Vector3(straffe, 0.0f, translation);
			    force = transform.localToWorldMatrix.MultiplyVector (force);
			    rbPlayer.AddForce (force, ForceMode.VelocityChange);
            }

			Vector3 v = rbPlayer.velocity;
			v.x = 0f;
			v.z = 0f;
			rbPlayer.velocity = v;

			if (fadeSpeed < 1.1f)
			{
				fadeSpeed += 0.1f;
			}

			if (translation == 0 && straffe == 0 )
			{
				fadeSpeed = 0.5f;
			}

			if ((translation != 0f || straffe != 0f) && Time.time - walksoundPlayed > timeBetweenSteps)
			{
				int soundChoice = Random.Range (1, 4);
				if (soundChoice == 1) {
					source.PlayOneShot (walkSound, Random.Range(0.15f, 0.25f));
				} else if (soundChoice == 2) {
					source.PlayOneShot (walkSound2, Random.Range(0.15f, 0.25f));
				} else {
					source.PlayOneShot (walkSound3, Random.Range(0.15f, 0.25f));
				}
				walksoundPlayed = Time.time;
			}
            alternateAirBuffer = false;
            jumpStartPosition = transform.position;
		}
        else if ((jumpStartPosition.x == transform.position.x) && (jumpStartPosition.z == transform.position.z))
        {
            alternateAirBuffer = true;
            translation = Input.GetAxis("Vertical") * speed;
            straffe = Input.GetAxis("Horizontal") * speed * airStraffe;
            translation *= Time.deltaTime;
            straffe *= Time.deltaTime;
            float airControlBuffer = CalculateAirControlBuffer(translation, straffe);
            Vector3 force = new Vector3(straffe * airControlBuffer *6, 0.0f, translation * airControlBuffer *6);
            force = transform.localToWorldMatrix.MultiplyVector(force);
            rbPlayer.AddForce(force, ForceMode.VelocityChange);
        } 
        else if (alternateAirBuffer)
        {
            alternateAirBuffer = true;
            translation = Input.GetAxis("Vertical") * speed;
            straffe = Input.GetAxis("Horizontal") * speed * airStraffe;
            translation *= Time.deltaTime;
            straffe *= Time.deltaTime;
            float airControlBuffer = CalculateAirControlBuffer(translation, straffe);
            Vector3 force = new Vector3(straffe * airControlBuffer *6, 0.0f, translation * airControlBuffer *6);
            force = transform.localToWorldMatrix.MultiplyVector(force);
            rbPlayer.AddForce(force, ForceMode.VelocityChange);
        }
        else
        {
			translation = Input.GetAxis ("Vertical") * speed;
			straffe = Input.GetAxis ("Horizontal") * speed * airStraffe;
			translation *= Time.deltaTime;
			straffe *= Time.deltaTime;
			float airControlBuffer = CalculateAirControlBuffer (translation, straffe);
			Vector3 force = new Vector3 (straffe * airControlBuffer, 0.0f, translation * airControlBuffer);
			force = transform.localToWorldMatrix.MultiplyVector (force);
			rbPlayer.AddForce (force, ForceMode.VelocityChange);
		}


		previousPosition = transform.position;
	}

	private float CalculateAirControlBuffer (float translation, float straffe)
	{
		Vector3 currentDirection = transform.position - previousPosition;
		currentDirection = new Vector3 (currentDirection.x, 0f, currentDirection.z);
		Vector3 direction = new Vector3 (straffe, 0f, translation);
		direction = transform.TransformDirection (direction);
		float angle = Vector3.Angle (currentDirection, direction);
		if (angle > 30) {
			float airControlBuffer = 1 + (angle / airBufferDivider);
			return airControlBuffer;
		}
		return 1.0f;
	}
		
	private void Jumping()
	{
        //Jump nuancé

        if (isAlenvers == true)
        {
            if ((Input.GetKeyDown("space") || Input.GetButtonDown("Fire2")) && onGround)
            {
                //rbPlayer.AddForce (new Vector3 (0, jumpForce, 0), ForceMode.Impulse);
                rbPlayer.velocity = new Vector3(rbPlayer.velocity.x, -jumpForce, rbPlayer.velocity.z);
                actualTimeJump = Time.time;
                jumped = true;
                source.PlayOneShot(jumpSound, 1.0f);
            }

            if ((Input.GetKey("space") || Input.GetButton("Fire2")) && Time.time - actualTimeJump < maxTimeJump)
            {
                //rbPlayer.velocity += new Vector3 (0, jumpForceHold, 0);
                rbPlayer.velocity = new Vector3(rbPlayer.velocity.x, (-jumpForce - (-jumpForceFade * (Time.time - actualTimeJump))), rbPlayer.velocity.z);
            }

            if ((Input.GetKeyUp("space") || Input.GetButtonUp("Fire2")) && Time.time - actualTimeJump < maxTimeJump && jumped)
            {
                rbPlayer.velocity = new Vector3(rbPlayer.velocity.x, 0f, rbPlayer.velocity.z);
                jumped = false;
            }
        }
        else if (isAlenvers == false)
        {
            if ((Input.GetKeyDown("space") || Input.GetButtonDown("Fire2")) && onGround)
            {
                //rbPlayer.AddForce (new Vector3 (0, jumpForce, 0), ForceMode.Impulse);
                rbPlayer.velocity = new Vector3(rbPlayer.velocity.x, jumpForce, rbPlayer.velocity.z);
                actualTimeJump = Time.time;
                jumped = true;
                source.PlayOneShot(jumpSound, 1.0f);
            }

            if ((Input.GetKey("space") || Input.GetButton("Fire2")) && Time.time - actualTimeJump < maxTimeJump)
            {
                //rbPlayer.velocity += new Vector3 (0, jumpForceHold, 0);
                rbPlayer.velocity = new Vector3(rbPlayer.velocity.x, (jumpForce - (jumpForceFade * (Time.time - actualTimeJump))), rbPlayer.velocity.z);
            }

            if ((Input.GetKeyUp("space") || Input.GetButtonUp("Fire2")) && Time.time - actualTimeJump < maxTimeJump && jumped)
            {
                rbPlayer.velocity = new Vector3(rbPlayer.velocity.x, 0f, rbPlayer.velocity.z);
                jumped = false;
            }
        }
		// Jump binaire
		/*if ((Input.GetKeyDown ("space") || Input.GetButtonDown ("Fire2")) && onGround) 
		{
			//rbPlayer.AddForce (new Vector3 (0, jumpForce, 0), ForceMode.Impulse);
			rbPlayer.velocity = new Vector3 (rbPlayer.velocity.x, jumpForceLow, rbPlayer.velocity.z);
			actualTimeJump = Time.time;
			maxJumped = false;
		}*/

		/*if ((Input.GetKey ("space") || Input.GetButton ("Fire2")) && Time.time-actualTimeJump > maxTimeJump && !maxJumped)
		{
			rbPlayer.velocity = new Vector3 (rbPlayer.velocity.x, jumpForce, rbPlayer.velocity.z);
			maxJumped = true;
		}*/
	}

	private void WallJumping()
	{
		float j = 0f;
		for (int i = 0; i < 16; i++) 
		{
            if (isAlenvers == true)
            {
                Vector3 vectorDirection = Quaternion.AngleAxis(j * 22.5f, Vector3.down) * Vector3.forward;
                RaycastHit hit;
                if (Physics.Raycast(transform.position, vectorDirection, out hit, 1f))
                {
                    if ((Input.GetKeyDown("space") || Input.GetButtonDown("Fire2")) && hit.normal.y < angleAttaque && !onGround && !doubleWalljumpCounter && hit.transform.tag == "WallJumpAble")
                    {
                        fallCounter = 0;
                        Vector3 v = rbPlayer.velocity;
                        v.y = 0f;
                        rbPlayer.velocity = v;
                        rbPlayer.velocity = new Vector3(rbPlayer.velocity.x, -wallJumpForce, rbPlayer.velocity.z);
                        rbPlayer.AddForce(hit.normal * (speed / wallJumpReduction), ForceMode.VelocityChange);
                        doubleWalljumpCounter = true;
                        source.PlayOneShot(jumpSound, 1.0f);
                        break;
                    }
                }
                else
                {
                    doubleWalljumpCounter = false;
                }
                j += 1f;
            }
            else
            {
                Vector3 vectorDirection = Quaternion.AngleAxis(j * 22.5f, Vector3.up) * Vector3.forward;
                RaycastHit hit;
                if (Physics.Raycast(transform.position, vectorDirection, out hit, 1f))
                {
                    if ((Input.GetKeyDown("space") || Input.GetButtonDown("Fire2")) && hit.normal.y < angleAttaque && !onGround && !doubleWalljumpCounter && hit.transform.tag == "WallJumpAble")
                    {
                        fallCounter = 0;
                        Vector3 v = rbPlayer.velocity;
                        v.y = 0f;
                        rbPlayer.velocity = v;
                        rbPlayer.velocity = new Vector3(rbPlayer.velocity.x, wallJumpForce, rbPlayer.velocity.z);
                        rbPlayer.AddForce(hit.normal * (speed / wallJumpReduction), ForceMode.VelocityChange);
                        doubleWalljumpCounter = true;
                        source.PlayOneShot(jumpSound, 1.0f);
                        break;
                    }
                }
                else
                {
                    doubleWalljumpCounter = false;
                }
                j += 1f;
            }
		}
	}
}
