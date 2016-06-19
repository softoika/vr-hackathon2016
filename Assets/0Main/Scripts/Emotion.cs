using UnityEngine;
using System.Collections;

public class Emotion : Photon.MonoBehaviour
{
    PhotonView photonViewE;
    // Use this for initialization
    void Start()
    {
        photonViewE = PhotonView.Get(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            photonView.RPC("Up", PhotonTargets.All);

        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            photonView.RPC("Down", PhotonTargets.All);
        }

    }
    [PunRPC]
    public void Up()
    {
    }
    [PunRPC]
    public void Down()
    {
    }
}
