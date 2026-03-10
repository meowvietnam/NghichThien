using System;
using UnityEngine;

[System.Serializable]
public class SecureBool
{
    [SerializeField] int encValue; // Giá trị đã mã hóa
    [SerializeField] int key;      // Khóa ngẫu nhiên
    [SerializeField] bool fakeValue; // Giá trị giả để đánh lừa tìm kiếm 0/1

    public SecureBool(bool value)
    {
        Set(value);
    }

    public bool Value
    {
        get
        {
            // Giải mã: Lấy encValue XOR với key
            // Nếu kết quả là 1 thì là true, 0 là false
            bool realValue = (encValue ^ key) == 1;

            // Kiểm tra khớp với fakeValue để phát hiện can thiệp bộ nhớ
            if (realValue != fakeValue)
            {
                // Giả sử bạn có AntiCheatManager như trong code cũ
                Debug.LogError("Bool memory cheat detected!");
                // AntiCheatManager.CheatDetected("Bool memory cheat detected");
            }

            // Làm mới mã hóa mỗi lần truy cập để địa chỉ bộ nhớ liên tục thay đổi giá trị
            Set(realValue);
            return realValue;
        }
        set
        {
            Set(value);
        }
    }

    private void Set(bool value)
    {
        // Tạo key ngẫu nhiên để giá trị encValue luôn biến đổi
        key = UnityEngine.Random.Range(10000, 999999);

        // Chuyển bool thành int (1 hoặc 0) rồi XOR với key
        int intVal = value ? 1 : 0;
        encValue = intVal ^ key;

        fakeValue = value;
    }

    // Chuyển đổi ngầm định để sử dụng như biến bool bình thường
    public static implicit operator bool(SecureBool s)
    {
        return s == null ? false : s.Value;
    }

    public static implicit operator SecureBool(bool v)
    {
        return new SecureBool(v);
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}