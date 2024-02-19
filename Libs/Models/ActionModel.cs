using System;

namespace Schedule.ModelSpace
{
  public struct ActionModel
  {
    public Action Success { get; set; }
    public Action<ErrorModel> Error { get; set; }
  }
}
