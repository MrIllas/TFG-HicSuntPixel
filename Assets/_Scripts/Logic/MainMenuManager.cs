using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Globals;
namespace Menus
{
    public class MainMenuManager : MonoBehaviour
    {
        public void StartNewGame()
        {
            StartCoroutine(Globals.WorldSaveGameManager.instance.LoadNewGame());
        }
    }
}

