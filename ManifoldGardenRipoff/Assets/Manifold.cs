using UnityEngine;
using System.Collections;

/*
    Notes:

    Next level
    |_Let the player go where ever they please but keep a cube of duplicates around consistently
        |_So, if the player is at (0,0,0), for example: they have 1 in each direction, when they fall one, they will still have 1 in each direction.
            |_Bottom layer is added
            |_Top layer is deleted
         |_So for sideways movement
            |_Layer toward movement side is added
            |_Layer behind movement side is removed


    Cube of 1 = 0   distance
            3 = 100

    VR game --> mirror room
                |_whatever you do is replicated
                |_if every main is allowed to interact with the others then you can grab objects outside the room... (harder)


    OKAY, MAKE SURE to change the player check to teleport the player more porportionally.
    |_This means that if it detects that the player is out of bounds, find out by how much and use (2*totalSize)-thisAmount to teleport the player

    FATAL NOTE: If check for bounds is more than doubled, then it can spawn player outside box. 


    Go to other level idea - thanks greg for being the father
    |_ release player make them fall through to next block down with different shit
*/
namespace Manifold
{
public class Manifold : MonoBehaviour
{

    public bool __DEBUG__ = true;
    public GameObject boundsGameObject;
    public GameObject playerObject;
    public float drawDistance = 200f;

    private Transform playerTransform;
    private Renderer boundsRenderer;
    private Bounds bounds;
    private Vector3 boundsCenter;
    private float boundsSize_X;
    private float boundsSize_Y;
    private float boundsSize_Z;

    private Transform myTransform;
    private Quaternion myOriginalRotation;

    private static readonly string[] axises = { "X", "Y", "Z" };
    private static readonly string[] directions = { "Pos", "Neg" };


    // Use this for initialization
    void Awake()
    {
        SetInitialReferences();
    }
    void Start()
    {
        CheckVariables();

        if (__DEBUG__) { Debug.Log("I was made at: " + boundsCenter); }

        foreach (string axis in axises)
        {
            if (__DEBUG__) Debug.Log(boundsCenter + " | " + "Axis: " + axis);
            foreach (string dir in directions)
            {
                if (__DEBUG__) Debug.Log(boundsCenter + " | " + "Direction: " + dir);
                CreateToward(axis, dir);
            }
        }
    }

    void CheckVariables()
    {
        if (boundsCenter == new Vector3(0, 0, 0))
        {
            if (boundsSize_X != boundsSize_Y || boundsSize_X != boundsSize_Z || boundsSize_Y != boundsSize_Z)
            {
                Debug.Log("POSSIBLE FATAL ERROR: containment object is not a cube.");
            }
        }
    }

    void SetInitialReferences()
    {
        myTransform = transform;
        myOriginalRotation = transform.rotation;
        try
        {
            playerTransform = playerObject.transform;
        }
        catch
        {
            Debug.Log("ERROR: No playerObject found.");
        }
        try
        {
            boundsRenderer = boundsGameObject.GetComponent<Renderer>();
            bounds = boundsRenderer.bounds;
            boundsCenter = bounds.center;
            boundsSize_X = bounds.size.x;
            boundsSize_Y = bounds.size.y;
            boundsSize_Z = bounds.size.z;
        }
        catch
        {
            Debug.Log("ERROR: No renderer found.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (boundsCenter == new Vector3(0, 0, 0))
        {

            CheckAndUpdateTransformPosition(playerTransform, axises[0]);
            CheckAndUpdateTransformPosition(playerTransform, axises[1]);
            CheckAndUpdateTransformPosition(playerTransform, axises[2]);

        }
    }

    public void CheckAndUpdateTransformPosition(Transform objectTransform, string axis)
    {
        if (__DEBUG__) Debug.Log(boundsCenter + " | " + "I am checking " + objectTransform.gameObject + "'s position on " + axis);
        string boundsCase = axis;
        float objectPosition_specific = 0;
        float boundsCenter_specific = 0;
        float boundsSize_specific = 0;
        float boundsReleaseCords_specific = 0;
        Vector3 transformVec_Pos = new Vector3(0, 0, 0);
        Vector3 transformVec_Neg = new Vector3(0, 0, 0);
        switch (boundsCase)
        {
            case "X":
                if (__DEBUG__) Debug.Log(axis);
                objectPosition_specific = objectTransform.position.x;
                boundsCenter_specific = boundsCenter.x;
                //boundsCatchCords_specific = playerPosition_specific - boundsCenter_specific;
                boundsSize_specific = boundsSize_X;
                boundsReleaseCords_specific = Mathf.Abs(Mathf.Abs(boundsSize_specific) - Mathf.Abs(objectPosition_specific));
                transformVec_Neg = new Vector3(boundsCenter_specific - boundsReleaseCords_specific, objectTransform.position.y, objectTransform.position.z);
                transformVec_Pos = new Vector3(boundsCenter_specific + boundsReleaseCords_specific, objectTransform.position.y, objectTransform.position.z);
                break;
            case "Y":
                if (__DEBUG__) Debug.Log(axis);
                objectPosition_specific = objectTransform.position.y;
                boundsCenter_specific = boundsCenter.y;
                boundsSize_specific = boundsSize_Y;
                boundsReleaseCords_specific = Mathf.Abs(Mathf.Abs(boundsSize_specific) - Mathf.Abs(objectPosition_specific));
                transformVec_Neg = new Vector3(objectTransform.position.x, boundsCenter_specific - boundsReleaseCords_specific, objectTransform.position.z);
                transformVec_Pos = new Vector3(objectTransform.position.x, boundsCenter_specific + boundsReleaseCords_specific, objectTransform.position.z);
                break;
            case "Z":
                if (__DEBUG__) Debug.Log(axis);
                objectPosition_specific = objectTransform.position.z;
                boundsCenter_specific = boundsCenter.z;
                boundsSize_specific = boundsSize_Z;
                boundsReleaseCords_specific = Mathf.Abs(Mathf.Abs(boundsSize_specific) - Mathf.Abs(objectPosition_specific));
                transformVec_Neg = new Vector3(objectTransform.position.x, objectTransform.position.y, boundsCenter_specific - boundsReleaseCords_specific);//boundsCenter.z - (boundsSize_Z * 0.5f));
                transformVec_Pos = new Vector3(objectTransform.position.x, objectTransform.position.y, boundsCenter_specific + boundsReleaseCords_specific);
                break;
            default:
                if (__DEBUG__) Debug.Log(boundsCenter + " | " + "ERROR: Invalid case statement.");
                break;
        }
        if (objectPosition_specific <= boundsCenter_specific - (boundsSize_specific * 0.5f))
        {
            if (__DEBUG__) Debug.Log("Pos");
            objectTransform.position = transformVec_Pos;
        }
        else if (objectPosition_specific >= boundsCenter_specific + (boundsSize_specific * 0.5f))
        {
            if (__DEBUG__) Debug.Log("Neg");
            objectTransform.position = transformVec_Neg;
        }
    }

    void CheckAndUpdatePlayerPosition_MethodTwo(string axis)
    {
        Debug.Log(boundsCenter + " | " + "I am checking player position on " + axis);
        string boundsCase = axis;
        float playerPosition_specific = 0;
        float boundsCenter_specific = 0;
        float boundsSize_specific = 0;
        Vector3 transformVec_Pos = new Vector3(0, 0, 0);
        Vector3 transformVec_Neg = new Vector3(0, 0, 0);
        switch (boundsCase)
        {
            case "X":
                if (__DEBUG__) Debug.Log(axis);
                playerPosition_specific = playerTransform.position.x;
                boundsCenter_specific = boundsCenter.x;
                //boundsCatchCords_specific = playerPosition_specific - boundsCenter_specific;
                boundsSize_specific = boundsSize_X;
                transformVec_Neg = new Vector3(boundsCenter_specific - (boundsSize_specific * 0.5f), playerTransform.position.y, playerTransform.position.z);
                transformVec_Pos = new Vector3(boundsCenter_specific + (boundsSize_specific * 0.5f), playerTransform.position.y, playerTransform.position.z);
                break;
            case "Y":
                if (__DEBUG__) Debug.Log(axis);
                playerPosition_specific = playerTransform.position.y;
                boundsCenter_specific = boundsCenter.y;
                boundsSize_specific = boundsSize_Y;
                transformVec_Neg = new Vector3(playerTransform.position.x, boundsCenter_specific - (boundsSize_specific * 0.5f), playerTransform.position.z);
                transformVec_Pos = new Vector3(playerTransform.position.x, boundsCenter_specific + (boundsSize_specific * 0.5f), playerTransform.position.z);
                break;
            case "Z":
                if (__DEBUG__) Debug.Log(axis);
                playerPosition_specific = playerTransform.position.z;
                boundsCenter_specific = boundsCenter.z;
                boundsSize_specific = boundsSize_Z;
                transformVec_Neg = new Vector3(playerTransform.position.x, playerTransform.position.y, boundsCenter_specific - (boundsSize_specific * 0.5f));
                transformVec_Pos = new Vector3(playerTransform.position.x, playerTransform.position.y, boundsCenter_specific + (boundsSize_specific * 0.5f));
                break;
            default:
                if (__DEBUG__) Debug.Log(boundsCenter + " | " + "ERROR: Invalid case statement.");
                break;
        }
        if (playerPosition_specific <= boundsCenter_specific - (boundsSize_specific * 0.5f))
        {
            if (__DEBUG__) Debug.Log("Pos");
            playerTransform.position = transformVec_Pos;
        }
        else if (playerPosition_specific >= boundsCenter_specific + (boundsSize_specific * 0.5f))
        {
            if (__DEBUG__) Debug.Log("Neg");
            playerTransform.position = transformVec_Neg;
        }
    }

    void CreateToward(string axis, string pos_neg)
    {
        // axis is "X","Y",or"Z"
        // pos_neg is "Pos" or "Neg"
        if (__DEBUG__) { Debug.Log(boundsCenter + " | " + "I entered " + pos_neg + axis); }
        string boundsCase = axis;
        bool boundsCenterCheck = false;
        float boundsCenter_specific = 0;
        float boundsSize_specific = 0;
        Vector3 transformVec = new Vector3(0, 0, 0);
        Vector3 spawnVec = new Vector3(0, 0, 0);

        switch (boundsCase)
        { // axis specific (X,Y, or Z)
            case "X": // X
                boundsCenter_specific = boundsCenter.x;
                boundsSize_specific = boundsSize_X;
                transformVec = new Vector3(boundsSize_specific, 0, 0);
                break;
            case "Y":
                boundsCenter_specific = boundsCenter.y;
                boundsSize_specific = boundsSize_Y;
                transformVec = new Vector3(0, boundsSize_specific, 0);
                break;
            case "Z":
                boundsCenter_specific = boundsCenter.z;
                boundsSize_specific = boundsSize_Z;
                transformVec = new Vector3(0, 0, boundsSize_specific);
                break;
            default:
                Debug.Log(boundsCenter + " | " + "Invalid case statement");
                break;
        }
        if (pos_neg == "Pos")
        { // pos_neg specific
            boundsCenterCheck = boundsCenter_specific <= drawDistance;
            spawnVec = boundsCenter + transformVec;
        }
        else if (pos_neg == "Neg")
        {
            boundsCenterCheck = boundsCenter_specific >= -drawDistance;
            spawnVec = boundsCenter - transformVec;
        }
        if (boundsCenterCheck)
        {
            if (__DEBUG__) Debug.Log(boundsCenter + " | " + "I entered bounds check.");
            if (Physics.CheckSphere(spawnVec, boundsSize_specific * 0.25f))
            {
                if (__DEBUG__) Debug.Log(boundsCenter + " | " + "Found object at: " + spawnVec);// Do nothing
            }
            else
            {
                Instantiate(gameObject, spawnVec, myOriginalRotation);
                if (__DEBUG__) Debug.Log(boundsCenter + " | " + " I created one at " + spawnVec);
            }
        }
    }
}
}
