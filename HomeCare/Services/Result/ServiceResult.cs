namespace HomeCare.Services.Result
{
    public class ServiceResult<T>: IServiceResult<T>
    {
        public ServiceResult(T _data)
        {
            Success = true;
            Data = _data;
        }

        public ServiceResult(string _errorMessage, ErrorTypeEnum _errorType = ErrorTypeEnum.NONE)
        {
            Success = false;
            ErrorMessage = _errorMessage;
            ErrorType = _errorType;
        }

        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }
        public ErrorTypeEnum ErrorType { get; set; } = ErrorTypeEnum.NONE;
    }
}
