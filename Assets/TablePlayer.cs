using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class TablePlayer : MonoBehaviour
{
   [System.Serializable] public class Chip
    {
        public int chipValue;
       // [HideInInspector]
        public int chipCount = 0;
        public Color chipColour;
        public Chip(int _chipValue, Color _chipColour)
        {
            chipValue = _chipValue;
            chipColour = _chipColour;
        }
        public Chip(Chip clone) {
            chipValue = clone.chipValue;
            chipColour = clone.chipColour;
        }
    }
    public CardPlayer cardPlayer;

    public List<Chip> chipList = new List<Chip>();
    public List<Chip> betChipList = new List<Chip>();

    
    [SerializeField] List<ChipStack> playerChipStackList = new List<ChipStack>();
    [SerializeField] List<ChipStack> betChipStackList = new List<ChipStack>();

    private void Start()
    {

    }

    public void Setup(int _clipValue)
    {
        foreach (Chip c in chipList)
            betChipList.Add(new Chip(c));
        CacluateCoinStack(chipList, _clipValue);
        ReorderChipStack(chipList, playerChipStackList);
        ReorderChipStack(betChipList, betChipStackList);
    }

    public void CacluateCoinStack(List<Chip> _chipList, int _chipValue) {
        int chipValue = _chipValue;
        for (int i = 0; i < _chipList.Count; i++)
        {
            bool highestCoinReached = false;
            _chipList[i].chipCount = 0;
            if (chipValue - (_chipList[i].chipValue * 9) > 0 && i < _chipList.Count - 1)
            {
                chipValue -= _chipList[i].chipValue * 9;
                _chipList[i].chipCount += 9;
            }
            else if (chipValue / _chipList[i].chipValue > 0)
            {
                int chipIncrement = chipValue / _chipList[i].chipValue;
                chipValue -= chipIncrement * _chipList[i].chipValue;
                _chipList[i].chipCount += chipIncrement;
                highestCoinReached = true;
            }
            else {
                highestCoinReached = true;
            }
             if (highestCoinReached && chipValue > 0) {
                for (int j = i-1; j > -1; j--)
                {
                    if (chipValue > 0 && chipValue >= _chipList[j].chipValue)
                    {
                        int chipIncrement = chipValue / _chipList[j].chipValue;
                        _chipList[j].chipCount += chipIncrement;
                        chipValue -= chipIncrement * _chipList[j].chipValue;
                    }
                    if (chipValue < 0)
                        break;
                }
            }
            if (chipValue < 0)
                break;
        }
    }
    void ReorderChipStack(List<Chip> _chipList,List<ChipStack> _chipStackList) {

        int coinStackCount = 0;
        foreach (Chip c in _chipList)
        {
            _chipStackList[coinStackCount].gameObject.SetActive(false);
            int chipStackSize = c.chipCount;

            float stackSize = 1;
            while (chipStackSize >=1)
            {
                if (chipStackSize < 10)
                    stackSize = chipStackSize / 10.0f;
                if(stackSize>0)
                    _chipStackList[coinStackCount].gameObject.SetActive(true);

                chipStackSize -= 10;

                _chipStackList[coinStackCount].transform.localScale = new Vector3(1, stackSize, 1);
                _chipStackList[coinStackCount].SetColour(c.chipColour);
                chipStackSize -= 10;
                coinStackCount++;
            }
        }
    }
    void ChipStackChange(List<Chip> _chipList,List<ChipStack> chipStackList)
    {
        Debug.Log("ChipStackChange");
        float stackSize = _chipList[0].chipCount / 10.0f;
        if (stackSize <= 0)
            chipStackList[0].gameObject.SetActive(false);
        else
        {
            chipStackList[0].gameObject.SetActive(true);
            chipStackList[0].transform.localScale = new Vector3(1, stackSize, 1);
        }
    }

    public void Bet(int _clipValue, int _betValue) {
        int dir = (int)Mathf.Sign(_betValue);
        chipList[0].chipCount -= 1 * dir;
        betChipList[0].chipCount += 1 * dir;
        UpdateChipCountAndStack(chipList, playerChipStackList, _clipValue);
        UpdateChipCountAndStack(betChipList, betChipStackList, _betValue);

    }

    void UpdateChipCountAndStack(List<Chip> _chipList, List<ChipStack> _chipStackList, int _chipValue) {

        //if (_chipList[0].chipCount > 10 || _chipList[0].chipCount <= 0)
        //{
        //}
        CacluateCoinStack(_chipList, _chipValue);
        ReorderChipStack(_chipList, _chipStackList);


    }

    public void ClearBet(int _clipValue)
    {
        foreach (Chip c in betChipList)
            c.chipCount = 0;

        foreach (var obj in betChipStackList)
        {
            obj.gameObject.SetActive(false);

        }

       
        UpdateChipCountAndStack(chipList, playerChipStackList, _clipValue);
    }
}
