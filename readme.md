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

# Deployments 

Pushing to GitHub triggers build action and deployment to Azure.

# Running Locally (Frontend)

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

# Running Locally (Backend)

Visual Studio

# Helpful Powershell Commands

GET videos
```
Invoke-Webrequest http://localhost:7051/api/videos
(Invoke-Webrequest http://localhost:7051/api/videos).Content | ConvertFrom-Json
```

# Troubleshooting

Find PID for process holding open port
```
netstat -aon | findstr :7051
```

# Review Original Readme for Vite Template

# React + TypeScript + Vite

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
