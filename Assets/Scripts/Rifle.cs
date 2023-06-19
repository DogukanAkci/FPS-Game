using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Rifle : MonoBehaviour
{
    [SerializeField] Transform muzzle;
    [SerializeField] private float gunTimer;
    [SerializeField] private float rateOfFireRiffle;
    [SerializeField] private float rateOfFireGun;
    [SerializeField] private ParticleSystem MuzzleFlash;
    [SerializeField] private AudioClip fireSound;
    [SerializeField] private float range = 300f;
    [SerializeField] private GameObject bullet;
    [SerializeField] private int totalBullet = 80, magazineBullet = 40;
    [SerializeField] private int totalGunBullet = 28, magazineGunBullet = 14;
    [SerializeField] private TMP_Text bulletText, magazineText;
    [SerializeField] private float minX, maxX, minY, maxY; //0-1 minX-maxX, 2-3 minY maxY
    [SerializeField] private CharakterController _charakterController;
    AudioSource audioSource;
    private int remainingBullet = 0,remainingGunBullet = 0;
    private Vector3 rot;
    private RaycastHit hit;
    public bool isReloading;
    
    public bool CanFireRifle => remainingBullet > 0 && !isReloading &&Input.GetMouseButton(0);
    public bool CanFireGun => remainingBullet > 0 && !isReloading &&Input.GetMouseButtonDown(0);

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        remainingBullet = magazineBullet;
        remainingGunBullet = magazineGunBullet;
        SetMagazineTexts();
        isReloading = false;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && CanFireRifle && Time.time > gunTimer&&_charakterController.weapons[0].activeSelf)
        {
            StartCoroutine(FireAnimationWait());
            SetMagazineTexts();
            gunTimer = Time.time + rateOfFireRiffle;
        }else if (Input.GetMouseButtonDown(0) && CanFireGun && Time.time > gunTimer&&_charakterController.weapons[1].activeSelf)
        {
            StartCoroutine(FireAnimationWait());
            SetMagazineTexts();
            gunTimer = Time.time + rateOfFireGun;
        }

        if (_charakterController.weapons[0].activeSelf)
        {
            if (!isReloading && (remainingBullet != magazineBullet) && (Input.GetKeyDown(KeyCode.R))&&totalBullet!=0)
            {
                StartCoroutine(Reload());
            }

            if (!isReloading && remainingBullet == 0 && !EventSystem.current.IsPointerOverGameObject()&&totalBullet!=0)
            {
                StartCoroutine(Reload());
            }
        }else if (_charakterController.weapons[1].activeSelf)
        {
            if (!isReloading && (remainingGunBullet != magazineGunBullet) && (Input.GetKeyDown(KeyCode.R))&&totalGunBullet!=0)
            {
                StartCoroutine(Reload());
            }

            if (!isReloading && remainingGunBullet == 0 && !EventSystem.current.IsPointerOverGameObject()&&totalGunBullet!=0)
            {
                StartCoroutine(Reload());
            }
        }
        

        rot = muzzle.localRotation.eulerAngles;
        if (rot.x != 0 || rot.y != 0)
            muzzle.localRotation =
                Quaternion.Slerp(muzzle.localRotation, Quaternion.Euler(0f, 0f, 0f), Time.deltaTime * 3);
    }

    void Fire()
    {
        if (Physics.Raycast(muzzle.transform.position, muzzle.forward, out hit, range))
        {
            var spawnedBullet = Instantiate(bullet, hit.point, Quaternion.identity);
            spawnedBullet.transform.LookAt(hit.transform.forward);
            Destroy(spawnedBullet.gameObject, 1.5f);
            Debug.Log(hit.transform.tag);
        }

        if (_charakterController.weapons[0].activeSelf)
            remainingBullet -= 1;
        else if (_charakterController.weapons[1].activeSelf)
            remainingGunBullet -= 1;
        Recoil();
        MuzzleFlash.Play();
        audioSource.Play();
        audioSource.clip = fireSound;
    }

    void Recoil()
    {
        Debug.Log("Recoil girdi");
        float recX = Random.Range(minX, maxX);
        float recY = Random.Range(minY, maxY);
        muzzle.DOLocalRotateQuaternion(Quaternion.Euler(rot.x - recY, rot.y + recX, rot.z), 1f);
    }

    IEnumerator Reload()
    {
        isReloading = true;
        if (_charakterController.weapons[0].activeSelf)
        {
            totalBullet += remainingBullet;
            remainingBullet = 0;
            SetMagazineTexts();
            
            // Start Reload Animation
            _charakterController._animator[0].SetBool("Reload", isReloading);

            yield return new WaitForSeconds(2f); // Delay for plug-in magazine to weapon in reload animation

            if (totalBullet >= magazineBullet)
            {
                totalBullet -= magazineBullet;
                remainingBullet = magazineBullet;
            }
            else
            {
                remainingBullet = totalBullet;
                totalBullet = 0;
            }
            SetMagazineTexts();

        }else if (_charakterController.weapons[1].activeSelf)
        {
            totalGunBullet += remainingGunBullet;
            remainingGunBullet = 0;
            SetMagazineTexts();
            
            // Start Reload Animation
            _charakterController._animator[1].SetBool("ReloadGun", isReloading);

            yield return new WaitForSeconds(1.5f); // Delay for plug-in magazine to weapon in reload animation

            if (totalGunBullet >= magazineGunBullet)
            {
                totalGunBullet -= magazineGunBullet;
                remainingGunBullet = magazineGunBullet;
            }
            else
            {
                remainingGunBullet = totalGunBullet;
                totalGunBullet = 0;
            }
            SetMagazineTexts();
        }

        isReloading = !isReloading;
        _charakterController._animator[0].SetBool("Reload",isReloading);
        _charakterController._animator[1].SetBool("ReloadGun",isReloading);
    }

    IEnumerator FireAnimationWait()
    {
        if (_charakterController.isRunning)
            yield return new WaitForSeconds(.2f);
        
        Fire();
    }

    private void SetMagazineTexts()
    {
        if (_charakterController.weapons[0].activeSelf)
        {
            bulletText.text = remainingBullet.ToString();
            magazineText.text = totalBullet.ToString();
        }else if (_charakterController.weapons[1].activeSelf)
        {
            bulletText.text = remainingGunBullet.ToString();
            magazineText.text = totalGunBullet.ToString();
        }
    }
}