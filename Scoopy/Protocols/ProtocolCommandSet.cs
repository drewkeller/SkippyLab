﻿using Newtonsoft.Json;
using System.IO;

namespace Scoopy.Protocols
{

    public class ProtocolCommandSet
    {
        public IProtocolCommandList Commands => new TriggerProtocol(null).Subcommands;

        #region Serialization/Deserialization  hmmm maybe this would be useful?

        private static JsonSerializer GetSerializer()
        {
            var ser = JsonSerializer.CreateDefault();
            ser.Formatting = Formatting.Indented;
            ser.NullValueHandling = NullValueHandling.Ignore;
            return ser;
        }

        public void Serialize(string filename)
        {
            using (var sw = new StringWriter())
            using (var writer = new JsonTextWriter(sw))
            {
                var ser = GetSerializer();
                ser.Serialize(writer, this);
                var json = sw.ToString();
                File.WriteAllText(filename, json);
            }
        }

        public static ProtocolCommandSet Deserialize(string filename)
        {
            var ser = GetSerializer();
            using (var file = File.OpenText(filename))
            using (var reader = new JsonTextReader(file))
            {
                var commandSet = ser.Deserialize<ProtocolCommandSet>(reader);
                return commandSet;
            }
        }

        #endregion Serialization
    }

}
