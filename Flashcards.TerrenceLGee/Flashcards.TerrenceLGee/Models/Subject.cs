using System.ComponentModel.DataAnnotations;

namespace Flashcards.TerrenceLGee.Models;

public enum Subject
{
    [Display(Name = "Agriculture")]
    Agriculture,
    [Display(Name = "Business")]
    Business,
    [Display(Name = "Computer Science")]
    ComputerScience,
    [Display(Name = "Culinary Arts")]
    CulinaryArts,
    [Display(Name = "Economics")]
    Economics,
    [Display(Name = "Geography")]
    Geography,
    [Display(Name = "History")]
    History,
    [Display(Name = "Journalism")]
    Journalism,
    [Display(Name = "Languages")]
    Languages,
    [Display(Name = "Law")]
    Law,
    [Display(Name = "Literature")]
    Literature,
    [Display(Name = "Mathematics")]
    Mathematics,
    [Display(Name = "Medicine")]
    Medicine,
    [Display(Name = "Military")]
    Military,
    [Display(Name = "Movies")]
    Movies,
    [Display(Name = "Music")]
    Music,
    [Display(Name = "Philosophy")]
    Philosophy,
    [Display(Name = "Physics")]
    Physics,
    [Display(Name = "Programming Languages")]
    ProgrammingLanguages,
    [Display(Name = "Psycology")]
    Psycology,
    [Display(Name = "Religion")]
    Religion,
    [Display(Name = "Science")]
    Science,
    [Display(Name = "Sociology")]
    Sociology,
    [Display(Name = "Television")]
    Television,
    [Display(Name = "Other")]
    Other,
}