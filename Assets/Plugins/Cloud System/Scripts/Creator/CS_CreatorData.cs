//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011-2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using System.Collections;
using Edelweiss.CloudSystem;

/// <summary>
/// Data only needed by the cloud creator. It has no direct runtime impact.
/// </summary>
[System.Serializable]
public class CS_CreatorData : CreatorData <CS_Cloud, CS_ParticleData, CS_CreatorData> {
}
