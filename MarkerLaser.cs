using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MarkerLaser : NetworkBehaviour
{

    // Use this for initialization
    void Start()
    {
        // 1
        laser = Instantiate(laserPrefab);
        // 2
        laserTransform = laser.transform;

        laser2 = Instantiate(laserPrefab2);
        laser3 = Instantiate(laserPrefab3);
        Record = Instantiate(RecordPrefab);
        Record.transform.SetParent(cameraeye.transform, false);
        Delete = Instantiate(DeletePrefab);
        Delete.transform.SetParent(cameraeye.transform, false);
        Save = Instantiate(SavePrefab);
        Save.transform.SetParent(cameraeye.transform, false);

    }

    // Update is called once per frame
    void Update()
    {
        if (Microphone.devices.Length <= 0)
        {
            //Throw a warning message at the console if there isn't  
            //Debug.LogWarning("Microphone not connected!");
        }
        else //At least one microphone is present  
        {
            //Set 'micConnected' to true  
            micConnected = true;

            //Get the default microphone recording capabilities  
            Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);

            //According to the documentation, if minFreq and maxFreq are zero, the microphone supports any frequency...  
            if (minFreq == 0 && maxFreq == 0)
            {
                //...meaning 44100 Hz can be used as the recording sampling rate  
                maxFreq = 44100;
            }

            //Get the attached AudioSource component  
            // goAudioSource = this.GetComponent<AudioSource>();
        }

        if (Controller.GetPress(SteamVR_Controller.ButtonMask.Trigger)/* && Menu == false*/)
        {
            RaycastHit hit;

            //Menu = true;

            // 2
            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100))
            {
                hitPoint = hit.point;

                //selectedMarker = null;
                //hitPoint = hit.point;
                ShowLaser(hit);
                laser2.SetActive(false);
                laser3.SetActive(false);
                //   {
                Delete.SetActive(true);
                Record.SetActive(true);
                Save.SetActive(true);

                selectM = false;
                if (hit.collider.tag == "Marker")
                {
                    laser.SetActive(false);
                    ShowLaser2(hit);
                    selectedMarker = hit.collider.gameObject;
                    print("Maekwe");
                    selectM = true;
                    print(selectM);
                }

                if (hit.collider.tag == "delete")
                {
                    laser.SetActive(false);
                    laser2.SetActive(false);
                    ShowLaser3(hit);
                    print("Dlete");
                    selectD = true;
                    selectM = true;
                }
                if (hit.collider.tag == "Record")
                {
                    laser.SetActive(false);
                    laser2.SetActive(false);

                    ShowLaser3(hit);
                    print("REC");
                    selectR = true;
                    selectM = true;
                }
				if (hit.collider.tag == "Save")
				{
					laser.SetActive(false);
					laser2.SetActive(false);

					ShowLaser3(hit);
					print("SAV");
					selectS = true;
					selectM = true;
				}
                
			}
				
        }
        else
        {
            laser.SetActive(false);
            laser2.SetActive(false);
            laser3.SetActive(false);
        }


        if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            Delete.SetActive(false);
            Record.SetActive(false);
            Save.SetActive(false);
            
            
            if (selectM == false)
                // add marker
            {
                CmdFire();
                Delete.SetActive(false);
                Record.SetActive(false);
                Save.SetActive(false);
            }

                if (selectD && (selectedMarker != null))
            {
                selectD = false;
                Destroy(selectedMarker);
                selectedMarker = null;
                selectM = false;
            }
			if (selectR && (selectedMarker != null))
			{
                Recording();
                selectR = false;
                recordingInProgress = true;
				//selectedMarker = null;
                //selectM = false;
            }
			if (selectS && (selectedMarker != null)&& recordingInProgress == true)
			{
                SaveRec();
				selectS = false;
				selectedMarker = null;
                selectM = false;
                recordingInProgress = false;
            }
        }

        
    }
        

           
        
    
    

    public GameObject selectedMarker;

    private SteamVR_TrackedObject trackedObj;
    public GameObject laserPrefab;
    // 2
    private GameObject laser;
    // 3
    private Transform laserTransform;
    // 4
    private Vector3 hitPoint;

    public GameObject laserPrefab2;
    private GameObject laser2;
    public GameObject laserPrefab3;
    private GameObject laser3;
    public GameObject cameraeye;

    public bool selectM = false;
	public bool selectD = false;
	public bool selectR = false;
	public bool selectS = false;
    public bool recordingInProgress = false;
    public bool Menu = false;
    public GameObject RecordPrefab;
    private GameObject Record;
    public GameObject DeletePrefab;
    private GameObject Delete;

    public GameObject SavePrefab;
    private GameObject Save;
    AudioClip myAudioClip;
    private bool micConnected = false;
    private int minFreq;
    private int maxFreq;



    public GameObject markerPrefab;
    public Transform markerSpawn;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    private void ShowLaser(RaycastHit hit)
    {
        // 1
        laser.SetActive(true);
        laserTransform = laser.transform;
        // 2
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
        // 3
        laserTransform.LookAt(hitPoint);
        // 4
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
            hit.distance);
    }

    private void ShowLaser2(RaycastHit hit)
    {
        // 1
        laser2.SetActive(true);
        laserTransform = laser2.transform;
        // 2
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
        // 3
        laserTransform.LookAt(hitPoint);
        // 4
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
            hit.distance);
    }

    private void ShowLaser3(RaycastHit hit)
    {
        // 1
        laser3.SetActive(true);
        laserTransform = laser3.transform;
        // 2
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
        // 3
        laserTransform.LookAt(hitPoint);
        // 4
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
            hit.distance);
    }

    void CmdFire()
    {
        RaycastHit hit;

        if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100))
        {
            hitPoint = hit.point;
            //creates market, prefab asset
            GameObject obj = Instantiate(markerPrefab, new Vector3(hit.point.x, hit.point.y, hit.point.z), Quaternion.identity) as GameObject;
            // spawn marker
            NetworkServer.Spawn(obj);
        }

    }
    void Recording()
    {
        if (micConnected)
        {
            //If the audio from any microphone isn't being captured  
            if (!Microphone.IsRecording(null))
            {
                myAudioClip = Microphone.Start(null, false, 10, 44100);
               
            }
            else //Recording is in progress  
            {

                GUI.Label(new Rect(200, Screen.height - 50, 200, 50), "Recording in progress...");
            }
        }
        else // No microphone  
        {
            //Print a red "Microphone not connected!" message at the center of the screen  
            GUI.contentColor = Color.red;
            GUI.Label(new Rect(0, Screen.height - 50, 200, 50), "Microphone not connected!");
        }
    }

    void SaveRec()
    {
        SavWav.Save("audio", myAudioClip);

    }
}



