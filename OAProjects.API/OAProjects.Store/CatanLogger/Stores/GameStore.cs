using Microsoft.EntityFrameworkCore;
using OAProjects.Data.CatanLogger.Context;
using OAProjects.Data.CatanLogger.Entities;
using OAProjects.Data.FinanceTracker.Context;
using OAProjects.Models.CatanLogger;
using OAProjects.Store.CatanLogger.Stores.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Store.CatanLogger.Stores;
public class GameStore(CatanLoggerDbContext _context) : IGameStore
{
    public IEnumerable<GameModel> GetGames(int groupId)
    {
        char[] separators = new char[] { ',' };

        IEnumerable<GameModel> query = _context.CL_GAME
            .Include(m => m.PLAYERS)
            .Include(m => m.DICE_ROLLS)
            .AsEnumerable()
            .Where(m => !m.GAME_DELETED && m.GROUP_ID == groupId)
            .Select(m => new GameModel
            {
                GameId = m.GAME_ID,
                Date = m.DATE_PLAYED,
                WinnerName = m.PLAYERS.FirstOrDefault(p => p.WINNER)?.PLAYER_NAME,
                WinnerColor = m.PLAYERS.FirstOrDefault(p => p.WINNER)?.PLAYER_COLOR,
                TurnOrder = !string.IsNullOrEmpty(m.TURN_ORDER) ? m.TURN_ORDER.Split(separators).Select((t, i) => new TurnOrderModel
                {
                    Order = i,
                    Color = t
                }) : new TurnOrderModel[0],
                Players = m.PLAYERS.Select(p => new PlayerModel
                {
                    PlayerId = p.PLAYER_ID,
                    Name = p.PLAYER_NAME,
                    Color = p.PLAYER_COLOR,
                    Winner = p.WINNER,
                    IsPlaying = p.IS_PLAYING
                }),
                DiceRolls = m.DICE_ROLLS.Select(d => new DiceRollModel
                {
                    DiceRollId = d.DICE_ROLL_ID,
                    DiceNumber = d.DICE_NUMBER,
                    DiceRolls = d.DICE_ROLLS
                }),
                TotalDiceRolls = m.DICE_ROLLS.Sum(d => d.DICE_ROLLS)
            });

        return query;
    }

    public GameModel? SaveGame(int groupId, GameModel model)
    {
        CL_GAME? gameEntity;
        if (model.GameId == -1)
        {
            gameEntity = new CL_GAME()
            {
                GROUP_ID = groupId,
                DATE_PLAYED = model.Date,
                TURN_ORDER = string.Join(",", model.TurnOrder.Select(m => m.Color))

            };
            _context.CL_GAME.Add(gameEntity);
            _context.SaveChanges();
        }
        else
        {
            gameEntity = _context.CL_GAME.Where(m => m.GAME_ID == model.GameId).FirstOrDefault();
        }

        if (groupId != gameEntity.GROUP_ID)
        {
            return null;
        }

        gameEntity.DATE_PLAYED = model.Date;
        gameEntity.TURN_ORDER = string.Join(",", model.TurnOrder.Select(m => m.Color));

        IEnumerable<CL_PLAYER> newPlayers = model.Players.Where(m => m.PlayerId <= -1).Select(m => new CL_PLAYER
        {
            GAME_ID = gameEntity.GAME_ID,
            PLAYER_COLOR = m.Color,
            PLAYER_NAME = m.Name,
            WINNER = m.Winner,
            IS_PLAYING = m.IsPlaying
        });

        _context.CL_PLAYER.AddRange(newPlayers);

        foreach (PlayerModel player in model.Players.Where(m => m.PlayerId > -1))
        {
            CL_PLAYER? playerEnity = _context.CL_PLAYER.FirstOrDefault(m => m.PLAYER_ID == player.PlayerId);

            if (playerEnity != null)
            {
                playerEnity.PLAYER_COLOR = player.Color;
                playerEnity.PLAYER_NAME = player.Name;
                playerEnity.WINNER = player.Winner;
                playerEnity.IS_PLAYING = player.IsPlaying;
            }
        }

        IEnumerable<CL_DICEROLL> newDiceRolls = model.DiceRolls.Where(m => m.DiceRollId <= -1).Select(m => new CL_DICEROLL
        {
            GAME_ID = gameEntity.GAME_ID,
            DICE_NUMBER = m.DiceNumber,
            DICE_ROLLS = m.DiceRolls,
        });

        _context.CL_DICEROLL.AddRange(newDiceRolls);

        foreach (DiceRollModel diceRoll in model.DiceRolls.Where(m => m.DiceRollId > -1))
        {
            CL_DICEROLL? diceRollEntity = _context.CL_DICEROLL.FirstOrDefault(m => m.DICE_ROLL_ID == diceRoll.DiceRollId);

            if (diceRollEntity != null)
            {
                diceRollEntity.DICE_NUMBER = diceRoll.DiceNumber;
                diceRollEntity.DICE_ROLLS = diceRoll.DiceRolls;
            }
        }

        _context.SaveChanges();

        return GetGames(groupId).First(m => m.GameId == gameEntity.GAME_ID);
    }

    public bool DeleteGame(int groupId, int gameId)
    {
        CL_GAME? entity = _context.CL_GAME.FirstOrDefault(m => m.GAME_ID == gameId
        //&& m.GROUP_ID == groupId
            );

        if (entity == null)
            return false;

        entity.GAME_DELETED = true;

        _context.SaveChanges();

        return true;
    }
}
