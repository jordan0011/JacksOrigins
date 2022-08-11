using UnityEngine;
using Photon.Bolt;

public class PlayerRenderer : EntityBehaviour<INewPlayerState>
{
    [SerializeField]
    private MeshRenderer _meshRenderer;
    [SerializeField]
    private GameObject _Rendererbject;
    private PlayerMotor _playerMotor;
    [SerializeField]
    private Color _enemyColor;
    [SerializeField]
    private Color _allyColor;

    [SerializeField]
    private Transform _camera;
    private Transform _sceneCamera;

    [SerializeField]
    private TextMesh _textMesh;

    [SerializeField]
    private GameObject cameraFreeLook;
    [SerializeField]
    private GameObject _worldCanvas;

    private void Awake()
    {
        _playerMotor = GetComponent<PlayerMotor>();

    }
    public void Init()
    {
        _Rendererbject.SetActive(BoltNetwork.IsClient);
        Debug.Log(BoltNetwork.IsClient);
        if (entity.IsControllerOrOwner)
        {
            _camera.gameObject.SetActive(true);
            cameraFreeLook.SetActive(true);
        }

        if (entity.HasControl)
        {
            _sceneCamera = GameObject.FindGameObjectWithTag("GameController").GetComponent<PlayerSetupController>().SceneCamera.transform;
            _sceneCamera.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            PlayerToken pt = (PlayerToken)entity.AttachToken;
            _textMesh.text = pt.name;
            //_meshRenderer.gameObject.SetActive(true);

            /*if (_playerMotor.IsEnemy)
            {
                _meshRenderer.material.color = _enemyColor;
                _textMesh.gameObject.SetActive(false);
            }
            else
            {
                _textMesh.gameObject.SetActive(true);
                _meshRenderer.material.color = _allyColor;
            }
            */
        }
    }
    public GameObject GetWorldCanvas()
    {
        return _worldCanvas;
    }
    public void OnDeath(bool b)
    {
        if (BoltNetwork.IsClient)
        {
            if (b)
            {
                _meshRenderer.gameObject.SetActive(false);

                if (entity.HasControl)
                {
                    _sceneCamera.gameObject.SetActive(true);
                }

                _worldCanvas.gameObject.SetActive(false);

                _camera.gameObject.SetActive(false);
                _textMesh.gameObject.SetActive(false);

            }
            else
            {
                _meshRenderer.gameObject.SetActive(true);

                if (entity.IsControllerOrOwner)
                    _camera.gameObject.SetActive(true);

                if (entity.HasControl)
                {
                    _sceneCamera.gameObject.SetActive(false);
                }
                else
                {
                   // _worldCanvas.gameObject.SetActive(true);

                    if (!_playerMotor.IsEnemy)
                    {
                        _textMesh.gameObject.SetActive(true);
                        //_meshRenderer.material.color = _allyColor;
                    }
                }
            }
        }
    }
}
