@allowed([
  'test'
  'prod'
])
param runtimeEnvironment string

param location string = resourceGroup().location

var defaultResourceName = '${runtimeEnvironment}-wam-api-ne'
var tables = [
  'users'
  'games'
  'scores'
]

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
  name: '${defaultResourceName}-log'
  location: location
  properties: {
    retentionInDays: 30
    sku: {
      name: 'PerGB2018'
    }
  }
}

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: '${defaultResourceName}-ai'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    IngestionMode: 'LogAnalytics'
    WorkspaceResourceId: logAnalytics.id
  }
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: uniqueString(defaultResourceName)
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  resource tableService 'tableServices' = {
    name: 'default'
    resource table 'tables' = [for tableName in tables: {
      name: tableName
    }]
  }
}

resource redisCache 'Microsoft.Cache/redis@2023-08-01' = {
  name: '${defaultResourceName}-cache'
  location: location
  properties: {
    sku: {
      name: 'Basic'
      family: 'C'
      capacity: 0
    }
    enableNonSslPort: false
    publicNetworkAccess: 'Enabled'
  }
}

resource appServicePlan 'Microsoft.Web/serverfarms@2023-01-01' = {
  name: '${defaultResourceName}-plan'
  location: location
  sku: {
    name: 'B1'
    tier: 'Basic'
  }
  kind: 'linux'
  properties: {
    reserved: true
    maximumElasticWorkerCount: 1
    perSiteScaling: false
    targetWorkerCount: 0
    targetWorkerSizeId: 0
  }
}

resource webApp 'Microsoft.Web/sites@2023-01-01' = {
  name: '${defaultResourceName}-app'
  location: location
  kind: 'linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    httpsOnly: true
    serverFarmId: appServicePlan.id
    siteConfig: {
      cors: {
        allowedOrigins: [
          'http://localhost:4200'
        ]
        supportCredentials: true
      }
      ftpsState: 'Disabled'
      http20Enabled: true
      webSocketsEnabled: false
      linuxFxVersion: 'DOTNETCORE|8.0'
      appSettings: [
        {
          name: 'Azure:StorageAccountName'
          value: storageAccount.name
        }
        {
          name: 'Cache:Endpoint'
          value: redisCache.properties.hostName
        }
        {
          name: 'Cache:Secret'
          value: redisCache.listKeys().primaryKey
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: applicationInsights.properties.ConnectionString
        }
      ]
    }
    clientAffinityEnabled: false
  }
}

resource storageTableDataContributorRoleDefinition 'Microsoft.Authorization/roleDefinitions@2022-05-01-preview' existing = {
  name: '0a9a7e1f-b9d0-4cc4-a60d-0319b160aaa3'
}
module storageTableDataContributorRoleAssignment 'roleAssignment.bicep' = {
  name: 'storageTableDataContributorRoleAssignment'
  params: {
    principalId: webApp.identity.principalId
    roleDefinitionId: storageTableDataContributorRoleDefinition.id
    principalType: 'ServicePrincipal'
  }
}
module storageTableDataContributorRoleAssignmentToEduard 'roleAssignment.bicep' = {
  name: 'storageTableDataContributorRoleAssignmentToEduard'
  params: {
    principalId: 'ce00c98d-c389-47b0-890e-7f156f136ebd'
    roleDefinitionId: storageTableDataContributorRoleDefinition.id
    principalType: 'User'
  }
}
