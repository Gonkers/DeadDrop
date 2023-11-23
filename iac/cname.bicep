param hostname string
param domainName string
param cname string

resource dnsZone 'Microsoft.Network/dnsZones@2023-07-01-preview' existing = {
  name: domainName
}

resource dnsRecord 'Microsoft.Network/dnsZones/CNAME@2023-07-01-preview' = {
  name: hostname
  parent: dnsZone
  properties: {
    TTL: 3600
    CNAMERecord: {
      cname: cname
    }
  }
}
