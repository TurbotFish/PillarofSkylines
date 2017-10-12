using UnityEngine;
using System.Collections;

public class InspectorButtonsTest : MonoBehaviour
{
    [TestButton("Generate world", "DoProcGen", isActiveInEditor = false)]
    [TestButton("Clear world", "ClearWorld", 2, isActiveInEditor = false)]
    [ProgressBar(hideWhenZero = true, label = "procGenFeedback")]
    public float procgenProgress = -1;

    [HideInInspector]
    public string procGenFeedback;

    // Silly little enumerator to test progress bars~
    IEnumerator DoProcGen()
    {
        // lets pretend we have some code here that procedurally generates a map
        procGenFeedback = "Initilizing";
        procgenProgress = 0.01f;
        yield return new WaitForSeconds(0.25f);
        procGenFeedback = "Seeding terrain";
        procgenProgress = 0.2f;
        yield return new WaitForSeconds(0.25f);
        procGenFeedback = "Plotting cities";
        procgenProgress = 0.4f;
        yield return new WaitForSeconds(0.25f);
        procGenFeedback = "Drawing roads";
        procgenProgress = 0.6f;
        yield return new WaitForSeconds(0.25f);
        procGenFeedback = "Reticulating splines";
        procgenProgress = 0.8f;
        yield return new WaitForSeconds(0.25f);
        procGenFeedback = "Finalizing";
        procgenProgress = 0.9f;
        yield return new WaitForSeconds(0.25f);
        procGenFeedback = "DONE in 6 seconds";
        procgenProgress = 1f;
    }

    // resets values
    void ClearWorld()
    {
        procgenProgress = 0;
    }
}
