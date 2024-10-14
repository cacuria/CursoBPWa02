namespace Bdb.Curso.Application.Shared.Dtos
{
    public class BaseResponseModel
    {
        public int StatusCode { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public dynamic Data { get; set; }
    }
}
