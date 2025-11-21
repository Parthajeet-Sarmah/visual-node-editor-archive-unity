using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboManager : MonoBehaviour 
{
	[SerializeField]private List<Combo> combos;
	[SerializeField]private float startResetTimerGap = 1.2f;
	private float resetTimerGap = 0f;
	private bool startRecording = false;

	private string currentSequence;

	private void Awake()
	{
		resetTimerGap = startResetTimerGap;
	}

	private Combo FindCombo(string sequence)
	{
		Combo currentCombo = combos.Find(c => c.comboSequence == sequence);

		if (currentCombo != null && currentCombo.unlocked)
			return currentCombo;

		return null;
	}

	private void SetSequence(ref string sequence)
	{
		if(Input.GetMouseButtonDown(0))
		{
			sequence += "L";
		}
		else if(Input.GetMouseButtonDown(1))
		{
			sequence += "R";
		}
		else if(Input.GetKeyDown(KeyCode.E))
		{
			sequence += "E";
		}
		else
		{
			return;
		}

		if (!startRecording)
			startRecording = true;

		resetTimerGap = startResetTimerGap;
	}

	private void ProcessSequence(string sequence)
	{
		Combo currentCombo = FindCombo (sequence);

		if (currentCombo == null)
			return;

		print (currentCombo.comboName);
	}

	private void Update()
	{
		if(resetTimerGap >= 0f)
		{
			SetSequence (ref currentSequence);

			if (!startRecording)
				return;

			resetTimerGap -= Time.deltaTime;
		}
		else if(resetTimerGap <= 0f)
		{
			ProcessSequence (currentSequence);
			currentSequence = "";
			startRecording = false;
			resetTimerGap = startResetTimerGap;
		}
	}
}

[System.Serializable]
public class Combo
{
	public bool unlocked;
	public string comboName;
	public string comboSequence;
}
