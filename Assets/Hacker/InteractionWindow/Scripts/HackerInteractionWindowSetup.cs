﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CameraRectType
{
    Normal,
    Map,
    Console,
    FloorChanger,
    HeartRate
}
public class HackerInteractionWindowSetup : MonoBehaviour
{
    public Camera cameraMap;
    public CameraPosition cameraPosition;
    public CameraRectType windowType = CameraRectType.Normal;

    public GameObject cameraButtonPrefab;
    public GameObject doorButtonPrefab;
    public GameObject speakerButtonPrefab;
    public GameObject pickupButtonPrefab;
    public Texture healthTexture;
    public Texture ammoTexture;
    private List<Camera> survCameras;
    private List<GameObject> survCameraButtons;

    // List of Doors for drawing    
    private List<GoodDoor> survDoors;
    private List<GameObject> survDoorButtons;

    // List of Speakers for drawing    
    private List<CodeVoice> survSpeakers;
    private List<GameObject> survSpeakerButtons;
    

    // List of Pickups for drawing    
    private List<PickupScript> survPickups;
    private List<GameObject> survPickupButtons;

    public bool WindowIsInteractive = true;

    private Vector2 WindowSize;
    
    public int numOfFloors = 2;
    public int viewFloor = 0;
    private float floorHeight;

    // Use this for initialization
    void Start()
    {
        WindowSize = new Vector2(Screen.width, Screen.height);

        GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width / 2.0f, Screen.height / 2.0f);

        cameraMap = GameObject.Find("HackerMapPrefab").GetComponent<Camera>();

        SetWindowSizes();
        //switch (cameraPosition)
        //{
        //    case CameraPosition.BottomLeft:
        //        GetComponent<RectTransform>().anchoredPosition = new Vector2(Screen.width / 4.0f, Screen.height / 4.0f);
        //        break;
        //    case CameraPosition.BottomRight:
        //        GetComponent<RectTransform>().anchoredPosition = new Vector2((Screen.width / 4.0f) + Screen.width / 2.0f, Screen.height / 4.0f);
        //        break;
        //    case CameraPosition.TopLeft:
        //        GetComponent<RectTransform>().anchoredPosition = new Vector2(Screen.width / 4.0f, (Screen.height / 4.0f) + Screen.height / 2.0f);
        //        break;
        //    case CameraPosition.TopRight:
        //        GetComponent<RectTransform>().anchoredPosition = new Vector2((Screen.width / 4.0f) + Screen.width / 2.0f, (Screen.height / 4.0f) + Screen.height / 2.0f);
        //        break;
        //    default:
        //        GetComponent<RectTransform>().anchoredPosition = new Vector2(Screen.width / 4.0f, Screen.height / 4.0f);
        //        break;
        //}



        Camera[] tempCameras = FindObjectsOfType<Camera>();
        survCameras = new List<Camera>();
        for (int i = 0; i < tempCameras.Length; i++)
        {
            if (tempCameras[i].CompareTag("HackerCamera"))
            {
                survCameras.Add(tempCameras[i]);
            }
        }
        survCameraButtons = new List<GameObject>();

        
        GoodDoor[] tempDoors = FindObjectsOfType<GoodDoor>();
        survDoors = new List<GoodDoor>();
        for (int i = 0; i < tempDoors.Length; i++)
        {
            if (tempDoors[i].locked)
            {
                survDoors.Add(tempDoors[i]);
            }
        }
        survDoorButtons = new List<GameObject>();

        CodeVoice[] tempSpeakers = FindObjectsOfType<CodeVoice>();
        survSpeakers = new List<CodeVoice>();
        for (int i = 0; i < tempSpeakers.Length; i++)
        {
            if (tempSpeakers[i].codeGenned)
            {
                survSpeakers.Add(tempSpeakers[i]);
            }
        }
        survSpeakerButtons = new List<GameObject>();

        PickupScript[] tempPickups = FindObjectsOfType<PickupScript>();
        survPickups = new List<PickupScript>();
        for (int i = 0; i < tempPickups.Length; i++)
        {
            if (tempPickups[i].isActiveAndEnabled)
            {
                survPickups.Add(tempPickups[i]);
            }
        }
        survPickupButtons = new List<GameObject>();

        Vector3 CameraPositionMax = survCameras[0].transform.position;
        Vector3 CameraPositionMin = survCameras[0].transform.position;

        floorHeight = (CameraPositionMax.y - CameraPositionMin.y) / numOfFloors;

        if(cameraMap != null)
        {
            CameraPositionMin.x = cameraMap.transform.position.x - cameraMap.orthographicSize;
            CameraPositionMax.x = cameraMap.transform.position.x + cameraMap.orthographicSize;
            CameraPositionMin.z = cameraMap.transform.position.z - cameraMap.orthographicSize;
            CameraPositionMax.z = cameraMap.transform.position.z + cameraMap.orthographicSize;
            Debug.Log("cameraMap bounds set");
        }
        else
        {
            Debug.Log("cameraMap is null");
        }

        for (int i = 0; i < survCameras.Count; i++)
        {
            GameObject tempButton = Instantiate(cameraButtonPrefab);
            tempButton.GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>());

            Vector3 LerpPosition = survCameras[i].transform.position;
            LerpPosition.x = Mathf.InverseLerp(CameraPositionMin.x, CameraPositionMax.x, survCameras[i].transform.position.x);
            LerpPosition.y = Mathf.InverseLerp(CameraPositionMin.y, CameraPositionMax.y, survCameras[i].transform.position.y);
            LerpPosition.z = Mathf.InverseLerp(CameraPositionMin.z, CameraPositionMax.z, survCameras[i].transform.position.z);

            tempButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                    0.45f * Mathf.Lerp(-GetComponent<RectTransform>().rect.width,
                    GetComponent<RectTransform>().rect.width, LerpPosition.x),

                    0.45f * Mathf.Lerp(-GetComponent<RectTransform>().rect.height,
                    GetComponent<RectTransform>().rect.height, LerpPosition.z));

            survCameraButtons.Add(tempButton);
            survCameraButtons[i].GetComponent<CameraButtonManipulation>().associatedCamera = survCameras[i];
        }

        // Code to put the doors as buttons


        for (int i = 0; i < survDoors.Count; i++)
        {
            GameObject tempButton = Instantiate(doorButtonPrefab);
            tempButton.GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>());

            Vector3 LerpPosition = survDoors[i].transform.position;
            LerpPosition.x = Mathf.InverseLerp(CameraPositionMin.x, CameraPositionMax.x, survDoors[i].transform.position.x);
            LerpPosition.y = Mathf.InverseLerp(CameraPositionMin.y, CameraPositionMax.y, survDoors[i].transform.position.y);
            LerpPosition.z = Mathf.InverseLerp(CameraPositionMin.z, CameraPositionMax.z, survDoors[i].transform.position.z);

            tempButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                    0.45f * Mathf.Lerp(-GetComponent<RectTransform>().rect.width,
                    GetComponent<RectTransform>().rect.width, LerpPosition.x),

                    0.45f * Mathf.Lerp(-GetComponent<RectTransform>().rect.height,
                    GetComponent<RectTransform>().rect.height, LerpPosition.z));

            RawImage image = tempButton.GetComponent<RawImage>();
            int doorNum = survDoors[i].roomNumber - 1;
            float doorNumBrightness = (doorNum / 8) * 0.25f;
            doorNum = doorNum % 8;
            Color color = Color.HSVToRGB(doorNum / 8.0f, 1.0f - doorNumBrightness, 1.0f);
            image.color = color;

            survDoorButtons.Add(tempButton);
        }

        for (int i = 0; i < survSpeakers.Count; i++)
        {
            GameObject tempButton = Instantiate(speakerButtonPrefab);
            tempButton.GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>());

            Vector3 LerpPosition = survSpeakers[i].transform.position;
            LerpPosition.x = Mathf.InverseLerp(CameraPositionMin.x, CameraPositionMax.x, survSpeakers[i].transform.position.x);
            LerpPosition.y = Mathf.InverseLerp(CameraPositionMin.y, CameraPositionMax.y, survSpeakers[i].transform.position.y);
            LerpPosition.z = Mathf.InverseLerp(CameraPositionMin.z, CameraPositionMax.z, survSpeakers[i].transform.position.z);

            tempButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                    0.45f * Mathf.Lerp(-GetComponent<RectTransform>().rect.width,
                    GetComponent<RectTransform>().rect.width, LerpPosition.x),

                    0.45f * Mathf.Lerp(-GetComponent<RectTransform>().rect.height,
                    GetComponent<RectTransform>().rect.height, LerpPosition.z));

            RawImage image = tempButton.GetComponent<RawImage>();
            int doorNum = survSpeakers[i].roomNumber - 1;
            float doorNumBrightness = (doorNum / 8) * 0.25f;
            doorNum = doorNum % 8;
            Color color = Color.HSVToRGB(doorNum / 8.0f, 1.0f - doorNumBrightness, 1.0f);
            image.color = color;

            survSpeakerButtons.Add(tempButton);
        }

        for (int i = 0; i < survPickups.Count; i++)
        {
            GameObject tempButton = Instantiate(pickupButtonPrefab);
            tempButton.GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>());

            Vector3 LerpPosition = survPickups[i].transform.position;
            LerpPosition.x = Mathf.InverseLerp(CameraPositionMin.x, CameraPositionMax.x, survPickups[i].transform.position.x);
            LerpPosition.y = Mathf.InverseLerp(CameraPositionMin.y, CameraPositionMax.y, survPickups[i].transform.position.y);
            LerpPosition.z = Mathf.InverseLerp(CameraPositionMin.z, CameraPositionMax.z, survPickups[i].transform.position.z);

            tempButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                    0.45f * Mathf.Lerp(-GetComponent<RectTransform>().rect.width,
                    GetComponent<RectTransform>().rect.width, LerpPosition.x),

                    0.45f * Mathf.Lerp(-GetComponent<RectTransform>().rect.height,
                    GetComponent<RectTransform>().rect.height, LerpPosition.z));

            RawImage image = tempButton.GetComponent<RawImage>();
            if(survPickups[i].itemType == PickupType.Ammo)
                image.texture = ammoTexture;
            if(survPickups[i].itemType == PickupType.Health)
                image.texture = healthTexture;

            survPickupButtons.Add(tempButton);
        }
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            viewFloor = 0;
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            viewFloor = 1;
        }

        if (WindowSize.x != Screen.width || WindowSize.y != Screen.height)
        {
            SetWindowSizes();
        }

        for (int i = 0, count = survCameras.Count; i < count; i++)
        {
            survCameraButtons[i].GetComponent<RectTransform>().rotation = 
                Quaternion.Euler(0.0f, 0.0f, -survCameras[i].transform.rotation.eulerAngles.y);
        }

        if (!Input.GetMouseButton(0))
        {
            Vector3 CameraPositionMax = survCameras[0].transform.position;
            Vector3 CameraPositionMin = survCameras[0].transform.position;

            for (int i = 0, count = survCameras.Count; i < count; i++)
            {
                Vector3 CameraPositionTemp = survCameras[i].transform.position;

                if (CameraPositionMax.x < CameraPositionTemp.x)
                {
                    CameraPositionMax.x = CameraPositionTemp.x + 0.0f;
                }
                if (CameraPositionMax.y < CameraPositionTemp.y)
                {
                    CameraPositionMax.y = CameraPositionTemp.y + 0.0f;
                }
                if (CameraPositionMax.z < CameraPositionTemp.z)
                {
                    CameraPositionMax.z = CameraPositionTemp.z + 0.0f;
                }

                if (CameraPositionMin.x > CameraPositionTemp.x)
                {
                    CameraPositionMin.x = CameraPositionTemp.x - 0.0f;
                }
                if (CameraPositionMin.y > CameraPositionTemp.y)
                {
                    CameraPositionMin.y = CameraPositionTemp.y - 0.0f;
                }
                if (CameraPositionMin.z > CameraPositionTemp.z)
                {
                    CameraPositionMin.z = CameraPositionTemp.z - 0.0f;
                }
            }

            if(cameraMap != null)
            {
                CameraPositionMin.x = cameraMap.transform.position.x - cameraMap.orthographicSize;
                CameraPositionMax.x = cameraMap.transform.position.x + cameraMap.orthographicSize;
                CameraPositionMin.z = cameraMap.transform.position.z - cameraMap.orthographicSize;
                CameraPositionMax.z = cameraMap.transform.position.z + cameraMap.orthographicSize;
            }
            else
            {
                Debug.Log("cameraMap is null");
            }

            for (int i = 0, count = survCameras.Count; i < count; i++)
            {
                survCameraButtons[i].GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>());

                Vector3 LerpPosition = survCameras[i].transform.position;
                LerpPosition.x = Mathf.InverseLerp(CameraPositionMin.x, CameraPositionMax.x, survCameras[i].transform.position.x);
                LerpPosition.y = Mathf.InverseLerp(CameraPositionMin.y, CameraPositionMax.y, survCameras[i].transform.position.y);
                LerpPosition.z = Mathf.InverseLerp(CameraPositionMin.z, CameraPositionMax.z, survCameras[i].transform.position.z);

                survCameraButtons[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(
                        0.5f * Mathf.Lerp(-GetComponent<RectTransform>().rect.width,
                        GetComponent<RectTransform>().rect.width, LerpPosition.x) + 0.0f,
                        0.5f * Mathf.Lerp(-GetComponent<RectTransform>().rect.height,
                        GetComponent<RectTransform>().rect.height, LerpPosition.z) - 0.0f);
                survCameraButtons[i].GetComponent<CameraButtonManipulation>().associatedCamera = survCameras[i];


                survCameraButtons[i].GetComponent<Button>().interactable = WindowIsInteractive;

                if(survCameras[i].transform.position.y >= CameraPositionMin.y + viewFloor * floorHeight
                && survCameras[i].transform.position.y <= CameraPositionMin.y + (viewFloor + 1) * floorHeight
                && survCameras[i].transform.position.x <= CameraPositionMax.x
                && survCameras[i].transform.position.x >= CameraPositionMin.x
                && survCameras[i].transform.position.z <= CameraPositionMax.z
                && survCameras[i].transform.position.z >= CameraPositionMin.z)
                {
                    survCameraButtons[i].SetActive(true);
                }
                else
                {
                    survCameraButtons[i].SetActive(false);
                }

                
            }

            for (int i = 0; i < survDoors.Count; i++)
            {
                survDoorButtons[i].GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>());

                Vector3 LerpPosition = survDoors[i].transform.position;
                LerpPosition.x = Mathf.InverseLerp(CameraPositionMin.x, CameraPositionMax.x, survDoors[i].transform.position.x);
                LerpPosition.y = Mathf.InverseLerp(CameraPositionMin.y, CameraPositionMax.y, survDoors[i].transform.position.y);
                LerpPosition.z = Mathf.InverseLerp(CameraPositionMin.z, CameraPositionMax.z, survDoors[i].transform.position.z);

                survDoorButtons[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(
                        0.45f * Mathf.Lerp(-GetComponent<RectTransform>().rect.width,
                        GetComponent<RectTransform>().rect.width, LerpPosition.x),

                        0.45f * Mathf.Lerp(-GetComponent<RectTransform>().rect.height,
                        GetComponent<RectTransform>().rect.height, LerpPosition.z));
                    
                if(survCameras[i].transform.position.y >= cameraMap.nearClipPlane + cameraMap.transform.position.y
                && survCameras[i].transform.position.y <= cameraMap.farClipPlane + cameraMap.transform.position.y
                && survDoors[i].transform.position.x <= CameraPositionMax.x
                && survDoors[i].transform.position.x >= CameraPositionMin.x
                && survDoors[i].transform.position.z <= CameraPositionMax.z
                && survDoors[i].transform.position.z >= CameraPositionMin.z)
                {
                    survDoorButtons[i].SetActive(true);
                }
                else
                {
                    survDoorButtons[i].SetActive(false);
                }

                if(survDoors[i].locked)
                {
                    
                }
                else
                {
                    survDoorButtons[i].GetComponent<Button>().interactable = false;
                }
            }


            for (int i = 0; i < survSpeakers.Count; i++)
            {
                survSpeakerButtons[i].GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>());

                Vector3 LerpPosition = survSpeakers[i].transform.position;
                LerpPosition.x = Mathf.InverseLerp(CameraPositionMin.x, CameraPositionMax.x, survSpeakers[i].transform.position.x);
                LerpPosition.y = Mathf.InverseLerp(CameraPositionMin.y, CameraPositionMax.y, survSpeakers[i].transform.position.y);
                LerpPosition.z = Mathf.InverseLerp(CameraPositionMin.z, CameraPositionMax.z, survSpeakers[i].transform.position.z);

                survSpeakerButtons[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(
                        0.45f * Mathf.Lerp(-GetComponent<RectTransform>().rect.width,
                        GetComponent<RectTransform>().rect.width, LerpPosition.x),

                        0.45f * Mathf.Lerp(-GetComponent<RectTransform>().rect.height,
                        GetComponent<RectTransform>().rect.height, LerpPosition.z));
                    
                if(survSpeakers[i].transform.position.x <= CameraPositionMax.x
                && survSpeakers[i].transform.position.x >= CameraPositionMin.x
                && survSpeakers[i].transform.position.z <= CameraPositionMax.z
                && survSpeakers[i].transform.position.z >= CameraPositionMin.z)
                {
                    survSpeakerButtons[i].SetActive(true);
                }
                else
                {
                    survSpeakerButtons[i].SetActive(false);
                }

                /// TODO 
                // remove speakers from the map when their code is entered
                
            }

            for (int i = 0; i < survPickups.Count; i++)
            {
                survPickupButtons[i].GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>());

                Vector3 LerpPosition = survPickups[i].transform.position;
                LerpPosition.x = Mathf.InverseLerp(CameraPositionMin.x, CameraPositionMax.x, survPickups[i].transform.position.x);
                LerpPosition.y = Mathf.InverseLerp(CameraPositionMin.y, CameraPositionMax.y, survPickups[i].transform.position.y);
                LerpPosition.z = Mathf.InverseLerp(CameraPositionMin.z, CameraPositionMax.z, survPickups[i].transform.position.z);

                survPickupButtons[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(
                        0.45f * Mathf.Lerp(-GetComponent<RectTransform>().rect.width,
                        GetComponent<RectTransform>().rect.width, LerpPosition.x),

                        0.45f * Mathf.Lerp(-GetComponent<RectTransform>().rect.height,
                        GetComponent<RectTransform>().rect.height, LerpPosition.z));

                if(survPickups[i].transform.position.x <= CameraPositionMax.x
                && survPickups[i].transform.position.x >= CameraPositionMin.x
                && survPickups[i].transform.position.z <= CameraPositionMax.z
                && survPickups[i].transform.position.z >= CameraPositionMin.z)
                {
                    survPickupButtons[i].SetActive(true);
                }
                else
                {
                    survPickupButtons[i].SetActive(false);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        
    }

    void SetWindowSizes()
    {
        WindowSize = new Vector2(Screen.width, Screen.height);

        switch (cameraPosition)
        {
            case CameraPosition.BottomLeft:
                GetComponent<RectTransform>().anchoredPosition = new Vector2(Screen.width / 4.0f, Screen.height / 4.0f);
                break;
            case CameraPosition.BottomRight:
                GetComponent<RectTransform>().anchoredPosition = new Vector2((Screen.width / 4.0f) + Screen.width / 2.0f, Screen.height / 4.0f);
                break;
            case CameraPosition.TopLeft:
                GetComponent<RectTransform>().anchoredPosition = new Vector2(Screen.width / 4.0f, (Screen.height / 4.0f) + Screen.height / 2.0f);
                break;
            case CameraPosition.TopRight:
                GetComponent<RectTransform>().anchoredPosition = new Vector2((Screen.width / 4.0f) + Screen.width / 2.0f, (Screen.height / 4.0f) + Screen.height / 2.0f);
                break;
            default:
                GetComponent<RectTransform>().anchoredPosition = new Vector2(Screen.width / 4.0f, Screen.height / 4.0f);
                break;
        }

        
        switch(windowType)
        {
            case CameraRectType.Normal:
                break;
            case CameraRectType.Map:
                GetComponent<RectTransform>().anchoredPosition += new Vector2((Screen.width / 4.0f * 0.325f / (Screen.height / (float)Screen.width)), Screen.height / 4.0f * 0.25f);
                //GetComponent<RectTransform>().localScale = new Vector3(0.75f, 0.75f, 1.0f);
                GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width / 2.0f * (Screen.height / (float)Screen.width), Screen.height / 2.0f) * 0.75f;
                //GetComponent<RectTransform>().localScale = Vector3.Scale(GetComponent<RectTransform>().localScale, new Vector3(0.5f, 1.0f, 1.0f));
                break;
            case CameraRectType.Console:
                break;
            case CameraRectType.FloorChanger:
                break;
            case CameraRectType.HeartRate:
                break;
                
        }

    }
}
