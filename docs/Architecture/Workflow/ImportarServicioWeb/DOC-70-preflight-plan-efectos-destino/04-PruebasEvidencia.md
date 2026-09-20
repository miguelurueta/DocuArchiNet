# Pruebas y evidencia

Resultados locales del 2026-09-20:

- `node --test tests/importar-servicio-web-preflight*.test.cjs`: 9/9 aprobadas.
- `node --test tests/importar-servicio-web-*.test.cjs`: 361/361 aprobadas.
- `msbuild GestionDocumental-Docuarchi.net.sln /t:Build /p:Configuration=Debug /m /verbosity:minimal`: código 0; conserva advertencias heredadas.
- `npm.cmd --prefix tools/e2e run test:doc56:policy`: 18/18 aprobadas, incluida la política DOC-70 de plan y fingerprint repetido.
- Cobertura estructural: contrato aditivo, plan individual/múltiple, campos prohibidos, pureza, SQL parametrizado, cero dependencias SII, fingerprint y rechazo antes del lock.
- E2E real de lectura en CERTIFICACION, tarea 220580: `success=true`; capacidades, consulta, preview, consumo seguro del descriptor y preflight confirmados.
- El adaptador ejecutó el preflight dos veces, con la selección en orden normal e inverso, y confirmó un fingerprint estable y un plan ejecutable.
- Los siete controles SELECT (`índices`, `cache`, `relaciones`, `expediente`, `intención`, `items` y `auditoría`) permanecieron sin cambios.
- Cierre operativo confirmado: `WorkflowCentroTrabajoModernActive=false`, usuarios/grupos vacíos y páginas legacy sin cambios.

Evidencia saneada: `tools/e2e/artifacts/workflow-e2e-platform-import-sii-read.json`. El archivo conserva códigos, conteos, latencias y banderas; no contiene secretos ni cuerpos SII.

La tarea 220580 ya estaba consumida y no se reutilizó para una mutación DOC-70. La ausencia de clientes SII en la frontera preflight está cubierta por pruebas estructurales; la lectura real demuestra además pureza persistente mediante siete controles SELECT sin cambios. Las llamadas SII posteriores de consulta, preview y ejecución pertenecen a etapas distintas del preflight.

Intento integrado del 2026-09-20 sobre la tarea descartable 220581:

- La consulta SII encontró un sello disponible y el preflight DOC-70 confirmó selección, tipología, configuración, plan y fingerprint.
- La ejecución creó la intención pero se detuvo de forma segura con `IMPORT_E2E_EXECUTE_FAILED_SII_SUBJECT_INCOMPLETE` antes de almacenar, vincular, indexar o registrar caché documental.
- El proveedor devolvió el nombre del establecimiento pero no su NIT directo; los datos crudos no se conservan como evidencia.
- La reserva E2E fue liberada y el gate quedó restaurado en `false`, con usuarios y grupos vacíos.
- El intento no satisface la aceptación integrada de DOC-70 porque no alcanzó efectos físicos confirmados. No se declara como prueba aprobada.

Durante el diagnóstico se confirmó que la ejecución también confunde “trámite sin gestión de expediente” con “creación deshabilitada”. Esa corrección cambia ejecución y reconciliación, está fuera de los Non-Goals de DOC-70 y quedó aislada en `Doc/Actualizacion/workflow/ImportarServicioWeb/PromptBackend/13-importacion-sin-expediente-por-configuracion.md`.

E2E integrada aprobada del 2026-09-20 sobre la tarea descartable 220582:

- Contexto real: ESAL, tipología `Constancia De Inscripción`, un sello seleccionado y creación de expediente habilitada por configuración autoritativa.
- Lectura/preflight: `success=true`, fingerprint repetido estable, plan ejecutable y siete controles SELECT sin cambios.
- Ejecución: `success=true`; una intención, un item y un documento almacenado, con creación/confirmación, consulta, ejecución y reconciliación sin código de error.
- Auditoría autoritativa: un item y dos documentos relacionados; almacenamiento, relación, índices, caché, inscripción, universo relacionado y reconciliación confirmados; `DOC67_EVIDENCE_VERDICT=PASSED`.
- Reserva consumida exactamente una vez; gate restaurado a `false`, usuarios/grupos vacíos y páginas legacy sin cambios.
- La evidencia conserva únicamente códigos, conteos, estados y latencias; no se almacenan credenciales ni respuestas SII crudas.
