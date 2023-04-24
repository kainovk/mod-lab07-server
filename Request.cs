namespace Client_Server
{
    public class Request
    {
        public Request(int processingTime)
        {
            this.ProcessingTime = processingTime;
        }

        public int ProcessingTime { get; }
    }
}