namespace Client_Server
{
    public class Client
    {
        private readonly Server _server;

        public Client(Server server)
        {
            this._server = server;
        }

        public void SendRequest(int processingTime)
        {
            var request = new Request(processingTime);
            _server.EnqueueRequest(request);
        }
    }
}