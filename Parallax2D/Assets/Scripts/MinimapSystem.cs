using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class MinimapSystem : MonoBehaviour, IPointerClickHandler, IDragHandler, IScrollHandler
{
    [Header("─── Camera & Rendering ───")]
    public Camera minimapCamera;
    public Transform playerTransform;
    public bool followPlayer = true;

    [Header("─── Minimap UI References ───")]
    public RawImage minimapRawImage;
    public Image minimapMaskImage;
    public Image borderImage;
    public RectTransform playerDot;

    [Header("─── Zoom Settings ───")]
    public float defaultZoom = 15f;
    public float minZoom = 5f;
    public float maxZoom = 40f;
    public float scrollZoomSpeed = 3f;
    public float buttonZoomSpeed = 8f;

    [Header("─── Click to Ping ───")]
    public bool clickShowsPing = true;
    public RectTransform pingMarker;
    public float pingDuration = 2f;

    [Header("─── Border Animation ───")]
    public bool animateBorder = true;
    public float pulseSpeed = 1.2f;
    public float pulseAmount = 0.03f;

    [Header("─── Enemy Dots ───")]
    public string enemyTag = "Enemy";
    public RectTransform enemyDotPrefab;
    public Color enemyDotColor = new Color(1f, 0.2f, 0.2f, 0.9f);
    public Color playerDotColor = new Color(0.2f, 0.8f, 1f, 1f);
    public float enemyRefreshInterval = 1f;

    private float _targetZoom;
    private RectTransform _minimapRect;
    private Vector2 _mapSize;
    private float _pingTimer;
    private List<RectTransform> _enemyDots = new List<RectTransform>();
    private List<Transform> _enemyTransforms = new List<Transform>();
    private float _enemyRefreshTimer;
    private Vector3 _borderBaseScale;
    private bool _isDragging;

    void Awake()
    {
        _minimapRect = GetComponent<RectTransform>();
    }

    void Start()
    {
        if (playerTransform == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) playerTransform = p.transform;
            else Debug.LogWarning("[MinimapSystem] No GameObject tagged 'Player' found.");
        }

        _targetZoom = defaultZoom;

        if (minimapCamera != null)
            minimapCamera.orthographicSize = defaultZoom;

        if (playerDot != null)
            playerDot.GetComponent<Image>().color = playerDotColor;

        if (borderImage != null)
            _borderBaseScale = borderImage.transform.localScale;

        if (pingMarker != null)
            pingMarker.gameObject.SetActive(false);

        RefreshEnemyDots();
    }

    void Update()
    {
        _mapSize = new Vector2(_minimapRect.rect.width, _minimapRect.rect.height);

        if (followPlayer && playerTransform != null && minimapCamera != null)
        {
            Vector3 camPos = minimapCamera.transform.position;
            camPos.x = playerTransform.position.x;
            camPos.y = playerTransform.position.y;
            minimapCamera.transform.position = camPos;
        }

        if (minimapCamera != null)
        {
            minimapCamera.orthographicSize = Mathf.Lerp(
                minimapCamera.orthographicSize,
                _targetZoom,
                Time.deltaTime * buttonZoomSpeed
            );
        }

        if (playerDot != null && playerTransform != null && minimapCamera != null)
        {
            if (followPlayer)
                playerDot.anchoredPosition = Vector2.zero;
            else
                playerDot.anchoredPosition = WorldToMinimapPos(playerTransform.position);
        }

        _enemyRefreshTimer -= Time.deltaTime;
        if (_enemyRefreshTimer <= 0f)
        {
            RefreshEnemyDots();
            _enemyRefreshTimer = enemyRefreshInterval;
        }
        UpdateEnemyDotPositions();

        if (pingMarker != null && pingMarker.gameObject.activeSelf)
        {
            _pingTimer -= Time.deltaTime;
            float alpha = Mathf.Clamp01(_pingTimer / pingDuration);
            Color c = pingMarker.GetComponent<Image>().color;
            c.a = alpha;
            pingMarker.GetComponent<Image>().color = c;

            float scale = 1f + Mathf.Sin((1f - alpha) * Mathf.PI) * 0.5f;
            pingMarker.localScale = Vector3.one * scale;

            if (_pingTimer <= 0f)
                pingMarker.gameObject.SetActive(false);
        }

        if (animateBorder && borderImage != null)
        {
            float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed * Mathf.PI * 2f) * pulseAmount;
            borderImage.transform.localScale = _borderBaseScale * pulse;
        }
    }

    public void OnScroll(PointerEventData eventData)
    {
        _targetZoom -= eventData.scrollDelta.y * scrollZoomSpeed;
        _targetZoom = Mathf.Clamp(_targetZoom, minZoom, maxZoom);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isDragging) return;

        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _minimapRect, eventData.position, eventData.pressEventCamera, out localPos);

        Vector3 worldPos = MinimapPosToWorld(localPos);

        if (clickShowsPing)
            ShowPing(localPos, worldPos);
        else
            Debug.Log($"[MinimapSystem] Clicked world pos: {worldPos}");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (followPlayer) return;
        _isDragging = true;

        if (minimapCamera == null) return;

        float unitsPerPixel = (minimapCamera.orthographicSize * 2f) / _mapSize.y;

        Vector3 camPos = minimapCamera.transform.position;
        camPos.x -= eventData.delta.x * unitsPerPixel;
        camPos.y -= eventData.delta.y * unitsPerPixel;
        minimapCamera.transform.position = camPos;
    }

    public void ZoomIn()
    {
        _targetZoom = Mathf.Clamp(_targetZoom - 5f, minZoom, maxZoom);
    }

    public void ZoomOut()
    {
        _targetZoom = Mathf.Clamp(_targetZoom + 5f, minZoom, maxZoom);
    }

    public void ToggleMinimap()
    {
        bool isActive = minimapRawImage.gameObject.activeSelf;
        minimapRawImage.gameObject.SetActive(!isActive);
        if (playerDot) playerDot.gameObject.SetActive(!isActive);
        foreach (var dot in _enemyDots)
            if (dot) dot.gameObject.SetActive(!isActive);
    }

    private Vector2 WorldToMinimapPos(Vector3 worldPos)
    {
        if (minimapCamera == null) return Vector2.zero;

        Vector3 viewportPos = minimapCamera.WorldToViewportPoint(worldPos);
        float x = (viewportPos.x - 0.5f) * _mapSize.x;
        float y = (viewportPos.y - 0.5f) * _mapSize.y;
        return new Vector2(x, y);
    }

    private Vector3 MinimapPosToWorld(Vector2 localPos)
    {
        if (minimapCamera == null) return Vector3.zero;

        float vx = (localPos.x / _mapSize.x) + 0.5f;
        float vy = (localPos.y / _mapSize.y) + 0.5f;

        Vector3 worldPos = minimapCamera.ViewportToWorldPoint(new Vector3(vx, vy, 0f));
        worldPos.z = 0f;
        return worldPos;
    }

    private void ShowPing(Vector2 localPos, Vector3 worldPos)
    {
        if (pingMarker == null) return;

        pingMarker.gameObject.SetActive(true);
        pingMarker.anchoredPosition = localPos;
        pingMarker.localScale = Vector3.one;

        Color c = pingMarker.GetComponent<Image>().color;
        c.a = 1f;
        pingMarker.GetComponent<Image>().color = c;

        _pingTimer = pingDuration;
        Debug.Log($"[MinimapSystem] Pinged world position: {worldPos}");
    }

    private void RefreshEnemyDots()
    {
        foreach (var dot in _enemyDots)
            if (dot != null) Destroy(dot.gameObject);
        _enemyDots.Clear();
        _enemyTransforms.Clear();

        if (enemyDotPrefab == null || minimapRawImage == null) return;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        foreach (var enemy in enemies)
        {
            _enemyTransforms.Add(enemy.transform);
            RectTransform dot = Instantiate(enemyDotPrefab, minimapRawImage.transform);
            Image img = dot.GetComponent<Image>();
            if (img != null) img.color = enemyDotColor;
            _enemyDots.Add(dot);
        }
    }

    private void UpdateEnemyDotPositions()
    {
        for (int i = 0; i < _enemyTransforms.Count; i++)
        {
            if (_enemyTransforms[i] == null)
            {
                if (i < _enemyDots.Count && _enemyDots[i] != null)
                    _enemyDots[i].gameObject.SetActive(false);
                continue;
            }

            if (i < _enemyDots.Count && _enemyDots[i] != null)
            {
                Vector2 dotPos = WorldToMinimapPos(_enemyTransforms[i].position);
                float radius = _mapSize.x * 0.5f;
                _enemyDots[i].gameObject.SetActive(dotPos.magnitude <= radius);
                _enemyDots[i].anchoredPosition = dotPos;
            }
        }
    }

    void LateUpdate()
    {
        _isDragging = false;
    }

    void OnDrawGizmosSelected()
    {
        if (minimapCamera == null) return;
        Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
        float size = minimapCamera.orthographicSize;
        float aspect = minimapCamera.aspect;
        Vector3 center = minimapCamera.transform.position;
        center.z = 0f;
        Gizmos.DrawWireCube(center, new Vector3(size * 2f * aspect, size * 2f, 0.1f));
    }
}
