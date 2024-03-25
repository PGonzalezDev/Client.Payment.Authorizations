using System.Text;
using System.Text.Json;

namespace Client.Payments.Authorizations.Services.Helper
{
    public static class RabbitMqMessageEncoderHelper
    {
        public static byte[] EncodeMessage<T>(T value) where T : class
        {
            string jsonMessage = JsonSerializer.Serialize<T>(value);
            byte[] byteMessage = Encoding.UTF8.GetBytes(jsonMessage);

            return byteMessage;
        }

        public static T DecodeMessage<T>(byte[] body) where T : class
        {
            string message = Encoding.UTF8.GetString(body);
            T messageDecoded = JsonSerializer.Deserialize<T>(message);

            return messageDecoded;
        }
    }
}
