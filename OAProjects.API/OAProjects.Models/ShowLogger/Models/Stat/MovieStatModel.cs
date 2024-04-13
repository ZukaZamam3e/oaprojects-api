namespace OAProjects.Models.ShowLogger.Models.Stat;
public class MovieStatModel
{
    public int UserId { get; set; }

    public string MovieName { get; set; }

    public int ShowId { get; set; }

    public int ShowTypeId { get; set; }

    public string? ShowTypeIdZ { get; set; }

    public DateTime DateWatched { get; set; }

    public decimal? AlistTicketAmt { get; set; }

    public decimal? TicketAmt { get; set; }

    public decimal? PurchaseAmt { get; set; }

    public decimal? BenefitsAmt { get; set; }

    public decimal? RewardsAmt { get; set; }

    public decimal? TotalAmt { get; set; }
}
