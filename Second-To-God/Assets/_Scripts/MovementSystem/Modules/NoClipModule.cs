using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Player Movement/NoClip Module")]
public class NoClipModule : PlayerModule
{
    public float flySpeed = 2f;
    public float sprintFlySpeed = 5f; 
    public float speedMultiplier = 1f;
    public LayerMask ignoreMask; 
	private Vector2 input;
    private bool isSprinting;
	private bool sprintToggleState = false;
	public bool toggleSprint = false;
	private Transform cameraHolder;

    public bool isLocked;
    public override bool IsLocked { get => isLocked; set => isLocked = value; }
    private bool isInitialized;
    public override bool IsInitialized { get => isInitialized; set => isInitialized = value; }

    public override void InitializeModule(FPCModule fPCModule)
    {
        IsLocked = false;
        fPCModule.GetComponent<CharacterController>().detectCollisions = false;
        fPCModule.GetComponent<CharacterController>().excludeLayers = ignoreMask;
        fPCModule.staminaReductionRate = 0;

		cameraHolder = Camera.main.transform.parent;

        if (fPCModule.TryGetModule(out MovementModule movementModule))
        {
            movementModule.IsLocked = true;
        }
        if (fPCModule.TryGetModule(out GravityModule gravityModule))
        {
            gravityModule.IsLocked = true;
        }
    }

    public override void OnModuleRemoved(FPCModule fPCModule)
    {
        fPCModule.GetComponent<CharacterController>().excludeLayers = default;

        if (fPCModule.TryGetModule(out MovementModule moduleOut))
        {
            moduleOut.IsLocked = false;
        }
        if (fPCModule.TryGetModule(out GravityModule gravityModule))
        {
            gravityModule.IsLocked = false;
        }
    }

    public override void HandleInput(FPCModule fPCModule)
    {
        input = InputHandler.Instance.playerActions.Move.ReadValue<Vector2>().normalized;
		
        bool sprintInput = InputHandler.Instance.playerActions.Sprint.IsPressed();

        if (toggleSprint)
        {
            if (sprintInput && !sprintToggleState)
            {
                isSprinting = !isSprinting; 
                sprintToggleState = true;
            }
            else if (!sprintInput)
            {
                sprintToggleState = false;
            }
        }
        else isSprinting = sprintInput;

        HandleScrolling();
    }

    private void HandleScrolling()
    {
        float y = Mouse.current.scroll.ReadValue().normalized.y;

        if (y > 0)
        {
            speedMultiplier += 0.5f;
        }
        else if (y < 0)
        {
            speedMultiplier -= 0.5f;
        }
        speedMultiplier = Mathf.Clamp(speedMultiplier, 1, 10f);
    }

    public override void UpdateModule(FPCModule fPCModule)
    {
        Vector3 movement = input.y * cameraHolder.forward + input.x * cameraHolder.right;

		float speed = isSprinting && fPCModule.currentStamina > 5 ? sprintFlySpeed : flySpeed;

		movement *= speed * speedMultiplier;

		fPCModule.movement = new Vector3(movement.x, movement.y, movement.z);
    }
}