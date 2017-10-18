//
// CS_CloudEditor.cs: Wrapper for CloudEditor.
//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using UnityEditor;
using System.Collections;
using Edelweiss.CloudSystem;
using Edelweiss.CloudSystemEditor;

[CustomEditor (typeof (CS_Cloud))]
public class CS_CloudEditor : CloudEditor <CS_Cloud, CS_ParticleData, CS_CreatorData> {
	
	public override CloudRendererTypeLookup CloudRendererTypeLookup {
		get {
			if (m_CloudRendererTypeLookup == null) {
				m_CloudRendererTypeLookup = new CS_CloudRendererTypeLookup ();
			}
			return (m_CloudRendererTypeLookup);
		}
	}
	
	protected override void OnSceneGUI () {
		base.OnSceneGUI ();
	}
}
