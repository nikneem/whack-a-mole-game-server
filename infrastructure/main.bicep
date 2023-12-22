targetScope = 'subscription'

@allowed([
  'test'
  'prod'
])
param runtimeEnvironment string

param location string = deployment().location

var defaultResourceName = '${runtimeEnvironment}-wam-api-ne'

resource targetResourceGroup 'Microsoft.Resources/resourceGroups@2023-07-01' = {
  name: '${defaultResourceName}-rg'
  location: location
}

module resourcesModule 'resources.bicep' = {
  name: 'resourcesModule'
  scope: targetResourceGroup
  params: {
    runtimeEnvironment: runtimeEnvironment
    location: location
  }
}
