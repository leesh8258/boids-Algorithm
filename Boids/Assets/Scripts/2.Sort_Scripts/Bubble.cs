using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : Sort
{
    public override IEnumerator ExecuteSort(List<SortElement> sortList, float speed)
    {
        for(int i = 0; i < sortList.Count - 1; i++)
        {
            for(int j = 0; j < sortList.Count - i - 1; j++)
            {
                SetColor(sortList[j].prefab, Color.green);
                SetColor(sortList[j+1].prefab, Color.red);

                if (sortList[j].value > sortList[j+1].value)
                {
                    Swap(sortList, j, j+1);
                }

                yield return new WaitForSeconds(speed);

                SetColor(sortList[j].prefab, Color.black);
                SetColor(sortList[j + 1].prefab, Color.black);

            }
        }
    }
}
