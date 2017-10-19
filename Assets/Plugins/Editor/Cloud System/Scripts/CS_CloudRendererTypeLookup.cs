//
// Edelweiss.CloudSystem.CloudRendererTypeLookup.cs:
//   Originally used as direct mapping from the enum to the actual type.
//   The inspector is not anymore using the CloudRendererTypeEnum directly,
//   but CloudRendererEnum and CloudRenderingMethodEnum. It could not be
//   remove as the enum is still used in the clouds directly.
//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using System;
using System.Collections;
using Edelweiss.CloudSystem;
using Edelweiss.CloudSystemEditor;

public class CS_CloudRendererTypeLookup : CloudRendererTypeLookup {
	
	public override Type TypeForCloudSystemRendererEnum (CloudRendererTypeEnum a_CloudSystemRendererEnum) {
		Type l_Result = null;
		
		if (a_CloudSystemRendererEnum == CloudRendererTypeEnum.CustomTintRenderer) {
			l_Result = typeof (CS_CustomTintRenderer);
		} else if (a_CloudSystemRendererEnum == CloudRendererTypeEnum.SimpleCustomTintRenderer) {
			l_Result = typeof (CS_SimpleCustomTintRenderer);
		} else if (a_CloudSystemRendererEnum == CloudRendererTypeEnum.LegacyTintRenderer) {
			l_Result = typeof (CS_UnityParticleSystemTintRenderer);
		} else if (a_CloudSystemRendererEnum == CloudRendererTypeEnum.ShurikenTintRenderer) {
			l_Result = typeof (CS_ShurikenTintRenderer);
			
		} else if (a_CloudSystemRendererEnum == CloudRendererTypeEnum.CustomVerticalColorRenderer) {
			l_Result = typeof (CS_CustomVerticalColorRenderer);
		} else if (a_CloudSystemRendererEnum == CloudRendererTypeEnum.SimpleCustomVerticalColorRenderer) {
			l_Result = typeof (CS_SimpleCustomVerticalColorRenderer);
		} else if (a_CloudSystemRendererEnum == CloudRendererTypeEnum.LegacyVerticalColorRenderer) {
			l_Result = typeof (CS_UnityParticleSystemVerticalColorRenderer);
		} else if (a_CloudSystemRendererEnum == CloudRendererTypeEnum.ShurikenVerticalColorRenderer) {
			l_Result = typeof (CS_ShurikenVerticalColorRenderer);
			
		} else if (a_CloudSystemRendererEnum == CloudRendererTypeEnum.CustomShadingGroupRenderer) {
			l_Result = typeof (CS_CustomShadingGroupRenderer);
		} else if (a_CloudSystemRendererEnum == CloudRendererTypeEnum.SimpleCustomShadingGroupRenderer) {
			l_Result = typeof (CS_SimpleCustomShadingGroupRenderer);
		} else if (a_CloudSystemRendererEnum == CloudRendererTypeEnum.LegacyShadingGroupRenderer) {
			l_Result = typeof (CS_UnityParticleSystemShadingGroupRenderer);
		} else if (a_CloudSystemRendererEnum == CloudRendererTypeEnum.ShurikenShadingGroupRenderer) {
			l_Result = typeof (CS_ShurikenShadingGroupRenderer);
		
		} else if (a_CloudSystemRendererEnum == CloudRendererTypeEnum.CustomVerticalColorWithShadingGroupRenderer) {
			l_Result = typeof (CS_CustomVerticalColorWithShadingGroupRenderer);
		} else if (a_CloudSystemRendererEnum == CloudRendererTypeEnum.SimpleCustomVerticalColorWithShadingGroupRenderer) {
			l_Result = typeof (CS_SimpleCustomVerticalColorWithShadingGroupRenderer);
		} else if (a_CloudSystemRendererEnum == CloudRendererTypeEnum.LegacyVerticalColorWithShadingGroupRenderer) {
			l_Result = typeof (CS_UnityParticleSystemVerticalColorWithShadingGroupRenderer);
		} else if (a_CloudSystemRendererEnum == CloudRendererTypeEnum.ShurikenVerticalColorWithShadingGroupRenderer) {
			l_Result = typeof (CS_ShurikenVerticalColorWithShadingGroupRenderer);
		}
		
		return (l_Result);
	}
}
