using Grpc.Core;
using Grpc.Net.Client;
using person;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static person.PersonInformationGet;

namespace Processing.Service
{
    public class PersonServiceGrpc
    {
        private readonly PersonInformationGetClient _client;
        public PersonServiceGrpc(PersonInformationGetClient personInformationGetClient)
        {
            _client = personInformationGetClient;
        }
        public async Task<HelloReply> Check(string Cookie)
        {
            var metadata = new Metadata
            {
                { "Cookie", Cookie }
            };
            var callOption = new CallOptions(metadata);
            return await _client.CheckAsync(new HelloRequest { Name = "123" }, callOption);
            
        }
    }
}
