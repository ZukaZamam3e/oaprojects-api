using System.ComponentModel.DataAnnotations.Schema;

namespace OAProjects.Data.CatanLogger.Entities;
public class CL_DICEROLL
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int DICE_ROLL_ID { get; set; }

    public int GAME_ID { get; set; }

    public int DICE_NUMBER { get; set; }

    public int DICE_ROLLS { get; set; }

    public CL_GAME GAME { get; set; }

}