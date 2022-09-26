using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(FSM))]
public class FSMCustomEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		FSM fsm = target as FSM;		
		
		EditorGUILayout.LabelField("FSM Name", fsm.fsmName);
		
		foreach(FSM.FSMState stateIterator in fsm.GetStates())
		{
			GUIStyle style = new GUIStyle("button");
			
			if(stateIterator == fsm.GetCurrentState())
			{
				style.fontSize = 15;
				style.normal.textColor = Color.green;
			}
			else if(stateIterator == fsm.GetPreviousState())
			{
				style.fontSize = 15;
				style.normal.textColor = Color.yellow;
			}
			
			GUILayout.Button(stateIterator.ToString(),style);
		}
		
		if (GUI.changed)
		{
			EditorUtility.SetDirty(fsm);
		}
	}
}
