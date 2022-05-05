using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public int posture = 10;
    public bool reversed;
    public float reversedCooldown;

    [Range(1,50)]
    public float hp;

    public GameObject UIInfos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public ParticleSystem death;

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(this.transform.position, GiroController.instance.cam.transform.position, Color.blue);

        
        UIInfos.SetActive(Vector3.Dot((this.transform.position - GiroController.instance.cam.transform.position).normalized, GiroController.instance.cam.transform.forward) > GameManager.instance.enemiesDetectionThreshold ? true : false);
    }

    private void OnDestroy()
    {
        Instantiate(death, this.transform.position, Quaternion.identity).Play();
    }


}
