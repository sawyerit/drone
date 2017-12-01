using System;
using System.Collections.Generic;
using System.Text;

namespace Drone.API.DNS
{
    class NaptrRecord : IRecordData
    {
        /// <summary>
        /// Implementation Reference RFC 3403
        /// </summary>
        /// <param name="buffer"></param>
         public NaptrRecord(DataBuffer buffer)
        {
            order = buffer.ReadShortUInt();
            priority = buffer.ReadShortUInt();
            flags = buffer.ReadCharString();
            services = buffer.ReadCharString();
            regexp = buffer.ReadCharString();
            replacement = buffer.ReadCharString();
        }
        /// <summary>
        /// Converts this data record to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("Order:{0}, Priority:{1} Flags:{2} Services:{3} RegExp:{4} Replacement:{5}",
                order, priority, flags, services, regexp, replacement);
        }

        ushort order;
        /// <summary>
        /// retiurn Order of record
        /// </summary>
        public ushort Order
        {
            get { return order; }
        }
        ushort priority;
        /// <summary>
        /// return Priority of record
        /// </summary>
        public ushort Priority
        {
            get { return priority; }
        }
        string flags;
        /// <summary>
        /// return flags of record
        /// </summary>
        public string Flags
        {
            get { return flags; }
        }
        string services;
        /// <summary>
        /// return services listed in record
        /// </summary>
        public string Services
        {
            get { return services; }
        }
        string regexp;
        /// <summary>
        /// return regexp of record
        /// </summary>
        public string Regexp
        {
            get { return regexp; }
        }
        string replacement;
        /// <summary>
        /// return replacement domain of record
        /// </summary>
        public string Replacement
        {
            get { return replacement; }
        }
    }
}
