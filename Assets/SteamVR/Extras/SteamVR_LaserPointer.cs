using UnityEngine;
using UnityEngine.Events;

/*
 * NOTE: This should be in its own namespace, e.g. Valve.VR.SteamVR.Extras.
 * Unfortunately, all the SteamVR_*-classes are in the root namespace instead
 * of Valve.VR.SteamVR, so I'd have to change all of that.
 * But at least moved PointerEventArgs and PointerEventHandler into 
 * SteamVR_LaserPointer to prevent naming conflicts ("PointerEventArgs" is
 * a very generic name which could easily cause naming conflicts!)
 */

public class SteamVR_LaserPointer : MonoBehaviour
{
    public struct PointerEventArgs
    {
        public uint controllerIndex;
        public uint flags;
        public float distance;
        public Transform target;
    }

    public delegate void PointerEventHandler(object sender, PointerEventArgs e);

    [System.Serializable]
    public class PointerEvent : UnityEvent<PointerEventArgs> { }

    /*
        * removed active/isActive and using "enabled" instead
        * the approach active/isActive was only half implemented
        * and using enabled for this purpose seems more natural
        */
    //public bool active = true;
    //private bool isActive = false;
    public Color color = new Color(0.0F, 0.8F, 1F); // TODO: Default to color in Steam dashboard
    public float thickness = 0.0001F;
    public float thicknessPressed = 0.01F; // added instead of hardcoded *5
    public GameObject holder;
    public GameObject pointer;
    public bool addRigidBody = false;
    //public Transform reference; // not used => removed

    // this is more efficient but can only be used from code
    public event PointerEventHandler PointerIn;
    public event PointerEventHandler PointerOut;

    // this adds a little overhead but can be assigned via Unity editor
    public PointerEvent unityPointerIn = new PointerEvent();
    public PointerEvent unityPointerOut = new PointerEvent();

    protected Transform previousContact = null;

    private SteamVR_TrackedController controller; // used in every Update

    // using Awake() instead of Start() because Awake() is called before OnEnable!
    public void Awake()
    {
        controller = GetComponent<SteamVR_TrackedController>();

        // only if it was not set up already (i.e. you could use your own)
        if (holder == null)
        {
            holder = new GameObject("Holder"); // "New Game Object" is not a good name ;-)
            holder.transform.parent = this.transform;
            holder.transform.localPosition = Vector3.zero;
            holder.transform.localRotation = Quaternion.identity; // added
        }
        holder.gameObject.SetActive(false); // disable until OnEnable() is called

        // only if it was not set up already (i.e. you could use your own)
        if (pointer == null)
        {
            pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pointer.name = "Pointer";
            pointer.transform.parent = holder.transform;
            pointer.transform.localPosition = new Vector3(0f, 0f, 50f);
            pointer.transform.localRotation = Quaternion.identity; // added

            /*
             * This requires that you have the shader Unlit/Color in the build.
             * To make sure this is the case, follow the instructions in the
             * manual: http://docs.unity3d.com/ScriptReference/Shader.Find.html
             */
            Material newMaterial = new Material(Shader.Find("Unlit/Color"));
            pointer.GetComponent<MeshRenderer>().material = newMaterial;

        }
        // this must be done also when you're using your own!
        pointer.transform.localScale = new Vector3(thickness, thickness, 100f);
        pointer.GetComponent<MeshRenderer>().material.SetColor("_Color", color);

        BoxCollider collider = pointer.GetComponent<BoxCollider>();
        if (addRigidBody)
        {
            if (collider)
            {
                collider.isTrigger = true;
            }
            Rigidbody rigidBody = pointer.AddComponent<Rigidbody>();
            rigidBody.isKinematic = true;
        }
        else
        {
            if (collider)
            {
                Object.Destroy(collider);
            }
        }
    }

    // we also need to properly handle the Dashboard
    public void OnEnable()
    {
        holder.gameObject.SetActive(true);
        SteamVR_Utils.Event.Listen("input_focus", OnInputFocus);
    }

    public void OnDisable()
    {
        holder.gameObject.SetActive(false);
        SteamVR_Utils.Event.Remove("input_focus", OnInputFocus);
    }

    private void OnInputFocus(params object[] args)
    {
        bool hasFocus = (bool)args[0];
        if (hasFocus)
        {
            holder.gameObject.SetActive(this.enabled);
        }
        else
        {
            holder.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // this whole thing fixed FROM HERE -->
        //if (!isActive && active) {
        //    isActive = true;
        //    // GetChild(0) fails when there's other children
        //    //this.transform.GetChild(0).gameObject.SetActive(true);
        //    holder.gameObject.SetActive(true);
        //} else if (isActive && !active) {
        //    isActive = false;
        //    holder.gameObject.SetActive(false);
        //}

        //if (!isActive) {
        //    return;
        //}
        // <-- UNTIL HERE ;-)

        float dist = 100f;

        // don't do GetComponent<>() each Update - moved to Start!
        //SteamVR_TrackedController controller = GetComponent<SteamVR_TrackedController>();

        Ray raycast = new Ray(transform.position, transform.forward);
        RaycastHit hitInfo;
        bool hasTarget = Physics.Raycast(raycast, out hitInfo);

        if (previousContact && previousContact != hitInfo.transform)
        {
            PointerEventArgs argsOut = new PointerEventArgs(); // args => argsOut for consistency
            if (controller != null)
            {
                argsOut.controllerIndex = controller.controllerIndex;
            }
            argsOut.distance = 0f;
            argsOut.flags = 0;
            argsOut.target = previousContact;
            OnPointerOut(argsOut);
            previousContact = null;
        }

        if (hasTarget && previousContact != hitInfo.transform)
        {
            PointerEventArgs argsIn = new PointerEventArgs();
            if (controller != null)
            {
                argsIn.controllerIndex = controller.controllerIndex;
            }
            argsIn.distance = hitInfo.distance;
            argsIn.flags = 0;
            argsIn.target = hitInfo.transform;
            OnPointerIn(argsIn);
            previousContact = hitInfo.transform;
        }

        if (!hasTarget)
        {
            previousContact = null;
        }
        else if (hitInfo.distance < 100f)
        {
            dist = hitInfo.distance;
        }

        pointer.transform.localPosition = new Vector3(0f, 0f, dist / 2f);

        float currentThickness = controller != null && controller.triggerPressed
            ? thicknessPressed : thickness;
        pointer.transform.localScale = new Vector3(currentThickness, currentThickness, dist);
    }

    public virtual void OnPointerIn(PointerEventArgs e)
    {
        if (PointerIn != null)
        {
            PointerIn(this, e);
        }
        unityPointerIn.Invoke(e);
    }

    public virtual void OnPointerOut(PointerEventArgs e)
    {
        if (PointerOut != null)
        {
            PointerOut(this, e);
        }
        unityPointerOut.Invoke(e);
    }

}