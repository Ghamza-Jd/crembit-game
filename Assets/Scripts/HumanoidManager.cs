using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class HumanoidManager : MonoBehaviour
{
    public GameObject interpreter;
    public TextMeshProUGUI score;

    private readonly HumanPart _humanPart = new HumanPart();

    private void Start()
    {
        FillHumanParts(gameObject, _humanPart);
        DeactivateAll(gameObject, _humanPart);
        gameObject.SetActive(true);
    }

    /// <summary>
    /// This function fills the HumanParts recursively from the hierarchy
    /// </summary>
    /// <param name="gameObject">Root GameObject</param>
    /// <param name="humanPart">Root HumanPart</param>
    private static void FillHumanParts(GameObject gameObject, HumanPart humanPart)
    {
        if (gameObject == null) return;
        if (humanPart.name == "humanoid" || humanPart.name == "ufo") return;
        humanPart.name = gameObject.name;
        humanPart.part = gameObject;
        humanPart.isLeaf = TagDataModel.TagMap.ContainsKey(humanPart.name) && TagDataModel.TagMap[humanPart.name].IsLeaf;
        for (var i = 0; i < gameObject.transform.childCount; i++)
        {
            var hp = new HumanPart();
            humanPart.humanParts.Add(hp);
            FillHumanParts(gameObject.transform.GetChild(i).gameObject, hp);
        }
    }

    /// <summary>
    /// Deactivate all of the human parts recursively
    /// </summary>
    /// <param name="gameObject">Root GameObject</param>
    /// <param name="humanPart">Root HumanPart</param>
    private static void DeactivateAll(GameObject gameObject, HumanPart humanPart)
    {
        if (gameObject == null) return;
        for (var i = 0; i < gameObject.transform.childCount; i++)
        {
            if (!humanPart.isLeaf)
                DeactivateAll(gameObject.transform.GetChild(i).gameObject, humanPart.humanParts[i]);
        }
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Delete the wrong tags, e.g: if the "hand" is between the "head" and "/head", "hand" will be deleted
    /// </summary>
    /// <param name="parts">tag names</param>
    /// <returns>Filtered Tag names</returns>
    private static List<string> FilterParts(IList<string> parts)
    {
        var result = new List<string>();
        foreach (var part in parts)
        {
            if (BetweenHisParents(parts, part))
                result.Add(part + ";'");
            else if (BetweenHisInvertedParents(parts, part))
                result.Add(part + ";\"");
        }
        return result;
    }

    /// <summary>
    /// Checks if a given tag is after the opening and before the closing of his parents
    /// </summary>
    /// <param name="parts"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    private static bool BetweenHisParents(IList<string> parts, string part)
    {
        var parent = string.Empty;
        if (TagDataModel.TagMap.ContainsKey(part))
            parent = TagDataModel.TagMap[part].ParentTag;
        if (parent == string.Empty) return !HasParent(parts, part);
        var opening = parts.IndexOf(parent);
        var closing = parts.IndexOf("/" + parent);
        var partIndex = parts.IndexOf(part);
        if (!TagDataModel.TagMap[part].HasClosing)
            return partIndex > opening && partIndex < closing;
        var partClosing = "/" + part;
        var partClosingIndex = parts.IndexOf(partClosing);
        return partIndex > opening && partIndex < closing && partClosingIndex > opening && partClosingIndex < closing;
    }

    /// <summary>
    /// Check if a given tag is after the closing and before the opening of his parents
    /// </summary>
    /// <param name="parts"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    private static bool BetweenHisInvertedParents(IList<string> parts, string part)
    {
        var parent = string.Empty;
        if (TagDataModel.TagMap.ContainsKey(part))
            parent = TagDataModel.TagMap[part].ParentTag;
        if (parent == string.Empty) return !HasParent(parts, part);
        var opening = parts.IndexOf(parent);
        var closing = parts.IndexOf("/" + parent);
        var partIndex = parts.IndexOf(part);
        if (!TagDataModel.TagMap[part].HasClosing)
            return partIndex > opening && partIndex < closing || partIndex > closing && partIndex < opening;
        var partClosing = "/" + part;
        var partClosingIndex = parts.IndexOf(partClosing);
        return partIndex > closing && partIndex < opening && partClosingIndex > closing && partClosingIndex < opening;
    }

    /// <summary>
    /// Checks if the parents of a given tag are present
    /// </summary>
    /// <param name="parts"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    private static bool HasParent(IList<string> parts, string part)
    {
        var partPos = parts.IndexOf(part);
        return (
            from p in parts
            where !TagDataModel.TagMap.ContainsKey(p) || TagDataModel.TagMap[p].HasClosing
            let opening = parts.IndexOf(p)
            let closing = parts.IndexOf("/" + p)
            where partPos > opening && partPos < closing
            select opening
        ).Any();
    }

    /// <summary>
    /// Traverse recursively through the humanPart tree and check if all parts are their, used to make a win condition
    /// in case we added levels and tutorials in the game
    /// </summary>
    /// <param name="gameObject">Root GameObject</param>
    /// <param name="humanPart">Root HumanoidPart</param>
    /// <returns></returns>
    private static bool ActiveTree(GameObject gameObject, HumanPart humanPart)
    {
        if (gameObject == null) return true;
        var res = true;
        for (var i = 0; i < gameObject.transform.childCount; i++)
        {
            if (!humanPart.isLeaf)
                res &= ActiveTree(gameObject.transform.GetChild(i).gameObject, humanPart.humanParts[i]);
        }

        return res & gameObject.activeSelf;
    }

    /// <summary>
    /// Search recursively for a specific tag in the tree and activate it
    /// </summary>
    /// <param name="partsName">Filtered list of tags</param>
    /// <param name="gameObject">Root GameObject</param>
    /// <param name="humanPart">Root HumanPart</param>
    private static void TreeSearchAndActivate(ICollection<string> partsName, GameObject gameObject, HumanPart humanPart)
    {
        if (gameObject == null) return;
        var enumerable = partsName.ToList();
        var localPartsName = enumerable.Select(t => t.Split(';')[0]).ToList();
        if (localPartsName.Contains(humanPart.name) || humanPart.name.Contains("humanoid") || humanPart.name.Contains("ufo"))
        {
            if (partsName.Contains(humanPart.name + ";'"))
            {
                humanPart.part.transform.localScale = new Vector2(humanPart.part.transform.localScale.x,
                    Mathf.Abs(humanPart.part.transform.localScale.y));
            }
            else if (partsName.Contains(humanPart.name + ";\""))
            {
                humanPart.part.transform.localScale = new Vector2(humanPart.part.transform.localScale.x,
                    -Mathf.Abs(humanPart.part.transform.localScale.y));
            }
            humanPart.part.SetActive(true);
        }
        else
        {
            humanPart.part.SetActive(false);
        }
        for (var i = 0; i < gameObject.transform.childCount; i++)
        {
            if (!humanPart.isLeaf)
            {
                TreeSearchAndActivate(enumerable, gameObject.transform.GetChild(i).gameObject, humanPart.humanParts[i]);
            }
        }
    }

    public int GetScore(IList<string> parts)
    {
        PlayerData.score = parts.Count * 5;
        return (parts == null ? 0 : parts.Count * 5);
    }

    /// <summary>
    /// Updates the Humanoid.
    /// In fact, its a method that calls the static function that responsible for the humanoid update.
    /// </summary>
    /// <param name="parts">List of parts</param>
    public void UpdateHumanoid(IList<string> parts)
    {
        TreeSearchAndActivate(FilterParts(parts), gameObject, _humanPart);
        DropZone.TabulateTags(interpreter);
        score.text = GetScore(parts) + "";
        string shape = "";
        foreach (string s in parts) shape += s;
        PlayerData.shape = shape;
    }

    public Boolean isGameOver()
    {
        return ActiveTree(gameObject, _humanPart);
    }

    [Serializable]
    public class HumanPart
    {
        public string name;
        /// <summary>
        /// If this part is a leaf or not
        /// this boolean is used to create abstract human parts
        /// e.g: if we want to make a hand with 5 fingers as 1 human part
        ///     in the early levels we set is boolean to true,
        ///     otherwise if fingers are nested inside the hand we make set is bool to false.
        /// </summary>
        public bool isLeaf;
        public GameObject part;
        public List<HumanPart> humanParts;

        public HumanPart() => humanParts = new List<HumanPart>();
    }
}
