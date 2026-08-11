## 1. Refinement

- [x] 1.1 Consolidar alcance final desde Jira + contexto de codigo.
- [x] 1.2 Ajustar design/spec con decisiones y riesgos definitivos.

## 2. Implementacion

- [x] 2.1 Implementar cambios funcionales del ticket.
- [x] 2.2 Mantener compatibilidad y evitar regresiones.

## Politica Frontend AppResponses<T>

Cuando el ticket cree o modifique servicios, hooks, componentes o flujos que consuman APIs con `AppResponses<T>`:

- [x] Crear o reutilizar `src/shared/api/appResponseError.ts` antes de agregar nuevos parsers locales de `AppResponses<T>`. No aplica: DOC-2 no consume ese envelope y no existe `src/shared/api`.
- [x] Usar `getUserVisibleAppResponseMessage` para mensajes visibles de UI. No aplica: no se agregan mensajes de API.
- [x] Priorizar `UserMessage/userMessage` antes de `Message/message/errorMessage`. No aplica: no se agregan mensajes de API.
- [x] Usar `response.message` solo si el helper confirma que no contiene detalle tecnico. No aplica: no se agregan mensajes de API.
- [x] No mostrar `requestId`, `code=`, SQL, rutas, stack trace, tokens ni mensajes internos en UI. No aplica: no se agregan mensajes de API.
- [x] Agregar prueba donde `UserMessage` gane sobre un `response.message` tecnico con `code` y `requestId`. No aplica: no hay consumidor `AppResponses<T>`.
- [x] Registrar diagnostico tecnico completo solo con `logAppResponseErrorDiagnostic` y solo bajo `window.__APP_RESPONSE_DEBUG__ = true`. No aplica: no hay diagnóstico `AppResponses<T>`.
- [x] Exponer `errorsDebugOn()` / `errorsDebugOff()` como alias de consola para activar y apagar `window.__APP_RESPONSE_DEBUG__`. No aplica: no hay diagnóstico `AppResponses<T>`.
- [x] No crear `console.error`, `console.warn` o `console.info` locales que impriman payloads completos de `AppResponses<T>` fuera del helper. Verificado: el adaptador DOC-2 no registra payloads ni usa consola.

Bloqueo estricto gradual: si `src/shared/api/appResponseError.ts` aun no existe, sembrar como primer paso obligatorio crearlo o reutilizarlo; despues de existir, los nuevos consumidores deben delegar la resolucion de errores al helper.

## 3. Pruebas

- [x] 3.1 Agregar/ajustar pruebas unitarias e integracion.
- [ ] 3.2 Ejecutar suite afectada y registrar evidencia. Bloqueado solo para `manual_qa`: falta ambiente WebForms, URL y cuentas piloto/no piloto.

## 4. Cierre

- [x] 4.1 Validar OpenSpec.
- [x] 4.2 Documentar diff final y decisiones de arquitectura.
