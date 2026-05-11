using AllMaps;
using Base;
using LuyenDan;
using LuyenDan.Inventory;
using System;
using UnityEngine;
public class TaskDailySnapshot 
{
    public int ID { get; set; }
    public ETaskDaily ETaskDaily { get; set; }
    public ItemRef ItemRef { get; set; }
    public bool IsGap { get; set; }
    public bool IsDone { get; set; }

}
