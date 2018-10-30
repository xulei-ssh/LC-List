
namespace WordPlugin
{
  public class Bookmark
  {
    #region Public Properties

    public string Name
    {
      get; set;
    }
    public Paragraph Paragraph
    {
      get; set;
    }

    #endregion

    #region Constructors

    public Bookmark()
    {
    }

    #endregion

    #region Public Methods

    public void SetText( string text )
    {
      this.Paragraph.ReplaceAtBookmark( text, this.Name );
    }

    #endregion
  }
}
