using System.Collections;
using System.Collections.Generic;
using System.Net;
using CBET;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardTest : MonoBehaviour {
    IEnumerator Start() {
        SendLeaderboard.Get().NewEntry(new LeaderboardEntry("Alexzander Bond", 5000, 69, 420));
        yield return new WaitForSeconds(1);
    }
}
