using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIEventHandlerBase : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IDropHandler, IScrollHandler, IUpdateSelectedHandler, IDeselectHandler, IMoveHandler, ISubmitHandler, ICancelHandler
{
    //
    // Static Fields
    //
    public static Action<GameObject> onClickNewbie;

    public static Action<GameObject, string> onClickSound;

    //
    // Fields
    //
    public Action<GameObject, PointerEventData> onPointerEnter;

    public float m_lastInvokeTime;

    public bool m_isPress;

    public float m_interval;

    public string soundId;

    public Action<GameObject, BaseEventData> onCancel;

    public Action<GameObject, BaseEventData> onSubmit;

    public Action<GameObject, AxisEventData> onMove;

    public Action<GameObject, BaseEventData> onDeselect;

    public GameObject m_currentBePressButton;

    public Action<GameObject, PointerEventData> onScroll;

    public Action<GameObject, PointerEventData> onDrop;

    public Action<GameObject, PointerEventData> onEndDrag;

    public Action<GameObject, PointerEventData> onDrag;

    public Action<GameObject, PointerEventData> onBeginDrag;

    public Action<GameObject> onPointerPressClick;

    public Action<GameObject, PointerEventData> onPointerClick;

    public Action<GameObject, PointerEventData> onPointerDoubleClick;

    public Action<GameObject, PointerEventData> onPointerUp;

    public Action<GameObject, PointerEventData> onPointerDown;

    public Action<GameObject, PointerEventData> onPointerExit;

    public Action<GameObject, BaseEventData> onUpdateSelected;


    void Update()
    {
        if (m_isPress)
        {
            if (Time.time - m_lastInvokeTime >= m_interval)
            {
                if (onPointerPressClick != null)
                {
                    onPointerPressClick(m_currentBePressButton);
                }
                m_lastInvokeTime = Time.time;
            }
        }
    }
    //
    // Static Methods
    //
    public static void AddListener(GameObject target, UIEventType type, Action<GameObject, BaseEventData> func, string soundId = null)
    {
        UIEventHandlerBase handler = target.GetComponent<UIEventHandlerBase>();
        if (handler == null)
        {
            handler = target.AddComponent<UIEventHandlerBase>();
        }
        if (!string.IsNullOrEmpty(soundId))
        {
            handler.soundId = soundId;
        }
        switch (type)
        {
            case UIEventType.ON_POINTER_ENTER:
                handler.onPointerEnter += delegate (GameObject o, PointerEventData evtData)
                  {
                      func(o, evtData);
                  };
                break;
            case UIEventType.ON_POINTER_EXIT:
                handler.onPointerExit += delegate (GameObject o, PointerEventData evtData)
                {
                    func(o, evtData);
                };
                break;
            case UIEventType.ON_POINTER_DOWN:
                handler.onPointerDown += delegate (GameObject o, PointerEventData evtData)
                {
                    func(o, evtData);
                };
                break;
            case UIEventType.ON_POINTER_UP:
                handler.onPointerUp += delegate (GameObject o, PointerEventData evtData)
                {
                    func(o, evtData);
                };
                break;
            case UIEventType.ON_POINTER_CLICK:
                handler.onPointerClick += delegate (GameObject o, PointerEventData evtData)
                {
                    func(o, evtData);
                };
                break;
            case UIEventType.ON_POINTER_DOUBLE_CLICK:
                handler.onPointerDoubleClick += delegate (GameObject o, PointerEventData evtData)
                  {
                      func(o, evtData);
                  };
                break;
            case UIEventType.ON_BEGIN_DRAG:
                handler.onBeginDrag += delegate (GameObject o, PointerEventData evtData)
                {
                    func(o, evtData);
                };
                break;
            case UIEventType.ON_DRAG:
                handler.onDrag += delegate (GameObject o, PointerEventData evtData)
                {
                    func(o, evtData);
                };
                break;
            case UIEventType.ON_END_DRAG:
                handler.onEndDrag += delegate (GameObject o, PointerEventData evtData)
                {
                    func(o, evtData);
                };
                break;
            case UIEventType.ON_SCROLL:
                handler.onScroll += delegate (GameObject o, PointerEventData evtData)
                {
                    func(o, evtData);
                };
                break;
            case UIEventType.ON_UPDATE_SELECTED:
                handler.onUpdateSelected += delegate (GameObject o, BaseEventData evtData)
                {
                    func(o, evtData);
                };
                break;
            case UIEventType.ON_DESELECT:
                handler.onDeselect += delegate (GameObject o, BaseEventData evtData)
                {
                    func(o, evtData);
                };
                break;
            case UIEventType.ON_MOVE:
                handler.onMove += delegate (GameObject o, AxisEventData evtData)
                {
                    func(o, evtData);
                };
                break;
            case UIEventType.ON_SUBMIT:
                handler.onSubmit += delegate (GameObject o, BaseEventData evtData)
                {
                    func(o, evtData);
                };
                break;
            case UIEventType.ON_CANCEL:
                 handler.onCancel += delegate (GameObject o, BaseEventData evtData)
                 {
                     func(o, evtData);
                 };
                break;
            default:
                break;
        }
    }
    public static void AddSingleListener(GameObject target, UIEventType type, Action<GameObject, BaseEventData> func, string soundId = null)
    {
            UIEventHandlerBase handler = target.GetComponent<UIEventHandlerBase>();
            if (handler == null)
            {
                handler = target.AddComponent<UIEventHandlerBase>();
            }
            if (!string.IsNullOrEmpty(soundId))
            {
                handler.soundId = soundId;
            }
            switch (type)
            {
                case UIEventType.ON_POINTER_ENTER:
                    handler.onPointerEnter = delegate (GameObject o, PointerEventData evtData)
                    {
                        func(o, evtData);
                    };
                    break;
                case UIEventType.ON_POINTER_EXIT:
                    handler.onPointerExit = delegate (GameObject o, PointerEventData evtData)
                    {
                        func(o, evtData);
                    };
                    break;
                case UIEventType.ON_POINTER_DOWN:
                    handler.onPointerDown = delegate (GameObject o, PointerEventData evtData)
                    {
                        func(o, evtData);
                    };
                    break;
                case UIEventType.ON_POINTER_UP:
                    handler.onPointerUp = delegate (GameObject o, PointerEventData evtData)
                    {
                        func(o, evtData);
                    };
                    break;
                case UIEventType.ON_POINTER_CLICK:
                    handler.onPointerClick = delegate (GameObject o, PointerEventData evtData)
                    {
                        func(o, evtData);
                    };
                    break;
            case UIEventType.ON_POINTER_DOUBLE_CLICK:
                handler.onPointerDoubleClick = delegate (GameObject o, PointerEventData evtData)
                {
                    func(o, evtData);
                };
                break;
            case UIEventType.ON_BEGIN_DRAG:
                    handler.onBeginDrag = delegate (GameObject o, PointerEventData evtData)
                    {
                        func(o, evtData);
                    };
                    break;
                case UIEventType.ON_DRAG:
                    handler.onDrag = delegate (GameObject o, PointerEventData evtData)
                    {
                        func(o, evtData);
                    };
                    break;
                case UIEventType.ON_END_DRAG:
                    handler.onEndDrag = delegate (GameObject o, PointerEventData evtData)
                    {
                        func(o, evtData);
                    };
                    break;
                case UIEventType.ON_SCROLL:
                    handler.onScroll = delegate (GameObject o, PointerEventData evtData)
                    {
                        func(o, evtData);
                    };
                    break;
                case UIEventType.ON_UPDATE_SELECTED:
                    handler.onUpdateSelected = delegate (GameObject o, BaseEventData evtData)
                    {
                        func(o, evtData);
                    };
                    break;
                case UIEventType.ON_DESELECT:
                    handler.onDeselect = delegate (GameObject o, BaseEventData evtData)
                    {
                        func(o, evtData);
                    };
                    break;
                case UIEventType.ON_MOVE:
                    handler.onMove = delegate (GameObject o, AxisEventData evtData)
                    {
                        func(o, evtData);
                    };
                    break;
                case UIEventType.ON_SUBMIT:
                    handler.onSubmit = delegate (GameObject o, BaseEventData evtData)
                    {
                        func(o, evtData);
                    };
                    break;
                case UIEventType.ON_CANCEL:
                    handler.onCancel = delegate (GameObject o, BaseEventData evtData)
                    {
                        func(o, evtData);
                    };
                    break;
                default:
                    break;
            }
    }

    public static void PlayeSound(GameObject gameObject, string soundId)
    {
        if(onClickSound!=null)
        {
            onClickSound.Invoke(gameObject, soundId);
        }
    }
    //
    // Methods
    //
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (onBeginDrag != null)
        {
            onBeginDrag.Invoke(gameObject, eventData);
        }
    }

    public void OnCancel(BaseEventData eventData)
    {
        if (onCancel != null)
            onCancel.Invoke(gameObject, eventData);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (onDeselect != null)
            onDeselect.Invoke(gameObject, eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (onDrag != null)
        {
            onDrag.Invoke(gameObject, eventData);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (onDrop != null)
            onDrop.Invoke(gameObject, eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (onEndDrag != null)
        {
            onEndDrag.Invoke(gameObject, eventData);
        }
    }

    public void OnMove(AxisEventData eventData)
    {
        if (onMove != null)
            onMove.Invoke(gameObject, eventData);
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        PlayeSound(gameObject, soundId);
        if (eventData.clickCount == 1)
        {
            if (onPointerClick != null)
                onPointerClick.Invoke(gameObject, eventData);
        }
        
        else if (eventData.clickCount == 2)
        {
            if (onPointerDoubleClick != null)
            {
                onPointerDoubleClick.Invoke(gameObject, eventData);
            }
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        m_currentBePressButton = gameObject;
        m_isPress = true;
        if (onPointerDown != null)
        {
            onPointerDown.Invoke(gameObject, eventData);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (onPointerEnter!= null)
            onPointerEnter.Invoke(gameObject, eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_isPress = false;
        m_currentBePressButton = null;
        if (onPointerExit != null)
            onPointerExit.Invoke(gameObject, eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_isPress = false;
        m_currentBePressButton = null;
        if (onPointerUp != null)
        {
            onPointerUp.Invoke(gameObject, eventData);
        }
    }

    public void OnScroll(PointerEventData eventData)
    {
        if (onScroll != null)
            onScroll.Invoke(gameObject, eventData);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        if (onSubmit != null)
        {
            onSubmit.Invoke(gameObject, eventData);
        }
    }

    public void OnUpdateSelected(BaseEventData eventData)
    {
        if (onUpdateSelected != null)
            onUpdateSelected.Invoke(gameObject, eventData);
    }
}
