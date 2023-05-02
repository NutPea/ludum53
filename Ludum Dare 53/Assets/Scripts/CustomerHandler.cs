
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomerHandler : MonoBehaviour
{
    public Collider pickUpCollider;
    public Collider killCollider;
    public GameObject pickUpIndicationGameobject;
    public MutantType.Mutant currentMutantTyp;
    [HideInInspector] public UnityEvent OnPickUpEvent = new UnityEvent();
    [HideInInspector] public UnityEvent OnDropEvent = new UnityEvent();
    private bool hasBeenDelivered;

    public float distanceBeforeDespawn = 50f;
    CarPassengerPickUpHandler carPassengerPickUpHandler;
    public Animator anim;
    public float wrongMutantKillAnimPropability = 0.33f;
    public float rightMutantKillAnimPropability = 0.33f;
    public List<Rigidbody> flyAwayPartsRigidbodys = new();
    public float flyAwayPartDestroyTimer = 3f;
    public float flyAwayVelocity = 3f;

    private void Start()
    {
        pickUpCollider.enabled = true;
        killCollider.enabled = false;
        
        anim ??= GetComponentInChildren<Animator>();
        anim.SetBool("Waiting", true);
        hasBeenDelivered = false;
    }

    private void Update()
    {
        if (hasBeenDelivered)
        {
            float distance = Vector2.Distance(carPassengerPickUpHandler.transform.position, transform.position);
            if(distance > distanceBeforeDespawn)
            {
                Destroy(gameObject);
            }
        }
    }

    public void HasKilledRightMutant()
    {
        float percentage = Random.Range(0.0f, 1.0f);
        if (percentage < rightMutantKillAnimPropability)
        {
            
            float percentageKill = Random.Range(0.0f, 1.0f);
            if (percentageKill > 0.5f)
            {
                anim.SetTrigger("RightKill1");
            }
            else
            {
                anim.SetTrigger("RightKill2");
            }

        }
    }

    public void HasKilledWrongMutant()
    {
        float percentage = Random.Range(0.0f, 1.0f);
        if (percentage < wrongMutantKillAnimPropability)
        {
            anim.SetTrigger("WrongKill");
        }
    }

    public void DropPassenger()
    {
        OnDropEvent.Invoke();
        hasBeenDelivered = true;
        anim.SetBool("Delivered", true);
        pickUpCollider.enabled = false;
        killCollider.enabled = true;
        Vector3 dirToPlayer = carPassengerPickUpHandler.transform.position - transform.position;
        dirToPlayer.y = 0;
        dirToPlayer = dirToPlayer.normalized;
        transform.forward = dirToPlayer;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

            if (hasBeenDelivered)
            {
                KillCustomer();
                return;
            }
            carPassengerPickUpHandler = other.gameObject.GetComponent<CarPassengerPickUpHandler>();
            if (!carPassengerPickUpHandler.HasPassenger)
            {
                pickUpIndicationGameobject.SetActive(false);
                carPassengerPickUpHandler.AddPassenger(this);
                OnPickUpEvent.Invoke();
                HasBeenPickedUp();
            }
        }
    }
    public void HasBeenPickedUp()
    {
        pickUpCollider.enabled = false;
        anim.SetBool("Waiting", false);
    }


    public void KillCustomer()
    {
        foreach (Rigidbody rigidbody in flyAwayPartsRigidbodys)
        {
            rigidbody.transform.parent = null;
            Destroy(rigidbody.gameObject, flyAwayPartDestroyTimer);
            rigidbody.gameObject.SetActive(true);
            Vector3 flyAwayDirection = rigidbody.transform.position - carPassengerPickUpHandler.transform.position;
            flyAwayDirection = flyAwayDirection.normalized;
            rigidbody.AddForce(flyAwayDirection * flyAwayVelocity + Vector3.up, ForceMode.Impulse);
        }
        Destroy(gameObject);
    }

}
