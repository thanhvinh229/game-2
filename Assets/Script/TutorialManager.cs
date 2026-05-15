using UnityEngine;
using TMPro;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public TextMeshProUGUI hintText;
    public CanvasGroup hintCanvasGroup;
    void Start()
   {
    StartCoroutine(InitialTutorialRoutine());
   }
 
   IEnumerator InitialTutorialRoutine()
   {
    // 1. Hướng dẫn cơ bản
    yield return StartCoroutine(HintRoutine("Sử dụng WASD để di chuyển, Chuột trái để tấn công", 4f));
    
    yield return new WaitForSeconds(0.5f);
 
    yield return StartCoroutine(HintRoutine("Di chuyển hướng bất kì + ấn chuột trái  để rút kiếm", 4f));
    
     yield return new WaitForSeconds(0.5f);
 
    // 2. Hướng dẫn mục tiêu (Cục đá)
    yield return StartCoroutine(HintRoutine("Hãy tìm tảng đá để bắt đầu thử thách!", 5f));
 
    yield return new WaitForSeconds(0.5f);
 
    yield return StartCoroutine(HintRoutine("Đừng lo , ngó nghiêng chút là bạn sẽ thấy nó thôi", 5f));
  }
 
    void Update()
    {
        
    }
 
    IEnumerator HintRoutine(string message, float duration)
    {
        hintText.text = message;
        
        // Fade In
        float elapsed = 0;
        while (elapsed < 0.5f)
        {
            hintCanvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / 0.5f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        hintCanvasGroup.alpha = 1f; 
 
        yield return new WaitForSeconds(duration);
 
        // Fade Out
        elapsed = 0;
        while (elapsed < 0.5f)
        {
            hintCanvasGroup.alpha = Mathf.Lerp(1, 0, elapsed / 0.5f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        hintCanvasGroup.alpha = 0f; 
        hintText.text = "";        
    }
}


