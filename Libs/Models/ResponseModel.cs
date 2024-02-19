using Schedule.EnumSpace;

namespace Schedule.ModelSpace
{
  public struct ResponseModel<T>
  {
    public T Data { get; set; }
    public ErrorModel Error { get; set; }

  }
}
