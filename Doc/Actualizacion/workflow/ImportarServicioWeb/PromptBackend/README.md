# Prompts de modernización backend de ImportarServicioWeb

Esta carpeta convierte las decisiones de `../Exploracion/03-exploracion-backend-importar-servicio-web.md` en entregas backend ejecutables. La radiografía del comportamiento vigente está en `../Exploracion/04-radiografia-backend-actual.md`.

El contrato normativo, mapeo de estados, propiedad de la ejecución y orden cruzado con frontend están en [`../CONTRATO-COMPARTIDO-FRONTEND-BACKEND.md`](../CONTRATO-COMPARTIDO-FRONTEND-BACKEND.md).

## Orden de ejecución

1. [`01-contratos-contexto-registro-multiproveedor.md`](01-contratos-contexto-registro-multiproveedor.md)
2. [`02-clientes-http-asincronos-seguridad.md`](02-clientes-http-asincronos-seguridad.md)
3. [`03-preflight-intencion-idempotencia.md`](03-preflight-intencion-idempotencia.md)
4. [`04-orquestacion-secuencial-estados-compensacion.md`](04-orquestacion-secuencial-estados-compensacion.md)
5. [`05-reconciliacion-lista-documentos.md`](05-reconciliacion-lista-documentos.md)
6. [`06-adaptador-sii-compatibilidad-asmx.md`](06-adaptador-sii-compatibilidad-asmx.md)
7. [`07-pruebas-backend-evidencia.md`](07-pruebas-backend-evidencia.md)

Cada prompt debe ejecutarse mediante un cambio OpenSpec propio o tareas atómicas trazables. Antes de implementar, debe resolver o registrar como bloqueo las preguntas abiertas que afecten su alcance; una suposición no puede convertirse silenciosamente en contrato productivo.

## Reglas comunes

- La modernización es una implementación paralela y aditiva: no reemplaza, reescribe ni redirige silenciosamente el código existente.
- El recorrido vigente permanece intacto y seleccionable mediante el gate hasta que un cambio posterior, independiente y autorizado apruebe su retiro.
- La nueva orquestación debe reutilizar `AlmacenaDocumentoTareaWorkflow(...)` como infraestructura existente y tratarla como una caja negra.
- Está prohibido modificar `AlmacenaDocumentoTareaWorkflow(...)`, su lógica interna o sus consumidores existentes dentro de cualquiera de estos prompts.
- Mantener .NET Framework 4.6.1 y compatibilidad legacy mientras no exista evidencia para retirarla.
- El contexto de tarea es explícito e inmutable; la sesión solo participa como control adicional.
- `INTEGRACIONSII` es un adaptador, nunca el fallback de un proveedor desconocido.
- El I/O HTTP nuevo es asíncrono de extremo a extremo, sin `.Result`, `.Wait()`, `GetAwaiter().GetResult()` ni `Task.Run` como puente.
- La primera modernización procesa elementos secuencialmente.
- No exponer secretos, rutas físicas, respuestas externas crudas ni mensajes internos.
- No ejecutar E2E real, carga, escrituras de ambiente ni gates sin autorización explícita.
- Antes de cualquier prueba autenticada de `PreviewEnviarTarea`, leer `tools/e2e/AGENT-RUNBOOK.md`.

## Frontera con los prompts frontend

Los prompts de `../Prompt/` consumen contratos confirmados por este backend. El frontend no sustituye autorización, idempotencia, persistencia, progreso ni reconciliación del servidor.

“Implementación paralela” describe la coexistencia entre arquitectura nueva y legacy; no autoriza procesamiento concurrente de elementos, que seguirá siendo secuencial.
