using System.ComponentModel.DataAnnotations.Schema;

namespace OAProjects.Data.ShowLogger.Entities;
public class SL_BOOK
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int BOOK_ID { get; set; }

    public int USER_ID { get; set; }

    public string BOOK_NAME { get; set; }

    public DateTime? START_DATE { get; set; }

    public DateTime? END_DATE { get; set; }

    public int? CHAPTERS { get; set; }

    public int? PAGES { get; set; }

    public string? BOOK_NOTES { get; set; }
}
