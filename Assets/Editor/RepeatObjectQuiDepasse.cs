using UnityEngine;
using UnityEditor;

public class RepeatObjectQuiDepasse : EditorWindow
{
    static Game.World.WorldController world;

    [MenuItem("GameObject/Repeat Object", false, 11)]
    public static void RepeatObject(MenuCommand menuCommand)
    {
        world = FindObjectOfType<Game.World.WorldController>();
        GameObject target = (GameObject)menuCommand.context;

        Transform copyParent = new GameObject().transform;
        copyParent.name = target.name + " REPETITIONS";
        copyParent.parent = target.transform.parent;
        copyParent.position = Vector3.zero;

        Undo.RegisterCreatedObjectUndo(copyParent.gameObject, "Repeated " + target.name);
        Undo.RegisterCompleteObjectUndo(target, "Repeated " + target.name);

        RepeatChildrenIfTheyDepassent(target, copyParent);

        if (copyParent.childCount == 0)
            DestroyImmediate(copyParent.gameObject);

    }

    static void RepeatChildrenIfTheyDepassent(GameObject target, Transform copyParent)
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

                Transform t = target.transform;
                bool fullOutsideZ = false, fullOutsideY = false;

                if (minDepasse % (int)Depassage.zPositive == 0) // on est complètement en dehors en z positive
                {
                    fullOutsideZ = true;

                    if (minDepasse % (int)Depassage.yPositive == 0) // on est complètement en dehors en y positive
                    {
                        // on le déplace
                        BuildCopy(target, copyParent, new Vector3(0, -world.WorldSize.y, -world.WorldSize.z), " (COPY Z-Y-)");
                        fullOutsideY = true;
                    }
                    else if (maxDepasse % (int)Depassage.yNegative == 0) // on est complètement en dehors en y negative
                    {
                        // on le déplace
                        BuildCopy(target, copyParent, new Vector3(0, world.WorldSize.y, -world.WorldSize.z), " (COPY Z-Y+)");
                        fullOutsideY = true;
                    }
                    else
                    {
                        // on le déplace
                        BuildCopy(target, copyParent, new Vector3(0, 0, -world.WorldSize.z), " (COPY Z-)");
                    }
                }
                else if (maxDepasse % (int)Depassage.zNegative == 0) // on est complètement en dehors en z negative
                {
                    fullOutsideZ = true;

                    if (minDepasse % (int)Depassage.yPositive == 0) // on est complètement en dehors en y positive
                    {
                        // on le déplace
                        BuildCopy(target, copyParent, new Vector3(0, -world.WorldSize.y, world.WorldSize.z), " (COPY Z+Y-)");
                        fullOutsideY = true;
                    }
                    else if (maxDepasse % (int)Depassage.yNegative == 0) // on est complètement en dehors en y negative
                    {
                        // on le déplace
                        BuildCopy(target, copyParent, new Vector3(0, world.WorldSize.y, world.WorldSize.z), " (COPY Z+Y+)");
                        fullOutsideY = true;
                    }
                    else
                    {
                        // on le déplace
                        BuildCopy(target, copyParent, new Vector3(0, 0, world.WorldSize.z), " (COPY Z+)");
                    }
                }
                else
                {
                    if (minDepasse % (int)Depassage.yPositive == 0) // on est complètement en dehors en y positive
                    {
                        // on le déplace
                        BuildCopy(target, copyParent, new Vector3(0, -world.WorldSize.y, 0), " (COPY Y-)");
                        fullOutsideY = true;
                    }
                    else if (maxDepasse % (int)Depassage.yNegative == 0) // on est complètement en dehors en y negative
                    {
                        // on le déplace
                        BuildCopy(target, copyParent, new Vector3(0, world.WorldSize.y, 0), " (COPY Y+)");
                        fullOutsideY = true;
                    }
                }

                // CROSSING

                if (!fullOutsideZ) // Si on est pas full dehors, check si on cross en Z
                {
                    if (minDepasse % (int)Depassage.zNegative == 0) // si on dépasse en z neg on fait une copie en z Pos
                        BuildCopy(target, copyParent, new Vector3(0, 0, world.WorldSize.z), " (COPY Z+)");

                    if (maxDepasse % (int)Depassage.zPositive == 0) // si on dépasse en z pos on fait une copie en z Neg
                        BuildCopy(target, copyParent, new Vector3(0, 0, -world.WorldSize.z), " (COPY Z-)");
                }

                if (!fullOutsideY) // Si on est pas full dehors, check si on cross en Y
                {
                    if (minDepasse % (int)Depassage.yNegative == 0) // si on dépasse en y neg on fait une copie en y Pos
                        BuildCopy(target, copyParent, new Vector3(0, world.WorldSize.y, 0), " (COPY Y+)");

                    if (maxDepasse % (int)Depassage.yPositive == 0) // si on dépasse en y pos on fait une copie en y Neg
                        BuildCopy(target, copyParent, new Vector3(0, -world.WorldSize.y, 0), " (COPY Y-)");


                    if (!fullOutsideZ) // Et maintenant les double cross
                    {
                        if (minDepasse % (int)Depassage.zNegative == 0 && minDepasse % (int)Depassage.yNegative == 0)
                            BuildCopy(target, copyParent, new Vector3(0, world.WorldSize.y, world.WorldSize.z), " (COPY Z+Y+)");

                        if (minDepasse % (int)Depassage.zNegative == 0 && maxDepasse % (int)Depassage.yPositive == 0)
                            BuildCopy(target, copyParent, new Vector3(0, -world.WorldSize.y, world.WorldSize.z), " (COPY Z+Y-)");

                        if (maxDepasse % (int)Depassage.zPositive == 0 && maxDepasse % (int)Depassage.yPositive == 0)
                            BuildCopy(target, copyParent, new Vector3(0, -world.WorldSize.y, -world.WorldSize.z), " (COPY Z-Y-)");

                        if (maxDepasse % (int)Depassage.zPositive == 0 && minDepasse % (int)Depassage.yNegative == 0)
                            BuildCopy(target, copyParent, new Vector3(0, world.WorldSize.y, -world.WorldSize.z), " (COPY Z-Y+)");

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

        foreach (Transform child in target.transform)
            RepeatChildrenIfTheyDepassent(child.gameObject, copyParent);
    }

    static void BuildCopy(GameObject original, Transform parent, Vector3 offsetPos, string addName)
    {
        GameObject copy = Instantiate(original, parent);
        Transform t = original.transform;

        copy.transform.position = t.position + offsetPos;
        copy.transform.rotation = t.rotation;
        copy.transform.localScale = t.lossyScale;

        //copy.name += addName;
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
