namespace Task1.DTOs;

public class CreateTransactionLogDto
{
    public long AccountId { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Details { get; set; }

}