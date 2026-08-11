## 1. Refinement

- [x] 1.1 Consolidar alcance final desde Jira + contexto de codigo.
- [x] 1.2 Ajustar design/spec con decisiones y riesgos definitivos.

## 2. Implementacion

- [x] 2.1 Implementar los entregables documentales del ticket; no hay cambio funcional autorizado.
- [x] 2.2 Mantener compatibilidad: no se modificaron archivos de aplicación, IDs ni contratos WebForms.

## Politica Frontend AppResponses<T>

Cuando el ticket cree o modifique servicios, hooks, componentes o flujos que consuman APIs con `AppResponses<T>`:

- [x] No aplica: DOC-1 no crea ni modifica consumidores de `AppResponses<T>`; el cambio es documental sobre WebForms.
- [x] No aplica: DOC-1 no crea ni modifica mensajes visibles de UI.
- [x] No aplica: DOC-1 no introduce resolución de envelopes de API.
- [x] No aplica: DOC-1 no introduce resolución de envelopes de API.
- [x] No aplica: DOC-1 no introduce mensajes ni payloads de API.
- [x] No aplica: DOC-1 no modifica consumidores de `AppResponses<T>`.
- [x] No aplica: DOC-1 no introduce diagnóstico de API.
- [x] No aplica: DOC-1 no introduce alias de consola.
- [x] No aplica: DOC-1 no introduce logging de envelopes de API.

Bloqueo estricto gradual: si `src/shared/api/appResponseError.ts` aun no existe, sembrar como primer paso obligatorio crearlo o reutilizarlo; despues de existir, los nuevos consumidores deben delegar la resolucion de errores al helper.

## 3. Pruebas

- [x] 3.1 No aplica: no se modificó código funcional; la matriz manual cubre la regresión a ejecutar.
- [x] 3.2 Ejecutar suite disponible y compilación Debug; registrar evidencia local y límites de prueba de navegador.

## 4. Cierre

- [x] 4.1 Validar OpenSpec (`openspec validate doc-1-contrato-regresion-workflow --strict`).
- [x] 4.2 Documentar diff y decisiones de arquitectura en los contratos DOC-1 y `design.md`.
