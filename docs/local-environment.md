# Local Development Environment

Este documento describe la configuración local utilizada para desarrollar el proyecto ServiceDesk.

## Sistema base

- Sistema operativo: Ubuntu 22.04.5 LTS
- Editor principal: Visual Studio Code
- Shell de trabajo: Bash
- Repositorio: Git y GitHub CLI

## Versiones verificadas

| Componente | Versión | Estado |
| --- | --- | --- |
| Git | 2.40.0 | Verificado |
| GitHub CLI | 2.98.0 | Verificado |
| Docker Engine | 29.6.1 | Verificado |
| Docker Compose | 5.3.1 | Verificado |
| .NET SDK | Por verificar | Pendiente |
| Node.js | Por verificar | Pendiente |
| npm | Por verificar | Pendiente |
| Visual Studio Code | Por verificar | En uso |

## Git y GitHub CLI

Verificar Git:

```bash
git --version
```

Verificar GitHub CLI:

```bash
gh --version
```

Comprobar autenticación con GitHub:

```bash
gh auth status
```

Configurar Git para utilizar las credenciales administradas por GitHub CLI:

```bash
gh auth setup-git
```

Comprobar acceso a repositorios:

```bash
gh repo list --limit 5
```

## Docker

Docker está instalado mediante Snap.

Verificar la instalación:

```bash
docker --version
docker compose version
```

Verificar el servicio:

```bash
snap services docker
```

La salida esperada debe mostrar `docker.dockerd` como `enabled` y `active`.

Comprobar acceso al daemon:

```bash
docker info
```

Verificar funcionamiento sin `sudo`:

```bash
docker run --rm hello-world
```

La prueba es correcta cuando aparece:

```text
Hello from Docker!
```

Verificar contenedores activos:

```bash
docker ps
```

El usuario local debe pertenecer al grupo `docker`.

```bash
groups
```

Si se requiere agregar el usuario al grupo:

```bash
sudo addgroup --system docker
sudo adduser $USER docker
```

Después se debe actualizar la sesión o ejecutar:

```bash
newgrp docker
```

En instalaciones mediante Snap puede ser necesario reiniciar Docker después de modificar el grupo:

```bash
sudo snap disable docker
sudo snap enable docker
```

## .NET

Verificar el SDK:

```bash
dotnet --version
dotnet --info
```

Para secretos de desarrollo del backend se utilizará `dotnet user-secrets`.

Inicialización:

```bash
dotnet user-secrets init
```

Ejemplo:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "valor-local"
```

Los secretos no deben almacenarse en archivos versionados.

## Node.js y React

Verificar Node.js y npm:

```bash
node --version
npm --version
```

El frontend React utilizará Vite.

Las variables locales del frontend pueden almacenarse en:

```text
.env.local
```

Solo deben colocarse valores que puedan ser visibles desde el navegador. Las credenciales y secretos privados nunca deben almacenarse en el frontend.

## Estrategia de secretos y variables

La configuración local seguirá estas reglas:

- Backend ASP.NET Core mediante `dotnet user-secrets`.
- React mediante `.env.local` únicamente para configuración pública.
- Docker Compose mediante variables locales en archivos excluidos de Git.
- `.env.example` documentará las variables requeridas sin incluir valores sensibles.
- Los secretos de CI/CD se almacenarán posteriormente en GitHub Actions Secrets.
- Nunca se incluirán contraseñas, tokens o claves privadas directamente en el código fuente.

El `.gitignore` debe incluir como mínimo:

```gitignore
.env
.env.local
.env.*.local
secrets.json
.vscode/settings.local.json
```

## Puertos reservados

| Servicio | Puerto |
| --- | ---: |
| React / Vite | 5173 |
| ASP.NET Core HTTP | 5080 |
| ASP.NET Core HTTPS | 7080 |
| SQL Server | 1433 |
| Servicios de agentes o IA | 8000-8099 |
| Dashboards y herramientas auxiliares | 3001-3099 |

Para comprobar si los principales puertos están ocupados:

```bash
ss -ltnp | grep -E ':5173|:5080|:7080|:1433|:8000|:8001|:3001'
```

También puede comprobarse un puerto individual:

```bash
ss -ltnp | grep :5173
```

## Visual Studio Code

Visual Studio Code se utilizará inicialmente como editor principal.

El objetivo es mantener una instalación ligera y evitar extensiones innecesarias.

El entorno debe permitir trabajar con:

- C# y ASP.NET Core.
- JavaScript, TypeScript, JSX y TSX.
- React y Vite.
- Git y GitHub CLI.
- Docker y Docker Compose mediante la terminal integrada.

Abrir el proyecto:

```bash
code .
```

## Verificación rápida

Los siguientes comandos permiten comprobar el entorno principal:

```bash
git --version
gh --version
gh auth status

dotnet --version

node --version
npm --version

docker --version
docker compose version
docker ps
```

La prueba funcional de Docker es:

```bash
docker run --rm hello-world
```

## Criterio de finalización

El entorno local se considera listo cuando:

- Git y GitHub CLI funcionan y GitHub CLI está autenticado.
- Docker Engine y Docker Compose funcionan sin `sudo`.
- .NET SDK está disponible desde la terminal.
- Node.js y npm están disponibles desde la terminal.
- Visual Studio Code puede abrir el repositorio y trabajar con C#, React y Docker.
- Los secretos locales están excluidos de Git.
- Los puertos definidos para los servicios locales están disponibles o documentados.
