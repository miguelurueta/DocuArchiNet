# DOC-87 — Registro de recibo SII para ruta

## Resultado oficial

La operación conserva la URL ASMX existente, pero queda aislada del escritor legacy compartido. El navegador envía únicamente recibo, trámite y actividad; el servidor autoriza desde sesión, reconsulta SII y recompone los datos persistibles. El trámite se resuelve con `subtipotramite` del radicado o, cuando está vacío, con `tipotramite` del recibo; cliente y servidor exigen la misma coincidencia única normalizada.

Workflow y Docuarchi usan conexiones diferentes. Por ello, la garantía aprobada es:

```text
Cliente
  -> ASMX (adaptador compatible)
  -> Servicio DOC-87 (sesión, permiso, SII y catálogos)
  -> bloqueo Workflow por ruta/recibo
  -> detección parametrizada [outbox + F_W_E_REGISTROPUBLICO]
       -> existente: ALREADY_REGISTERED, sin nueva tarea
  -> transacción Workflow [tarea + outbox]
  -> commit
  -> bloqueo Docuarchi por recibo
  -> relación idempotente
  -> estado YES, REGISTERED_RELATION_PENDING o duplicado funcional
```

Una caída de Docuarchi no elimina la tarea ni pierde el trabajo: el evento queda `RETRYABLE`. Repetir el registro del mismo recibo reutiliza el evento y no crea una segunda tarea, pero responde `ALREADY_REGISTERED` o `ALREADY_REGISTERED_RELATION_PENDING`, nunca éxito de alta. Los recibos históricos sin outbox también se detectan en `F_W_E_REGISTROPUBLICO`. Una relación existente hacia otro expediente queda `REVIEW_REQUIRED` y nunca se sobrescribe.

## Matriz de consumidores protegidos

| Superficie | Tratamiento DOC-87 |
| --- | --- |
| Registrar tarea para ruta | Reemplaza solo su adaptador y persistencia |
| Registrar tarea para flujo | Solo se completa `atrib_campo_beetwen="0"` en sus ocho controles; lógica y serialización intactas |
| RUE y trámites virtuales | Solo se completa `atrib_campo_beetwen="0"` en sus ocho controles compartidos; lógica y eventos intactos |
| Importación SII/ENLASE | Sin cambios |
| Radicación Simplificada y adjuntos | Sin cambios |
| `general_control_java.js` | Sin cambios |
| `zeroFillFReciboSII` | Sin cambios; ruta usa canonizador exclusivo |
| `FileUploadHandler_.ashx` | Sin cambios |
| `ClassGestionTareasFlujoTrabajo` | Sin cambios |
| `ClassRaRelacionRadicadoExternoExpediente` | Sin cambios |

## Despliegue

1. Detener escrituras de **Registrar tarea para ruta** durante la ventana.
2. Ejecutar `SQL/01-apply-workflow-outbox.sql` en la base Workflow.
3. Ejecutar `SQL/02-preflight-workflow-outbox.sql`; es exclusivamente de lectura y debe confirmar InnoDB e índices únicos.
4. Desplegar aplicación y recursos versionados. No se modifica el esquema de `F_W_E_REGISTROPUBLICO`; la existencia del recibo se consulta antes de insertar.
5. Ejecutar pruebas focales y recorrido autorizado con un recibo desechable.
8. Inspeccionar por `SELECT` que el evento termine `CONFIRMED`, `NO_APLICA`, `RETRYABLE` o `REVIEW_REQUIRED` según el caso.

No se requiere gate ni recarga/postback. La migración debe existir antes de desplegar el binario.

## Rollback

1. Retirar primero la versión aplicativa DOC-87.
2. Consultar y resolver todos los eventos del outbox.
3. Ejecutar `SQL/03-rollback-workflow-outbox.sql` únicamente si la tabla está vacía.

El rollback devuelve `DOC87_ROLLBACK_BLOCKED_OUTBOX_HAS_EVENTS` y conserva la tabla cuando existen eventos, para impedir pérdida de trabajo y mantener compatibilidad con MySQL 5.1.

## Evidencia local

- 16 pruebas focales DOC-87 aprobadas, incluidos los contratos de flujo y flujo SII, subtipo vacío, duplicado histórico, reintento de evento y un arnés contra el ensamblado compilado sin bases.
- 1 integración MySQL 5.1 aprobada sobre dos esquemas temporales: rollback por cada etapa, reintento, conflicto y concurrencia 2×; limpieza confirmada al finalizar.
- 160 pruebas protegidas aprobadas para Workflow, importación SII, Radicación Simplificada y adjuntos.
- Solución compilada con cero errores.
- OpenSpec estricto y `git diff --check` aprobados.
- E2E real ejecutada con autorización: la interfaz produjo exactamente un registro público, una tarea y un evento outbox `NO_APLICA`, sin duplicados ni relación Docuarchi improcedente.
- El primer cierre del runner fue un falso negativo: un enlace legacy cambió únicamente el fragmento `#` y el contador lo interpretó como navegación. El arnés ahora elimina el fragmento antes de comparar URL y una prueba conductual protege esta regla.
- La reserva también se consume cuando una falla posterior ocurre después de que los controles de solo lectura demuestran una mutación, evitando reutilizar accidentalmente datos ya consumidos.
- La tarea E2E permanece abierta únicamente para repetir el recorrido completo con otro recibo descartable y obtener un artefacto verde que incluya el reintento idempotente. El recibo ya utilizado no debe relanzarse.
