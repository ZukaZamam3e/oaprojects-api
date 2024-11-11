using System.ComponentModel.DataAnnotations.Schema;

namespace OAProjects.Data.CatanLogger.Entities;
public class CL_GAME
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int GAME_ID { get; set; }

    public int GROUP_ID { get; set; }

    public DateTime DATE_PLAYED { get; set; }

    public string? TURN_ORDER { get; set; }

    public bool GAME_DELETED { get; set; }

    public CL_GROUP GROUP { get; set; }

    public ICollection<CL_PLAYER> PLAYERS { get; set; }
    public ICollection<CL_DICEROLL> DICE_ROLLS { get; set; }

}

