# Prompt 04 — Preparación individual y múltiple

Implementa la captura de requisitos previa a cualquier escritura utilizando el mismo contrato para una colección de uno o varios elementos.

Depende de `PreflightImport` y `CreateImportIntent` publicados por el Prompt backend 03 sobre los contratos de B01.

## Objetivo

Unificar la preparación de importaciones individuales y múltiples sin hacer que **Guardar todas** inicialice implícitamente el contexto.

## Rutas canónicas de implementación

```txt
js/workflow/importar-servicio-web/
├── importar-servicio-web-preparation.js
├── importar-servicio-web-requirements.js
└── importar-servicio-web-intent-client.js

Tests/
├── importar-servicio-web-preparation.test.cjs
├── importar-servicio-web-preflight-contract.test.cjs
└── importar-servicio-web-intent-client.test.cjs
```

- El popup secundario se agrega en `workflow/Webworkflow.aspx` y reutiliza `Styles/importar-servicio-web-modern.css`.
- `intent-client.js` usa `importar-servicio-web-api.js`; no duplica transporte ni persistencia.
- Consumir fixtures B03 desde `Tests/Fixtures/Workflow/ImportarServicioWeb/intents-v1/`.
- No modificar `JSExpediente.js`, `JSProgresBar.js`, mutadores legacy, `ClassAlmacenamiento` ni almacenamiento.

## Ruta documental obligatoria

```txt
docs/modulos/workflow/importar-servicio-web/SCRUMCORE-000-preparacion-individual-multiple/
```

Sustituir `SCRUMCORE-000` por el ticket real; crear el paquete canónico y `Diagramas/` exclusivamente allí.

## Implementa

- Popup secundario contextual para una fila, con identidad inequívoca y selector de tipología.
- Preparación múltiple para los elementos seleccionados.
- Validación de tipología obligatoria según configuración de digitalización.
- Plan de efectos confirmado por backend: destino, documentos y requisitos adicionales declarados por el adaptador.
- Una colección de exactamente un elemento para el recorrido individual.
- Contrato de preflight o estado bloqueado documentado cuando el backend aún no pueda preparar el contexto SII independientemente.

## Restricciones

- No mantengas caminos de persistencia separados para individual y múltiple.
- El frontend no persiste la intención ni ejecuta sus efectos; solicita al backend su creación idempotente.
- Está prohibido modificar o invocar directamente `AlmacenaDocumentoTareaWorkflow(...)`, `ClassAlmacenamiento` o mutadores legacy.
- El núcleo no debe conocer caché, expediente ni índices SII.
- No presentes como ejecutable un plan que el backend no haya confirmado.
- Cancelar la preparación no produce mutaciones y devuelve el foco a la fila.

## Aceptación

- Guardar permanece deshabilitado mientras falten datos obligatorios.
- La preparación individual no exige seleccionar ni importar todos los elementos.
- Los requisitos específicos se obtienen desde el adaptador.

## Correcciones opsxj:prompt-review

Estas reglas fueron agregadas desde `opsxj:prompt-review` para cubrir hallazgos estructurales corregibles. Deben ajustarse al contexto real del ticket antes de enviar a implementacion.

## Rol esperado
Definir el rol tecnico esperado para ejecutar el ticket.

## Objetivo
Describir el objetivo funcional y tecnico verificable.

## Restricciones criticas
- No introducir cambios fuera del alcance declarado.
- No romper comportamiento existente ni contratos publicos.

## Criterios de aceptacion
- El comportamiento implementado cumple el flujo esperado y queda validado con evidencia.

## Contexto obligatorio
Leer F01–F03, contratos `PreflightImport`/`CreateImportIntent`, fixtures B03, `workflow/Webworkflow.aspx` y el comportamiento legacy de preparación solo como referencia. No modificar ejecutores o mutadores existentes.

## Pruebas obligatorias
Ejecutar pruebas unitarias/focales, build/tsc segun impacto y E2E con Playwright cuando el flujo lo requiera; registrar comandos y resultados.

## Documentacion tecnica
Actualizar exclusivamente el paquete de **Ruta documental obligatoria**, con flujo individual/múltiple, contrato, requisitos, estados, pruebas y diagramas.

## Entregable final
Entregar codigo, pruebas, documentacion, diagramas y evidencia coherente con lo realmente implementado.

Exigir `npm run build` o `tsc` segun impacto y registrar el resultado.

Exigir pruebas unitarias/focales con Vitest o Testing Library segun el alcance.

Registrar comandos ejecutados, resultados obtenidos y evidencia en `05-PruebasEvidencia.md`.

Cuando el ticket afecte un flujo completo de usuario, navegacion, integracion entre vistas, persistencia de estado u operacion transaccional, exigir E2E real con Playwright; si no aplica, documentar justificacion formal y evidencia manual.
