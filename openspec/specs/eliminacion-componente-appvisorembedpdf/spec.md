# eliminacion-componente-appvisorembedpdf Specification

## Purpose
TBD - created by archiving change scrumcore-200-eliminacion-componente-appvisorembedpdf. Update Purpose after archive.
## Requirements
### Requirement: Eliminar componente `AppVisorEmbedPdf`
El sistema SHALL eliminar completamente el componente `src/app/Components/UI/AppVisorEmbedPdf` y cualquier referencia a este en el código fuente de la SPA.

#### Scenario: Código fuente sin referencias al componente
- **WHEN** se realiza una búsqueda en el repositorio por `AppVisorEmbedPdf` o `VisorEmbedPdf`
- **THEN** no se encuentran matches dentro de `src/**`

#### Scenario: Compilación sin imports residuales
- **WHEN** se ejecuta el build del frontend
- **THEN** el build no falla por imports/rutas/exports relacionados con `AppVisorEmbedPdf`

### Requirement: Eliminar dependencias `@embedpdf/*`
El sistema SHALL remover todas las dependencias `@embedpdf/*` del `package.json` y actualizar el lockfile para que no queden entradas activas relacionadas.

#### Scenario: `package.json` sin dependencias `@embedpdf/*`
- **WHEN** se revisa el `package.json`
- **THEN** no existen paquetes bajo el scope `@embedpdf/`

#### Scenario: Lockfile actualizado
- **WHEN** se ejecuta `npm install`
- **THEN** el `package-lock.json` refleja la eliminación de `@embedpdf/*` (sin referencias activas a esos paquetes)

### Requirement: Flujos que dependían del visor se degradan de forma controlada
Si existían pantallas/acciones que invocaban el visor, la UI SHALL remover la opción o proveer una alternativa existente (p. ej. descarga/preview existente) sin producir errores en runtime.

#### Scenario: Navegación sin crash en pantallas afectadas
- **WHEN** un usuario accede a una pantalla donde antes existía la acción de abrir el visor
- **THEN** la aplicación no genera errores de runtime por componentes faltantes

