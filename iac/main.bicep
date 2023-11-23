param serviceName string = 'deaddrop'
param domainName string = 'gonk.wtf'
param location string = 'Central US'
param serviceRepository string = 'https://github.com/Gonkers/DeadDrop'

targetScope = 'subscription'

var resourceNames = {
  resourceGroup: 'rg-${serviceName}-cus-01'
  staticWebApp: 'swa-${serviceName}-cus-01'
}

var defaultTags = {
  service: serviceName
  repository: serviceRepository
}

resource resourceGroup 'Microsoft.Resources/resourceGroups@2023-07-01' = {
  name: resourceNames.resourceGroup
  location: location
  tags: defaultTags
}

module staticWebApp 'static-web-app.bicep' = {
  name: resourceNames.staticWebApp
  scope: resourceGroup
  params: {
    name: resourceNames.staticWebApp
    location: location
    hostname: serviceName
    domainName: domainName
    tags: defaultTags
  }
}
