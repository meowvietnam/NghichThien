namespace Tutorials
{
    /// <summary>
    /// Nơi tập trung TẤT CẢ khoá PlayerPrefs của hệ thống tutorial.
    ///
    /// Thêm một tutorial mới thì chỉ cần đăng ký khoá của nó vào đây một lần;
    /// AdminPanel.RenewTutorial() duyệt theo danh sách này nên sẽ tự reset được,
    /// không còn cảnh sót khoá mỗi khi thêm bước hướng dẫn mới.
    /// </summary>
    public static class TutorialKeys
    {
        public static readonly string[] All =
        {
            // AllMaps: giới thiệu bản đồ -> hand-tut chỉ vào Thiên Tinh Thành -> nút Tiến vào
            TutorialAllMaps.tutorialAllMapKey,
            TutorialAllMaps.tutorialPointThienTinhKey,

            // Thiên Tinh Thành (S23) -> Thiên Tinh Các (scene TangKinhCac)
            TutorialThienTinhCac.tutorialThienTinhCacKey,
            TutorialTangKinhCac.tutorialTangKinhCacKey,

            // Khách sạn -> Bế Quan -> trang bị (giai đoạn 2)
            TutorialThienTinhCac.tutorialKhachSanKey,
            TutorialHome.tutorialClickBeQuanKey,
            TutorialBeQuanFlow.tutorialBeQuanFlowKey,
            TutorialTrangBi.tutorialTrangBiKey,

            // Các tutorial sẵn có
            TutorialHome.tutorialHome,
            TutorialBeQuan.tutorialBeQuan,
            TutorialBoCao.tutorialBoCao,
            TutorialLuyenDan.tutorialLuyenDan,
        };
    }
}
