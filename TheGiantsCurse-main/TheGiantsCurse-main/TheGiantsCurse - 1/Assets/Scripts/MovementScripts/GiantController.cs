using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GiantController : NetworkBehaviour
{
    [SerializeField] private float movementSpeed = 7f;
    [SerializeField] private float turnSpeed = 720;
    [SerializeField] private float jumpSpeed = 11f;
    [SerializeField] private float leapCooldown = 5f;
    [SerializeField] private float clubCooldown = 2f;
    [SerializeField] private float boulderCooldown = 10f;
    [SerializeField] private float boulderSpeed = 20f;
    [SerializeField] private float leapCrashRange = 8f;

    //the club for the melee attack and the boulder that will be thrown
    [SerializeField] private GameObject club;
    [SerializeField] private GameObject boulder;
    [SerializeField] private GameObject leapLandingArea;

    [SerializeField] private Animator animator;

    [SerializeField] private AudioClip destroySound;

    private GameObject boulderInstance, leapLandingInstance;

    private bool movementEnabled, rotationEnabled, leapReady, clubReady, boulderReady, doingAction;

    private float speed;

    private Rigidbody rb;
    
    private Vector3 movementInput;

    [SerializeField] private GameObject cameraPrefab;
    protected GameObject cameraInstance;
    private Camera mainCamera;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        /*cameraInstance = Instantiate(cameraPrefab, null);
        mainCamera = cameraInstance.GetComponentInChildren<Camera>();
        cameraInstance.GetComponent<CameraFollow>().ChangeFollow(this.gameObject);
        GameObject.FindWithTag("tmpCam").SetActive(false);
        HazardEvent.instance.SetCamera(cameraInstance.transform.GetChild(0).gameObject);*/
        club.SetActive(false);
        movementEnabled = true;
        rotationEnabled = true;
        leapReady = true;
        clubReady = true;
        boulderReady = true;
        doingAction = false;
        speed = movementSpeed;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            Destroy(this);
        else
        {
            cameraInstance = Instantiate(cameraPrefab, null);
            mainCamera = cameraInstance.GetComponentInChildren<Camera>();
            cameraInstance.GetComponent<CameraFollow>().ChangeFollow(this.gameObject);
            if (GameObject.FindWithTag("tmpCam") != null) 
                GameObject.FindWithTag("tmpCam").SetActive(false);
            HazardEvent.instance.SetCamera(cameraInstance.transform.GetChild(0).gameObject);
            FinalTrackManagement.instance.AssignGiantCliendId();
        }
        //mainCamera.GetComponentInParent<CameraFollow>().ChangeFollow(this.gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector3(transform.position.x, 0, transform.position.z), leapCrashRange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().Death();
        }

        else if (collision.gameObject.CompareTag("Destroyable"))
        {
            collision.gameObject.GetComponent<DestroyableObject>().ObstacleDestruction();
        }

        else if (collision.gameObject.CompareTag("Explosive"))
        {
            collision.gameObject.GetComponent<Explosive>().Explode();
        }

        else if(collision.gameObject.CompareTag("Grapple") || collision.gameObject.CompareTag("IceBlock") || collision.gameObject.CompareTag("Torch"))
        {
            Destroy(collision.gameObject);
        }
    }

    private void Start()
    {
        if(IsOwner) 
            mainCamera.GetComponentInParent<CameraFollow>().ChangeFollow(this.gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (clubReady && !doingAction)
            {
                ClubAttack();
            }
            else
                Debug.Log("Club is not ready yet");
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (boulderReady && !doingAction)
            {
                ThrowBoulder();
            }
            else
                Debug.Log("Boulder is not ready yet");
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (leapReady && !doingAction)
            {
                Leap();
            }
            else
                Debug.Log("Leap is not ready yet");
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            cameraInstance = Instantiate(cameraPrefab, null);
            mainCamera = cameraInstance.GetComponentInChildren<Camera>();
            cameraInstance.GetComponent<CameraFollow>().ChangeFollow(this.gameObject);
            if (GameObject.FindWithTag("tmpCam") != null)
                GameObject.FindWithTag("tmpCam").SetActive(false);
            HazardEvent.instance.SetCamera(cameraInstance.transform.GetChild(0).gameObject);
            FinalTrackManagement.instance.AssignGiantCliendId();
        }
    }

    
    private void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        movementInput = new Vector3(horizontal, 0, vertical);

        if (movementEnabled)
        {
            if (movementInput != Vector3.zero)
            {
                animator.SetBool("run", true);
            }
            else
                animator.SetBool("run", false);
            rb.MovePosition(transform.position + movementInput * speed * Time.fixedDeltaTime);
        }

        if (rotationEnabled)
            RotateLook();
    }

    private void RotateLook()
    {
        if (movementInput != Vector3.zero)
        {
            var rot = Quaternion.LookRotation(movementInput, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, turnSpeed * Time.deltaTime);
        }
    }
    private void ClubAttack()
    {
        Debug.Log("Swing the club");
        movementEnabled = false;
        rotationEnabled = false;
        animator.SetTrigger("attack1");
        club.SetActive(true);
        club.transform.localPosition = new Vector3(0, -0.4f, 0.6f);
        club.GetComponent<Animator>().SetTrigger("Attack");
        clubReady = false;
        doingAction = true;
    }

    public void ClubAttackEnd()
    {
        movementEnabled = true;
        rotationEnabled = true;
        doingAction = false;
        club.SetActive(false);
        StartCoroutine(ClubRecharge());
    } 

    private IEnumerator ClubRecharge()
    {
        yield return new WaitForSeconds(clubCooldown);
        Debug.Log("The club is now ready again");
        clubReady = true;
    }

    private void Leap()
    {
        doingAction = true;
        leapReady = false;
        //transform.position = new Vector3(transform.position.x, transform.position.y + 17, transform.position.z);
        StartCoroutine(Jump());
    }

    private IEnumerator Jump()
    {
        movementEnabled = false;
        rotationEnabled = false;

        float elapsedTime = 0f;
        float time = 0.25f;

        Vector3 start = transform.position;
        Vector3 end = new Vector3(transform.position.x, 30, transform.position.z);

        while (elapsedTime < time)
        {
            transform.position = Vector3.Lerp(start, end, elapsedTime/time);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        leapLandingInstance = Instantiate(leapLandingArea, transform);
        leapLandingInstance.transform.SetParent(null);
        leapLandingInstance.transform.position = new Vector3(transform.position.x, 0.01f, transform.position.z);

        movementEnabled = true;
        rotationEnabled = true;

        StartCoroutine(LeapAction());
    }

    private IEnumerator LeapAction()
    {
        Debug.Log("MIGHT AS WELL JUMP");
        speed = jumpSpeed;

        float elapsedTime = 0f;
        float time = 5f;

        while (elapsedTime < time)
        {
            leapLandingInstance.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(LeapEnd());
    }

    public IEnumerator LeapEnd()
    {
        movementEnabled = false;
        rotationEnabled = false;

        float elapsedTime = 0f;
        float time = 0.25f;

        Vector3 start = transform.position;
        Vector3 end = new Vector3(transform.position.x, -0.2f, transform.position.z);

        while (elapsedTime < time)
        {
            transform.position = Vector3.Lerp(start, end, elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        LandingCrash();

        movementEnabled = true;
        rotationEnabled = true;

        speed = movementSpeed;

        Destroy(leapLandingInstance);
        Debug.Log("You should land here");
        doingAction = false;
        StartCoroutine(LeapRecharge());
    }

    private void LandingCrash()
    {
        mainCamera.GetComponent<CameraShake>().SmallShake(0.5f, 1f);

        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, leapCrashRange);
        foreach (var objectHit in objectsInRange)
        {
            if (objectHit.CompareTag("Player"))
            {
                objectHit.GetComponent<PlayerController>().Stun(1.2f);
            }
        }
    }

    private IEnumerator LeapRecharge()
    {
        yield return new WaitForSeconds(leapCooldown);
        Debug.Log("The leap is now ready again");
        leapReady = true;
    }

    private void ThrowBoulder()
    {
        Debug.Log("Throw the boulder");
        movementEnabled = false;
        rotationEnabled = false;
        boulderReady = false;
        doingAction = true;
        animator.SetTrigger("attack2");
        
    }

    public void Boulder()
    {
        boulderInstance = Instantiate(boulder, transform);
        boulderInstance.transform.localPosition = new Vector3(0, 1, 3f);
        boulderInstance.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
        boulderInstance.transform.SetParent(null);
        boulderInstance.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * boulderSpeed, ForceMode.Impulse);
        boulderInstance.gameObject.GetComponent<Rigidbody>().AddTorque(transform.right * 5, ForceMode.Impulse);
        StartCoroutine(BoulderTravel());
    }

    private IEnumerator BoulderTravel()
    {
        yield return new WaitForSeconds(0.5f);
        movementEnabled = true;
        rotationEnabled = true;
        doingAction = false;
        yield return new WaitForSeconds(4f);
        Destroy(boulderInstance);
        StartCoroutine(BoulderRecharge());
    }

    private IEnumerator BoulderRecharge()
    {
        yield return new WaitForSeconds(boulderCooldown);
        Debug.Log("The boulder is now ready again");
        boulderReady = true;
    }
}
