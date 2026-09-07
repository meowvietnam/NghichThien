using LuyenDan;

public class TaskDailySnapshot
{
    public int ID { get; set; }
    public EDailyQuest EDailyQuest { get; set; }
    public ItemRef ItemRef { get; set; }
    public bool IsGap { get; set; }
    public bool IsDone { get; set; }
}