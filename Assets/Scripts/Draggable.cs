using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	
	public Transform placeholderParent;
	public Transform parentToReturnTo;
	public Transform originalParent;
	public GameObject closingTag;
	public bool isDeleted = true;
	public TagDataModel tagData;
	public int depth;

	private GameObject _placeholder;
	private GameObject _newTag;
	private bool _hasClosing;
	private bool _closed;

	private void Start()
	{
		transform.GetComponentInChildren<Image>().color = TagDataModel.TagMap[tagData.name].Color;
		transform.GetComponentInChildren<TextMeshProUGUI>().SetText(tagData.name);
		_hasClosing = TagDataModel.TagMap[tagData.name].HasClosing;
		originalParent = transform.parent;
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		var components = new[]
		{
			typeof(LayoutElement)
		};
		_placeholder = new GameObject("Place Holder", components);
		_placeholder.GetComponent<RectTransform>().sizeDelta = new Vector2(transform.GetComponent<RectTransform>().sizeDelta.x, transform.GetComponent<RectTransform>().sizeDelta.y);
		_placeholder.transform.SetParent( transform.parent );
		
		var le = _placeholder.GetComponent<LayoutElement>();
		le.preferredWidth = GetComponent<LayoutElement>().preferredWidth;
		le.preferredHeight = GetComponent<LayoutElement>().preferredHeight;
		le.flexibleWidth = 0;
		le.flexibleHeight = 0;
		
		_placeholder.transform.SetSiblingIndex( transform.GetSiblingIndex() );
		parentToReturnTo = transform.parent;
		placeholderParent = parentToReturnTo;
		transform.SetParent( transform.parent.parent );
		GetComponent<CanvasGroup>().blocksRaycasts = false;
	}
	
	public void OnDrag(PointerEventData eventData){
		transform.position = eventData.position;
		if (_placeholder.transform.parent != placeholderParent)
		{
			_placeholder.transform.SetParent(placeholderParent);
		}
		var newSiblingIndex = placeholderParent.childCount;
		for(var i=0; i < placeholderParent.childCount; i++) 
		{
			if (!(transform.position.y > placeholderParent.GetChild(i).position.y))
			{
				continue;
			}
			newSiblingIndex = i;
			if (_placeholder.transform.GetSiblingIndex() < newSiblingIndex)
			{
				newSiblingIndex--;
			}
			break;
		}
		_placeholder.transform.SetSiblingIndex(newSiblingIndex);
	}
	
	public void OnEndDrag(PointerEventData eventData) {
		transform.SetParent( parentToReturnTo );
		transform.SetSiblingIndex( _placeholder.transform.GetSiblingIndex() );
		if (_hasClosing && !_closed && !isDeleted)
		{
			if (_newTag == null)
			{
				_newTag = Instantiate(closingTag, parentToReturnTo, false);
				_newTag.GetComponent<Draggable>().tagData.name = "/" + tagData.name;
				_newTag.GetComponent<CanvasGroup>().blocksRaycasts = true;
				_newTag.name = "/" + tagData.name;
			}
			else
			{
				_newTag.SetActive(true);
			}
			_newTag.transform.SetSiblingIndex(_placeholder.transform.GetSiblingIndex() + 1);
			_closed = true;
		}
		GetComponent<CanvasGroup>().blocksRaycasts = true;
		Destroy(_placeholder);
		SendMessageUpwards(nameof(DropZone.RenderHumanoid));
	}

	public void DestroyNewTag()
	{
		if (_newTag == null) return;
		_newTag.SetActive(false);
		_closed = false;
	}
}
