namespace TokenCardCare.Server.Model;

public class ApiResponse<T>
{
    public int Code { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
}

public class ApiResponse : ApiResponse<object>
{
    public static ApiResponse<T> Success<T>(string message, T data)
    {
        return new ApiResponse<T>
        {
            Code = 0,
            Data = data,
            Message = message
        };
    }

    public static ApiResponse Success(string message) 
    {
        return new ApiResponse
        {
            Code = 0,
            Message = message,
            Data = null
        };
    }

    public static ApiResponse Fail(int code, string message)
    {
        return new ApiResponse
        {
            Code = code,
            Message = message,
            Data = null
        };
    }
}
