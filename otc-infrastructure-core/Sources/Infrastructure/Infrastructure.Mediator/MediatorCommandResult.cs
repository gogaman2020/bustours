namespace Infrastructure.Mediator
{
    public class MediatorCommandResult<T>
    {
        public T Result { get; private set; }
        public string ErrorMessage { get; private set; }
        public object ErrorData { get; private set; }

        public static MediatorCommandResult<T> Success(T result)
        {
            return new MediatorCommandResult<T>
            {
                Result = result
            };
        }

        public static MediatorCommandResult<T> Fail()
        {
            return new MediatorCommandResult<T>();
        }

        public static MediatorCommandResult<T> Fail(string message, object data = null)
        {
            return new MediatorCommandResult<T>
            {
                ErrorMessage = message,
                ErrorData = data
            };
        }
    }
}
