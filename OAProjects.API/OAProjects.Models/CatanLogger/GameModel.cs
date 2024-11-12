using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Models.CatanLogger;
public class GameModel
{
    public int GameId { get; set; }

    public DateTime Date { get; set; }

    public IEnumerable<PlayerModel> Players { get; set; }

    public int TotalDiceRolls { get; set; }

    public string? WinnerName { get; set; }

    public string? WinnerColor { get; set; }

    public IEnumerable<DiceRollModel> DiceRolls { get; set; }

    public IEnumerable<TurnOrderModel> TurnOrder { get; set; }
}
