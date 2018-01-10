using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Z_Grappling : MonoBehaviour
{
    public Transform grapStart;    //point de départ du grappin
    public float grapRange = 1000;    //La portée initiale du grappin
    public float vectorTolerance = 0.01f;    //Plus la tolérance est élevée, moins il va créer de nouveaux points lorsqu'il rencontrera des obstacles proches de sont dernier point d'ancrage
    public float angleLimit = 3;    //l'angle limite pour vérifier que deux points du grappin sont bien alignés avant d'en supprimer 1
    public float limit_distanceGrap = 0;    //la _distance limite entre le grappin et le lanceur pour casser automatiquement le grappin si celui si est lancé.
    public float drawSpeed = 2;     //vitesse d'animation du grappin
    public float forceGrap = 30;    //la force de traction du grappin lorsqu'on utilise le bouton spécial pour le "rembobiner"
    public float grapDirTolerance = 93;    //l'angle dans lequel on peut se rapprocher du grappin lorsqu'on avance vers l'origine.
    public GameObject grapHook;     //L'objet instantié lors de la collision lorsqu'on lance le grappin
    public GameObject grapPoint;    //le gameobject qu'on instancie en child de l'obstacle lorsque le grappin rencontre quelque chose
    public string grapObjectTag = "grapObj"; //le tag des objets sur lesquels on peut lancer le grappin 
	public float airControlStrength = 10;
	public float airSlowingStrength = 0.1f;
	public GameObject grapParticles;

	private Camera _camera;
    private float _grapLength;    //La longueur totale du grappin une fois lancé
    private LineRenderer _grapRenderer;    //Le line renderer pour voir le grappin
    private bool _grapOut;    //booléenne pour savoir si le grappin est lancé.
    private GameObject _grapBody;    //une variable pour stocker le grapHook instancié
    private List<Vector3> _listPos = new List<Vector3>();    //La liste des positions du grappin
    private List<GameObject> _listGrapPoints = new List<GameObject>();    //La liste des gameObjects vides qu'on instancie lorsque le grappin rencontre quelque chose
    private float _grapLastPart;    //_distance entre les deux derniers points du grappin, pour les raycasts).
    private bool _isLaunching;    //bool pour savoir si le grappin est en train d'être lancé.
    private bool _isRewinding;    //bool pour savoir si le grappin est rewindé;
    private int _rewindPos;    //int pour compter à quelle position le rewind du grappling en est
    private float _dist; //variable intervenant lors de l'anim du linerenderer
    private float _counter; //variable intervenant lors de l'anim du linerenderer --> lorsque >= 1, le grappin a atteint sa cible
    private bool _grapReady;    //bool pour savoir si on peut réutiliser le grappin
	private CharacterController _controller; //characterController pour chopper la velocité
	private bool _inAir; //pour savoir quand activer et desactiver le RB.
	private float _inAirCounter; //pour savoir combien de temps s'écoule pendant que le perso est dans les airs.
	private Rigidbody _rb;
	private GameObject _grapParticles;

    void Start()
    {
		_camera = FindObjectOfType<PoS_Camera> ().GetComponent<Camera> ();
        _grapRenderer = null;
        _grapBody = null;
        _grapOut = false;
        _grapReady = true;
        _isLaunching = true;
        _isRewinding = false;
		_controller = gameObject.GetComponent<CharacterController>();
		_inAir = false;
		_rb = GetComponent<Rigidbody>();
    }


    void Update()
    {
        //si on click gauche sur la souris et qu'on maintient le click...
        if (Input.GetKeyDown(KeyCode.Mouse0) && _grapOut == false && _grapReady == true && _isRewinding == false)
        {
			Ray ray = _camera.ScreenPointToRay(new Vector2(Screen.width/2,Screen.height/2));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                RaycastGrappling(hit.point - grapStart.position);
            }
        }

        //...si on relâche le click gauche de la souris
        if (Input.GetKey(KeyCode.Mouse0) == false || _isRewinding == true)
        {
            if (_grapReady == false)
            {
                _grapReady = true;
            }
            if (_grapOut == true)
            {
                AnimateGrapplingBack();
            }
        }

        //lorsque le grappin est activé :
        if (_grapOut == true)
        {
            //La dernière position, donc la numéro _listPos.count - 1, est celle de la voiture (plus un léger offset en hauteur pour que ça ne soit pas au ras du sol)
            _listPos[_listPos.Count - 1] = grapStart.position;

            AnimateGrapplingLaunch();

            //ça permet de bouger vers l'origine du grappin, si on se dirige par là avec le perso (et ne pas être bloqué, comme si le grappin était rigide)
			float angleDir = Vector3.Angle(_rb.velocity, (_listPos[_listPos.Count - 2] - _listPos[_listPos.Count - 1]));
            if (angleDir < grapDirTolerance)
            {
                _grapBody.GetComponent<ConfigurableJoint>().connectedBody = null;
            }

            else if (_counter >= 1 && _isRewinding == false)
            {
				_grapBody.GetComponent<ConfigurableJoint>().connectedBody = _rb;
            }

            if (_grapBody.transform.parent == null)
            {
                _grapBody.GetComponent<Rigidbody>().isKinematic = false;
            }

            //On tire un raycast entre l'avant dernier et le dernier point d'ancrage du grappin, s'il rencontre un obstacle, on crée un nouveau
            // point qui deviendra le point de référence pour ce rayscast
            _grapLastPart = Vector3.Distance(_listPos[_listPos.Count - 1], _listPos[_listPos.Count - 2]);
            RaycastHit hit;
            if (Physics.Raycast(grapStart.position, _listPos[_listPos.Count - 2] - _listPos[_listPos.Count - 1], out hit, _grapLastPart))
            {
                if (hit.collider.tag != "Player" && Vector3.Distance(hit.point, _listPos[_listPos.Count - 2]) > vectorTolerance && _isRewinding == false)
                {
                    //on instancie le grapPoint qui va etre en child de l'objet touché (et ainsi se déplacer avec lui)
                    GameObject grapPointTemp;
                    grapPointTemp = Instantiate(grapPoint, hit.point + hit.normal*0.1f, Quaternion.identity) as GameObject;
                    _listGrapPoints.Add(grapPointTemp);
                    _listPos.Add(grapPointTemp.transform.position);
                    grapPointTemp.transform.SetParent(hit.collider.transform);
                    //Les 4 lignes bordéliques qui suivent c'est pour rentrer le point au bon endroit dans la list et décaler la position de la voiture
                    Vector3 intermediaire;
                    intermediaire = _listPos[_listPos.Count - 1];
                    _grapBody.GetComponent<ConfigurableJoint>().anchor = _grapBody.transform.InverseTransformPoint(intermediaire);
                    _listPos[_listPos.Count - 1] = grapStart.position;
                    _listPos[_listPos.Count - 2] = intermediaire;
                }
            }

            //on update les positions de la _listPos grâce à la liste de _listGrapPoints
            for (int i = 0; i < _listPos.Count - 1; i++)
            {
                _listPos[i] = _listGrapPoints[i].transform.position;
            }

            //On calcule également la longueur du grappin en faisant une opération de type (A+B)+(B+C)+(C+D) ...
            //Pas utilisée pour le moment mais laissé car p-e utile plus tard ?
            for (int i = 0; i < _listPos.Count - 2; i++)
            {
                _grapLength += (_listPos[i + 1] - _listPos[i]).magnitude;
            }

            //la partie en dessous permet de "défaire" les noeuds du grappin pour ne pas avoir la physique d'une corde collante, lorsqu'on se désenroule d'un objet par exemple
            RaycastHit hit2;
            if (_listPos.Count >= 3)
            {
                if (Physics.Raycast(grapStart.position, _listPos[_listPos.Count - 3] - _listPos[_listPos.Count - 1], out hit2, grapRange))
                {
                    if (hit2.collider.tag != "Player" && Vector3.Distance(hit2.point, _listPos[_listPos.Count - 3]) < vectorTolerance && Vector3.Angle(_listPos[_listPos.Count - 2] - _listPos[_listPos.Count - 1], _listPos[_listPos.Count - 3] - _listPos[_listPos.Count - 1]) < angleLimit)
                    {
                        _listPos.Remove(_listPos[_listPos.Count - 2]);
                        _listGrapPoints.Remove(_listGrapPoints[_listPos.Count - 1]);
                        _grapBody.GetComponent<ConfigurableJoint>().anchor = _grapBody.transform.InverseTransformPoint(_listPos[_listPos.Count - 2]);
                    }
                }
            }

            //On check si la _distance avec le dernier point du grappin est trop petite, on casse le grappin
            if (_grapLastPart <= limit_distanceGrap && _grapLastPart != 0)
            {
                StartCoroutine(DestroyGrappling(0));
            }

            //gère l'utilisation du "rewind vers l'origine du grappin"
            if (Input.GetKey(KeyCode.Mouse1) && _counter >= 1)
            {
                _grapReady = false;
                _grapBody.GetComponent<ConfigurableJoint>().connectedBody = null;
				_rb.AddForce(forceGrap * (_listPos[_listPos.Count - 2] - _listPos[_listPos.Count - 1]));
            }
        }

		if (_inAir)
		{
			CheckIfGroundBelow();
			if (!_grapOut)
			{
				InAirControl ();

				InAirRotation(3);
			}
		}

    }


    void LateUpdate()
    {
        //Si le grappin est lancé, on update à chaque frame les positions du line renderer
        if (_grapRenderer != null && _counter > 1)
        {
            _grapRenderer.positionCount = _listPos.Count;
            for (int i = 0; i < _listPos.Count; i++)
            {
                _grapRenderer.SetPosition(i, _listPos[i]);
            }
        }
    }


    /// <summary>
    /// Fonction pour lancer le grappin
    /// </summary>
    /// <param name="_dir"> direction du grappin</param>
    void RaycastGrappling(Vector3 _dir)
    {
        RaycastHit hit;
        if (Physics.Raycast(grapStart.position, _dir, out hit, grapRange) && _grapOut == false)
        {
            if (hit.collider.tag == grapObjectTag)
            {
                _grapOut = true;
                _listPos.Add(hit.point);
                _listPos.Add(grapStart.position);
                GameObject _hook;
                _hook = Instantiate(grapHook, hit.point, Quaternion.identity) as GameObject;
                _grapBody = _hook;
                _listGrapPoints.Add(_grapBody);
                _grapBody.transform.SetParent(hit.collider.transform);
				_grapBody.GetComponent<ConfigurableJoint>().connectedBody = _rb;
                _grapBody.GetComponent<ConfigurableJoint>().anchor = Vector3.zero;
                _grapRenderer = _hook.GetComponent<LineRenderer>();
                _grapLastPart = Vector3.Distance(_listPos[0], _listPos[1]);
                _dist = Vector3.Distance(hit.point, grapStart.position);
                _counter = 0;
                _grapBody.GetComponent<ConfigurableJoint>().connectedBody = null;
            }
        }
    }


    /// <summary>
    /// Fonction pour détruire le grappin et tous les objets instanciés
    /// </summary>
    /// <param name="t">temps à attendre avant la destruction</param>
    /// <returns></returns>
    IEnumerator DestroyGrappling(float _t)
    {
        yield return new WaitForSeconds(_t);
        _grapOut = false;
        Destroy(_grapBody);
        _grapRenderer = null;
        _grapBody = null;
        _listPos.Clear();
        //on supprime les instances des points du grappin;
        for (int i = 0; i < _listGrapPoints.Count - 1; i++)
        {
            Destroy(_listGrapPoints[i]);
        }
        _listGrapPoints.Clear();
        _isLaunching = true;
        _isRewinding = false;
		Destroy(_grapParticles,5);
    }


    /// <summary>
    /// Fonction pour animer le Line Renderer lorsque le grappin est lancé
    /// </summary>
    void AnimateGrapplingLaunch()
    {
		if (_counter == 0)
			_grapParticles = Instantiate (grapParticles, grapStart.position, Quaternion.identity) as GameObject;
		
        if (_counter < 1 && _isLaunching == true)
        {
            _counter += 0.1f * drawSpeed;
            float x = Mathf.Lerp(0, _dist, _counter);
            Vector3 pointA = grapStart.position;
            Vector3 pointB = _listPos[0];
            Vector3 pointAlongLine = x * Vector3.Normalize(pointB - pointA) + pointA;
            _grapRenderer.SetPosition(1, pointAlongLine);
            _grapRenderer.SetPosition(0, grapStart.position);
			_grapParticles.transform.position = pointAlongLine;
			//Debug.Log (_grapParticles.transform.position);
        }
        if (_counter >= 1 && _isLaunching == true)
        {
            _isLaunching = false;
			_grapBody.GetComponent<ConfigurableJoint>().connectedBody = _rb;
			StartSwinging();

            _rewindPos = 0;
        }
    }


    /// <summary>
    /// Fonction pour animer le Line Renderer lorsque le grappin se rembobine
    /// </summary>
    void AnimateGrapplingBack()
    {
        _isRewinding = true;
        int i = _listPos.Count - 1;
        _grapReady = false;
        //_grapBody.GetComponent<ConfigurableJoint>().connectedBody = null;
        if (_counter >= 0 && _isLaunching == false)
        {
            _dist = (_listPos[_listPos.Count - 2 - _rewindPos] - _listPos[_listPos.Count - 1 - _rewindPos]).magnitude;
            _counter -= 0.15f * drawSpeed;
            float x = Mathf.Lerp(0, _dist, _counter);
            Vector3 pointA = _listPos[_rewindPos + 1];
            Vector3 pointB = _listPos[_rewindPos];
            Vector3 pointAlongLine = x * Vector3.Normalize(pointB - pointA) + pointA;
            for (int k = 0; k <= _rewindPos; k++)
            {
                _grapRenderer.SetPosition(k, pointAlongLine);
            }
            _grapRenderer.SetPosition(_listPos.Count - 1, grapStart.position);
        }
        if (_counter <= 0 && _isLaunching == false && _rewindPos < i - 1)
        {
            _rewindPos++;
            _counter = 1;
        }
        if (_counter <= 0 && _isLaunching == false && _rewindPos >= i - 1)
        {
            _isLaunching = true;
            StartCoroutine(DestroyGrappling(0));
        }
    }


	//To use with CharacterController
	void StartSwinging()
	{
		_inAir = true;
		_rb.isKinematic = false;
		_rb.velocity = _controller.velocity;
		_controller.enabled = false;
		//gameObject.GetComponent<ThirdPersonController>().enabled = false;
		gameObject.GetComponent<CapsuleCollider>().enabled = true;
		_inAirCounter = 0;
		_rb.constraints = RigidbodyConstraints.None;
		Debug.Log ("none");
	}

	void CheckIfGroundBelow()
	{
		RaycastHit _hit;
		if (Physics.Raycast(transform.position, Vector3.down, out _hit, 0.05f*Mathf.Abs(_rb.velocity.y)) && _hit.collider.tag != "Player")
		{
			StopSwinging();	
			_rb.constraints = RigidbodyConstraints.FreezeAll;
			Debug.Log ("all");
		}
	}


	void StopSwinging()
	{
		_inAir = false;
		gameObject.GetComponent<CapsuleCollider>().enabled = false;
		_rb.isKinematic = true;
		//_rb.velocity = _controller.velocity;
		_controller.enabled = true;
		_controller.SimpleMove(Vector3.zero);

		//gameObject.GetComponent<ThirdPersonController>().enabled = true;
		transform.rotation = Quaternion.Euler(0,transform.rotation.eulerAngles.y, 0);
	}

	void InAirRotation (float _t)
	{
		if (_inAirCounter<_t)
		{
			_inAirCounter+=0.1f;
		}

		if (Mathf.Abs(transform.rotation.x) > 0.1f || Mathf.Abs(transform.rotation.z) > 0.1f)
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0,transform.rotation.y,0),_inAirCounter/_t);
		}
		else
			_rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
		

	}

	void InAirControl ()
	{
		Vector2 _dir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		//Transform _rot = GetComponent<ThirdPersonController>().rotator;
		//_rb.AddForce((_rot.right*_dir.x + _rot.forward*_dir.y)*airControlStrength);
		if (!_grapOut)
		{
			if (_rb.velocity.x > 0)
				_rb.velocity -= new Vector3(airSlowingStrength,0,0);
			if (_rb.velocity.z > 0)
				_rb.velocity -= new Vector3(0,0, airSlowingStrength);
		}
	}
}
