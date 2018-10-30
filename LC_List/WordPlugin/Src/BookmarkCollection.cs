
using System.Collections.Generic;
using System.Linq;

namespace WordPlugin
{
  public class BookmarkCollection : List<Bookmark>
  {
    public BookmarkCollection()
    {
    }

    public Bookmark this[ string name ]
    {
      get
      {
        return this.FirstOrDefault( x => x.Name.Equals( name, System.StringComparison.CurrentCultureIgnoreCase ) );
      }
    }
  }
}
