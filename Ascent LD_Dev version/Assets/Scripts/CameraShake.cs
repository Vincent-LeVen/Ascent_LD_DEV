using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public Vector3 resetPos;
    [HideInInspector] GameObject character;
    [HideInInspector] PlayerController playerController;

    private void Start()
    {
        resetPos = transform.localPosition;
        character = this.transform.parent.gameObject;
        playerController = character.GetComponent<PlayerController>();
    }

    public IEnumerator CamShake (float duration, float magnitude)
    {
       
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            //float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(/*x +*/resetPos.x, y + resetPos.y, resetPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        if (playerController.isSliding)
        {           
                transform.localPosition = new Vector3(resetPos.x, resetPos.y -3.22f, resetPos.z);       
        } else
        {
        transform.localPosition = resetPos;
        }

    }
}
