using System;
using System.Collections;
using System.Collections.Generic;
using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using Unity.VisualScripting;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class Clicker : MonoBehaviour
{
    private GameObject GetObjectUnderMouse()
    {
        Camera camera = playerCamera.GetComponent<Camera>();
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        bool didHit = Physics.Raycast(ray, out RaycastHit hit, 1000.0f);
        if (!didHit) return null;

        return hit.transform.gameObject;
    }

    private void UpdateClicking()
    {
        if (priorClick_ != null) priorClick_.Dehighlight();

        if (stage_ == GameStage.Observation)
        {
            priorClick_ = null;
            return;
        }

        GameObject clickableObject = GetObjectUnderMouse();
        if (clickableObject == null)
        {
            priorClick_ = null;
            return;
        }

        bool isClickable = clickableObject.TryGetComponent<TileDirector>(out TileDirector director);

        if (!isClickable)
        {
            priorClick_ = null;
            return;
        }

        priorClick_ = director;
        director.Highlight();

        int side = stage_ == GameStage.PositivePlayerTurn ? 1 : -1;

        if (Input.GetMouseButtonDown((int)UnityEngine.UIElements.MouseButton.LeftMouse))
        {
            if (director.GetBalance() == 0 &&
                director.GetPrimaryTeam() == side)
            {
                director.ConvertToTower(side);
            }
        }

        if (Input.GetMouseButtonDown((int)UnityEngine.UIElements.MouseButton.RightMouse))
        {
            if (!director.IsTower())
            {
                director.ChangeBalance(side * 1);
            }
        }
    }

    void UpdateGameStage()
    {
        if (!Input.GetKeyDown(KeyCode.Space)) return;

        if (stage_ == GameStage.Observation)
        {
            stage_ = GameStage.PositivePlayerTurn;
            field_.Pause();
            pauseButton_.Lock();
            pauseButton_.UpdateState();

        }
        else if (stage_ == GameStage.PositivePlayerTurn)
        {
            stage_ = GameStage.NegativePlayerTurn;
        }
        else if (stage_ == GameStage.NegativePlayerTurn)
        {
            stage_ = GameStage.Observation;
            field_.Unpause();
            pauseButton_.Unlock();
            pauseButton_.UpdateState();
        }
    }

    private Vector2 RelativeMousePosition()
    {
        float posX = Input.mousePosition.x / Screen.width;
        float posY = Input.mousePosition.y / Screen.height;

        return new Vector2(posX - 0.5f, posY - 0.5f) * 2.0f;
    }

    void UpdateCameraPosition()
    {
        int index = (int)stage_;
        CameraConfig config = cameraMounts_[index];

        Camera camera = GetComponent<Camera>();

        float coefficient = (float)Math.Clamp(Time.deltaTime * transitionSpeed, 0.0, 1.0);

        transform.position =
            Vector3.Lerp(transform.position, config.position, coefficient);
        camera.fieldOfView =
            camera.fieldOfView * (1.0f - coefficient) +
            config.fov * coefficient;

        float mouseDelta = Input.GetAxis("Mouse ScrollWheel");

        cameraMounts_[index].fov = Math.Clamp(cameraMounts_[index].fov * (1.0f - mouseDelta), 10.0f, 100.0f);
        Vector2 mouseRelative = RelativeMousePosition();
        float aimAmount = aimDelta * (90.0f / config.fov);
        Quaternion aim =
            Quaternion.AngleAxis(-mouseRelative.y * aimAmount, Vector3.right) *
            Quaternion.AngleAxis(mouseRelative.x * aimAmount, Vector3.up);

        Quaternion rotationTarget = config.rotation * aim;

        transform.rotation =
            Quaternion.Lerp(transform.rotation, rotationTarget, coefficient);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateClicking();

        UpdateGameStage();

        UpdateCameraPosition();
    }

    void Start()
    {
        cameraMounts_[(int)GameStage.Observation] = new CameraConfig(neutralCameraOrigin);
        cameraMounts_[(int)GameStage.PositivePlayerTurn] = new CameraConfig(positiveCameraOrigin);
        cameraMounts_[(int)GameStage.NegativePlayerTurn] = new CameraConfig(negativeCameraOrigin);

        field_ = field.GetComponent<FieldGenerator>();
        pauseButton_ = pauseButton.GetComponentInChildren<PauseUnpauseBtn>();
    }

    public GameObject playerCamera;
    public GameObject field;

    private FieldGenerator field_;

    private TileDirector priorClick_ = null;

    private enum GameStage
    {
        Observation,
        PositivePlayerTurn,
        NegativePlayerTurn,
    }

    private struct CameraConfig
    {
        public CameraConfig(GameObject source)
        {
            position = source.transform.position;
            rotation = source.transform.rotation;
            fov = source.GetComponent<Camera>().fieldOfView;
        }

        public Vector3 position;
        public Quaternion rotation;
        public float fov;
    }

    private GameStage stage_ = GameStage.Observation;

    private CameraConfig[] cameraMounts_ = new CameraConfig[3];
    public GameObject neutralCameraOrigin;
    public GameObject positiveCameraOrigin;
    public GameObject negativeCameraOrigin;

    public float transitionSpeed = 5.0f;
    public float aimDelta = 3.0f;

    public GameObject pauseButton;
    private PauseUnpauseBtn pauseButton_;
}
