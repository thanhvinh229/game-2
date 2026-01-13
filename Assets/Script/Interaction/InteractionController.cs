using UnityEngine;

public class InteractionController : MonoBehaviour
{
    [SerializeField] private InteractionInputData _interactionInputData;
    [SerializeField] InteractionPrompt _interactionPromt;
    [SerializeField] private LayerMask _interactionLayer;
    [SerializeField] private float _maxRayCasDistance = 3f;
    [SerializeField] private float _raycastRadius = 0.05f;

    private Interactable _currentInteractable;
    private Camera _camera;
    void Awake()
    {
        _camera = Camera.main;
    }

    void Update()
    {
        CheckForInteractable();
    }

     void OnEnable()
    {
        _interactionInputData.Enable();
    }

     void OnDisable()
    {
        _interactionInputData.Disable();
    }

    private void CheckForInteractable()
    {
        RaycastHit hit;
        if (Physics.SphereCast(_camera.transform.position, _raycastRadius, _camera.transform.forward, out hit, _maxRayCasDistance, _interactionLayer))
        {
            if (hit.collider.gameObject.TryGetComponent<Interactable>(out var interactable))
            {
                //Display UI

                _interactionPromt.gameObject.SetActive(true);
                _interactionPromt.SetPrompt(" take ");

                if(_currentInteractable == interactable)
                {
                    return;
                }
                //Interact

                if(interactable.CanInteract && _interactionInputData.InteractionPressed)
                {
                    interactable.Oninteract();
                }
          
            }
            
        }
        else
        {
            _interactionPromt.gameObject.SetActive(false);
        }
    }



    void OnDrawGizmos()
    {
        if (_camera == null)
        {
            return;
         
        }
        Gizmos.color = Color.green;
        Gizmos.DrawRay(_camera.transform.position, _camera.transform.forward * _maxRayCasDistance);

    }
}
