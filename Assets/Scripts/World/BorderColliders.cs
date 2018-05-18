#if UNITY_EDITOR
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Game.World {

    [ExecuteInEditMode]
    public class BorderColliders : MonoBehaviour {

        [TestButton("Stop Repetition", "StopRepetition", isActiveInEditor = true, isActiveAtRuntime = false)]
        [TestButton("Repeat Border Colliders", "LaunchRepetition", isActiveInEditor = true, isActiveAtRuntime = false)]
        [ProgressBar(hideWhenZero = true, label = "completionPercent")]
        public float completionPercent = -1;

        
        string parentName = "BorderColliders";

        WorldController world;
        int[] inJob;

        int totalJobs, jobsDone;


        public void LaunchRepetition() {

            // Just in case
            StopAllCoroutines();
            
            EditorApplication.update += ExecuteCoroutine;

            //Launch
            world = FindObjectOfType<WorldController>();
            completionPercent = 0;

            FindAllBorderColliders();
            
            /*
            Region[] allRegions = FindObjectsOfType<Region>();
            DoAllRegions(allRegions);*/
        }
        
        public void StopRepetition() {
            StopAllCoroutines();
            completionPercent = -1;

            EditorApplication.update -= ExecuteCoroutine;
            print("BORDER COLLIDERS REPETITION STOPPED");
        }

        void FindAllBorderColliders()
        {
            EditorStartCoroutine(_FindAllBorderColliders());
        }

        IEnumerator _FindAllBorderColliders() {

            float outsideReach = 100;
            Vector3 boxPos = Vector3.zero;

            Vector3 yBoxReach = new Vector3(world.WorldSize.x + outsideReach, outsideReach, world.WorldSize.z + outsideReach);
            Vector3 zBoxReach = new Vector3(world.WorldSize.x + outsideReach, world.WorldSize.y + outsideReach, outsideReach);

            Transform uniqueParent = CreateContainer(world.transform);

            //Transform topParent = CreateContainer(world.transform, " Top");
            //Transform westParent = CreateContainer(world.transform, " West");
            //Transform eastParent = CreateContainer(world.transform, " East");
            //Transform bottomParent = CreateContainer(world.transform, " Bottom");
            

            //top

            boxPos.y = world.WorldSize.y / 2 + outsideReach;
            Collider[] topBorderColliders = Physics.OverlapBox(boxPos, yBoxReach);
            boxPos.y = 0;

            //west

            boxPos.z = world.WorldSize.z / 2 + outsideReach;
            Collider[] westBorderColliders = Physics.OverlapBox(boxPos, zBoxReach);
            boxPos.z = 0;

            //east

            boxPos.z = -(world.WorldSize.z / 2 + outsideReach);
            Collider[] eastBorderColliders = Physics.OverlapBox(boxPos, zBoxReach);
            boxPos.z = 0;

            //bottom

            boxPos.y = -(world.WorldSize.y / 2 + outsideReach);
            Collider[] bottomBorderColliders = Physics.OverlapBox(boxPos, yBoxReach);
            boxPos.y = 0;


            List<Collider> allColliders = new List<Collider>(topBorderColliders);
            allColliders.AddRange(westBorderColliders);
            allColliders.AddRange(eastBorderColliders);
            allColliders.AddRange(bottomBorderColliders);

            allColliders = allColliders.Distinct().ToList();

            List<List<Collider>> orderedList = new List<List<Collider>>();
            orderedList.Add(new List<Collider>()); // list for non-mesh colliders

            List<Mesh> meshes = new List<Mesh>();
            

            foreach (Collider col in allColliders)  {

                if (col is MeshCollider)
                {
                    Mesh mesh = ((MeshCollider)col).sharedMesh;

                    if (meshes.Contains(mesh)) {
                        orderedList[meshes.IndexOf(mesh) + 1].Add(col);

                    } else {
                        meshes.Add(mesh);

                        while (orderedList.Count < meshes.Count + 1)
                            orderedList.Add(new List<Collider>());

                        orderedList[meshes.Count].Add(col);
                    }

                } else {
                    orderedList[0].Add(col);
                }
            }

            allColliders.Clear();

            int ix = 0;

            foreach(List<Collider> list in orderedList)
            {
                ix++;
                if (ix < meshes.Count)
                    print("Found the mesh " + meshes[ix] + " with " + meshes[ix].triangles.Length + " triangles");

                allColliders.AddRange(list);
            }
            
            jobsDone = 0;
            totalJobs = allColliders.Count;

            Debug.Log("Found " + totalJobs + " colliders to repeat");

            EditorStartCoroutine(_DoAllColliders(allColliders.ToArray(), uniqueParent));
            while (jobsDone < totalJobs)
                yield return null;

            /*
            int jobsGoal = topBorderColliders.Length-1;
            StartCoroutine(_DoAllColliders(topBorderColliders, topParent));
            while(jobsDone < jobsGoal)
                yield return null;

            jobsGoal += westBorderColliders.Length - 1;
            StartCoroutine(_DoAllColliders(westBorderColliders, westParent));
            while (jobsDone < jobsGoal)
                yield return null;

            jobsGoal += eastBorderColliders.Length - 1;
            StartCoroutine(_DoAllColliders(eastBorderColliders, eastParent));
            while (jobsDone < jobsGoal)
                yield return null;
            
            jobsGoal += bottomBorderColliders.Length - 1;
            StartCoroutine(_DoAllColliders(bottomBorderColliders, bottomParent));
            while (jobsDone < jobsGoal)
                yield return null;
            */

            Debug.Log("Border Colliders Repetition DONE!");

            completionPercent = -1;
            EditorApplication.update -= ExecuteCoroutine;
        }
        
        IEnumerator _DoAllColliders(Collider[] colliderArray, Transform parent)
        {
            foreach (Collider col in colliderArray)
            {
                RepeatBorderColliders(col.gameObject, parent);
                jobsDone++;
                completionPercent = jobsDone / (float)totalJobs;
                yield return null;
            }
        }

        public static IEnumerator EditorStartCoroutine(IEnumerator newCorou)
        {
            CoroutineInProgress.Add(newCorou);
            return newCorou;
        }

        private static List<IEnumerator> CoroutineInProgress = new List<IEnumerator>();
        int currentExecute = 0;
        void ExecuteCoroutine()
        {
            if (CoroutineInProgress.Count <= 0)
                return;
            
            currentExecute = (currentExecute + 1) % CoroutineInProgress.Count;

            bool finish = !CoroutineInProgress[currentExecute].MoveNext();

            if (finish)
            {
                CoroutineInProgress.RemoveAt(currentExecute);
            }
        }



        void DoAllRegions(Region[] allRegions) {
            StartCoroutine(_DoAllRegions(allRegions));
        }

        IEnumerator _DoAllRegions(Region[] allRegions)
        {
            int regionID = 0;
            inJob = new int[allRegions.Length];

            foreach (Region targetRegion in allRegions)
            {
                Transform copyParent = CreateContainer(targetRegion.transform);
                
                Debug.Log("# Repeating colliders in " + targetRegion);

                yield return null;


                Undo.RegisterCreatedObjectUndo(copyParent.gameObject, "Repeated " + targetRegion.name);
                Undo.RegisterCompleteObjectUndo(targetRegion, "Repeated " + targetRegion.name);

                RepeatBorderColliders(targetRegion.gameObject, copyParent, regionID);

                // while in job return null

                regionID++;
                yield return null;
            }

            bool[] finishedRegion = new bool[allRegions.Length];
            bool finished = false;

            while (! finished)
            {
                int total = 0;

                for (int i = 0; i < allRegions.Length; i++)
                {
                    if (inJob[i] == 0 && !finishedRegion[i]) {
                            finishedRegion[i] = true;
                            Debug.Log("# DONE Repeating in " + allRegions[i]);
                    }
                    total += inJob[i];
                }

                if (total == 0)
                    finished = true;

                yield return null;
            }
            
            Debug.Log("### DONE REPEATING. ###");
        }


        Transform CreateContainer(Transform region, string addName = "") {

            Transform container = region.Find(parentName + addName);

            if (container)
                DestroyImmediate(container.gameObject);

            container = new GameObject().transform;
            container.name = parentName + addName;
            container.parent = region;
            container.position = Vector3.zero;
            
            return container;
        }


        void RepeatBorderColliders(GameObject target, Transform copyParent, int regionID = 0)
        {
            Collider col = target.GetComponent<Collider>();

            if (col)
            {
                Vector3 minPoint = world.transform.position - world.WorldSize / 2;
                Vector3 maxPoint = world.transform.position + world.WorldSize / 2;

                int minDepasse = (int)CheckPointInBounds(col.bounds.min, minPoint, maxPoint);
                int maxDepasse = (int)CheckPointInBounds(col.bounds.max, minPoint, maxPoint);

                if (minDepasse + maxDepasse != minDepasse * maxDepasse * 2) // si le collider dépasse
                {
                    // FULL OUTSIDE

                    bool fullOutsideZ = false, fullOutsideY = false;

                    if (minDepasse % (int)Depassage.zPositive == 0) // on est complètement en dehors en z positive
                    {
                        fullOutsideZ = true;

                        if (minDepasse % (int)Depassage.yPositive == 0) // on est complètement en dehors en y positive
                        {
                            // on le déplace
                            BuildCopy(col, copyParent, new Vector3(0, -world.WorldSize.y, -world.WorldSize.z), " (COPY Z-Y-)");
                            fullOutsideY = true;
                        }
                        else if (maxDepasse % (int)Depassage.yNegative == 0) // on est complètement en dehors en y negative
                        {
                            // on le déplace
                            BuildCopy(col, copyParent, new Vector3(0, world.WorldSize.y, -world.WorldSize.z), " (COPY Z-Y+)");
                            fullOutsideY = true;
                        }
                        else
                        {
                            // on le déplace
                            BuildCopy(col, copyParent, new Vector3(0, 0, -world.WorldSize.z), " (COPY Z-)");
                        }
                    }
                    else if (maxDepasse % (int)Depassage.zNegative == 0) // on est complètement en dehors en z negative
                    {
                        fullOutsideZ = true;

                        if (minDepasse % (int)Depassage.yPositive == 0) // on est complètement en dehors en y positive
                        {
                            // on le déplace
                            BuildCopy(col, copyParent, new Vector3(0, -world.WorldSize.y, world.WorldSize.z), " (COPY Z+Y-)");
                            fullOutsideY = true;
                        }
                        else if (maxDepasse % (int)Depassage.yNegative == 0) // on est complètement en dehors en y negative
                        {
                            // on le déplace
                            BuildCopy(col, copyParent, new Vector3(0, world.WorldSize.y, world.WorldSize.z), " (COPY Z+Y+)");
                            fullOutsideY = true;
                        }
                        else
                        {
                            // on le déplace
                            BuildCopy(col, copyParent, new Vector3(0, 0, world.WorldSize.z), " (COPY Z+)");
                        }
                    }
                    else
                    {
                        if (minDepasse % (int)Depassage.yPositive == 0) // on est complètement en dehors en y positive
                        {
                            // on le déplace
                            BuildCopy(col, copyParent, new Vector3(0, -world.WorldSize.y, 0), " (COPY Y-)");
                            fullOutsideY = true;
                        }
                        else if (maxDepasse % (int)Depassage.yNegative == 0) // on est complètement en dehors en y negative
                        {
                            // on le déplace
                            BuildCopy(col, copyParent, new Vector3(0, world.WorldSize.y, 0), " (COPY Y+)");
                            fullOutsideY = true;
                        }
                    }

                    // CROSSING

                    if (!fullOutsideZ) // Si on est pas full dehors, check si on cross en Z
                    {
                        if (minDepasse % (int)Depassage.zNegative == 0) // si on dépasse en z neg on fait une copie en z Pos
                            BuildCopy(col, copyParent, new Vector3(0, 0, world.WorldSize.z), " (COPY Z+)");

                        if (maxDepasse % (int)Depassage.zPositive == 0) // si on dépasse en z pos on fait une copie en z Neg
                            BuildCopy(col, copyParent, new Vector3(0, 0, -world.WorldSize.z), " (COPY Z-)");
                    }

                    if (!fullOutsideY) // Si on est pas full dehors, check si on cross en Y
                    {
                        if (minDepasse % (int)Depassage.yNegative == 0) // si on dépasse en y neg on fait une copie en y Pos
                            BuildCopy(col, copyParent, new Vector3(0, world.WorldSize.y, 0), " (COPY Y+)");

                        if (maxDepasse % (int)Depassage.yPositive == 0) // si on dépasse en y pos on fait une copie en y Neg
                            BuildCopy(col, copyParent, new Vector3(0, -world.WorldSize.y, 0), " (COPY Y-)");


                        if (!fullOutsideZ) // Et maintenant les double cross
                        {
                            if (minDepasse % (int)Depassage.zNegative == 0 && minDepasse % (int)Depassage.yNegative == 0)
                                BuildCopy(col, copyParent, new Vector3(0, world.WorldSize.y, world.WorldSize.z), " (COPY Z+Y+)");

                            if (minDepasse % (int)Depassage.zNegative == 0 && maxDepasse % (int)Depassage.yPositive == 0)
                                BuildCopy(col, copyParent, new Vector3(0, -world.WorldSize.y, world.WorldSize.z), " (COPY Z+Y-)");

                            if (maxDepasse % (int)Depassage.zPositive == 0 && maxDepasse % (int)Depassage.yPositive == 0)
                                BuildCopy(col, copyParent, new Vector3(0, -world.WorldSize.y, -world.WorldSize.z), " (COPY Z-Y-)");

                            if (maxDepasse % (int)Depassage.zPositive == 0 && minDepasse % (int)Depassage.yNegative == 0)
                                BuildCopy(col, copyParent, new Vector3(0, world.WorldSize.y, -world.WorldSize.z), " (COPY Z-Y+)");

                        } // fin doublecross

                    } // fin outsideY

                    // MOVING

                    if (fullOutsideY || fullOutsideZ)
                    { // s'il est full outside, on désactive l'objet. On le fait à la fin pour pas que les copies soient désactivées
                        Undo.RegisterCompleteObjectUndo(target, "Repeated " + target.name);
                        target.SetActive(false);
                        target.name += " (MOVED)";
                    }

                } // fin collider dépasse
            } // fin collider

            
            //StartCoroutine(_DoForChildren(target.transform, copyParent, regionID));
        }

        IEnumerator _DoForChildren(Transform target, Transform copyParent, int regionID) {
            inJob[regionID]++;
            int count = 0;

            foreach (Transform child in target) {
                count++;
                RepeatBorderColliders(child.gameObject, copyParent, regionID);
                
                /*if (count == objectInterval) {
                    count = 0;*/
                    yield return null;
                //}
            }
            inJob[regionID]--;
        }


        static void BuildCopy(Collider original, Transform parent, Vector3 offsetPos, string addName)
        {
            GameObject copy = new GameObject();
            copy.transform.parent = parent;

            ComponentUtility.CopyComponent(original);
            ComponentUtility.PasteComponentAsNew(copy);
            
            Transform t = original.transform;

            copy.transform.position = t.position + offsetPos;
            copy.transform.rotation = t.rotation;
            copy.transform.localScale = t.lossyScale;

            copy.name = original.name + addName;
        }


        static Depassage CheckPointInBounds(Vector3 point, Vector3 min, Vector3 max)
        {
            int result = 1;

            if (point.z < min.z)
                result *= (int)Depassage.zNegative;
            else if (point.z > max.z)
                result *= (int)Depassage.zPositive;

            if (point.y < min.y)
                result *= (int)Depassage.yNegative;
            else if (point.y > max.y)
                result *= (int)Depassage.yPositive;

            return (Depassage)result;
        }

        enum Depassage
        {
            None = 1,
            zPositive = 2,
            zNegative = 3,
            yPositive = 5,
            yNegative = 7,
        }


    }
}
#endif