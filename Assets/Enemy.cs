using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Enemy : MonoBehaviour
{

    public int posture = 10;
    public bool reversed;
    public float reversedCooldown;

    [Range(1,50)]
    public float hp;
    float initialHp;

    public GameObject UIInfos;
    GiroController giro;

    Color initialColor;
    void Start()
    {
        giro = GiroController.instance;
        initialHp = hp;        

        initialColor = GetComponent<MeshRenderer>().material.GetColor("_Color");
    }

    public ParticleSystem death;

    // Update
    void Update()
    {
        Debug.DrawLine(this.transform.position, giro.cam.transform.position, Color.blue);

        UIInfos.SetActive(Vector3.Dot((this.transform.position - giro.cam.transform.position).normalized, giro.cam.transform.forward) > GameManager.instance.enemiesDetectionThreshold ? true : false);
    }

    private void OnDestroy()
    {
        Instantiate(death, this.transform.position, Quaternion.identity).Play();
    }

    public void TakeDamages(float amount)
    {
        hp -= amount;

        if (hp <= 0)
        {
            reversed = true;
            Methods.SetMaterialColor(this.gameObject, Color.red);
            StopAllCoroutines();
            StartCoroutine(ReversedCooldown());
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(reversed && other.CompareTag("Player") && giro.)
        {
            giro.carriedEnemy = this;
            GetComponent<BoxCollider>().enabled = false;
            GetComponent<SphereCollider>().enabled = false;
            this.transform.parent = giro.grabPoint;
            this.transform.localPosition = Vector3.zero;
        }
    }

    public IEnumerator ReversedCooldown()
    {
        yield return new WaitForSeconds(reversedCooldown);
        reversed = false;
        hp = initialHp;
        Methods.SetMaterialColor(this.gameObject, initialColor);
    }



}
