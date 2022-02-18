namespace Core.Results
{
    //Data taşımayacak sonuçların imzası
    public interface IResult
    {
        public bool Success { get; }
        public string Message { get; }
    }
}
