using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{

    public enum DropZoneType
    {
        Interpreter,
        Delete,
        Container
    }

    public DropZoneType dropZoneType;

    private HumanoidManager _humanoidManager;

    private void Start() => _humanoidManager = FindObjectOfType<HumanoidManager>();

    /// <summary>
    /// Loop through the tags in the interpreter panel and then send the list of tags to the UpdateHumanoid method.
    /// This method will be called by one of this Objects children (Draggable)
    /// </summary>
    public void RenderHumanoid()
    {
        var parts = GetParts();
        if (parts != null) _humanoidManager.UpdateHumanoid(GetParts());
    }

    private List<string> GetParts()
    {
        var parts = new List<string>();
        if (dropZoneType != DropZoneType.Interpreter) return null;
        for (var i = 0; i < transform.childCount; i++)
        {
            var part = transform.GetChild(i);
            if (part.gameObject.activeSelf)
            {
                parts.Add(part.name);
            }
        }
        return parts;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
        {
            return;
        }

        var d = eventData.pointerDrag.GetComponent<Draggable>();
        if (d != null)
        {
            d.placeholderParent = transform;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
        {
            return;
        }
        var d = eventData.pointerDrag.GetComponent<Draggable>();
        if (d != null && d.placeholderParent == transform)
        {
            d.placeholderParent = d.parentToReturnTo;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        var d = eventData.pointerDrag.GetComponent<Draggable>();
        if (d == null) return;
        switch (dropZoneType)
        {
            case DropZoneType.Interpreter:
                d.parentToReturnTo = transform;
                d.isDeleted = false;
                break;
            case DropZoneType.Delete:
                d.parentToReturnTo = d.originalParent;
                d.isDeleted = true;
                d.DestroyNewTag();
                ResetAllPoses();
                break;
            case DropZoneType.Container:
                ResetAllPoses();
                // Here goes our custom soring of tags in the container panel
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static void TabulateTags(GameObject interpreter)
    {
        var depth = 0;
        var visited = new List<string>();
        for (var i = 0; i < interpreter.transform.childCount; i++)
        {
            var d = interpreter.transform.GetChild(i);
            if (d == null || !TagDataModel.TagMap.ContainsKey(d.name))
            {
                continue;
            }
            if (d.name[0] == '/' && visited.Contains(d.name.Substring(1)))
            {
                depth--;
            }
            ResetPos(d);
            var tagColor = d.transform.GetChild(0).GetComponent<RectTransform>().localPosition;
            d.transform.GetChild(0).GetComponent<RectTransform>().localPosition = new Vector3(tagColor.x + 50 * depth, tagColor.y, tagColor.z);
            var tagText = d.transform.GetChild(1).GetComponent<RectTransform>().localPosition;
            d.transform.GetChild(1).GetComponent<RectTransform>().localPosition = new Vector3(tagText.x + 50 * depth, tagText.y, tagText.z);
            if (!TagDataModel.TagMap[d.name].HasClosing) continue;
            visited.Add(d.name);
            depth++;
        }
    }

    private void ResetAllPoses()
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            ResetPos(child);
        }
    }

    private static void ResetPos(Component component)
    {
        if (component.transform.childCount < 2) return;
        component.transform.GetChild(0).GetComponent<RectTransform>().localPosition = new Vector3(-37.41f, 0f);
        component.transform.GetChild(1).GetComponent<RectTransform>().localPosition = new Vector3(5f, 0f);
    }
}
