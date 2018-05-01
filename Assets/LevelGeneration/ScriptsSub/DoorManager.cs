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
	CodeVoice[] speakers;

	// Use this for initialization
	void Start () {
		singleton = this;
		doors = GetComponentsInChildren<GoodDoor>();
		previousPositions = new Vector3[doors.Length];
		for (int i = 0; i < doors.Length; i++)
		{
			previousPositions[i] = doors[i].transform.position;
		}
		speakers = FindObjectsOfType<CodeVoice>();
	}

	public void parseDoorUpdate(ref StringBuilder buffer)
	{
		string[] values = buffer.ToString().Split(' ');
		int id = int.Parse(values[1]);
		if (id > -1)
		{		
			Vector3 newPos = new Vector3(float.Parse(values[2]), float.Parse(values[3]), float.Parse(values[4]));
			Quaternion newQuat = new Quaternion(float.Parse(values[5]), float.Parse(values[6]), float.Parse(values[7]), float.Parse(values[8]));

			doors[id].transform.position = newPos;
			doors[id].transform.rotation = newQuat;
		}
		else if (id == -1)
		{
			transform.Find(values[2] + " " + values[3]).GetComponentInChildren<GoodDoor>().SetCode(values[4]);
		}
		else if (id == -2)
		{
			speakers[int.Parse(values[2])].setCode(values[3]);
		}
	}

	public void SendDoorMessages()
	{
		for (int i = 0, length = doors.Length; i < length; i++)
		{
			if (doors[i].GetCode() != "")
			{
				SendDoorCodeUpdate(ref doors[i]);
			}
		}
		for (int i = 0, count = speakers.Length; i < count; i++)
		{
			SendVoiceCodeUpdate(ref speakers[i], i);
		}
	}

	void SendDoorCodeUpdate(ref GoodDoor givenDoor)
	{
		StringBuilder doorCode = new StringBuilder(((int)MessageType.DoorUpdate).ToString() + " " + -1 + " " + givenDoor.transform.parent.name + " " + givenDoor.GetCode());
		PhaNetworkingAPI.SendTo(PhaNetworkingAPI.mainSocket, doorCode, doorCode.Length, PhaNetworkingAPI.targetIP);
	}

	void SendVoiceCodeUpdate(ref CodeVoice speaker, int index)
	{
		StringBuilder doorCode = new StringBuilder(((int)MessageType.DoorUpdate).ToString() + " " + -2 + " " + index + " " + speaker.getCode());
		PhaNetworkingAPI.SendTo(PhaNetworkingAPI.mainSocket, doorCode, doorCode.Length, PhaNetworkingAPI.targetIP);
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
