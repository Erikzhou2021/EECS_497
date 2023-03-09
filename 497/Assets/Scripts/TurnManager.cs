using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Mirror
{
    public class TurnManager : NetworkBehaviour
    {
        List<CharacterNetwork> players = new List<CharacterNetwork>();
        public void AddPlayer (CharacterNetwork _player)
        {
            players.Add(_player);
        }
    }

}
