# CORRECCION-INTEGRAL-REGISTRO-RECIBO-SII

- Ticket: DOC-87
- Cambio OpenSpec: doc-87-correccion-integral-registro-recibo-sii
- Clasificacion: cross_cutting (Transversal)

## Evidencia requerida

- Pruebas focales DOC-87: 16/16 aprobadas, incluidos los contratos de flujo y flujo SII, subtipo vacío, duplicado histórico, reintento de evento y el arnés del servicio contra el ensamblado compilado sin abrir bases.
- Integración MySQL 5.1 real y aislada: 1/1 aprobada. Fuerza fallos en las cinco escrituras de Workflow y confirma rollback total; valida caída/reintento/conflicto Docuarchi y una carrera fija de dos solicitudes con una sola tarea y un solo evento.
- Políticas DOC-87 de la plataforma E2E: 6/6 aprobadas; incluyen normalización de navegación por fragmento y consumo conservador de la reserva si una falla ocurre después de una mutación observable. Regresión compartida DOC-83/plataforma: 191/191 aprobadas.
- Suites protegidas de Workflow, importación SII, Radicación Simplificada y adjuntos: 160/160 aprobadas.
- `msbuild.exe GestionDocumental-Docuarchi.net.sln /t:Build /p:Configuration=Debug /m`: 0 errores; advertencias legacy preexistentes.
- DOC-87 está registrado como `registro-ruta-sii-execution` en `test:workflow:platform`; reutiliza sesión, TTY, reserva de recurso, controles ODBC y evidencia saneada comunes.
- La E2E real autorizada se ejecutó el 2026-10-05 con un recibo descartable. El alta alcanzó exactamente un registro público, una tarea y un evento outbox; el evento terminó `NO_APLICA`, por lo que no correspondía crear relación Docuarchi. No se observaron duplicados y no se activó ningún gate.
- La corrida fue clasificada inicialmente como `REGISTRO_RUTA_SII_E2E_UI_CONTRACT_INVALID` después del alta. La causa fue del arnés: `framenavigated` contabilizaba como navegación el cambio de fragmento producido por un enlace legacy `href="#"`. La aplicación sí confirmó la operación y los controles `SELECT` observaron la mutación esperada.
- El runner ahora compara la URL del documento sin fragmento y conserva la detección de cambios reales de ruta. Además, si los controles confirman una mutación antes de una falla posterior, la reserva se consume en vez de liberarse.

## Estado de cierre E2E

- La integración usó exclusivamente `doc87_workflow_it` y `doc87_docuarchi_it` sobre el MySQL local confirmado como ambiente de pruebas. Ambos esquemas fueron eliminados en `finally`; no se escribieron tablas de aplicación.
- La ejecución final autorizada del 2026-10-05 terminó correctamente de principio a fin: bloqueó el contexto obsoleto y la actividad `0`, realizó un alta única por interfaz y confirmó el resultado mediante seis controles de solo lectura.
- El reintento fue clasificado como `ALREADY_REGISTERED`; no creó una segunda tarea, evento, registro público ni relación. La tarea OpenSpec 5.4 quedó completada con evidencia saneada en `tools/e2e/artifacts/workflow-e2e-platform-registro-ruta-sii-execution.json`.
- Estado del gate al finalizar la validación local: configuración retirada/ausente; no se agregó ni activó `WorkflowCentroTrabajoModernActive` y no se alteraron listas de usuarios o grupos.

## QA/E2E WebForms

La prueba real usa la sesión autenticada compartida, controles ODBC exclusivamente `SELECT`, un recibo desechable y comprobación final de gate apagado/listas vacías. No persiste secretos ni evidencia sensible. El spec Playwright aislado fue retirado para evitar infraestructura paralela.

La integración de repositorios se ejecuta únicamente con `DOC87_MYSQL_INTEGRATION_AUTHORIZED=true`; obtiene el DSN local desde el almacén de Windows, nunca imprime la conexión y elimina sus dos esquemas temporales aun ante fallo.
