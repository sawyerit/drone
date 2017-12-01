using System.Net.NetworkInformation;
using System.Net;
using System;

namespace Drone.API.DNS
{
    class Utils
    {
        public static string[] GetNetworkInterfaces()
        {
          NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
          if (nics == null || nics.Length < 1) {
            throw new Exception("No network interfaces found");
          }

          String[] s = new string[nics.Length];
          int i = 0;
          foreach (NetworkInterface adapter in nics) {
            s[i] = adapter.Description;
            i++;
          }
          return s;
        }

        private static string[] GetAdapterIpAdresses(NetworkInterface adapter){
          if (adapter == null) {
            throw new Exception("No network interfaces found");
          }
          IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
          string[] s = null;
          IPAddressCollection dnsServers = adapterProperties.DnsAddresses;
          if (dnsServers != null) {
            s = new string[dnsServers.Count];
            int i = 0;
            foreach (IPAddress dns in dnsServers) {
              s[i] = dns.ToString();
              i++;
            }
          }
          return s;
        }
    }
}
