namespace YourExam.Application.Interfaces;

public interface ITextVariableGeneratorService
{
    /// <summary>
    /// Thay thế các biến văn bản (VD: [NhanVat_1], [VatThe_HoaQua]) trong chuỗi mẫu
    /// bằng các giá trị ngẫu nhiên từ từ điển (Entities.json).
    /// Hỗ trợ hậu tố phân biệt (VD: _1, _2) để đảm bảo các giá trị được sinh ra 
    /// từ cùng một từ khóa gốc là duy nhất trong cùng một context câu hỏi.
    /// </summary>
    /// <param name="contentTemplate">Mẫu câu hỏi (VD: "[NhanVat_1] có [a] [VatThe_HoaQua]")</param>
    /// <returns>Chuỗi đã được thay thế (VD: "An có [a] quả táo")</returns>
    string ReplaceTextVariables(string contentTemplate);
}
