
using System.Drawing;

namespace WordPlugin
{
  /// <summary>
  /// Represents a border of a table or table cell
  /// </summary>
  public class Border
  {

    #region Public Properties

    public BorderStyle Tcbs { get; set; }
    public BorderSize Size { get; set; }
    public int Space { get; set; }
    public Color Color { get; set; }

    #endregion

    #region Constructors

    public Border()
    {
      this.Tcbs = BorderStyle.Tcbs_single;
      this.Size = BorderSize.one;
      this.Space = 0;
      this.Color = Color.Black;
    }

    public Border( BorderStyle tcbs, BorderSize size, int space, Color color )
    {
      this.Tcbs = tcbs;
      this.Size = size;
      this.Space = space;
      this.Color = color;
    }

    #endregion
  }
}
