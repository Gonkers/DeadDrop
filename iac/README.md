# Deployment
```bash
az bicep lint --file ./main.bicep
az deployment sub what-if --location CentralUS --template-file .\main.bicep
az deployment sub create --location CentralUS --template-file .\main.bicep
```