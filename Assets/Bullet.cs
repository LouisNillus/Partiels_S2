using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bullet : MonoBehaviour
{
    public float damages = 1f;
    public Vector3 fireDir;

    public void SetColor(Color col)
    {
        Methods.SetMaterialColor(this.gameObject, col);
    }

    public ParticleSystem enemyTouched;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Vector3 origin = other.transform.position;
            Debug.DrawRay(this.transform.position, fireDir, Color.red, 10f);
            other.transform.DOMove((origin - fireDir).ChangeY(origin.y), 0.25f);

            GetComponent<Collider>().enabled = false;

            other.GetComponent<Enemy>().TakeDamages(damages);
            ParticleSystem ps = Instantiate(enemyTouched, other.transform);
            ps.transform.localPosition = Vector3.zero;
            ps.Play();
        }
    }

}
