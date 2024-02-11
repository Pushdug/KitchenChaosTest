using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEditor.Experimental;
using UnityEngine.Serialization;

public class Player : NetworkBehaviour, IKitchenObjectParent
{
    public static event EventHandler OnAnyPlayerSpawned;
    public static event EventHandler OnAnyPickedSomething;

    public static void ResetStaticData()
    {
        Player.OnAnyPlayerSpawned = null;
    }

    public static Player LocalInstance { get; private set; }

    public event EventHandler OnPickedSomething;

    public event EventHandler<OnSelectCounterChangedEventArgs> OnSelectCounterChanged;
    public class OnSelectCounterChangedEventArgs :EventArgs
    {
        public BaseCounter selectedCounter;
    }

    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private LayerMask collisionsLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;
    [SerializeField] private List<Vector3> spawnPositionsList;

    private bool isWalking;
    private Vector3 lastInteractDIrection;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }

        transform.position = spawnPositionsList[(int)OwnerClientId];

        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void Start()
    {
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
        GameInput.Instance.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
    {
        if (!GameManager.Instance.IsGamePlaying())
        {
            return;
        }

        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        if (!GameManager.Instance.IsGamePlaying())
        {
            return;
        }

        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        HandleMovement();
        HandleInteractions();
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    private void HandleInteractions()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();

        Vector3 moveDirection = new Vector3(inputVector.x, 0, inputVector.y);

        if (moveDirection != Vector3.zero)
        {
            lastInteractDIrection = moveDirection;
        }

        float interactDistance = 2;
       if (Physics.Raycast(transform.position, lastInteractDIrection, out RaycastHit raycastHit, interactDistance, countersLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                if (baseCounter != selectedCounter)
                {
                    SetSelectedCounter(baseCounter);
                }
             
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            SetSelectedCounter(null);
        }
    }

    private void HandleMovement()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();

        Vector3 moveDirection = new Vector3(inputVector.x, 0, inputVector.y);

        float playerRadius = 0.7f;
        float playerHeight = 2;
        float moveDistance = moveSpeed * Time.deltaTime;
        bool canMove = !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirection, Quaternion.identity, moveDistance,collisionsLayerMask);
        if (!canMove)
        {
            Vector3 moveDirectionX = new Vector3(moveDirection.x, 0, 0);
            canMove = (moveDirection.x < -0.5f || moveDirection.x > 0.5f) && !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirectionX, Quaternion.identity, moveDistance,collisionsLayerMask);

            if (canMove)
            {
                moveDirection = moveDirectionX;
            }
            else
            {
                Vector3 moveDirectionZ = new Vector3(0, 0, moveDirection.z);
                canMove = (moveDirection.z < -0.5f || moveDirection.z > 0.5f) && !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirectionZ, Quaternion.identity, moveDistance,collisionsLayerMask);

                if (canMove)
                {
                    moveDirection = moveDirectionZ;
                }
            }
        }
        if (canMove)
        {
            transform.position += moveDirection * moveDistance;
        }
        isWalking = moveDirection != Vector3.zero;
        float rotationSpeed = 10;
        transform.forward = Vector3.Slerp(transform.forward, moveDirection, rotationSpeed * Time.deltaTime);
    }

    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;

        OnSelectCounterChanged?.Invoke(this, new OnSelectCounterChangedEventArgs()
        {
            selectedCounter = selectedCounter
        });
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;

        if (kitchenObject != null)
        {
            OnPickedSomething?.Invoke(this, EventArgs.Empty);
            OnAnyPickedSomething?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
}
