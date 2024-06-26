﻿using UnityEngine;

public class PenguinController : MonoBehaviour
{
    public float moveSpeed = 30f;
    public float rotateSpeed = 30f;
    public float hitForce = 10f;
    public float ballRange = 4f;
    public float respawnDelay = 1f;
    public float clearDistance = 10f;

    public enum Role
    {
        Attack,
        Defense
    }

    public Role myRole = Role.Attack;
    public TextMesh displayText;

    public GameObject enemyGoal;
    public GameObject ownGoal;
    public GameObject teammate;
    public GameObject[] enemyPenguins;

    private Rigidbody rb;
    private GameObject ball;
    private GameObject rink;
    private Vector3 rinkExtents;
    private Vector3 initialPosition;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        ball = GameObject.FindGameObjectWithTag("Ball");
        rink = GameObject.FindGameObjectWithTag("Rink");
        rinkExtents = rink.GetComponent<Renderer>().bounds.extents;
        initialPosition = transform.position;
    }

    public void Respawn()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        gameObject.SetActive(false);
        Invoke("RespawnPenguin", respawnDelay);
    }

    private void RespawnPenguin()
    {
        transform.position = initialPosition;
        // Randomize Z position so we get some variety
        transform.position = new Vector3(transform.position.x, transform.position.y, Random.Range(-rinkExtents.z, rinkExtents.z));
        gameObject.SetActive(true);
    }

    private void FixedUpdate()
    {
        switch (myRole)
        {
            case Role.Attack:
                if (BallInRange() && BallIsBetweenMeAndEnemyGoal())
                {
                    displayText.text = "Shooting";
                    displayText.color = Color.red;
                    KickBallTowardsGoal();
                    break;
                }
                if (BallInRange() && BallIsOnOurSide())
                {
                    displayText.text = "Dribbling";
                    displayText.color = Color.blue;
                    MoveTowards(enemyGoal.transform.position);
                    break;
                }
                displayText.text = "Attack Move";
                displayText.color = Color.blue;
                MoveTowards(ball.transform.position);
                break;

            case Role.Defense:
                if (BallInRange() && BallIsOnOurSide() && !TeamIsOnOurSide())
                {
                    displayText.text = "Passing";
                    displayText.color = Color.yellow;
                    KickBallTowardsTeammate();
                    break;
                }
                if (BallInRange() && BallIsOnOurSide() && EnemyIsOnOurSide())
                {
                    displayText.text = "Clearing";
                    displayText.color = Color.yellow;
                    KickBallTowardsGoal();
                    break;
                }
                displayText.text = "Zone Defense";
                displayText.color = Color.green;
                Vector3 inBetween = (ball.transform.position + ownGoal.transform.position) / 2;
                MoveTowards(inBetween);
                break;
        }
        ClampPositionWithinRink();
    }

    private bool BallInRange()
    {
        return (transform.position - ball.transform.position).magnitude < ballRange;
    }

    private bool IAmCloserToBallThanTeammate()
    {
        return (transform.position - ball.transform.position).magnitude < (teammate.transform.position - ball.transform.position).magnitude;
    }

    private bool BallIsOnOurSide()
    {
        Vector3 ballToOwnGoal = ownGoal.transform.position - ball.transform.position;
        Vector3 ballToEnemyGoal = enemyGoal.transform.position - ball.transform.position;
        return ballToOwnGoal.magnitude < ballToEnemyGoal.magnitude;
    }

    private bool BallIsBetweenMeAndEnemyGoal()
    {
        Vector3 ballToEnemyGoal = enemyGoal.transform.position - ball.transform.position;
        Vector3 selfToBall = ball.transform.position - transform.position;
        return selfToBall.magnitude < ballToEnemyGoal.magnitude;
    }

    private bool EnemyIsOnOurSide()
    {
        foreach (GameObject enemyPenguin in enemyPenguins)
        {
            if ((enemyPenguin.transform.position - ownGoal.transform.position).magnitude < (enemyPenguin.transform.position - enemyGoal.transform.position).magnitude)
            {
                return true;
            }
        }
        return false;
    }

    private bool TeamIsOnOurSide()
    {
        return (teammate.transform.position - ownGoal.transform.position).magnitude < (teammate.transform.position - enemyGoal.transform.position).magnitude;
    }

    private void KickBallTowardsGoal()
    {
        Vector3 kickDirection = enemyGoal.transform.position - ball.transform.position;
        ball.GetComponent<Rigidbody>().AddForce(kickDirection.normalized * hitForce, ForceMode.Impulse);
    }

    private void KickBallTowardsTeammate()
    {
        Vector3 kickDirection = teammate.transform.position - ball.transform.position;
        ball.GetComponent<Rigidbody>().AddForce(kickDirection.normalized * hitForce, ForceMode.Impulse);
    }

    private void MoveTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        rb.AddForce(direction * moveSpeed);

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball") || collision.gameObject.CompareTag("Penguin"))
        {
            Vector3 hitDirection = collision.contacts[0].point - transform.position;
            hitDirection = hitDirection.normalized;
            collision.gameObject.GetComponent<Rigidbody>().AddForce(hitDirection * hitForce, ForceMode.Impulse);
        }
    }

    private void ClampPositionWithinRink()
    {
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -rinkExtents.x, rinkExtents.x);
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, -rinkExtents.z, rinkExtents.z);
        transform.position = clampedPosition;
    }
}