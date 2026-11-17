using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace Flashcards;

public static class EnumExtensions
{
  public static string GetDisplayName(this Enum value)
  {
    var member = value.GetType().GetMember(value.ToString())[0];

    return member
        .GetCustomAttribute<DisplayAttribute>()?
        .GetName()
        ?? value.ToString();
  }

  // internal static List<string> GetEditableColumn()
  // {
  //   List<string> choices = new();

  //   foreach (string columnName in Enum.GetNames<Column>())
  //   {
  //     if (columnName != Column.id.ToString() && columnName != Column.duration.ToString())
  //       choices.Add(((Column)Enum.Parse(typeof(Column), columnName)).GetDisplayName());
  //   }
  //   return choices;
  // }
}