namespace Infrastructure.Queue.Model
{
    public class MqMessageT<T>
        where T : class
    {
        /// <summary>
        /// Ключ для проверки что новый режим доступен. 
        /// </summary>
        public const string Key = "v.01";
        
        /// <summary>
        /// Поле где должен быть этот ключ
        /// </summary>
        public string C { get; set; }
        public T Data { get; set; }
        public int OrganizationId { get; set; }
        //todo traceid
        //public string TraceId { get; set; } = "none";
    }
}