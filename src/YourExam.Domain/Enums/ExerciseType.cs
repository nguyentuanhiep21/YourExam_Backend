namespace YourExam.Domain.Enums;

public enum ExerciseType
{
    // ── Toán ────────────────────────────────────────
    Calculation    = 1,
    WordProblem    = 2,
    Comparison     = 3,
    FillInTheBlank = 4, // Điền vào chỗ trống (Toán)

    // ── Tiếng Việt ──────────────────────────────────
    Phonetics                = 5, // Nhận biết vần (TN/TL)
    Spelling                 = 6, // Quy tắc Chính tả (TN/TL)
    WordOrder                = 7, // Sắp xếp từ thành câu có nghĩa (TN/TL)
    OddOneOut                = 8, // Tìm từ khác loại (TN)
    Reading                  = 9, // Đọc hiểu văn bản ngắn (TN/TL)
    FillInBlank              = 10 // Chọn từ (TN)
}
