## 1. Integracion de campo ASUNTO

- [x] 1.1 Localizar en `camposPlantilla` el registro `name_campo = "ASUNTO"` dentro del flujo de renderizado de radicacion.
- [x] 1.2 Conectar el control `data-ident="pl-radicacion-spe-ASUNTO"` al flujo de autocompletado con metadatos declarativos de plantilla.

## 2. Consumo de API y UX del autocompletado

- [x] 2.1 Implementar consulta a `/api/PlantillaRadicado/solicitaAutoCompleteCampos` para sugerencias de ASUNTO.
- [x] 2.2 Manejar estados de carga, resultados vacios y error sin bloquear ingreso manual del campo.

## 3. Pruebas y evidencia

- [x] 3.1 Agregar/actualizar pruebas Vitest para escenarios `[SPEC:ASA-001]`, `[SPEC:ASA-002]` y no regresion del renderer dinamico.
- [x] 3.2 Ejecutar pruebas del modulo de radicacion y registrar evidencia de ejecucion en este archivo.

## Test Evidence

Run: 2026-02-24 local

```text
> .\node_modules\.bin\vitest.cmd --run src/modules/radicacion/components/RadicacionForm.spec.test.tsx

 RUN  v4.0.18 D:/imagenesda/GestorDocumental/DocuArchiCore/DocuArchiCore.react

✓ src/modules/radicacion/components/RadicacionForm.spec.test.tsx (10 tests)

 Test Files  1 passed (1)
      Tests  10 passed (10)
```
