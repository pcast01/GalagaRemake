using UnityEngine;
using System.Collections;

public static class GalagaHelper {

    public enum EntranceFlightPatterns
    {
        round1_DownLeft,
        round1_DownRight,
        rd1_LeftUp,
        rd1_RightUp,
        rd1_TopLeft,
        rd1_TopRight,
        rd1_Left,
        rd1_Right
    }

    public enum Formations
    {
        Round1Phase1,
        Round1Phase2,
        Round1Phase3_1,
        Round1Phase4_1,
        Round1Phase5_1
    }

    public static int EnemiesSpawned;
    public static Formations CurrentRoundPhase = Formations.Round1Phase1;
    public static Transform RespawnPoint(string formationName)
    {
        GameObject spawnPoint = new GameObject();
        if (formationName == "Round1Phase1EnemyFormation")
        {
            spawnPoint = GameObject.FindGameObjectWithTag("Respawn");
        }
        else if (formationName == "Round1Phase2EnemyFormation")
        {
            spawnPoint = GameObject.FindGameObjectWithTag("Spawn_BottomLeft");
        }
        else if (formationName == "Round1Phase3_1EnemyFormation")
        {
            spawnPoint = GameObject.FindGameObjectWithTag("Spawn_BottomRight");
        }
        else if (formationName == "Round1Phase4_1EnemyFormation")
        {
            spawnPoint = GameObject.FindGameObjectWithTag("Respawn");
        }
        else if (formationName == "Round1Phase5_1EnemyFormation")
        {
            spawnPoint = GameObject.FindGameObjectWithTag("Respawn");
        }
        return spawnPoint.transform; 
    }

    public static Transform[] EntrancePatterns(EntranceFlightPatterns pattern)
    {
        EnemySpawner spawner = GameObject.Find("Round1Phase1EnemyFormation").GetComponent<EnemySpawner>();
        GameObject form2 = GameObject.FindGameObjectWithTag("phase2").gameObject;
        GameObject form31 = GameObject.FindGameObjectWithTag("phase31").gameObject;
        GameObject spawnPt = GameObject.FindGameObjectWithTag("Respawn");
        GameObject middlePt = GameObject.FindGameObjectWithTag("Point2");
        GameObject rightEntrancePt = GameObject.FindGameObjectWithTag("begin_Right");
        GameObject leftEntrancePt = GameObject.FindGameObjectWithTag("begin_Left");
        GameObject bottomLeftPt = GameObject.FindGameObjectWithTag("Spawn_BottomLeft");
        GameObject bottomRightPt = GameObject.FindGameObjectWithTag("Spawn_BottomRight");
        // Get next spawn position from spawner script on current formation.
        Transform nextSpawnPos = spawner.currentSpawnPos;
        if (EnemiesSpawned > 8 && EnemiesSpawned < 17) // Round 2
        {
            EnemySpawner form2Spawn = GameObject.FindGameObjectWithTag("phase2").GetComponent<EnemySpawner>();
            nextSpawnPos = form2Spawn.currentSpawnPos;
        }
        else if(EnemiesSpawned > 16 && EnemiesSpawned < 25) // Round3_1
        {
            EnemySpawner form31Spawn = GameObject.FindGameObjectWithTag("phase31").GetComponent<EnemySpawner>();
            nextSpawnPos = form31Spawn.currentSpawnPos;
        }
        else if (EnemiesSpawned > 24 && EnemiesSpawned < 33)
        {
            EnemySpawner form41Spawn = GameObject.FindGameObjectWithTag("phase41").GetComponent<EnemySpawner>();
            nextSpawnPos = form41Spawn.currentSpawnPos;
        }
        else if (EnemiesSpawned > 32 && EnemiesSpawned < 41)
        {
            EnemySpawner form51Spawn = GameObject.FindGameObjectWithTag("phase51").GetComponent<EnemySpawner>();
            nextSpawnPos = form51Spawn.currentSpawnPos;
        }

        switch (pattern)
        {
            case EntranceFlightPatterns.round1_DownLeft:
                Transform[] rd1_Flight1 = new Transform[5];
                rd1_Flight1[0] = spawnPt.transform;
                rd1_Flight1[1] = middlePt.transform;
                rd1_Flight1[2] = leftEntrancePt.transform;
                rd1_Flight1[3] = spawner.gameObject.transform;
                rd1_Flight1[4] = nextSpawnPos;
                return rd1_Flight1;
                break;
            case EntranceFlightPatterns.round1_DownRight:
                Transform[] rd1_Flight2 = new Transform[5];
                rd1_Flight2[0] = spawnPt.transform;
                rd1_Flight2[1] = middlePt.transform;
                rd1_Flight2[2] = rightEntrancePt.transform;
                rd1_Flight2[3] = spawner.gameObject.transform;
                rd1_Flight2[4] = nextSpawnPos;
                return rd1_Flight2;
                break;
            case EntranceFlightPatterns.rd1_Left:
                Transform[] rd1_Flight3 = new Transform[5];
                rd1_Flight3[0] = bottomLeftPt.transform;
                rd1_Flight3[1] = leftEntrancePt.transform;
                rd1_Flight3[2] = middlePt.transform;
                rd1_Flight3[3] = form2.transform;
                rd1_Flight3[4] = nextSpawnPos;
                return rd1_Flight3;
                break;
            case EntranceFlightPatterns.rd1_Right:
                Transform[] rd_Flight4 = new Transform[5];
                rd_Flight4[0] = bottomRightPt.transform;
                rd_Flight4[1] = rightEntrancePt.transform;
                rd_Flight4[2] = middlePt.transform;
                rd_Flight4[3] = form31.transform;
                rd_Flight4[4] = nextSpawnPos;
                return rd_Flight4;
                break;
            case EntranceFlightPatterns.rd1_TopLeft:
                Transform[] rd1_Flight5 = new Transform[5];
                rd1_Flight5[0] = spawnPt.transform;
                rd1_Flight5[1] = middlePt.transform;
                rd1_Flight5[2] = bottomLeftPt.transform;
                rd1_Flight5[3] = leftEntrancePt.transform;
                rd1_Flight5[4] = nextSpawnPos;
                return rd1_Flight5;
                break;
            case EntranceFlightPatterns.rd1_TopRight:
                Transform[] rd1_Flight6 = new Transform[5];
                rd1_Flight6[0] = spawnPt.transform;
                rd1_Flight6[1] = middlePt.transform;
                rd1_Flight6[2] = bottomRightPt.transform;
                rd1_Flight6[3] = rightEntrancePt.transform;
                rd1_Flight6[4] = nextSpawnPos;
                return rd1_Flight6;
                break;
            default:
                Transform[] test = new Transform[2];
                test[0] = spawnPt.transform;
                test[1] = middlePt.transform;
                return test;
                break;
        }
    }

}
