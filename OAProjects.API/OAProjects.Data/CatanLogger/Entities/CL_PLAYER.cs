using System.ComponentModel.DataAnnotations.Schema;

namespace OAProjects.Data.CatanLogger.Entities;
public class CL_PLAYER
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PLAYER_ID { get; set; }

    public int GAME_ID { get; set; }

    public string PLAYER_NAME { get; set; }

    public string PLAYER_COLOR { get; set; }

    public bool WINNER { get; set; }

    public bool IS_PLAYING { get; set; }

    public CL_GAME GAME { get; set; }
}
