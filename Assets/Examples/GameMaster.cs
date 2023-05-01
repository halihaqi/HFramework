using System;
using HFramework;
using UnityEngine;

namespace Examples
{
    public class GameMaster : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        [SerializeField] private Camera cam;
        
        private void Awake()
        {
            HEntry.Init();
            HEntry.ProcedureMgr.Initialize(new InitProcedure());
            HEntry.ProcedureMgr.StartProcedure<InitProcedure>();
        }

        private void Start()
        {
            var thirdCam = cam.gameObject.AddComponent<ThirdPersonCam>();
            player.AddComponent<ThirdPersonController>().BindCamera(cam, thirdCam);
            HEntry.AudioMgr.PlaySound("Audio/Background", false);
        }
    }
}
