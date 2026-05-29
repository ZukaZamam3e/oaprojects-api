using Microsoft.AspNetCore.SignalR;
using OAProjects.Models.ShowQuiz;

namespace OAProjects.API.SignalR;

public class ShowQuizHub : Hub
{
    public async Task UpdateClient(string user, string message)
    {
        await Clients.Others.SendAsync("ReceiveClientUpdate", user, message);
    }

    //public async Task UpdateClient(string user, CategoryQuestionModel selectedQuestion, List<CategoryModel> categories, int turnOrder, int selectedPlayerId, List<PlayerModel> players)
    //{
    //    await Clients.All.SendAsync("ReceiveMessage", user, "testing");
    //}
}
