# Overview

- Frontend: React + TypeScript (vite)
- Backend/API: Azure functions + C#
- Hosted in Azure: Static Web APP
- Media: Azure Blobs

## Frontend Created With

npm create vite@latest matzen-web -- --template react-ts
cd matzen-wb
npm install

## Backend Created With

Visual Studio 2022
Azure Function Template 
.NET 8 - isolated workers

## Deployments 

Pushing to GitHub triggers build action and deployment to Azure.

## Run Locally (Frontend)

Visual Studio Code
cd frontend
set-executionpolicy -scope process bypass
npm run dev

This starts a server with origin `http://localhost:5173/`

To run in preview mode:
```
npm run build 
npm run preview
```

This starts a server with origin `http://localhost:4173/`

## Run Locally (Backend)

Visual Studio

## Helpful Powershell Commands

GET videos
```
Invoke-Webrequest http://localhost:7051/api/videos
(Invoke-Webrequest http://localhost:7051/api/videos).Content | ConvertFrom-Json
```

## Troubleshooting

Find PID for process holding open port
```
netstat -aon | findstr :7051
```

# Azure resource notes

## Initial setup
These steps can be done with the Azure CLI.

- Create resource group
- Create storage account
  - Configure CORS (temporary, during dev)
     - Settings > Resource Sharing (CORS)
     - Choose the Blob service
     - Add GET|HEAD|OPTIONS, 
       - http://localhost:5173
       - http://localhost:4173
     - Allowed/Exposed Headers: *
     - MaxAge: 3600
  - Create container
  - Upoad sample media
- Create keyvault
  - Add vault name to local.settings.json (azure func csproj)
  - Add Role Assignment 
      - Role: Key Vault Secrets Officer
      - Assign access to: User, group, or service principal 
          - Select yourself (this will allow you to add secrets) 
  - Add Secrets
      - BlobContainerName
      - StorageAccountName
      - StorageKey (access key)
  - Add app registration (service principle)
      - Choose name and create
      - Manage | Certifications & Secrets
          - Add new client secret with description:
             - Service principle for azure functions
          - Record for safe keeping:
             - the client secret value (not the id)
             - Directory (tenant) ID 
                 - Found in the overview of the app reg
             - Application (client) ID
                 - Also found on the overvew
    - Give app registration permission to key vault
       - Open Key Vault that you created earlier
       - Add Role Assignment 
          - Role: Key Vault Secrets User
          - Assign access to: User, group, or service principal 
             - Select the app registration that you created

## Manually test initial setup

At this point, test the azure functions on your laptop. First test normal manual testing.

1. Run azure functions in Visual Studio
2. Open powershell command prompt.
3. az login
4. Invoke-Webrequest http://localhost:7051/api/videos

Now test login with the client ID and secret you created for the app registration. In normal operation, only the azure functions need to connect with clientid/secret (a managed identity would be better, but not available for free web site). 

1. Add user environment variables for:
   - AZURE_CLIENT_ID   (a.k.a Application ID or Client ID)
   - AZURE_CLIENT_SECRET
   - AZURE_TENANT_ID
2. Restart Visual Studio
3. Run azure functions in Visual Studio
4. Invoke-Webrequest http://localhost:7051/api/videos
5. Delete the environment variables when done.

Now test that running the react web page can connect to the local azure functions.

1. Run azure functions in Visual Studio
2. Open Visual Studio Code, open the workspace, and then open a terminal.
   ```
   cd front end
   set-executionpolicy -scope process bypass
   npm run dev
   ```
3. The web site should come up and download the video (at least in the very first rendition of the site).

## Publish initial setup

Now you can publish the initial setup. This implies that the code is already checked into your github account.

# Original Readme for Vite Template

## React + TypeScript + Vite

NOTE: The following is preserved from the original Vite template. This needs review.

This template provides a minimal setup to get React working in Vite with HMR and some ESLint rules.

Currently, two official plugins are available:

- [@vitejs/plugin-react](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react) uses [Babel](https://babeljs.io/) for Fast Refresh
- [@vitejs/plugin-react-swc](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react-swc) uses [SWC](https://swc.rs/) for Fast Refresh

## Expanding the ESLint configuration

If you are developing a production application, we recommend updating the configuration to enable type-aware lint rules:

```js
export default tseslint.config([
  globalIgnores(['dist']),
  {
    files: ['**/*.{ts,tsx}'],
    extends: [
      // Other configs...

      // Remove tseslint.configs.recommended and replace with this
      ...tseslint.configs.recommendedTypeChecked,
      // Alternatively, use this for stricter rules
      ...tseslint.configs.strictTypeChecked,
      // Optionally, add this for stylistic rules
      ...tseslint.configs.stylisticTypeChecked,

      // Other configs...
    ],
    languageOptions: {
      parserOptions: {
        project: ['./tsconfig.node.json', './tsconfig.app.json'],
        tsconfigRootDir: import.meta.dirname,
      },
      // other options...
    },
  },
])
```

You can also install [eslint-plugin-react-x](https://github.com/Rel1cx/eslint-react/tree/main/packages/plugins/eslint-plugin-react-x) and [eslint-plugin-react-dom](https://github.com/Rel1cx/eslint-react/tree/main/packages/plugins/eslint-plugin-react-dom) for React-specific lint rules:

```js
// eslint.config.js
import reactX from 'eslint-plugin-react-x'
import reactDom from 'eslint-plugin-react-dom'

export default tseslint.config([
  globalIgnores(['dist']),
  {
    files: ['**/*.{ts,tsx}'],
    extends: [
      // Other configs...
      // Enable lint rules for React
      reactX.configs['recommended-typescript'],
      // Enable lint rules for React DOM
      reactDom.configs.recommended,
    ],
    languageOptions: {
      parserOptions: {
        project: ['./tsconfig.node.json', './tsconfig.app.json'],
        tsconfigRootDir: import.meta.dirname,
      },
      // other options...
    },
  },
])
```
