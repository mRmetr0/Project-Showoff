using UnityEngine;

public class InstrumentSign : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    
    private Vector3 _showPos, _hidePos, startPos, endPos;
    private DragAndDrop inst;
    private float _t;
    private bool _shouldMove;
    private void Awake()
    {
        _showPos = transform.position;
        _hidePos = _showPos + (Vector3.up * transform.localScale.y * 5f);
        inst = GetComponentInChildren<DragAndDrop>();
    }

    private void OnEnable()
    {
        ButtonManager.onPlay += Hide;
        ButtonManager.onStop += Show;
    }

    private void OnDisable()
    {
        ButtonManager.onPlay -= Hide;
        ButtonManager.onStop -= Show;
    }

    private void Update()
    {
        if (!_shouldMove) return;
        _t += Time.deltaTime * moveSpeed;
        transform.position = Vector3.Lerp(startPos, endPos, _t);
        if (_t < 1) return;
        _shouldMove = false;
        _t = 0;
    }

    private void Hide()
    {
        inst.Usable = false;
        startPos = _showPos;
        endPos = _hidePos;
        GeneralSetup();
    }
    private void Show()
    {
        inst.Usable = true;
        startPos = _hidePos;
        endPos = _showPos;
        GeneralSetup();
    }

    private void GeneralSetup()
    {
        _shouldMove = true;
        transform.position = startPos;
        _t = 0;
    }
}
