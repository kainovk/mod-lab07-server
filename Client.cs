namespace Client_Server
{
    public class Client
    {
        private readonly Server server;

        public Client(Server server)
        {
            this.server = server;
        }

        public void SendRequest(int processingTime)
        {
            Request request = new Request(processingTime);
            server.EnqueueRequest(request);
        }
    }
}