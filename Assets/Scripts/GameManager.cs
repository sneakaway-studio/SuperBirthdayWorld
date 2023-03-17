using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameManager
{

    public static List<int> bots;

    public static List<int> UpdateBots(int num = -1)
    {
        if (num > -1)
        {
            if (!bots.Contains(num))
            {
                bots.Add(num);
            }
        }
        bots.Sort();
        Debug.Log(bots);
        return bots;
    }


}
