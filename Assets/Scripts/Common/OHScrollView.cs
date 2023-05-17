using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class OHScrollView : MonoBehaviour
{
    [SerializeField]
    private RectTransform _itemPrototype;
    [SerializeField]
    private RectTransform _scrollViewTransform;
    [SerializeField]
    private RectTransform _scrollContent;
    [SerializeField]
    private ScrollRect _scrollRect;
    [SerializeField, Range(0, 30)]
    private int _instantateItemCount = 9;
    [SerializeField]
    private int _lintItemCount = 0;
    [SerializeField]
    private int _gapSizex = 0;
    [SerializeField]
    private int _gapSizey = 0;

    [SerializeField]
    private E_SCROLLDIRECT _direction;

    public OnItemPositionChange _onUpdateItem = new OnItemPositionChange();

    [System.NonSerialized]
    public LinkedList<RectTransform> _itemList = new LinkedList<RectTransform>();

    protected float _diffPreFramePosition = 0;

    protected int _currentItemNo = 0;

    public enum E_SCROLLDIRECT
    {
        VERTICAL,
        HORIZONTAL,
    }

    // cache component

    private RectTransform _rectTransform;
    protected RectTransform rectTransform
    {
        get
        {
            if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
            return _rectTransform;
        }
    }

    private float _anchoredPosition
    {
        get
        {
            return _direction == E_SCROLLDIRECT.VERTICAL ? -rectTransform.anchoredPosition.y : rectTransform.anchoredPosition.x;
        }
    }
    public int lineItemCount
    {
        get
        {
            if (_direction == E_SCROLLDIRECT.VERTICAL)
            {
                return (int)(_scrollViewTransform.rect.width / (_gapSizex + _itemPrototype.sizeDelta.x));
            }
            else
            {
                return (int)(_scrollViewTransform.rect.height / (_gapSizey + _itemPrototype.sizeDelta.y));
            }
        }
    }

    private float _itemScale = -1;
    public float itemScale
    {
        get
        {
            if (_itemPrototype != null && _itemScale == -1)
            {
                _itemScale = _direction == E_SCROLLDIRECT.VERTICAL ? _itemPrototype.sizeDelta.y : _itemPrototype.sizeDelta.x;
            }
            return _itemScale;
        }
    }

    protected void Start()
    {
        var controllers = GetComponents<MonoBehaviour>()
                .Where(item => item is IInfiniteScrollSetup)
                .Select(item => item as IInfiniteScrollSetup)
                .ToList();

        var scrollRect = GetComponentInParent<ScrollRect>();
        scrollRect.horizontal = _direction == E_SCROLLDIRECT.HORIZONTAL;
        scrollRect.vertical = _direction == E_SCROLLDIRECT.VERTICAL;
        scrollRect.content = rectTransform;
        _itemPrototype.gameObject.SetActive(false);
        Vector2 pivotvecter = Vector2.zero;

        GridLayoutGroup gridGroup = _scrollContent.GetComponent<GridLayoutGroup>();
        gridGroup.cellSize = new Vector2(_itemPrototype.sizeDelta.x, _itemPrototype.sizeDelta.y);
        gridGroup.spacing = new Vector2(_gapSizex, _gapSizey);

        switch (gridGroup.startCorner)
        {
            case GridLayoutGroup.Corner.UpperLeft:
                pivotvecter.x = 0;
                pivotvecter.y = 1;
                break;
            case GridLayoutGroup.Corner.UpperRight:
                pivotvecter.x = 1;
                pivotvecter.y = 1;
                break;
            case GridLayoutGroup.Corner.LowerLeft:
                pivotvecter.x = 0;
                pivotvecter.y = 0;
                break;
            case GridLayoutGroup.Corner.LowerRight:
                pivotvecter.x = 1;
                pivotvecter.y = 0;
                break;
        }

        _itemPrototype.pivot = pivotvecter;
        for (int i = 0; i < _instantateItemCount; i++)
        {
            var item = GameObject.Instantiate(_itemPrototype) as RectTransform;
            item.SetParent(_scrollContent, false);
            item.name = i.ToString();
            item.anchoredPosition = _direction == E_SCROLLDIRECT.VERTICAL ? new Vector2((itemScale+ _gapSizex) * (i% lineItemCount), (-itemScale-_gapSizey) * (i / lineItemCount)) :
                new Vector2((itemScale+_gapSizex) * (i/ lineItemCount), (-itemScale - _gapSizey) *( i % lineItemCount));
            _itemList.AddLast(item);

            item.gameObject.SetActive(true);

            foreach (var controller in controllers)
            {
                controller.OnUpdateItem(i, item.gameObject);
            }
        }
        foreach (var controller in controllers)
        {
            controller.OnPostSetupItems();
        }
    }

    void Update()
    {
        if (_itemList.First == null)
        {
            return;
        }
        if(_direction == E_SCROLLDIRECT.VERTICAL)
        {
            if (rectTransform.anchoredPosition.y < 0)
            {
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 0);
                return;
            }
        }
        else
        {
            if (rectTransform.anchoredPosition.x < 0)
            {
                rectTransform.anchoredPosition = new Vector2(0, rectTransform.anchoredPosition.y);
                return;
            }
        }
        int gapsize = _direction == E_SCROLLDIRECT.VERTICAL ? _gapSizey : _gapSizex;
        while (_anchoredPosition - _diffPreFramePosition < -(itemScale+ gapsize) * 2)
        {
            var item = _itemList.First.Value;
            _itemList.RemoveFirst();
            _itemList.AddLast(item);
            var pos = (itemScale+ gapsize) * (_instantateItemCount/ lineItemCount);
            item.anchoredPosition = (_direction == E_SCROLLDIRECT.VERTICAL) ? new Vector2((itemScale + _gapSizex) *((_currentItemNo + _instantateItemCount) % lineItemCount), -pos+ _diffPreFramePosition) : 
                new Vector2(pos+ _diffPreFramePosition, (-itemScale - _gapSizey) * ((_currentItemNo + _instantateItemCount) % lineItemCount));

            _onUpdateItem.Invoke(_currentItemNo + _instantateItemCount, item.gameObject);

            item.gameObject.transform.name = (_currentItemNo + _instantateItemCount).ToString();
            _currentItemNo++;

            if ((_currentItemNo + _instantateItemCount) % lineItemCount  == 0)
            {
                if (_direction == E_SCROLLDIRECT.VERTICAL)
                {
                    _diffPreFramePosition -= itemScale + gapsize;
                }
                else
                {
                    _diffPreFramePosition += itemScale + gapsize;
                }
            }
        }

        while (_anchoredPosition - _diffPreFramePosition > 0)
        {
            if (_currentItemNo <= 0)
            {
                _scrollContent.anchoredPosition = new Vector3(0, 0, 0);
                break;
            }
            _currentItemNo--;
            if (_currentItemNo% lineItemCount == 0)
            {
                _diffPreFramePosition += itemScale + gapsize;
            }

            var item = _itemList.Last.Value;
            _itemList.RemoveLast();
            _itemList.AddFirst(item);

            var pos = (itemScale + gapsize) * (_currentItemNo/ lineItemCount);
            item.anchoredPosition = (_direction == E_SCROLLDIRECT.VERTICAL) ? new Vector2((itemScale+_gapSizex) *Mathf.Abs((_currentItemNo % lineItemCount)), -pos) : new Vector2(pos, (-itemScale-_gapSizey) * Mathf.Abs((_currentItemNo % lineItemCount)));
            _onUpdateItem.Invoke(_currentItemNo, item.gameObject);
            item.gameObject.transform.name = _currentItemNo.ToString();
        }
    }
    public interface IInfiniteScrollSetup
    {
        void OnPostSetupItems();
        void OnUpdateItem(int itemCount, GameObject obj);
    }
    [System.Serializable]
    public class OnItemPositionChange : UnityEngine.Events.UnityEvent<int, GameObject> { }
}

