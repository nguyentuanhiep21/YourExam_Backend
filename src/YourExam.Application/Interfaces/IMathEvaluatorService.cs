namespace YourExam.Application.Interfaces;

public interface IMathEvaluatorService
{
    /// <summary>
    /// Đánh giá biểu thức toán học trả về kết quả số học (vd: "x + y").
    /// Dùng RIÊNG cho dạng bài Tính toán (Calculation) để máy tự giải bài và tìm ra ĐÁP ÁN SỐ.
    /// </summary>
    double EvaluateMathExpression(string expression, Dictionary<string, double> variables);

    /// <summary>
    /// Đánh giá biểu thức logic trả về boolean (vd: "x + y <= 80").
    /// Dùng CHUNG cho mọi dạng bài có sinh biến ngẫu nhiên.
    /// Làm nhiệm vụ KIỂM THỬ xem các con số vừa random có hợp lệ (vd: không bị âm, không lẻ) không. 
    /// Nếu False -> Bỏ đi random lại.
    /// </summary>
    bool EvaluateConstraint(string constraintExpression, Dictionary<string, double> variables);

    /// <summary>
    /// Đánh giá biểu thức trả về chuỗi (vd: "if(x > y, '>', '<')").
    /// Dùng RIÊNG cho dạng bài So sánh (Comparison) để máy luận logic và trả về ĐÁP ÁN CHỮ (như dấu >, <, =).
    /// </summary>
    string EvaluateStringExpression(string expression, Dictionary<string, double> variables);
}
