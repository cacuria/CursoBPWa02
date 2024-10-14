namespace Bdb.Curso.Application.Shared.Dtos
{
    public static class ResponseApiService
    {
        public static BaseResponseModel Response(int statusCode, object? Data = null, string? message = null)
        {
            bool succes = false;

            if (statusCode >= 200 && statusCode <= 300)
                succes = true;

            var result = new BaseResponseModel
            {
                StatusCode = statusCode,
                Success = succes,
                Message = message,
                Data = Data
            };

            return result;
        }

    }
}
