namespace OnionSetUp.Application.Common.Response
{
    public class Response<T>
    {
        private Response()
        {
            
        }
        public T Data { get; private set; }
        public bool IsSucces { get; private set; }
        public int StatusCode { get; private set; }
        public IReadOnlyList<string> Errors { get; private set; }

        public static Response<T> Succes(T data, int statusCode)
        {
            return new Response<T>()
            {
                IsSucces=true,
                Data=data,
                StatusCode=statusCode,
            };
        }
        public static Response<T> Succes(int statusCode)
        {
            return new Response<T>()
            {
                IsSucces=true,
                StatusCode=statusCode
            };
        }
        public static Response<T> Fail(IEnumerable<string> errors, int statusCode)
        {
            return new Response<T>()
            {
               IsSucces=false,
               Errors=new List<string>(errors),
               StatusCode=statusCode
            };
        }
        public static Response<T> Fail(string error, int statusCode)
        {
            return new Response<T>()
            {
                IsSucces=false,
                Errors=new List<string>{ error}
            };
        }
    }
}
