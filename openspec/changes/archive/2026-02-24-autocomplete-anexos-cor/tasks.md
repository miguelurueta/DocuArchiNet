## 1. Descubrimiento y ajuste de contratos

- [x] 1.1 Revisar `RadicacionForm` y localizar el control con `data-ident="pl-radicacion-spe-ANEXOS_COR"` para confirmar props actuales (required, disabled, title, tooltipAyuda)
- [x] 1.2 Identificar en `camposPlantilla` el objeto con `name_campo = "ANEXOS_COR"` y el origen de `ComportamientoCampo`
- [x] 1.3 Validar la función de consumo Axios existente para autocompletados (query/mutation) y su manejo centralizado de errores

## 2. Servicio y tipado de autocompletado

- [x] 2.1 Definir tipos TypeScript para request/response del endpoint `/api/PlantillaRadicado/solicitaAutoCompleteCampos`
- [x] 2.2 Implementar o extender el servicio de API para autocompletado usando Axios centralizado
- [x] 2.3 Agregar manejo de loading y errores en la capa de consulta

## 3. Componente reutilizable de autocompletado

- [x] 3.1 Crear componente reutilizable (MUI/AntD) que reciba `campo` y derive `data-ident`, `tbl_control`, `name_campo`, `title` y `tooltipAyuda`
- [x] 3.2 Implementar consulta con debounce o `minLength` antes de llamar al servicio
- [x] 3.3 Renderizar opciones usando `texValue` y mostrar indicador de loading

## 4. Integración en RadicacionForm

- [x] 4.1 Integrar el componente en el render de campos dinámicos cuando `name_campo = "ANEXOS_COR"`
- [x] 4.2 Asegurar que `data-ident` sea `pl-radicacion-spe-ANEXOS_COR` y que atributos existentes se conserven
- [x] 4.3 Mantener tooltip con `title_control` y `tooltipAyuda` junto al label

## 5. Tests y evidencia

- [x] 5.1 Crear/actualizar pruebas de UI para el autocompletado con respuesta mock (incluyendo loading y opciones)
- [x] 5.2 Validar manejo de error con mock de fallo en el servicio
- [x] 5.3 Ejecutar tests relevantes y registrar evidencia en la documentación OpenSpec

## Test Evidence

Run: 2026-02-24 local (failed)

```text
> docuarchicore-react@0.0.0 test
> vitest RadicacionForm.spec.test.tsx --run

failed to load config from D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore.react\vite.config.ts

Startup Error
Error: spawn EPERM
    at ChildProcess.spawn (node:internal/child_process:421:11)
    at Object.spawn (node:child_process:796:9)
    at ensureServiceIsRunning (...\node_modules\esbuild\lib\main.js:1978:29)
```

Run: 2026-02-24 local (failed)

```text
> npx vitest --run src/modules/radicacion/components/RadicacionForm.spec.test.tsx

failed to load config from D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore.react\vite.config.ts

Startup Error
Error: spawn EPERM
    at ChildProcess.spawn (node:internal/child_process:421:11)
    at Object.spawn (node:child_process:796:9)
    at ensureServiceIsRunning (...\node_modules\esbuild\lib\main.js:1978:29)
```

Run: 2026-02-24 local (failed)

```text
> cmd /c "set ESBUILD_BINARY_PATH=D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore.react\node_modules\esbuild\esbuild.exe&& npx vitest --run src/modules/radicacion/components/RadicacionForm.spec.test.tsx"

[esbuild] Ignoring bad configuration: ESBUILD_BINARY_PATH=D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore.react\node_modules\esbuild\esbuild.exe
failed to load config from D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore.react\vite.config.ts

Startup Error
Error: spawn EPERM
    at ChildProcess.spawn (node:internal/child_process:421:11)
    at Object.spawn (node:child_process:796:9)
    at ensureServiceIsRunning (...\node_modules\esbuild\lib\main.js:1978:29)
```

Run: 2026-02-24 local

```text
> npx vitest --run src/modules/radicacion/components/RadicacionForm.spec.test.tsx

 RUN  v4.0.18 D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore.react

✓ src/modules/radicacion/components/RadicacionForm.spec.test.tsx (5 tests) 3960ms
     ✓ [SPEC:RAD-001] muestra opciones dinámicas de trámite desde plantilla 1214ms
     ✓ [SPEC:RAD-002] no muestra opciones si plantilla no trae opciones 654ms
     ✓ [SPEC:RAD-003] llena opciones del select TipoRadicado desde plantilla 696ms
     ✓ [SPEC:RAD-004] renderiza autocompletado de ANEXOS_COR y consulta la API con los parámetros correctos 807ms
     ✓ [SPEC:RAD-005] muestra mensaje de error cuando falla el autocompletado 588ms

 Test Files  1 passed (1)
      Tests  5 passed (5)
   Start at  11:19:17
   Duration  7.32s (transform 171ms, setup 96ms, import 2.43s, tests 3.96s, environment 678ms)
```

Run: 2026-02-24 local (failed)

```text
> $env:ESBUILD_BINARY_PATH=''; npx vitest --run src/modules/radicacion/components/RadicacionForm.spec.test.tsx

ERROR: Command parsing failed in PowerShell

failed to load config from D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore.react\vite.config.ts

Startup Error
Error: spawn EPERM
    at ChildProcess.spawn (node:internal/child_process:421:11)
    at Object.spawn (node:child_process:796:9)
    at ensureServiceIsRunning (...\node_modules\esbuild\lib\main.js:1978:29)
```
