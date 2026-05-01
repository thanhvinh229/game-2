using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSound : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private AudioSource uiAudioSource;
    [SerializeField] private AudioClip clickSound;

   
    public void OnPointerClick(PointerEventData eventData)
    {
        if (uiAudioSource != null && clickSound != null)
        {
            uiAudioSource.PlayOneShot(clickSound);
        }
    }
}
