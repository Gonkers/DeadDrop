param location string = resourceGroup().location
param name string
param hostname string
param domainName string
param tags object

var fqdn = '${hostname}.${domainName}'

resource staticWebApp 'Microsoft.Web/staticSites@2022-09-01' = {
  name: name
  location: location
  tags: tags
  sku: {
    name: 'Free'
    tier: 'Free'
  }
  properties: {
    allowConfigFileUpdates: true
    provider: 'None'
    stagingEnvironmentPolicy: 'Enabled'
  }

  resource customDomain 'customDomains' = {
    name: fqdn
    dependsOn: [cname]
  }
}

module cname 'cname.bicep' = {
  name: 'dns'
  scope: resourceGroup('dns')
  params: {
    cname: staticWebApp.properties.defaultHostname
    domainName: domainName
    hostname: hostname
  }
}
