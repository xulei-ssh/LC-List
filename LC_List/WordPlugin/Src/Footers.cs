﻿/*************************************************************************************

   DocX – DocX is the community edition of Xceed Words for .NET

   Copyright (C) 2009-2016 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at http://wpftoolkit.codeplex.com/license 

   For more features and fast professional support,
   pick up Xceed Words for .NET at https://xceed.com/xceed-words-for-net/

  ***********************************************************************************/

namespace WordPlugin
{
  public class Footers
  {
     #region Public Properties

    public Footer Odd
    {
      get;
      set;
    }

    public Footer Even
    {
      get;
      set;
    }

    public Footer First
    {
      get;
      set;
    }

    #endregion

    #region Constructors

    internal Footers()
    {
    }

    #endregion
  }
}
