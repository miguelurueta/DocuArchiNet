# Pruebas y evidencia

## Evidencia funcional existente

Fecha base: 2026-09-21.

```text
node --test tests/importar-servicio-web-core.test.cjs tests/importar-servicio-web-provider-registry-ui.test.cjs tests/importar-servicio-web-accessibility.test.cjs tests/importar-servicio-web-gate-regression.test.cjs tests/importar-servicio-web-legacy-regression.test.cjs
```

Resultado registrado: 18 aprobadas, 0 fallidas. Al ejecutar conjuntamente la prueba documental y las regresiones DOC-72 se obtuvieron 22 aprobadas y 0 fallidas. MSBuild produjo `bin/GestionDocumental-Docuarchi.net.dll` con código 0 y advertencias legacy preexistentes.

## Prueba automática de documentación

La validación se divide en fuentes estructurales independientes:

1. `tests/doc72-technical-documentation.test.cjs` usa Acorn para construir AST JavaScript, exige el inventario fijo de cuatro diagramas, valida la gramática Mermaid utilizada, parámetros y uso de cada símbolo.
2. `tools/validation/Doc72SourceValidator` usa el parser oficial Roslyn de Visual Basic para resolver clases, pertenencia de métodos, visibilidad, parámetros por posición, retornos, sobrecargas y propiedades DTO directamente desde los árboles sintácticos. Esta es la comprobación reproducible utilizada en CI porque el checkout limpio no contiene varias DLL propietarias referenciadas desde `bin/`.
3. `tools/validation/Validate-Doc72DotNetSymbols.ps1` ofrece una comprobación adicional por reflexión cuando está disponible el ensamblado completo compilado localmente.

Comandos:

```powershell
node --test tests/doc72-technical-documentation.test.cjs
dotnet run --project ./tools/validation/Doc72SourceValidator/Doc72SourceValidator.csproj -- `
  ./Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-72-nucleo-registro-adaptadores/diagram-contract.json .
powershell.exe -NoProfile -ExecutionPolicy Bypass -File ./tools/validation/Validate-Doc72DotNetSymbols.ps1 `
  -ManifestPath ./Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-72-nucleo-registro-adaptadores/diagram-contract.json `
  -AssemblyPath ./bin/GestionDocumental-Docuarchi.net.dll
```

La workflow `.github/workflows/opsxj-validation.yml` ejecuta Acorn y Roslyn sin depender de las DLL privadas del despliegue, y se activa cuando cambian documentación, módulos DOC-72, DTOs, interfaces, endpoint, code-behind, prueba o validadores.

Resultados registrados el 2026-09-21:

- prueba documental Node: 4 aprobadas, 0 fallidas;
- prueba documental .NET por Roslyn y, localmente, por reflexión: 13 firmas y 17 tipos válidos;
- prueba documental más regresiones DOC-72: 22 aprobadas, 0 fallidas;
- suite `tools/opsxj`: 14 archivos y 124 pruebas aprobadas;
- `openspec validate doc-72-nucleo-intrfaz-integracion-sii --strict`: válido.

## Qué garantiza y qué no

La prueba valida existencia y correspondencia estructural: archivos requeridos, sintaxis Mermaid admitida, símbolos JavaScript analizados por AST y firmas .NET analizadas por Roslyn, con reflexión adicional disponible localmente. Falla indicando diagrama, símbolo y motivo.

No demuestra por sí sola la fidelidad completa del comportamiento, la corrección de reglas de negocio, la cobertura de todos los casos de uso ni el funcionamiento E2E. Tampoco ejecuta sesiones autenticadas, red, base de datos o proveedor externo.

## Gobierno E2E

No se activó `WorkflowCentroTrabajoModernActive` ni se ejecutó E2E/carga por falta de autorización explícita. El gate permanece `false`, con usuarios y grupos vacíos.
