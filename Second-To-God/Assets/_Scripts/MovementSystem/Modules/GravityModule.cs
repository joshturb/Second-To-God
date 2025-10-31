using UnityEngine;

[CreateAssetMenu(menuName = "Player Movement/Gravity Module")]
public class GravityModule : PlayerModule
{
    public LayerMask groundMask;
    public float detectionLength = 0.5f;
    public float gravityMultiplier = 1f;
    private float gravity;

    public bool isLocked;
    public override bool IsLocked { get => isLocked; set => isLocked = value; }

    private bool _isInitialized;

    public override bool IsInitialized { get => _isInitialized; set => _isInitialized = value; }

    public override void InitializeModule(FPCModule fPCModule)
    {
        IsLocked = false;
        gravity = Physics.gravity.y * gravityMultiplier;
    }

    public override void OnModuleRemoved(FPCModule fPCModule) {	}

    public override void HandleInput(FPCModule fPCModule)
    {
        fPCModule.isGrounded = GroundedCheck(fPCModule);
    }

	public bool GroundedCheck(FPCModule fPCModule)
	{
		return Physics.CheckSphere(fPCModule.transform.position, detectionLength, groundMask, QueryTriggerInteraction.Ignore);
	}

    public override void UpdateModule(FPCModule fPCModule)
    {
        if (fPCModule.isGrounded)
        {
            fPCModule.movement.y = 0;
        }
        else
        {
            fPCModule.movement.y += gravity * Time.deltaTime;
            fPCModule.movement.y = fPCModule.movement.y < gravity ? gravity : fPCModule.movement.y;
        }
    }
}
