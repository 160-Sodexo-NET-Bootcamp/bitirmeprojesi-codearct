namespace Core.Results
{
    //İçinde Data barındıracak sonuçların imzası
    public interface IDataResult<T> : IResult
    {
        T Data { get; }
    }
}
