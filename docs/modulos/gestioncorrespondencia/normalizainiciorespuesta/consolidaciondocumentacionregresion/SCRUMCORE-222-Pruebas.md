# SCRUMCORE-222 - Pruebas

## Estrategia

Hardening sin cambios funcionales. Se ejecuta por capas y con bloqueos de entorno claramente separados de regresiones de código:

- Validar estabilidad del refactor transversal.
- Asegurar que no hay regresiones en documento + visor + adjuntos.
- Probar únicamente en alcance de cambios sin introducir features nuevas.

## Unitarias

Ejecutadas:

- `src/modules/gestionCorrespondencia/tests/useListaDocumentosRadicadosTreeTable.test.tsx`
- `src/modules/gestionCorrespondencia/tests/useGestionRespuestaDocumentos.test.tsx`
- `src/modules/gestionCorrespondencia/tests/useGestionRespuestaDocumentosTable.test.tsx`
- `src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx`

Comandos ejecutados:

```bash
npx vitest run src/modules/gestionCorrespondencia/tests/useListaDocumentosRadicadosTreeTable.test.tsx
npx vitest run src/modules/gestionCorrespondencia/tests/useGestionRespuestaDocumentos.test.tsx
npx vitest run src/modules/gestionCorrespondencia/tests/useGestionRespuestaDocumentosTable.test.tsx
npx vitest run src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx
```

Resultados:

- `useListaDocumentosRadicadosTreeTable.test.tsx`: **1 archivo / 4 pruebas passed**.
- `useGestionRespuestaDocumentos.test.tsx`: **1 archivo / 7 pruebas passed**.
- `useGestionRespuestaDocumentosTable.test.tsx`: **1 archivo / 10 pruebas passed**.
- `DocumentosWorkbench.test.tsx`: **1 archivo / 6 pruebas passed**.

Pendientes:

- Cobertura adicional en algunos escenarios de integración extensa no incluida en este ciclo.

## Integración

Ejecutadas:

- Validación explícita de contrato de hook `useListaDocumentosRadicadosTreeTable` contra contexto transversal (`nombreGabinete`, `gabineteLoading`, `gabineteError`, `reloadGabinete`) desde tests de hook y workbench.
- Validación de guardias de flujo (`radicado` requerido y estado NO de existencia) y fallback de paginación total.
- No se detectaron cambios de contratos públicos de `load/loadChildren` ni de acción de árbol.

Pendientes:

- Evidencia manual completa integrada en entorno real (responsive + interacción completa de pestañas/visor/documentos/adjuntos).

## Interacción en navegador

Pendientes:

- Flujo completo GestionRespuesta con árbol + visor + adjuntos (manual + automatización navegable).
- Validación de teclado, foco y estados de carga/reintento en condiciones reales de uso.

## E2E

Pendientes (bloqueadas por entorno):

- `test:e2e` y escenarios SCRUM-205.
- Escenario de error de gabinete con `reloadGabinete`.
- Escenario responsive (mobile/tablet) con interacción intensiva.

Bloqueo actual:

- Variables requeridas no presentes en el entorno local:
  - `PLAYWRIGHT_LOGIN_EMPRESA_ID`
  - `PLAYWRIGHT_LOGIN_MODULO_ID`
  - `PLAYWRIGHT_LOGIN_USER`
  - `PLAYWRIGHT_LOGIN_PASSWORD`

## QT / Calidad

Ejecutadas:

- `npx eslint` sobre archivos tocados (ver archivo de salida de lint) sin errores.
- `npx vitest run ...` sobre los archivos listados en unitarias.

Pendientes:

- `npm run build` / `npx tsc -b` completo del repositorio.
- `npx eslint .` global completo.

## Cobertura y matriz de evidencias

| Dominio                                   | Unitario | Integración | Navegador | E2E |
|-------------------------------------------|----------|-------------|-----------|-----|
| Contexto transversal (gabinete)            | Sí       | Sí          | Parcial    | No (bloqueado) |
| Hook documentos (`useListaDocumentos...`)  | Sí       | Sí          | Parcial    | No (bloqueado) |
| Árbol documental (`ver_documento`)         | Sí       | Sí          | Parcial    | No (bloqueado) |
| Adjuntos / visor                           | Parcial  | Parcial      | Parcial    | No (bloqueado) |

## Defectos y mitigación

- Se detectó warning de lint en estado previo y se corrigió sin alterar contratos.
- No hay evidencia de defectos funcionales nuevos en los escenarios ejecutados.

## Conclusión de calidad

Con la evidencia local ejecutada, SCRUMCORE-222 avanza en el bloque de hardening sin cambios funcionales ni de contrato.  
Quedan pendientes solo los escenarios de ejecución navegable/E2E e integración manual/release por variables y setup externo de Playwright.
