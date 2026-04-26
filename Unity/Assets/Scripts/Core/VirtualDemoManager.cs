/*
 Code Artifact: VirtualDemoManager.cs
 Description: Builds and animates a virtual HUD-Link demo scene that shows the project features with mock data.
 Programmer: HudLink development team (Brandon Dodge, Zach Sevart, Asa Maker, Jonathan Gott, Josh Dwoskin)
 Date Created: 2026-04-26
 Revision History:
 - 2026-04-26 - HudLink development team - Added an automatic Quest-ready virtual demo for sprint presentation.
 - 2026-04-26 - HudLink development team - Kept the demo board world-anchored so walkthrough movement is useful.
 - 2026-04-26 - HudLink development team - Replaced the center mesh panel with a mock NSDK room scan scene.
 Preconditions: HudLinkScene contains a camera and HudLinkBootstrap can pass the active HudController before runtime updates.
 Acceptable Inputs: A valid HudController reference, Unity main-thread lifecycle calls, and normal frame delta values.
 Unacceptable Inputs: Missing Unity UI packages, missing TextMesh Pro package, or a scene with no camera at all.
 Postconditions: A world-space demo board, simple virtual environment, animated mock metrics, and HUD widget updates are visible.
 Return Values: Unity lifecycle methods return void; helper methods return created Unity objects or UI components.
 Error and Exception Conditions: Unity can log setup errors if required UI components or shaders are unavailable.
 Side Effects: Creates runtime GameObjects, publishes demo events, updates HUD widgets, and allocates simple runtime materials.
 Invariants: Demo data is local mock data only, widget IDs stay aligned with HudController routing, and the safe-zone idea remains visible.
 Known Faults: This is a presentation demo and does not connect to a live phone, Health Connect, GPS, BLE, or notification service.
 Major Blocks: Setup builds the environment and board, update animates metrics, and publishing feeds the existing HUD/event pipeline.
 Line Comments: Major branches and calculations are commented so another teammate can tune the demo quickly before a meeting.
*/
using System.Collections.Generic;
using HudLink.Events;
using HudLink.HUD;
using HudLink.Network;
using HudLink.Widgets;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace HudLink.Core
{
    /// <summary>
    /// Runtime-only demo surface for Quest or editor playback.
    /// The demo intentionally uses generated UI so the team does not need to maintain extra prefabs.
    /// </summary>
    public sealed class VirtualDemoManager : MonoBehaviour
    {
        private const float DemoCycleSeconds = 48f;
        private const float CardWidth = 330f;
        private const float CardHeight = 158f;
        private const float CardContentX = 0f;
        private const float CardInnerWidth = 286f;

        [Header("Demo Setup")]
        [SerializeField] private HudController hudController;
        [SerializeField] private bool autoStart = true;
        [SerializeField] private bool keepDemoLockedInFrontOfViewer;
        [SerializeField] private float boardDistanceMeters = 1.15f;
        [SerializeField] private float boardScale = 0.0019f;
        [SerializeField] private float widgetUpdateInterval = 0.35f;

        private readonly Queue<string> _eventLog = new Queue<string>();
        private readonly List<TextMeshProUGUI> _eventRows = new List<TextMeshProUGUI>();
        private readonly List<Renderer> _roomMeshRenderers = new List<Renderer>();

        private Transform _viewer;
        private Transform _root;
        private Transform _environmentRoot;
        private Transform _teammateMarker;
        private Transform _scanSweep;
        private RectTransform _board;
        private GameObject _runner;
        private Image _connectionDot;
        private Image _heartBar;
        private Image _activityBar;
        private Image _stressBar;
        private Image _batteryBar;
        private Image _signalBar;
        private Image _privacyBar;
        private Image _radarFriend;
        private Image _radarWaypoint;
        private Image _meshScanBar;
        private TextMeshProUGUI _phaseText;
        private TextMeshProUGUI _bridgeText;
        private TextMeshProUGUI _consentText;
        private TextMeshProUGUI _heartText;
        private TextMeshProUGUI _activityText;
        private TextMeshProUGUI _stressText;
        private TextMeshProUGUI _gpsText;
        private TextMeshProUGUI _headingText;
        private TextMeshProUGUI _notificationText;
        private TextMeshProUGUI _environmentText;
        private TextMeshProUGUI _mediaText;
        private TextMeshProUGUI _systemText;
        private TextMeshProUGUI _performanceText;
        private TextMeshProUGUI _privacyText;
        private TextMeshProUGUI _radarText;
        private TextMeshProUGUI _meshStatusText;
        private TextMeshProUGUI _meshObjectText;

        private float _elapsedSeconds;
        private float _nextWidgetUpdateSeconds;
        private int _lastPhase = -1;
        private int _notificationIndex;
        private bool _isConnected = true;

        private static readonly string[] MockNotifications =
        {
            "Calendar: demo in 10 min",
            "Messages: team ready",
            "Email: release notes OK",
            "Phone: missed call hidden"
        };

        public void Configure(HudController controller)
        {
            // HudLinkBootstrap calls this before the first demo update.
            hudController = controller;
        }

        private void Start()
        {
            if (autoStart)
            {
                BuildDemo();
            }
        }

        private void Update()
        {
            if (_root == null)
            {
                return;
            }

            _elapsedSeconds += Time.deltaTime;
            HandleRecenterInput();
            if (keepDemoLockedInFrontOfViewer)
            {
                PositionDemoInFrontOfViewer();
            }

            float cycleTime = Mathf.Repeat(_elapsedSeconds, DemoCycleSeconds);
            int phase = GetPhase(cycleTime);
            if (phase != _lastPhase)
            {
                ApplyPhase(phase);
                _lastPhase = phase;
            }

            AnimateWorldObjects(cycleTime);
            UpdateDemoReadouts(cycleTime, phase);

            if (_elapsedSeconds >= _nextWidgetUpdateSeconds)
            {
                _nextWidgetUpdateSeconds = _elapsedSeconds + widgetUpdateInterval;
                PushMockDataToHud(cycleTime, phase);
                PublishMockEvents(cycleTime, phase);
            }
        }

        public void BuildDemo()
        {
            if (_root != null)
            {
                return;
            }

            ResolveViewer();
            _root = new GameObject("HudLink Virtual Demo").transform;
            SnapDemoInFrontOfViewer();
            BuildEnvironment();
            BuildDemoBoard();
            AddEvent("Demo started");
            AddEvent("Room scan ready");
        }

        private void ResolveViewer()
        {
            // Camera.main works both in the editor and on Quest when the XR rig tags the center eye camera.
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                mainCamera = FindFirstObjectByType<Camera>();
            }

            if (mainCamera != null)
            {
                _viewer = mainCamera.transform;
            }
        }

        private void BuildEnvironment()
        {
            _environmentRoot = new GameObject("Virtual Demo Environment").transform;
            _environmentRoot.SetParent(_root, false);

            BuildMockRoom();
            BuildRouteMarkers();
            CreateDataTower("Phone Bridge", new Vector3(-1.55f, -0.95f, 1.2f), new Color(0.35f, 0.55f, 1f, 1f));
            CreateDataTower("Local Storage", new Vector3(1.55f, -0.95f, 1.2f), new Color(0.35f, 0.9f, 0.62f, 1f));
            CreateRoomLabel("NSDK 4.0 MOCK ROOM", new Vector3(0f, 0.46f, 3.04f), WidgetStyles.AccentCyan, 150f, 28f);
            CreateRoomLabel("Scene mesh + semantic anchors", new Vector3(0f, 0.24f, 3.04f), WidgetStyles.TextSecondary, 180f, 24f);
        }

        private void BuildMockRoom()
        {
            _roomMeshRenderers.Clear();

            CreateRoomBlock("NSDK Floor Mesh", new Vector3(0f, -1.35f, 2.05f), new Vector3(2.9f, 0.035f, 2.35f), new Color(0.025f, 0.04f, 0.045f, 1f), true);
            CreateRoomBlock("NSDK Back Wall Mesh", new Vector3(0f, -0.48f, 3.23f), new Vector3(2.9f, 1.72f, 0.035f), new Color(0.04f, 0.055f, 0.07f, 1f), true);
            CreateRoomBlock("NSDK Left Wall Mesh", new Vector3(-1.45f, -0.48f, 2.05f), new Vector3(0.035f, 1.72f, 2.35f), new Color(0.035f, 0.05f, 0.065f, 1f), true);
            CreateRoomBlock("NSDK Right Wall Mesh", new Vector3(1.45f, -0.48f, 2.05f), new Vector3(0.035f, 1.72f, 2.35f), new Color(0.035f, 0.05f, 0.065f, 1f), true);

            BuildFloorMeshGrid();
            BuildBackWallMeshGrid();

            CreateTable();
            CreateCouch();
            CreateWallScreen();
            CreateDetectedPerson();

            GameObject sweep = CreateRoomBlock("NSDK Scan Sweep", new Vector3(-1.2f, -0.52f, 2.05f), new Vector3(0.035f, 1.45f, 2.1f), new Color(0.05f, 0.9f, 1f, 0.85f), false);
            _scanSweep = sweep.transform;

            CreateRoomLabel("FLOOR", new Vector3(-1.02f, -1.08f, 1.18f), WidgetStyles.AccentGreen, 86f, 24f);
            CreateRoomLabel("WALL", new Vector3(-1.18f, -0.05f, 3.01f), WidgetStyles.AccentBlue, 78f, 24f);
            CreateRoomLabel("TABLE", new Vector3(-0.28f, -0.68f, 2.03f), WidgetStyles.AccentYellow, 86f, 24f);
            CreateRoomLabel("COUCH", new Vector3(0.86f, -0.74f, 2.58f), WidgetStyles.AccentGreen, 88f, 24f);
            CreateRoomLabel("PERSON", new Vector3(0.86f, -0.08f, 1.48f), WidgetStyles.AccentRed, 94f, 24f);
        }

        private void BuildFloorMeshGrid()
        {
            for (int i = 0; i <= 8; i++)
            {
                float x = Mathf.Lerp(-1.35f, 1.35f, i / 8f);
                CreateRoomBlock("Floor Mesh Line X", new Vector3(x, -1.315f, 2.05f), new Vector3(0.01f, 0.012f, 2.24f), new Color(0.08f, 0.55f, 0.68f, 1f), true);
            }

            for (int i = 0; i <= 7; i++)
            {
                float z = Mathf.Lerp(0.94f, 3.16f, i / 7f);
                CreateRoomBlock("Floor Mesh Line Z", new Vector3(0f, -1.31f, z), new Vector3(2.72f, 0.012f, 0.01f), new Color(0.08f, 0.55f, 0.68f, 1f), true);
            }
        }

        private void BuildBackWallMeshGrid()
        {
            for (int i = 0; i <= 8; i++)
            {
                float x = Mathf.Lerp(-1.35f, 1.35f, i / 8f);
                CreateRoomBlock("Back Wall Mesh Line X", new Vector3(x, -0.48f, 3.19f), new Vector3(0.01f, 1.55f, 0.012f), new Color(0.08f, 0.48f, 0.65f, 1f), true);
            }

            for (int i = 0; i <= 5; i++)
            {
                float y = Mathf.Lerp(-1.2f, 0.23f, i / 5f);
                CreateRoomBlock("Back Wall Mesh Line Y", new Vector3(0f, y, 3.185f), new Vector3(2.72f, 0.01f, 0.012f), new Color(0.08f, 0.48f, 0.65f, 1f), true);
            }
        }

        private void BuildRouteMarkers()
        {
            // A ring of small tiles gives the demo a visible route for GPS and activity metrics.
            for (int i = 0; i < 24; i++)
            {
                float angle = i * Mathf.PI * 2f / 24f;
                GameObject tile = CreateRoomBlock("Route Tile", new Vector3(Mathf.Cos(angle) * 1.05f, -1.25f, 2.05f + Mathf.Sin(angle) * 0.55f), new Vector3(0.12f, 0.025f, 0.035f), new Color(0.12f, 0.55f, 0.72f, 1f), false);
                tile.transform.localRotation = Quaternion.Euler(0f, -angle * Mathf.Rad2Deg, 0f);
            }
        }

        private void CreateTable()
        {
            CreateRoomBlock("Detected Table Top", new Vector3(-0.38f, -0.93f, 2.02f), new Vector3(0.72f, 0.07f, 0.48f), new Color(0.62f, 0.48f, 0.22f, 1f), false);
            CreateRoomBlock("Detected Table Leg A", new Vector3(-0.68f, -1.13f, 1.84f), new Vector3(0.05f, 0.36f, 0.05f), new Color(0.42f, 0.32f, 0.18f, 1f), false);
            CreateRoomBlock("Detected Table Leg B", new Vector3(-0.08f, -1.13f, 1.84f), new Vector3(0.05f, 0.36f, 0.05f), new Color(0.42f, 0.32f, 0.18f, 1f), false);
            CreateRoomBlock("Detected Table Leg C", new Vector3(-0.68f, -1.13f, 2.2f), new Vector3(0.05f, 0.36f, 0.05f), new Color(0.42f, 0.32f, 0.18f, 1f), false);
            CreateRoomBlock("Detected Table Leg D", new Vector3(-0.08f, -1.13f, 2.2f), new Vector3(0.05f, 0.36f, 0.05f), new Color(0.42f, 0.32f, 0.18f, 1f), false);
        }

        private void CreateCouch()
        {
            CreateRoomBlock("Detected Couch Seat", new Vector3(0.78f, -1.08f, 2.67f), new Vector3(0.78f, 0.22f, 0.38f), new Color(0.08f, 0.34f, 0.22f, 1f), false);
            CreateRoomBlock("Detected Couch Back", new Vector3(0.78f, -0.86f, 2.84f), new Vector3(0.78f, 0.46f, 0.12f), new Color(0.06f, 0.28f, 0.2f, 1f), false);
            CreateRoomBlock("Detected Couch Arm Left", new Vector3(0.34f, -0.98f, 2.67f), new Vector3(0.1f, 0.38f, 0.42f), new Color(0.06f, 0.28f, 0.2f, 1f), false);
            CreateRoomBlock("Detected Couch Arm Right", new Vector3(1.22f, -0.98f, 2.67f), new Vector3(0.1f, 0.38f, 0.42f), new Color(0.06f, 0.28f, 0.2f, 1f), false);
        }

        private void CreateWallScreen()
        {
            CreateRoomBlock("Detected Wall Screen", new Vector3(0.22f, -0.23f, 3.16f), new Vector3(0.72f, 0.42f, 0.018f), new Color(0.04f, 0.18f, 0.24f, 1f), false);
            CreateRoomLabel("SCREEN", new Vector3(0.22f, 0.06f, 3.12f), WidgetStyles.AccentCyan, 86f, 24f);
        }

        private void CreateDetectedPerson()
        {
            _teammateMarker = new GameObject("Detected Person Anchor").transform;
            _teammateMarker.SetParent(_environmentRoot, false);
            _teammateMarker.localPosition = new Vector3(0.86f, -1.18f, 1.5f);

            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            body.name = "Detected Person Body";
            body.transform.SetParent(_teammateMarker, false);
            body.transform.localPosition = new Vector3(0f, 0.28f, 0f);
            body.transform.localScale = new Vector3(0.12f, 0.28f, 0.12f);
            body.GetComponent<Renderer>().sharedMaterial = CreateMaterial("Person Body Material", new Color(0.55f, 0.9f, 0.72f, 1f));

            _runner = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _runner.name = "Detected Person Head";
            _runner.transform.SetParent(_teammateMarker, false);
            _runner.transform.localPosition = new Vector3(0f, 0.66f, 0f);
            _runner.transform.localScale = Vector3.one * 0.16f;
            _runner.GetComponent<Renderer>().sharedMaterial = CreateMaterial("Person Head Material", new Color(0.3f, 0.9f, 0.55f, 1f));
        }

        private GameObject CreateRoomBlock(string name, Vector3 localPosition, Vector3 localScale, Color color, bool pulseWithScan)
        {
            GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
            block.name = name;
            block.transform.SetParent(_environmentRoot, false);
            block.transform.localPosition = localPosition;
            block.transform.localScale = localScale;

            Renderer renderer = block.GetComponent<Renderer>();
            renderer.sharedMaterial = CreateMaterial(name + " Material", color);
            if (pulseWithScan)
            {
                _roomMeshRenderers.Add(renderer);
            }

            return block;
        }

        private TextMeshProUGUI CreateRoomLabel(string text, Vector3 localPosition, Color color, float width, float height)
        {
            GameObject canvasObject = new GameObject(text + " Label");
            canvasObject.transform.SetParent(_environmentRoot, false);
            canvasObject.transform.localPosition = localPosition;
            canvasObject.transform.localScale = Vector3.one * 0.0025f;

            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = _viewer != null ? _viewer.GetComponent<Camera>() : Camera.main;
            canvasObject.AddComponent<CanvasScaler>().dynamicPixelsPerUnit = 12f;

            RectTransform rect = canvasObject.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(width, height);

            Image background = canvasObject.AddComponent<Image>();
            background.color = new Color(0.02f, 0.04f, 0.05f, 0.8f);

            TextMeshProUGUI label = AddLabel(rect, text, 12, FontStyles.Bold, TextAlignmentOptions.Center, Vector2.zero, new Vector2(width - 8f, height - 6f), color);
            return label;
        }

        private void CreateDataTower(string label, Vector3 localPosition, Color color)
        {
            GameObject tower = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            tower.name = label;
            tower.transform.SetParent(_environmentRoot, false);
            tower.transform.localPosition = localPosition;
            tower.transform.localScale = new Vector3(0.12f, 0.35f, 0.12f);
            tower.GetComponent<Renderer>().sharedMaterial = CreateMaterial(label + " Material", color);
        }

        private void BuildDemoBoard()
        {
            GameObject canvasObject = new GameObject("Virtual Feature Demo Board");
            canvasObject.transform.SetParent(_root, false);

            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = _viewer != null ? _viewer.GetComponent<Camera>() : Camera.main;
            canvasObject.AddComponent<CanvasScaler>().dynamicPixelsPerUnit = 12f;
            canvasObject.AddComponent<GraphicRaycaster>();

            _board = canvasObject.GetComponent<RectTransform>();
            _board.sizeDelta = new Vector2(1320f, 840f);
            canvasObject.transform.localScale = Vector3.one * boardScale;

            Image boardBackground = canvasObject.AddComponent<Image>();
            boardBackground.color = new Color(0.015f, 0.02f, 0.025f, 0.06f);

            BuildHeader(_board);
            BuildCards(_board);
        }

        private void SnapDemoInFrontOfViewer()
        {
            if (_viewer == null)
            {
                _root.position = new Vector3(0f, 1.4f, boardDistanceMeters);
                _root.rotation = Quaternion.identity;
                return;
            }

            Vector3 forward = _viewer.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude < 0.001f)
            {
                forward = Vector3.forward;
            }

            _root.position = _viewer.position + forward.normalized * boardDistanceMeters;
            _root.rotation = Quaternion.LookRotation(forward.normalized, Vector3.up);
        }

        private void BuildHeader(RectTransform parent)
        {
            RectTransform header = CreatePanel(parent, "Demo Header", new Vector2(0f, 366f), new Vector2(1260f, 72f), new Color(0.05f, 0.08f, 0.1f, 0.88f));
            AddLabel(header, "HUD-Link Virtual Demo", 28, FontStyles.Bold, TextAlignmentOptions.Left, new Vector2(-585f, 12f), new Vector2(720f, 34f), WidgetStyles.TextPrimary);
            AddLabel(header, "Mock bridge, health, GPS, privacy, room scan, events, and safety states", 15, FontStyles.Normal, TextAlignmentOptions.Left, new Vector2(-585f, -22f), new Vector2(760f, 24f), WidgetStyles.TextSecondary);
            _phaseText = AddLabel(header, "Phase: starting", 18, FontStyles.Bold, TextAlignmentOptions.Right, new Vector2(405f, 0f), new Vector2(390f, 42f), WidgetStyles.AccentCyan);
        }

        private void BuildCards(RectTransform parent)
        {
            BuildCenterRoomWindow(parent);

            RectTransform bridge = CreateCard(parent, "Bridge + Consent", -460f, 222f);
            _bridgeText = AddLabel(bridge, "Connected\nBridge active", 18, FontStyles.Bold, TextAlignmentOptions.Left, new Vector2(CardContentX, 13f), new Vector2(CardInnerWidth, 44f), WidgetStyles.TextPrimary);
            _connectionDot = CreateDot(bridge, new Vector2(128f, 28f), WidgetStyles.AccentGreen);
            _consentText = AddLabel(bridge, "Consent OK: health, GPS, notices", 12, FontStyles.Normal, TextAlignmentOptions.Left, new Vector2(CardContentX, -42f), new Vector2(CardInnerWidth, 26f), WidgetStyles.TextSecondary);

            RectTransform health = CreateCard(parent, "Health + Activity", -460f, 52f);
            _heartText = AddLabel(health, "HR 72 BPM", 23, FontStyles.Bold, TextAlignmentOptions.Left, new Vector2(CardContentX, 24f), new Vector2(CardInnerWidth, 30f), WidgetStyles.AccentRed);
            _heartBar = CreateBar(health, new Vector2(0f, -2f), new Vector2(CardInnerWidth, 12f), WidgetStyles.AccentRed);
            _activityText = AddLabel(health, "Steps 4,200 | Goal 46%", 14, FontStyles.Bold, TextAlignmentOptions.Left, new Vector2(CardContentX, -30f), new Vector2(CardInnerWidth, 22f), WidgetStyles.AccentGreen);
            _activityBar = CreateBar(health, new Vector2(0f, -54f), new Vector2(CardInnerWidth, 10f), WidgetStyles.AccentGreen);

            RectTransform location = CreateCard(parent, "GPS + Environment", -460f, -118f);
            _gpsText = AddLabel(location, "3.2 mph\n38.9543, -95.2558", 18, FontStyles.Bold, TextAlignmentOptions.Left, new Vector2(CardContentX, 17f), new Vector2(CardInnerWidth, 42f), WidgetStyles.AccentCyan);
            _headingText = AddLabel(location, "NE 42 deg | +/- 5 m", 12, FontStyles.Normal, TextAlignmentOptions.Left, new Vector2(CardContentX, -28f), new Vector2(CardInnerWidth, 20f), WidgetStyles.TextSecondary);
            _environmentText = AddLabel(location, "22 C | UV 3 | AQI 42", 12, FontStyles.Bold, TextAlignmentOptions.Left, new Vector2(CardContentX, -54f), new Vector2(CardInnerWidth, 20f), WidgetStyles.AccentYellow);

            RectTransform notifications = CreateCard(parent, "Notifications + Media", -460f, -288f);
            _notificationText = AddLabel(notifications, "Calendar: demo in 10 min", 15, FontStyles.Bold, TextAlignmentOptions.Left, new Vector2(CardContentX, 24f), new Vector2(CardInnerWidth, 28f), WidgetStyles.TextPrimary);
            _mediaText = AddLabel(notifications, "Sprint Review Mix 0%", 14, FontStyles.Bold, TextAlignmentOptions.Left, new Vector2(CardContentX, -4f), new Vector2(CardInnerWidth, 24f), WidgetStyles.AccentBlue);
            AddLabel(notifications, "Message bodies redacted.", 12, FontStyles.Normal, TextAlignmentOptions.Left, new Vector2(CardContentX, -48f), new Vector2(CardInnerWidth, 26f), WidgetStyles.TextSecondary);

            RectTransform system = CreateCard(parent, "System + Performance", 460f, 222f);
            _systemText = AddLabel(system, "Headset 86% | Phone 74%", 15, FontStyles.Bold, TextAlignmentOptions.Left, new Vector2(CardContentX, 25f), new Vector2(CardInnerWidth, 22f), WidgetStyles.TextPrimary);
            _batteryBar = CreateBar(system, new Vector2(0f, 4f), new Vector2(CardInnerWidth, 12f), WidgetStyles.AccentGreen);
            _signalBar = CreateBar(system, new Vector2(0f, -20f), new Vector2(CardInnerWidth, 12f), WidgetStyles.AccentBlue);
            _performanceText = AddLabel(system, "72 FPS target | 78 mock", 12, FontStyles.Normal, TextAlignmentOptions.Left, new Vector2(CardContentX, -50f), new Vector2(CardInnerWidth, 22f), WidgetStyles.TextSecondary);

            RectTransform privacy = CreateCard(parent, "Privacy + Retention", 460f, 52f);
            _privacyText = AddLabel(privacy, "Local only\nCloud OFF", 19, FontStyles.Bold, TextAlignmentOptions.Left, new Vector2(CardContentX, 17f), new Vector2(CardInnerWidth, 42f), WidgetStyles.AccentGreen);
            _privacyBar = CreateBar(privacy, new Vector2(0f, -17f), new Vector2(CardInnerWidth, 12f), WidgetStyles.AccentGreen);
            _stressText = AddLabel(privacy, "Stress low", 12, FontStyles.Normal, TextAlignmentOptions.Left, new Vector2(CardContentX, -50f), new Vector2(CardInnerWidth, 20f), WidgetStyles.TextSecondary);
            _stressBar = CreateBar(privacy, new Vector2(0f, -66f), new Vector2(CardInnerWidth, 8f), WidgetStyles.AccentYellow);

            RectTransform radar = CreateCard(parent, "Proximity Radar", 460f, -118f);
            CreateRadar(radar);
            _radarText = AddLabel(radar, "Friend 18 m\nWaypoint 34 m", 12, FontStyles.Bold, TextAlignmentOptions.Left, new Vector2(CardContentX, -58f), new Vector2(CardInnerWidth, 28f), WidgetStyles.TextSecondary);

            RectTransform modular = CreateCard(parent, "Widget Events", 460f, -288f);
            AddLabel(modular, "IWidget -> BaseWidget\nEdge slots stay clear", 13, FontStyles.Bold, TextAlignmentOptions.Left, new Vector2(CardContentX, 18f), new Vector2(CardInnerWidth, 42f), WidgetStyles.TextPrimary);
            for (int i = 0; i < 2; i++)
            {
                _eventRows.Add(AddLabel(modular, "", 10, FontStyles.Normal, TextAlignmentOptions.Left, new Vector2(CardContentX, -24f - i * 22f), new Vector2(CardInnerWidth, 18f), WidgetStyles.TextSecondary));
            }
        }

        private RectTransform CreateCard(RectTransform parent, string title, float x, float y)
        {
            RectTransform card = CreatePanel(parent, title, new Vector2(x, y), new Vector2(CardWidth, CardHeight), new Color(0.08f, 0.09f, 0.12f, 0.94f));
            card.gameObject.AddComponent<RectMask2D>();
            Image accent = CreateImage(card, "Accent", new Vector2(0f, 69f), new Vector2(CardWidth - 20f, 4f), WidgetStyles.AccentCyan);
            accent.transform.SetAsFirstSibling();
            AddLabel(card, title, 13, FontStyles.Bold, TextAlignmentOptions.Left, new Vector2(CardContentX, 58f), new Vector2(CardInnerWidth, 22f), WidgetStyles.AccentCyan);
            return card;
        }

        private void BuildCenterRoomWindow(RectTransform parent)
        {
            RectTransform roomWindow = CreatePanel(parent, "Virtual Room Window", new Vector2(0f, -36f), new Vector2(500f, 575f), new Color(0.01f, 0.025f, 0.03f, 0.02f));
            CreateImage(roomWindow, "Room Window Top Border", new Vector2(0f, 286f), new Vector2(500f, 4f), new Color(0.25f, 0.9f, 1f, 0.55f));
            CreateImage(roomWindow, "Room Window Bottom Border", new Vector2(0f, -286f), new Vector2(500f, 4f), new Color(0.25f, 0.9f, 1f, 0.35f));
            CreateImage(roomWindow, "Room Window Left Border", new Vector2(-250f, 0f), new Vector2(4f, 575f), new Color(0.25f, 0.9f, 1f, 0.35f));
            CreateImage(roomWindow, "Room Window Right Border", new Vector2(250f, 0f), new Vector2(4f, 575f), new Color(0.25f, 0.9f, 1f, 0.35f));

            AddLabel(roomWindow, "VIRTUAL ROOM VIEW", 14, FontStyles.Bold, TextAlignmentOptions.Center, new Vector2(0f, 254f), new Vector2(430f, 24f), WidgetStyles.AccentCyan);
            _meshStatusText = AddLabel(roomWindow, "NSDK 4.0: room mesh active", 15, FontStyles.Bold, TextAlignmentOptions.Center, new Vector2(0f, -224f), new Vector2(430f, 26f), WidgetStyles.TextPrimary);
            _meshObjectText = AddLabel(roomWindow, "Anchors: floor, wall, table, couch, screen, person", 13, FontStyles.Normal, TextAlignmentOptions.Center, new Vector2(0f, -250f), new Vector2(430f, 24f), WidgetStyles.TextSecondary);
            _meshScanBar = CreateBar(roomWindow, new Vector2(0f, -272f), new Vector2(390f, 8f), WidgetStyles.AccentCyan);
        }

        private RectTransform CreatePanel(RectTransform parent, string name, Vector2 anchoredPosition, Vector2 size, Color color)
        {
            GameObject panelObject = new GameObject(name, typeof(RectTransform));
            panelObject.transform.SetParent(parent, false);

            RectTransform rect = panelObject.GetComponent<RectTransform>();
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPosition;

            Image image = panelObject.AddComponent<Image>();
            image.color = color;
            return rect;
        }

        private TextMeshProUGUI AddLabel(RectTransform parent, string text, int fontSize, FontStyles style, TextAlignmentOptions alignment, Vector2 anchoredPosition, Vector2 size, Color color)
        {
            GameObject labelObject = new GameObject("Label", typeof(RectTransform));
            labelObject.transform.SetParent(parent, false);

            RectTransform rect = labelObject.GetComponent<RectTransform>();
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPosition;

            TextMeshProUGUI label = labelObject.AddComponent<TextMeshProUGUI>();
            label.text = text;
            label.fontSize = fontSize;
            label.enableAutoSizing = true;
            label.fontSizeMax = fontSize;
            label.fontSizeMin = Mathf.Max(8, fontSize - 6);
            label.fontStyle = style;
            label.alignment = alignment;
            label.color = color;
            label.margin = new Vector4(2f, 0f, 2f, 0f);
            label.textWrappingMode = TextWrappingModes.Normal;
            label.overflowMode = TextOverflowModes.Truncate;
            return label;
        }

        private Image CreateImage(RectTransform parent, string name, Vector2 anchoredPosition, Vector2 size, Color color)
        {
            GameObject imageObject = new GameObject(name, typeof(RectTransform));
            imageObject.transform.SetParent(parent, false);

            RectTransform rect = imageObject.GetComponent<RectTransform>();
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPosition;

            Image image = imageObject.AddComponent<Image>();
            image.color = color;
            return image;
        }

        private Image CreateBar(RectTransform parent, Vector2 anchoredPosition, Vector2 size, Color fillColor)
        {
            Image background = CreateImage(parent, "Bar Background", anchoredPosition, size, new Color(0.18f, 0.2f, 0.24f, 0.95f));
            Image fill = CreateImage(background.rectTransform, "Bar Fill", Vector2.zero, size, fillColor);
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillOrigin = 0;
            fill.fillAmount = 0.5f;
            return fill;
        }

        private Image CreateDot(RectTransform parent, Vector2 anchoredPosition, Color color)
        {
            Image dot = CreateImage(parent, "Status Dot", anchoredPosition, new Vector2(18f, 18f), color);
            return dot;
        }

        private void CreateRadar(RectTransform parent)
        {
            Image outer = CreateImage(parent, "Radar Outer", new Vector2(0f, -2f), new Vector2(82f, 82f), new Color(0.04f, 0.08f, 0.1f, 0.95f));
            outer.sprite = null;
            CreateImage(outer.rectTransform, "Radar Cross X", Vector2.zero, new Vector2(74f, 2f), new Color(0.2f, 0.7f, 0.8f, 0.55f));
            CreateImage(outer.rectTransform, "Radar Cross Y", Vector2.zero, new Vector2(2f, 74f), new Color(0.2f, 0.7f, 0.8f, 0.55f));
            _radarFriend = CreateDot(outer.rectTransform, new Vector2(14f, 16f), WidgetStyles.AccentGreen);
            _radarWaypoint = CreateDot(outer.rectTransform, new Vector2(-19f, -12f), WidgetStyles.AccentYellow);
        }

        private void PositionDemoInFrontOfViewer()
        {
            if (_viewer == null)
            {
                return;
            }

            Vector3 forward = _viewer.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude < 0.001f)
            {
                forward = Vector3.forward;
            }

            Vector3 targetPosition = _viewer.position + forward.normalized * boardDistanceMeters;
            targetPosition.y = _viewer.position.y;
            _root.position = Vector3.Lerp(_root.position, targetPosition, Time.deltaTime * 2.5f);
            _root.rotation = Quaternion.Slerp(_root.rotation, Quaternion.LookRotation(forward.normalized, Vector3.up), Time.deltaTime * 2.5f);
        }

        private void HandleRecenterInput()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null && keyboard.rKey.wasPressedThisFrame)
            {
                SnapDemoInFrontOfViewer();
            }
        }

        private int GetPhase(float cycleTime)
        {
            if (cycleTime < 12f)
            {
                return 0;
            }

            if (cycleTime < 24f)
            {
                return 1;
            }

            if (cycleTime < 36f)
            {
                return 2;
            }

            return 3;
        }

        private void ApplyPhase(int phase)
        {
            _isConnected = phase != 2;

            switch (phase)
            {
                case 0:
                    _phaseText.text = "Phase: live wellness HUD";
                    AddEvent("Widgets active");
                    break;
                case 1:
                    _phaseText.text = "Phase: privacy redaction";
                    AddEvent("Privacy redaction");
                    break;
                case 2:
                    _phaseText.text = "Phase: connection recovery";
                    AddEvent("Bridge recovery");
                    break;
                default:
                    _phaseText.text = "Phase: proximity and safety";
                    AddEvent("Safety alert");
                    break;
            }

            GlobalEventBus.Publish(new ConnectionStatusEvent(1, _isConnected, "VirtualDemoBridge"));
        }

        private void AnimateWorldObjects(float cycleTime)
        {
            if (_teammateMarker != null)
            {
                float angle = cycleTime * 0.65f;
                _teammateMarker.localPosition = new Vector3(0.86f + Mathf.Sin(angle) * 0.18f, -1.18f, 1.5f + Mathf.Cos(angle) * 0.12f);
            }

            if (_runner != null)
            {
                _runner.transform.localPosition = new Vector3(0f, 0.66f + Mathf.Sin(cycleTime * 2.4f) * 0.025f, 0f);
            }

            if (_scanSweep != null)
            {
                float scanPosition = Mathf.PingPong(cycleTime * 0.22f, 1f);
                Vector3 localPosition = _scanSweep.localPosition;
                localPosition.x = Mathf.Lerp(-1.28f, 1.28f, scanPosition);
                _scanSweep.localPosition = localPosition;
            }

            if (_radarFriend != null)
            {
                _radarFriend.rectTransform.anchoredPosition = new Vector2(Mathf.Sin(cycleTime * 0.8f) * 25f, Mathf.Cos(cycleTime * 0.55f) * 23f);
            }

            if (_radarWaypoint != null)
            {
                _radarWaypoint.rectTransform.anchoredPosition = new Vector2(Mathf.Cos(cycleTime * 0.45f) * 29f, Mathf.Sin(cycleTime * 0.6f) * 23f);
            }

            for (int i = 0; i < _roomMeshRenderers.Count; i++)
            {
                if (_roomMeshRenderers[i] == null)
                {
                    continue;
                }

                float pulse = Mathf.PingPong(cycleTime * 0.55f + i * 0.045f, 1f);
                _roomMeshRenderers[i].sharedMaterial.color = Color.Lerp(new Color(0.05f, 0.28f, 0.35f, 1f), new Color(0.12f, 0.72f, 0.9f, 1f), pulse);
            }
        }

        private void UpdateDemoReadouts(float cycleTime, int phase)
        {
            float bpm = 72f + Mathf.Sin(cycleTime * 1.2f) * 6f + (phase == 3 ? 18f : 0f);
            float activity = Mathf.PingPong(cycleTime * 0.035f, 0.42f) + 0.43f;
            float stress = Mathf.Clamp01(0.18f + (phase == 3 ? 0.52f : 0f) + Mathf.Sin(cycleTime * 0.7f) * 0.08f);
            float speed = 3.1f + Mathf.Sin(cycleTime * 0.8f) * 0.7f;
            float heading = Mathf.Repeat(cycleTime * 18f, 360f);
            float battery = Mathf.Clamp01(0.88f - Mathf.Repeat(cycleTime, DemoCycleSeconds) * 0.002f);
            float signal = _isConnected ? 0.88f + Mathf.Sin(cycleTime) * 0.08f : 0.26f;
            float privacy = phase == 1 ? 1f : 0.65f;

            _bridgeText.text = _isConnected ? "Connected\nBridge active" : "Degraded\nRetrying bridge";
            _connectionDot.color = _isConnected ? WidgetStyles.AccentGreen : WidgetStyles.AccentYellow;
            _consentText.text = phase == 1 ? "Consent OK; body hidden" : "Consent OK: health, GPS, notices";
            _heartText.text = $"HR {Mathf.RoundToInt(bpm)} BPM";
            _heartBar.fillAmount = Mathf.InverseLerp(55f, 145f, bpm);
            _activityText.text = $"Steps {4200 + Mathf.RoundToInt(cycleTime * 24f)} | Goal {Mathf.RoundToInt(activity * 100f)}%";
            _activityBar.fillAmount = activity;
            _stressText.text = stress > 0.65f ? "Stress elevated" : "Stress low";
            _stressBar.fillAmount = stress;
            _gpsText.text = $"{speed:F1} mph\n38.954{Mathf.RoundToInt(Mathf.Abs(Mathf.Sin(cycleTime)) * 9)}, -95.255{Mathf.RoundToInt(Mathf.Abs(Mathf.Cos(cycleTime)) * 9)}";
            _headingText.text = $"{DegreesToCardinal(heading)} {heading:F0} deg | +/- {4f + Mathf.Abs(Mathf.Sin(cycleTime)) * 3f:F1} m";
            _notificationText.text = phase == 1 ? "Messages: Content hidden" : MockNotifications[_notificationIndex % MockNotifications.Length];
            _environmentText.text = $"{22f + Mathf.Sin(cycleTime * 0.25f) * 2f:F0} C | UV {(int)(3 + Mathf.PingPong(cycleTime * 0.2f, 3f))} | AQI {(int)(42 + Mathf.PingPong(cycleTime * 1.4f, 30f))}";
            _mediaText.text = $"Sprint Review Mix {Mathf.RoundToInt(Mathf.Repeat(cycleTime * 2.2f, 100f))}%";
            _systemText.text = $"Headset {Mathf.RoundToInt(battery * 100f)}% | Phone {Mathf.RoundToInt((battery - 0.12f) * 100f)}%";
            _performanceText.text = $"72 FPS target | {Mathf.RoundToInt(78f + Mathf.Sin(cycleTime) * 5f)} mock";
            _privacyText.text = phase == 1 ? "Redaction ON\nLocal only" : "Local only\nCloud OFF";
            _radarText.text = phase == 3 ? "Friend 8 m\nWaypoint 22 m" : "Friend 18 m\nWaypoint 34 m";
            _meshStatusText.text = phase == 2 ? "NSDK 4.0: room tracking limited" : "NSDK 4.0: virtual room scan active";
            _meshObjectText.text = phase == 3 ? "Anchors: floor, wall, table, couch, moving person" : "Anchors: floor, wall, table, couch, screen";

            _batteryBar.fillAmount = battery;
            _signalBar.fillAmount = Mathf.Clamp01(signal);
            _privacyBar.fillAmount = privacy;
            _meshScanBar.fillAmount = _isConnected ? Mathf.PingPong(cycleTime * 0.18f, 1f) : 0.28f;
        }

        private void PushMockDataToHud(float cycleTime, int phase)
        {
            if (hudController == null)
            {
                return;
            }

            long timestamp = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            int bpm = Mathf.RoundToInt(72f + Mathf.Sin(cycleTime * 1.2f) * 6f + (phase == 3 ? 18f : 0f));
            float heading = Mathf.Repeat(cycleTime * 18f, 360f);

            hudController.UpdateWidget("heart_rate", new HeartRateWidgetData
            {
                Bpm = bpm,
                IsValid = _isConnected,
                TimestampMs = timestamp,
                Source = DataSource.Mock
            });

            hudController.UpdateWidget("gps", new GpsWidgetData
            {
                SpeedMph = Mathf.Max(0f, 3.1f + Mathf.Sin(cycleTime * 0.8f) * 0.7f),
                HeadingDegrees = heading,
                Latitude = 38.9543 + Mathf.Sin(cycleTime * 0.2f) * 0.0001,
                Longitude = -95.2558 + Mathf.Cos(cycleTime * 0.2f) * 0.0001,
                HasFix = _isConnected,
                TimestampMs = timestamp,
                Source = DataSource.Mock
            });

            hudController.UpdateWidget("notifications", new NotificationWidgetData
            {
                AppName = phase == 1 ? "Messages" : "Calendar",
                Title = phase == 1 ? "Content hidden" : MockNotifications[_notificationIndex % MockNotifications.Length],
                IsRedacted = phase == 1,
                TimestampMs = timestamp,
                Source = DataSource.Mock
            });

            _notificationIndex++;
        }

        private void PublishMockEvents(float cycleTime, int phase)
        {
            float activity = Mathf.PingPong(cycleTime * 0.035f, 0.42f) + 0.43f;
            float stress = Mathf.Clamp01(0.18f + (phase == 3 ? 0.52f : 0f));

            WidgetEventBus.Publish(new ActivityEvent(1, 4200 + Mathf.RoundToInt(cycleTime * 24f), 520, 38, activity));
            WidgetEventBus.Publish(new EnvironmentDataEvent(1, 22f + Mathf.Sin(cycleTime * 0.25f) * 2f, 3, 42));
            WidgetEventBus.Publish(new MediaPlaybackEvent(1, "Sprint Review Mix", "HUD-Link", Mathf.Repeat(cycleTime / 40f, 1f), true));
            WidgetEventBus.Publish(new HRVStressEvent(1, stress, 62f - stress * 18f));
            WidgetEventBus.Publish(new SystemStateEvent(1, 0.76f, 0.88f, _isConnected ? 92 : 28));
            WidgetEventBus.Publish(new ProximityAlertEvent(1, "teammate-alpha", phase == 3 ? 8f : 18f, new Vector2(Mathf.Sin(cycleTime), Mathf.Cos(cycleTime))));
        }

        private void AddEvent(string message)
        {
            string compactMessage = message.Length > 28 ? message.Substring(0, 25) + "..." : message;
            string stampedMessage = $"{System.DateTime.Now:HH:mm} {compactMessage}";
            _eventLog.Enqueue(stampedMessage);
            while (_eventLog.Count > 2)
            {
                _eventLog.Dequeue();
            }

            string[] rows = _eventLog.ToArray();
            for (int i = 0; i < _eventRows.Count; i++)
            {
                _eventRows[i].text = i < rows.Length ? rows[rows.Length - 1 - i] : string.Empty;
            }
        }

        private string DegreesToCardinal(float degrees)
        {
            string[] directions = { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };
            int index = Mathf.RoundToInt(degrees / 45f) % directions.Length;
            return directions[index];
        }

        private Material CreateMaterial(string name, Color color)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }

            Material material = new Material(shader);
            material.name = name;
            material.color = color;
            return material;
        }
    }
}
