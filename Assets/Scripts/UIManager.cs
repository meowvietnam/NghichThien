using LongDH.LongDHModules;

public class UIManager : BaseBehaviour
{
    private void Awake()
    {
        // Kiểm tra nếu trong Pool đã có một Manager loại này rồi
        var existing = Pool.Get<UIManager>();

        if (existing != null && existing != this)
        {
            // Nếu đã có bản cũ (từ Scene trước), xóa bản mới này đi
            Destroy(gameObject);
            return;
        }

        // Nếu chưa có, mình là "trùm", đăng ký vào Pool và giữ lại
        Pool.Set(this);
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        var listPanel = transform.GetComponentsInChildren<PanelGroup>(true);
        foreach (var item in listPanel)
        {
            item.Init();
        }    
    }
}
