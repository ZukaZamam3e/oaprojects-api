using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Models.ShowQuiz;

public class CategoryModel
{
    public string Name { get; set; }

    public string ImgUrl { get; set; }

    public List<CategoryQuestionModel> Questions { get; set; }
}
