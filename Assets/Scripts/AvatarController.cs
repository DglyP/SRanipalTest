using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject scriptGameObject = gameObject;

        StartCoroutine(ChangePupilSize());
    }

    IEnumerator ChangePupilSize()
    {
        yield return new WaitForSeconds(5f);
        Debug.Log("FiveSecondsPassed");

        // Assuming you have a reference to the Renderer component
        Renderer rendererComponent = gameObject.GetComponent<Renderer>();

        // Assuming the material you want to modify is at index 0
        Material material = rendererComponent.materials[0];

        // Change the color of the material
        material.color = Color.red;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
