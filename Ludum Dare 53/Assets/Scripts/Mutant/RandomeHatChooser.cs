using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomeHatChooser : MonoBehaviour
{

    public Transform spawnPos;
    public float spawnPercentage = 0.5f;

    public List<GameObject> hats;

    // Start is called before the first frame update
    void Start()
    {
        float percentage = Random.Range(0.0f, 1.0f);
        if(percentage < spawnPercentage)
        {
            int randomeHat = Random.Range(0, hats.Count);
            GameObject hat = GameObject.Instantiate(hats[randomeHat], Vector3.zero, Quaternion.identity);
            hat.transform.parent = spawnPos;
            hat.transform.localPosition = Vector3.zero;
            hat.transform.forward = spawnPos.transform.forward;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
