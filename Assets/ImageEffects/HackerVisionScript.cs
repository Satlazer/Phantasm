﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class HackerVisionScript : MonoBehaviour
{
	public Material nightVisionMaterial;
    public Material thermalMaterial;
    public Material sonarMaterial;
    public Material filmGrainMaterial;
    public Material waveMaterial;
    public Texture thermalRamp;
    public Material[] ChromaticAberrationMaterial;

	private Camera CameraSettings;

    private Color ambientLightTemp;
    public Color ambientLight = new Color(0.25f, 0.25f, 0.25f);

    private Light[] all_my_damn_lights;

    private bool[] lightStatus;
    private float[] lightBrightness;
	
	[Space(10)]
	public FilmGrain filmGrain;
    private float scrollAmount;
    private float scrollSpeed;
    private float filmGrainGlitchAmount = 0.0f;
    

    private float filmGrainBarrel = 1.15f;
	
	[Space(10)]
    public Color SonarColor = new Color(0.5f, 0.5f, 1.0f);
    [Range(0.0f, 1.0f)]
    public float SonarDiffusePass = 1.0f;
    public float SonarTimeMult = 1.0f;
    public float SonarMult = 0.02f;
	
	[Space(10)]
    public MovieTexture Movie;
    private float MovieAmount = 1.0f;
    private float filmGrainFaceAmount = 0.0f;

    enum HackerVisionMode { Normal = 0, Night = 1, Thermal = 2, Sonar = 3, Last = 4};
    HackerVisionMode Vision = HackerVisionMode.Normal;


	
	[Space(10)]
	private GameObject	GOAgent;
	private GameObject	GOAgentMesh;
	private GameObject	GOPhantom;
	private Agent	EntityAgent;
	private Phantom	EntityPhantom;
	private Plasma.Visibility	VisibleAgent;
	private Plasma.Visibility	VisiblePhantom;
	
	[Space(10)]
	public CameraMaterials agentMaterials;
	public CameraMaterials phantomMaterials;
	private int cameraModeArray = 1;

	
	[Header("Camera Wave Amount")]
    public Vector2 WaveCount = new Vector2(40.0f, 40.0f);
    public Vector2 WaveIntensity = new Vector2(0.01f, 0.01f);
    public Vector2 WaveTimeMult = new Vector2(1.0f, 1.0f);
	[Space(5)]

    [Tooltip("Determines concentration of aberration closer to the center of the screen")]
    [Range(0.1f, 3.0f)]
    public float CA_Dispersal = 1.0f;

    [Tooltip("Determines amount of Chromatic Aberration")]
    [Range(0.0f, 0.2f)]
    public float CA_Offset = 0.025f;

    RenderTexture temp;

    float timeNudge;

    Matrix4x4 ProjBiasMatrix = new Matrix4x4();

    float timeSinceOffset = 0.0f;
    float timeOffsetLength = 0.1f;
    static float timeOffsetLengthMin = 0.1f;
    static float timeOffsetLengthMax = 0.1f * 3.0f;
    float timeOffsetRange = 0.05f;
    Vector2 timeOffsetValue;

    float timeSinceSwap = 1.0f;


	[System.Serializable]
    public struct CameraMaterials
    {
		public Material normal;
		public Material camera;
		public Material thermal;
		public Material sonar;
	}

	[System.Serializable]
    public struct FilmGrain
    {
		public Texture ScrollingTexture;
		public Texture MultTexture;
		public Texture GlitchTexture;
		public Texture FaceTexture;

		[Range(0.0f, 1.0f)]
		public float nightVisionAmount;

		[Range(0.0f, 1.0f)]
		public float normalAmount;

	}

	private int defaultCameraLayersActive;

    // Awake is called when the script instance is being loaded
    public void Awake()
    {
        //all_my_damn_lights.FindAll(s => s.Equals("Light")); //= GetComponent<Light>();

        //Movie.loop = true;

        timeSinceOffset = 10.0f;

        CameraSettings = GetComponent<Camera>();
		defaultCameraLayersActive = CameraSettings.cullingMask;

        all_my_damn_lights = FindObjectsOfType(typeof(Light)) as Light[];
        for (int i = 0; i < all_my_damn_lights.Length; ++i)
        {
            //Debug.Log("It Works");
            lightStatus = new bool[all_my_damn_lights.Length];
            lightBrightness = new float[all_my_damn_lights.Length];
            //lightStatus.Add(all_my_damn_lights[i].enabled);
        }

        scrollSpeed = Random.Range(0.8f, 1.2f);
        scrollAmount = Random.Range(0.0f, 1000.0f);
        temp = new RenderTexture(Screen.width, Screen.height, 0);
        timeNudge = Random.Range(0.0f, 1000.0f);

        filmGrainMaterial.SetTexture("uScrollingTexture", filmGrain.ScrollingTexture);
        filmGrainMaterial.SetTexture("uMultTexture", filmGrain.MultTexture);
        filmGrainMaterial.SetTexture("uScrollingGlitchTexture", filmGrain.GlitchTexture);
        filmGrainMaterial.SetTexture("uMovie", Movie);
        filmGrainMaterial.SetFloat("uMovieAmount", MovieAmount);
        filmGrainMaterial.SetTexture("uSpookyFaceTexture", filmGrain.FaceTexture);
        filmGrainMaterial.SetFloat("uSpookyAmount", 0.0f);
        //Movie.Play();

        ProjBiasMatrix.SetRow(0, new Vector4(2.0f, 0.0f, 0.0f, -1.0f));
        ProjBiasMatrix.SetRow(1, new Vector4(0.0f, 2.0f, 0.0f, -1.0f));
        ProjBiasMatrix.SetRow(2, new Vector4(0.0f, 0.0f, 2.0f, -1.0f));
        ProjBiasMatrix.SetRow(3, new Vector4(0.0f, 0.0f, 0.0f, 1.0f));

        ambientLightTemp = RenderSettings.ambientLight;

        thermalRamp.wrapMode = TextureWrapMode.Clamp;

        Shader.SetGlobalFloat("_EmissionVisionMult", 0.0f);
    }

    void Start()
    {
        //all_my_damn_lights.FindAll(s => s.Equals("Light")); //= GetComponent<Light>();

        //Movie.loop = true;

        timeSinceOffset = 10.0f;

        CameraSettings = GetComponent<Camera>();
        all_my_damn_lights = FindObjectsOfType(typeof(Light)) as Light[];
        for (int i = 0; i < all_my_damn_lights.Length; ++i)
        {
            //Debug.Log("It Works");
            lightStatus = new bool[all_my_damn_lights.Length];
            lightBrightness = new float[all_my_damn_lights.Length];
            //lightStatus.Add(all_my_damn_lights[i].enabled);
        }

        scrollSpeed = Random.Range(0.8f, 1.2f);
        scrollAmount = Random.Range(0.0f, 1000.0f);
        temp = new RenderTexture(Screen.width, Screen.height, 0);
        timeNudge = Random.Range(0.0f, 1000.0f);

        filmGrainMaterial.SetTexture("uScrollingTexture", filmGrain.ScrollingTexture);
        filmGrainMaterial.SetTexture("uMultTexture", filmGrain.MultTexture);
        filmGrainMaterial.SetTexture("uScrollingGlitchTexture", filmGrain.GlitchTexture);
        //filmGrainMaterial.SetTexture("uMovie", Movie);
        filmGrainMaterial.SetFloat("uMovieAmount", MovieAmount);
        filmGrainMaterial.SetTexture("uSpookyFaceTexture", filmGrain.FaceTexture);
        filmGrainMaterial.SetFloat("uSpookyAmount", 0.0f);
        //Movie.Play();

        ProjBiasMatrix.SetRow(0, new Vector4(2.0f, 0.0f, 0.0f, -1.0f));
        ProjBiasMatrix.SetRow(1, new Vector4(0.0f, 2.0f, 0.0f, -1.0f));
        ProjBiasMatrix.SetRow(2, new Vector4(0.0f, 0.0f, 2.0f, -1.0f));
        ProjBiasMatrix.SetRow(3, new Vector4(0.0f, 0.0f, 0.0f,  1.0f));

        ambientLightTemp = RenderSettings.ambientLight;

        thermalRamp.wrapMode = TextureWrapMode.Clamp;

        Shader.SetGlobalFloat("_EmissionVisionMult", 0.0f);
    }

    public void Update()
    {


		GOAgent = GameObject.Find("Agent(Clone)");
		GOAgentMesh = GameObject.Find("agent_Mesh");
        GOPhantom = GameObject.Find("Phantom(Clone)");
		if(GOAgent != null)
		{			
			EntityAgent = GOAgent.GetComponent<Agent>();
			VisibleAgent = EntityAgent.visibility;
		}
		if(GOPhantom != null)
		{	
			EntityPhantom = GOPhantom.GetComponent<Phantom>();
			VisiblePhantom = EntityPhantom.visibility;
		}
		
		
		//Camera Layers

		// 08	EnemyVisible	
		// 09	Enemy			
		// 10	EnemyThermal	
		// 11	EnemySonar		
		
		// Default Camera Layers get added with different layer depending on camera active
		
		/*
			Set Materials for Agent and Phantom next
		 */
		 

		if (Vision == HackerVisionMode.Normal || Vision == HackerVisionMode.Night)
        {
			if(GOAgentMesh != null)
			{
				Renderer[] agentRenderers = GOAgentMesh.GetComponentsInChildren<Renderer>();//.material = agentMaterials.camera;
				
				for (int c = 0; c < agentRenderers.Length; c++)
				{
					agentRenderers[c].material = agentMaterials.camera;
				}
			}

			CameraSettings.cullingMask = (1 << 9) | defaultCameraLayersActive;

			if(GOPhantom != null)
			{
				if(VisiblePhantom.camera > 0)
				{
					GOPhantom.layer = 9;
					if(VisiblePhantom.camera == Plasma.SeenBy.Camera.Translucent)
					{
						GOPhantom.GetComponent<Renderer>().material = phantomMaterials.normal;
					}
					else
					{
						GOPhantom.GetComponent<Renderer>().material = phantomMaterials.camera;
					}
				}
			}
		}
		else if (Vision == HackerVisionMode.Thermal)
        {
			if(GOAgentMesh != null)
			{
				Renderer[] agentRenderers = GOAgentMesh.GetComponentsInChildren<Renderer>();//.material = agentMaterials.camera;
				for(int c = 0; c < agentRenderers.Length; c++)
				{
					agentRenderers[c].material = agentMaterials.thermal;
				}
			}

			CameraSettings.cullingMask = (1 << 10) | defaultCameraLayersActive;

			if(GOPhantom != null)
			{
				if(VisiblePhantom.thermal > 0)
				{
					GOPhantom.layer = 10;
					if(VisiblePhantom.thermal == Plasma.SeenBy.Thermal.Visible)
					{
						GOPhantom.GetComponent<Renderer>().material = phantomMaterials.thermal;
					}
				}
			}
		}
		else if (Vision == HackerVisionMode.Sonar)
        {
			if(GOAgentMesh != null)
			{
				Renderer[] agentRenderers = GOAgentMesh.GetComponentsInChildren<Renderer>();//.material = agentMaterials.camera;
				for(int c = 0; c < agentRenderers.Length; c++)
				{
					agentRenderers[c].material = agentMaterials.sonar;
				}
			}

			CameraSettings.cullingMask = (1 << 11) | defaultCameraLayersActive;

			if(GOPhantom != null)
			{
				if(VisiblePhantom.sonar > 0)
				{
					GOPhantom.layer = 11;
					if(VisiblePhantom.sonar == Plasma.SeenBy.Sonar.Visible)
					{
						GOPhantom.GetComponent<Renderer>().material = phantomMaterials.sonar;
					}
				}
			}
		}
		else
		{
			GOPhantom.layer = 8;
		}





        if (Vision != HackerVisionMode.Sonar)
        {
            for (int i = 0; i < all_my_damn_lights.Length; ++i)
            {
                lightStatus[i] = all_my_damn_lights[i].enabled;
            }
        }
        if (Vision != HackerVisionMode.Night)
        {
            for (int i = 0; i < all_my_damn_lights.Length; ++i)
            {
                lightBrightness[i] = all_my_damn_lights[i].intensity;
            }
        }

        // Film Grain transition between vision modes
        if (timeSinceOffset > timeOffsetLength * 5.0f && Time.timeSinceLevelLoad > 5.0f)
        {
            float RandomOffsetChance = Random.Range(0.0f, 1000.0f);
            if (RandomOffsetChance > 992.0f)
            {
                timeSinceOffset = 0.0f;
                timeOffsetValue = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
                timeOffsetLength = Random.Range(timeOffsetLengthMin, timeOffsetLengthMax);
                //
                //Movie.Play();
                //MovieAmount = 1.0f;
                filmGrainMaterial.SetFloat("uMovieAmount", MovieAmount);

                if (Random.Range(0.0f, 1.0f) > 0.95f)
                {
                    filmGrainFaceAmount = 1.0f;
                }
            }
        }

        timeSinceOffset += Time.deltaTime;
        timeSinceSwap += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.N))
        {
            timeSinceSwap = 0.0f;

            Vision++;
            if (Vision == HackerVisionMode.Last)
                Vision = HackerVisionMode.Normal;
            // Loop back to beginning

			if (Vision == HackerVisionMode.Normal || Vision == HackerVisionMode.Night)
				cameraModeArray = 1;
			else if (Vision == HackerVisionMode.Thermal)
				cameraModeArray = 2;
			else if (Vision == HackerVisionMode.Sonar)
				cameraModeArray = 3;
        }



    }

    public void OnPreRender()
    {
		




        //_Light.enabled = true;

        if (Vision == HackerVisionMode.Night)
        {
            // If night vision is on, turn the ambient light up and store actual ambient light
            ambientLightTemp = RenderSettings.ambientLight;
            RenderSettings.ambientLight = ambientLight;
        }

        if (Vision == HackerVisionMode.Normal)
        {
            Shader.SetGlobalFloat("_EmissionVisionMult", 0.0f);
        }
        else
        {
            Shader.SetGlobalFloat("_EmissionVisionMult", 10.0f);
        }

        if (Vision == HackerVisionMode.Sonar)
        {
            for (int i = 0; i < all_my_damn_lights.Length; ++i)
            {
                all_my_damn_lights[i].enabled = false;
            }
            //_Light.enabled = false;
        }
        else
        {
            for (int i = 0; i < all_my_damn_lights.Length; ++i)
            {
                all_my_damn_lights[i].enabled = lightStatus[i];
            }

        }

        if (Vision == HackerVisionMode.Night)
        {
            for (int i = 0; i < all_my_damn_lights.Length; ++i)
            {
                all_my_damn_lights[i].intensity = lightBrightness[i] * 4.0f;
            }
            //_Light.enabled = false;
        }
        else
        {
            for (int i = 0; i < all_my_damn_lights.Length; ++i)
            {
                all_my_damn_lights[i].intensity = lightBrightness[i];
            }

        }


		
    }

    // OnRenderImage is called after all rendering is complete to render image
    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        {
            Material CA_Active;

            if (CA_Offset < 0.55f / 16)
            {
                CA_Active = ChromaticAberrationMaterial[0];
            }
            else if (CA_Offset < 0.55f / 8)
            {
                CA_Active = ChromaticAberrationMaterial[1];
            }
            else if (CA_Offset < 0.55f / 4)
            {
                CA_Active = ChromaticAberrationMaterial[2];
            }
            else
            {
                CA_Active = ChromaticAberrationMaterial[3];
            }

            CA_Active = ChromaticAberrationMaterial[2];

            CA_Active.SetFloat("_Dispersal", CA_Dispersal);
            CA_Active.SetFloat("_Offset", CA_Offset);

            Graphics.Blit(source, temp, CA_Active);
            Graphics.Blit(temp, source);
        }



        float filmGrainAmountTotal = 0.0f;

        // Reset the ambient light
        RenderSettings.ambientLight = ambientLightTemp;



        if (timeSinceOffset < timeOffsetLength && Vision != HackerVisionMode.Sonar)
        {
            filmGrainMaterial.SetFloat("uScrollingGlitchAmount", 1.0f);
            filmGrainAmountTotal += 0.1f;


            filmGrainFaceAmount -= Time.deltaTime * 2.0f;
            filmGrainFaceAmount = Mathf.Max(0.0f, filmGrainFaceAmount);
            //filmGrainFaceAmount
            //filmGrainMaterial.SetFloat("uSpookyAmount", Mathf.Pow(Mathf.InverseLerp(timeOffsetLength, 0.0f, timeSinceOffset), 1.0f));
            filmGrainMaterial.SetFloat("uSpookyAmount", filmGrainFaceAmount);

            filmGrainMaterial.SetVector("uOffsetAmount", new Vector2(
                timeOffsetValue.x + Random.Range(-timeOffsetRange, timeOffsetRange),
                timeOffsetValue.y + Random.Range(-timeOffsetRange, timeOffsetRange)));
        }
        else
        {
            //Movie.Stop();
            //MovieAmount = 0.0f;
            filmGrainMaterial.SetFloat("uMovieAmount", MovieAmount);
            filmGrainMaterial.SetFloat("uSpookyAmount", 0.0f);

            filmGrainMaterial.SetFloat("uScrollingGlitchAmount", 0.0f);
            filmGrainMaterial.SetVector("uOffsetAmount", new Vector2(0.0f, 0.0f));
        }

        filmGrainMaterial.SetVector("uScrollAmount", new Vector2(timeNudge + Time.time * scrollSpeed, timeNudge + Time.time * scrollSpeed));

        filmGrainAmountTotal += Mathf.Clamp(filmGrain.normalAmount + Mathf.InverseLerp(0.3f, 0.1f, timeSinceSwap), 0.0f, 1.0f);
        float RandomNum = Random.Range(0.0f, 1.0f);
        filmGrainMaterial.SetFloat("RandomNumber", RandomNum);
        filmGrainMaterial.SetFloat("uAmount", filmGrainAmountTotal);
        filmGrainMaterial.SetFloat("uDistortion", filmGrainBarrel);

        waveMaterial.SetVector("uWaveCount", WaveCount);
        waveMaterial.SetVector("uWaveIntensity", WaveIntensity);
        waveMaterial.SetVector("uTime", WaveTimeMult * Time.time);

        //agentMaterials.camera.SetColor("_EmissionColor", new Color(0.0f, 0.0f, 0.0f));
        //phantomMaterials.camera.SetColor("_EmissionColor", new Color(0.0f, 0.0f, 0.0f));
        //Shader.SetGlobalFloat(emissionShader.GetInstanceID(), 1.0f); 
        //_EmissionMult
        //emissionShader.

        if (Vision == HackerVisionMode.Normal)
        {

            Graphics.Blit(source, temp, waveMaterial);
            Graphics.Blit(temp, destination, filmGrainMaterial);
        }
        else if (Vision == HackerVisionMode.Night)
        {
            //float RandomNum = Random.Range(0.0f, 1.0f);
            nightVisionMaterial.SetFloat("RandomNumber", RandomNum);
            nightVisionMaterial.SetFloat("uAmount", 0.0f);
            nightVisionMaterial.SetFloat("uLightMult", 1.2f);


            filmGrainMaterial.SetFloat("uAmount", Mathf.Max(filmGrain.nightVisionAmount, filmGrainAmountTotal));

            Graphics.Blit(source, temp, nightVisionMaterial);
            Graphics.Blit(temp, destination, filmGrainMaterial);
        }
        else if (Vision == HackerVisionMode.Thermal)
        {
            //thermalRamp.filterMode = FilterMode.Point;
            thermalMaterial.SetTexture("ThermalRamp", thermalRamp);
			
			agentMaterials.thermal.SetColor("_EmissionColor", new Color(1.0f, 1.0f, 1.0f));
			phantomMaterials.thermal.SetColor("_EmissionColor", new Color(1.0f, 1.0f, 1.0f));

            Graphics.Blit(source, temp, thermalMaterial);
            Graphics.Blit(temp, destination, filmGrainMaterial);
        }
        else if (Vision == HackerVisionMode.Sonar)
        {
            sonarMaterial.SetVector("uColorAdd", new Vector4(SonarColor.r, SonarColor.g, SonarColor.b, SonarTimeMult * Time.time));
            sonarMaterial.SetVector("uParameter", new Vector4(SonarMult, SonarDiffusePass, 0.0f, 0.0f));

            Matrix4x4 inverseMatrix = Matrix4x4.Inverse(CameraSettings.projectionMatrix) * ProjBiasMatrix;
            sonarMaterial.SetMatrix("uProjBiasMatrixInverse", inverseMatrix);

            filmGrainMaterial.SetFloat("uAmount", filmGrainAmountTotal - filmGrain.normalAmount);
            filmGrainMaterial.SetFloat("uDistortion", 1.0f);

            Graphics.Blit(source, temp, sonarMaterial);
            Graphics.Blit(temp, destination, filmGrainMaterial);
        }
    }
}
