using UnityEngine;
using System;
using DG.Tweening;
using Interfaces;
using ScriptableObjects;
using Random = UnityEngine.Random;

public class AgentController: MonoBehaviour, IClickable
{
    #region Variables

    public static Action<AgentController> UpdateAgentStatus;
    
    public bool Selected { get; private set; }
    public string Name { get; private set; }
    
    [field: SerializeField, Min(1), Header("Parameters")] public int Health { get; private set; } = 3;
    
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float turnSpeed = 1;
    [SerializeField] private float animDuration = 1;
    
    [SerializeField] private Ease scaleEase = Ease.InOutBack;
    
    [Header("Components")]
    [SerializeField] private ParticleSystem particlesOfDestruction;
    [SerializeField] private ParticleSystem sparksParticles;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider col;
    [SerializeField] private Outline outline;
    [SerializeField] private NameData names;
      
    private bool _canMove;
    private int _defaultHealth;
    
    private Vector3 _moveDirection;
    private Vector3 _defaultScale;   

    private Vector3 RandomDirection => new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
    private Quaternion TargetRotation => Quaternion.LookRotation(_moveDirection.normalized, Vector3.up);

    #endregion

    #region MonoBehaviour methods

    private void Awake()
    {
        _defaultHealth = Health;
        _defaultScale = transform.localScale;
    }

    private void FixedUpdate()
    {
        if (_canMove)//move && turn
        {
            rb.velocity = _moveDirection.normalized * moveSpeed;
            transform.rotation = Quaternion.Slerp(transform.rotation, TargetRotation, turnSpeed);
        }
    }

    private void OnEnable()
    {
        //new agent initialization
        SpawnAnimation();
        _moveDirection = RandomDirection;
        transform.rotation = TargetRotation;
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
        
        //reflect & damage
        if (collisionObject.CompareTag(gameObject.tag))
        {
            _moveDirection *= -1;
            TakeDamage();
        }

        //reflect
        if (collisionObject.CompareTag("Border"))
        {
            Vector3 normal = collision.contacts[0].normal;
            _moveDirection = Vector3.Reflect(_moveDirection, normal);
        }
        
        //sparks particles
        if (!collisionObject.CompareTag("Ground"))
        {
            Vector3 newSparksPos = collision.contacts[0].point;
            Transform sparksTransform = sparksParticles.transform;
            sparksTransform.position = new Vector3(newSparksPos.x, sparksTransform.position.y, newSparksPos.z);
        
            sparksParticles.Play();
        }
    }

    #endregion

    #region private methods

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
        
        particlesOfDestruction.Play();
        
        transform.DOScale(0, animDuration)
            .SetEase(scaleEase)
            .OnComplete(() =>
            {
                //return to object pool
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

    private void OnAgentClick()
    {
        Selected = !Selected;
        outline.enabled = Selected;
        UpdateAgentStatus?.Invoke(this);
    }

    private void DeselectAgent(AgentController agentController)
    {
        //disable the "selected" status when choosing another agent
        if (agentController != this)
        {
            Selected = false;
            outline.enabled = false;
        }
    }

    #endregion

    #region interface implementation

    public void OnMouseClick()
    {
        OnAgentClick();
    }

    #endregion
}