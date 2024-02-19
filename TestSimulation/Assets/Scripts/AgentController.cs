using UnityEngine;
using System;
using DG.Tweening;
using Random = UnityEngine.Random;

public class AgentController: MonoBehaviour
{
    public static Action<AgentController> UpdateAgentStatus;
    [field: SerializeField] public int Health { get; private set; } = 3;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider col;
    [SerializeField] private Outline outline;
    [SerializeField] private NameData names;
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float turnSpeed = 1;
    [SerializeField] private float animDuration = 1;
    [SerializeField] private ParticleSystem sparksParticles;
    [SerializeField] private Ease scaleEase = Ease.InOutBack;
    private Vector3 _moveDirection;
    public bool Selected { get; private set; }
    public string Name { get; private set; }
    private int _defaultHealth;
    private bool _canMove;

    private Vector3 _defaultScale; 

    private void Awake()
    {
        _defaultHealth = Health;
        _defaultScale = transform.localScale;
    }

    private void Update()
    {
        if (_canMove)
        {
            _moveDirection = _moveDirection.normalized;
            rb.velocity = _moveDirection.normalized * moveSpeed;
            Quaternion toRotation = Quaternion.LookRotation(_moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, turnSpeed * Time.deltaTime);   
        }
    }

    private void OnEnable()
    {
        SpawnAnimation();
        _moveDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
        Name = names.GenerateName();
        Health = _defaultHealth;
        
        //Subscriptions
        UpdateAgentStatus += DeselectAgent;
    }

    private void OnDisable()
    {
        //Unsubscribes
        UpdateAgentStatus -= DeselectAgent;
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject collisionObject = collision.gameObject;
        
        if (collisionObject.CompareTag(gameObject.tag))
        {
            _moveDirection *= -1;
            TakeDamage();
        }

        if (collisionObject.CompareTag("Border"))
        {
            Vector3 normal = collision.contacts[0].normal;
            _moveDirection = Vector3.Reflect(_moveDirection, normal);
        }

        if (!collisionObject.CompareTag("Ground"))
        {
            //particles
            Vector3 newSparksPos = collision.contacts[0].point;
            Transform sparksTransform = sparksParticles.transform;
            sparksTransform.position = new Vector3(newSparksPos.x, sparksTransform.position.y, newSparksPos.z);
        
            sparksParticles.Play();
        }
    }

    private void TakeDamage()
    {
        Health--;
        if (Selected) UpdateAgentStatus?.Invoke(this);
        if (Health <= 0) Die();
    }

    private void Die()
    {
        if (Selected)
        {
            Selected = false;
            outline.enabled = false;
            UpdateAgentStatus?.Invoke(this);
        }

        col.enabled = false;
        rb.useGravity = false;
        _canMove = false;
        
        transform.DOScale(0, animDuration)
            .SetEase(scaleEase)
            .OnComplete(() =>
            {
                GameManager.AgentDied?.Invoke(this);
            });
    }

    private void SpawnAnimation()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(_defaultScale, animDuration)
            .SetEase(scaleEase)
            .OnComplete(() =>
            {
                col.enabled = true;
                rb.useGravity = true;
                _canMove = true;
            });
    }

    public void OnAgentClick()
    {
        Selected = !Selected;
        outline.enabled = Selected;
        UpdateAgentStatus?.Invoke(this);
    }

    private void DeselectAgent(AgentController agentController)
    {
        if (agentController != this)
        {
            Selected = false;
            outline.enabled = false;
        }
    }
}