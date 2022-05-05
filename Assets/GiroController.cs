using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using DG.Tweening;
using Invector.vCharacterController;
using UnityEngine.UI;
using TMPro;

public class GiroController : MonoBehaviour
{
    vThirdPersonController controller;

    public TextMeshProUGUI loadText;

    public RectTransform reticule;


    public Camera cam;
    bool aimFlipFlop;

    public GameObject bullet;
    public Transform bulletSpawn;

    public Sprite shootReticule;
    public Sprite throwReticule;

    public PostProcessProfile profile;

    Vignette vignette;

    public static GiroController instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<vThirdPersonController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        profile.TryGetSettings(out vignette);        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1)) Aim();
        if (Input.GetMouseButtonUp(0)) Shoot();
        if (Input.GetMouseButton(0)) LoadShot();

        Debug.DrawRay(cam.transform.position, cam.transform.forward * 10f, Color.yellow);

        loadText.text = shotLoad.ToString("F2");
    }

    public void Aim()
    {
        aimFlipFlop = !aimFlipFlop;
        DOTween.To(() => cam.fieldOfView, x => cam.fieldOfView = x, aimFlipFlop ? 40 : 60, 0.5f);

        float a = reticule.sizeDelta.x;

        DOTween.To(() => a, x => a = x, aimFlipFlop ? 75f : 150f, 0.5f).OnUpdate(() => reticule.sizeDelta = new Vector2(a,a));
        DOTween.To(() => vignette.intensity.value, x => vignette.intensity.value = x, aimFlipFlop ? 0.15f : 0f, 0.5f).OnComplete(() => minVignette = aimFlipFlop ? 0.15f : 0f);
        controller.moveSpeed = aimFlipFlop ? 1 : 4;
    }

    public void Shoot()
    {
        GameObject go = Instantiate(bullet, bulletSpawn.transform.position, Quaternion.identity);
        Bullet bul = go.GetComponent<Bullet>();

        float loadProgression = Mathf.InverseLerp(minTimeToBeConsideredAsLoad, timeToFullyLoadShot, shotLoad);

        bul.damages += Mathf.Lerp(0, maxShootForce, loadProgression);

        Vector3 target = cam.transform.position + (cam.transform.forward * 10).NoisyVector(aimFlipFlop ? 0.05f : 0.3f);

        bul.fireDir = (bul.transform.position - target).normalized;

        go.transform.DOMove(target, 0.25f).OnComplete(() => Destroy(go));

        bul.SetColor(Color.Lerp(Color.white, Color.blue, loadProgression));
        
        vignette.intensity.value = minVignette;
        shotLoad = 0f;

        //StartCoroutine(ShootRoutine(0.25f));
    }

    float shotLoad;
    float minVignette = 0f;

    [Range(0f, 5f)]
    public float minTimeToBeConsideredAsLoad;
    [Range(0.5f,5f)]
    public float timeToFullyLoadShot;
     [Range(0f,10f)]
    public float maxShootForce;
    public void LoadShot()
    {
        if (shotLoad < timeToFullyLoadShot)
        {
            vignette.intensity.value = Mathf.Lerp(minVignette, minVignette + 0.2f, Mathf.InverseLerp(minTimeToBeConsideredAsLoad, timeToFullyLoadShot, shotLoad));
            shotLoad += Time.deltaTime;
        }
        else shotLoad = timeToFullyLoadShot;
    }

    private void OnApplicationQuit()
    {
        vignette.intensity.value = 0f;
    }

    public IEnumerator ShootRoutine(float duration)
    {
        GameObject go = Instantiate(bullet, bulletSpawn.transform.position, Quaternion.identity);
        Bullet bul = go.GetComponent<Bullet>();
        float loadProgression = Mathf.InverseLerp(minTimeToBeConsideredAsLoad, timeToFullyLoadShot, shotLoad);

        float t = 0f;
        Vector3 origin = go.transform.position;
        Vector3 destination = cam.transform.position + (cam.transform.forward * 10).NoisyVector(aimFlipFlop ? 0.05f : 0.3f);

        go.GetComponent<Bullet>().damages += Mathf.Lerp(0, maxShootForce, loadProgression);
        bul.SetColor(Color.Lerp(Color.white, Color.blue, loadProgression));

        while(t< duration)
        {
            go.transform.position = Vector3.Lerp(origin, destination, t / duration);
            t += Time.deltaTime;
            yield return null;
        }



        vignette.intensity.value = minVignette;
        shotLoad = 0f;

        Destroy(go);
    }
}
