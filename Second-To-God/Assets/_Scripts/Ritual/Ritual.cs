using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ritual : MonoBehaviour
{
	public static Ritual Instance { get; private set; }

	public Dictionary<int, bool> ritualSpotDictionary = new();
	public RitualSpot[] ritualSpots;
	public int ritualItems = 0;
	public bool RitualComplete = false;

	public static event Action<Ritual> OnRitualItemPlaced;
	public static Action OnGameEnd;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
		// Optionally persist across scenes:
		// DontDestroyOnLoad(gameObject);
	}

	private void Start()
	{
		for (int i = 0; i < ritualSpots.Length; i++)
		{
			ritualSpots[i].id = i;
			ritualSpotDictionary[ritualSpots[i].id] = false;
		}
	}

	public void AddRitualPiece(int id)
	{
		if (ritualSpotDictionary[id] == true)
			return;

		ritualSpotDictionary[id] = true;
		ritualItems++;

		OnRitualItemPlaced?.Invoke(this);

		if (IsRitualComplete())
		{
			CompleteRitual();
		}
	}

	public void RemoveRitualPiece(int id)
	{
		if (ritualSpotDictionary[id] == false)
			return;

		ritualSpotDictionary[id] = false;
		ritualItems--;
	}

	private bool IsRitualComplete()
	{
		return ritualSpotDictionary.Values.All(value => value);
	}

	private void CompleteRitual()
	{
		RitualComplete = true;
		Debug.Log("The ritual is complete!");
	}
}
