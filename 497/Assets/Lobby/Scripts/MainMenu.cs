﻿using UnityEngine;

namespace Mirror
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private NetworkManagerTennis networkManager = null;

        [Header("UI")]
        [SerializeField] private GameObject landingPagePanel = null;

        public void HostLobby()
        {
            networkManager.StartHost();

            landingPagePanel.SetActive(false);
        }
    }
}
