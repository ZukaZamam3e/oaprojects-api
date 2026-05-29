namespace OAProjects.Models.ShowQuiz;

public class CategoryQuestionModel
{
    public int QuestionId { get; set; }

    public string ShowName { get; set; }

    public int Score { get; set; }

    public string Question { get; set; }

    public string Answer { get; set; }

    public List<string> Hints { get; set; }

    public List<string> ShownHints { get; set; }

    public int AwardedPlayerId { get; set; }

    public bool AnswerRevealed { get; set; }

    public string QuestionShown { get; set; }
}
