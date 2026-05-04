namespace HomeCare.Services.Result
{
    public interface IServiceResult<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }
        public ErrorTypeEnum ErrorType { get; set; }
    }

    
}
