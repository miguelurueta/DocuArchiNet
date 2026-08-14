## 1. Refinement

- [ ] 1.1 Consolidar alcance final desde Jira + contexto de codigo.
- [ ] 1.2 Ajustar design/spec con decisiones y riesgos definitivos.

## 2. Implementacion

- [ ] 2.1 Implementar cambios funcionales del ticket.
- [ ] 2.2 Mantener compatibilidad y evitar regresiones.

## Politica Frontend AppResponses<T>

Cuando el ticket cree o modifique servicios, hooks, componentes o flujos que consuman APIs con `AppResponses<T>`:

- [ ] Crear o reutilizar `src/shared/api/appResponseError.ts` antes de agregar nuevos parsers locales de `AppResponses<T>`.
- [ ] Usar `getUserVisibleAppResponseMessage` para mensajes visibles de UI.
- [ ] Priorizar `UserMessage/userMessage` antes de `Message/message/errorMessage`.
- [ ] Usar `response.message` solo si el helper confirma que no contiene detalle tecnico.
- [ ] No mostrar `requestId`, `code=`, SQL, rutas, stack trace, tokens ni mensajes internos en UI.
- [ ] Agregar prueba donde `UserMessage` gane sobre un `response.message` tecnico con `code` y `requestId`.
- [ ] Registrar diagnostico tecnico completo solo con `logAppResponseErrorDiagnostic` y solo bajo `window.__APP_RESPONSE_DEBUG__ = true`.
- [ ] Exponer `errorsDebugOn()` / `errorsDebugOff()` como alias de consola para activar y apagar `window.__APP_RESPONSE_DEBUG__`.
- [ ] No crear `console.error`, `console.warn` o `console.info` locales que impriman payloads completos de `AppResponses<T>` fuera del helper.

Bloqueo estricto gradual: si `src/shared/api/appResponseError.ts` aun no existe, sembrar como primer paso obligatorio crearlo o reutilizarlo; despues de existir, los nuevos consumidores deben delegar la resolucion de errores al helper.


## 3. Pruebas

- [ ] 3.1 Agregar/ajustar pruebas unitarias e integracion.
- [ ] 3.2 Ejecutar suite afectada y registrar evidencia.

## 4. Cierre

- [ ] 4.1 Validar OpenSpec.
- [ ] 4.2 Documentar diff final y decisiones de arquitectura.
