using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace Mirror
{
    public class UIPlayer : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text;
        CharacterNetwork player;

        public void SetPlayer(CharacterNetwork player)
        {
            this.player = player;
            text.text = "Player" + player.playerIndex.ToString();
        }
    }
}

