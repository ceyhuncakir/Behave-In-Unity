using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Perspective : Sense
{
    public int fieldOfView = 45;
    public int viewDistance = 100;

    private Transform playerTransform;
    private Vector3 rayDirection;
    public float damage = 50f;
    public ParticleSystem muzzleFlash;
    public GameObject hiteffect;
    public float impactforce = 30f;
    public float shootDelay = 5f;

    protected override void Initialize()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected override void UpdateSense()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= detectionRate)
        {

            if(playerTransform != null)
            {
                DetectAspect();
            }
        }
    }

    //Detect perspective field of view for the AI Character
    void DetectAspect()
    {
        RaycastHit hit;
        rayDirection = playerTransform.position - transform.position;

        if ((Vector3.Angle(rayDirection, transform.forward)) < fieldOfView)
        {
            // Detect if player is within the field of view
            if (Physics.Raycast(transform.position, rayDirection, out hit, viewDistance))
            {
                Aspect aspect = hit.collider.GetComponent<Aspect>();
                if (aspect != null)
                {
                    //Check the aspect
                    if (aspect.aspectType != aspectName)
                    {
                        shoot();
                    } 
                }

                GameObject hitgameobject = Instantiate(hiteffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(hitgameobject, 1f);
            }
        }
    }

    void shoot()
    {
        muzzleFlash.Play();
        RaycastHit hit;
        if (Physics.Raycast(transform.position, rayDirection, out hit, viewDistance))
        {
            PlayerTank target = hit.transform.GetComponent<PlayerTank>();

            if(target != null)
            {

                target.TakeDamage(damage);


                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForce(-hit.normal * impactforce);
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if (playerTransform == null)
        {
            return;
        }
        Debug.DrawLine(transform.position, playerTransform.position, Color.red);

        Vector3 frontRayPoint = transform.position + (transform.forward * viewDistance);

        //Approximate perspective visualization
        Vector3 leftRayPoint = frontRayPoint;
        leftRayPoint.x += fieldOfView * 0.5f;

        Vector3 rightRayPoint = frontRayPoint;
        rightRayPoint.x -= fieldOfView * 0.5f;

        Debug.DrawLine(transform.position, frontRayPoint, Color.green);
        Debug.DrawLine(transform.position, leftRayPoint, Color.green);
        Debug.DrawLine(transform.position, rightRayPoint, Color.green);
    }
}
