﻿using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DoorManager : PhaNetworkingMessager {

	private static DoorManager singleton;
	public static DoorManager Singleton 
	{ 
		get 
		{ 
			return singleton; 
		} 
	}

	GoodDoor[] doors;
	Vector3[] previousPositions;
	// Use this for initialization
	void Start () {
		singleton = this;
		doors = GetComponentsInChildren<GoodDoor>();
		previousPositions = new Vector3[doors.Length];
		for (int i = 0; i < doors.Length; i++)
		{
			previousPositions[i] = doors[i].transform.position;
		}
	}

	public void parseDoorUpdate(ref StringBuilder buffer)
	{
		string[] values = buffer.ToString().Split(' ');
		int id = int.Parse(values[1]);
		Debug.Log(buffer);
		if (id != -1)
		{		
			Vector3 newPos = new Vector3(float.Parse(values[2]), float.Parse(values[3]), float.Parse(values[4]));
			Quaternion newQuat = new Quaternion(float.Parse(values[5]), float.Parse(values[6]), float.Parse(values[7]), float.Parse(values[8]));

			doors[id].transform.position = newPos;
			doors[id].transform.rotation = newQuat;
		}
		else
		{
			GameObject.Find(values[2]).GetComponent<GoodDoor>().SetCode(values[3]);
		}
	}

	public void SendDoorMessages()
	{
		for (int i = 0, length = doors.Length; i < length; i++)
		{
			SendDoorCodeUpdate(doors[i]);
		}
		//yield return new WaitForSeconds(4.0f);
	}

	void SendDoorCodeUpdate(GoodDoor givenDoor)
	{
		StringBuilder doorCode = new StringBuilder(((int)MessageType.DoorUpdate).ToString() + " " + -1 + " " + givenDoor.transform.parent.name + " " + givenDoor.GetCode());
		PhaNetworkingAPI.SendTo(PhaNetworkingAPI.mainSocket, doorCode, doorCode.Length, PhaNetworkingAPI.targetIP);
		Debug.Log(doorCode);
	}
	
	/// <summary>
	/// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
	/// </summary>
	void FixedUpdate()
	{
		if (PhaNetworkManager.characterSelection == 0)
		{
			for (int i = 0; i < doors.Length; i++)
			{
				if (previousPositions[i] != doors[i].transform.position)
				{
					SendDoorUpdate(i, doors[i].transform.position, doors[i].transform.rotation);
					previousPositions[i] = doors[i].transform.position;
				}
			}
		}
	}
}
