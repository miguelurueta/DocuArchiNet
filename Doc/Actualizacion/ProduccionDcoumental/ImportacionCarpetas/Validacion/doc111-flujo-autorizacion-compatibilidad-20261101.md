# DOC-111: continuación del flujo, autorización y compatibilidad

Actualización: el usuario autorizó expresamente la propuesta EXPEDIENTE mediante «contina aprobado». Se registraron ambiente/cuenta/recursos/version/nombres exactos en `tools/e2e/artifacts/doc111-expediente-authorization-15.json`. La sesión 15 se abrió para capturar credenciales privadas; su resultado se conserva en `doc111-expediente-session-15.json`. Hasta comprobar el recorrido completo, 13.12 sigue pendiente.

Antes de abrirla se detectó una compilación local posterior en bin, SHA-256 `4134f57f66fc259b5f33a553577c9a07e6bf176b2da027aaeeb8576325c282c5`. Coincide con obj/Debug; los 110 archivos del manifiesto de fuentes no cambiaron y los 776 cuerpos de métodos revisados de coordinador/repositorio/adaptador/autoridad/composición/catálogo coinciden con el build validado anterior. Se conservó esa compilación, sin sobrescribirla. Reporte `tools/e2e/artifacts/doc111-active-binary-comparison.json`. Login/Producción HTTP 200 sin error a las 16:14:45 UTC; precheck en `doc111-expediente-precheck.json`. Se actualizó el guard de la consola a esa huella después de la comprobación.

Estado vigente: **123/130**. Se cierran 5.9 y 6.6 con verificación local; quedan siete tareas de aceptación/revisión. El lote 2 permanece COMPLETADO/revisión 25 y conserva la aprobación manual del usuario sobre los índices de sus dos documentos.

## 5.9 — Revocación con sesión todavía autenticada

El arnés existente realiza primero una consulta ASMX autorizada. Conserva el mismo ID de sesión, identidad, flags antiguos IMPORTAR_CARPETAS/PuedeImportar y token CSRF; cambia exclusivamente la respuesta del doble de autoridad vigente. Comprueba las doce operaciones operativas ASMX (apertura, perfiles, plan/confirmación, consultas/CSV, avance, detención, reconciliación y cierre sin inicio) y el handler de recepción.

Todas responden 403/ACCESO_DENEGADO sin datos, antes de leer el cuerpo, consultar recursos/configuración o componer puertos de preparación/recepción/ejecución. Cada ASMX vuelve a consultar la autoridad; el lote del doble no cambia. HTTP, sesión y autoridad son sintéticos; coordinador/fachadas/guards son reales. No se revocaron permisos del usuario 141 ni se ejecutó HTTP autenticado o DML real.

## 6.6 — Compatibilidad administrativa DOC-110

Se reutilizan los dobles RepositoryDouble y CatalogDouble del arnés DOC-110, con visibilidad interna exclusivamente de test. En la matriz importar/no importar y administrar/no administrar se comprueba que el guard, token, contexto, listado y rechazo administrativo mantengan sus resultados antes/después de la lectura operativa. Listar sin administración sigue denegado; un filtro inválido sigue rechazado; no hay escrituras administrativas ni elevación por importar.

Se ejecutó el arnés Framework completo DOC-110/DOC-111 y la coherencia SQL/DDL: **PASS**. No hubo cambios productivos VB, scripts de negocio, configuración o DLL; no requiere recompilar/desplegar el sitio. Estas dos tareas son verificaciones locales de contrato, sin presentarlas como revocación o administración MySQL/HTTP real.

Manifiesto `doc111-sha-flujo-autorizacion-20261101.json`: 110 archivos, fuentes SHA-256 `6d740dc885cd55790b9912068721fc6b7ccfcb338d830cf2c2b912bfa4e9abce`. Los 27 archivos protegidos siguen idénticos a HEAD normalizando finales de línea. DLL vigente `73e7bcc9476172ae6cf06be53099d06506b2f6ea89fe6a83b311296b1197f176`, sin nuevo despliegue.

## Siguiente REC preparado: EXPEDIENTE

Inspección SELECT de publicaciones disponibles: el perfil 24 activo conserva versión 37 COMPLETO_OBLIGATORIO/EXPEDIENTE y versión 38 COMPLETO_OBLIGATORIO/LOTE. La vía de lectura operativa existente acepta la versión publicada histórica 37. La comparación de snapshots confirma la misma configuración excepto el ámbito de integridad; las reglas equivalen en todos sus datos salvo ReglaId propios de cada versión. No se cambia la versión actualmente seleccionada/publicada 38.

Referencias: `tools/e2e/artifacts/doc111-flow-prerequisites-readonly.json` y `doc111-version37-comparison-readonly.json`. A las 16:06:03 UTC los nombres propuestos DOC111-20261101-EXP-01/02 no coincidían con ningún código o alias existente de empresa 2. No hay publicación PARCIAL para el gabinete 10 entre los candidatos inspeccionados.

Se preparó `tools/e2e/profiles/doc111-importacion-carpetas-expediente.runtime.json` reutilizando ambiente, DSN, nivel 7, gabinete 10, tipología 79, índice 14/TIPO=EEE y PDF existentes. Solo cambia la versión a 37, modo EXPEDIENTE y los dos nombres nuevos. `--check` PASS, sin login o efectos.

Consola preparada `.tmp/doc111-expediente-console-15.ps1`. Antes de ejecutarse exige aprobación específica registrada para esos nombres/versión y coincidencia del perfil/DLL. La aprobación de índices del lote 2 no se amplía a crear expedientes o documentos nuevos. Se solicitó esa autorización al usuario; hasta recibirla no se abre la sesión mutante ni se crea plan/lote/expediente/documento. La consola reutiliza el runner/login/ODBC existentes y credenciales exclusivamente en TTY enmascarado. No hay repetición del lote 2 ni nueva configuración de activación.

## Tareas restantes

| Tarea | Estado y dependencia real |
|---|---|
| 13.12 EXPEDIENTE | Perfil/versión y prueba preparados; autorización específica de recursos nuevos y credenciales pendientes. |
| 13.13 LOTE | Dos expedientes/documentos completos e índices aprobados. La sesión 14 falló en LOGIN; no acredita la comprobación final autenticada/reapertura ni el replay pendiente. |
| 13.14 PARCIAL | Sin publicación compatible disponible; requiere preparar y autorizar recursos/perfil de prueba antes de efectos. |
| 13.15 Esquema ausente | Aceptación de degradación integral pendiente; no se ocultarán tablas del ambiente actual sin autorización concreta. |
| 13.16 Carga individual/SII | Regresiones locales y preservación estática existentes; contraste real pendiente, sin atribuir importación SII no ejecutada. |
| 13.17 Matriz final | Esta es una actualización intermedia; cierre depende de la aceptación restante. |
| 13.18 Coherencia/gobierno final | OpenSpec estricto y auditorías se actualizan sin eludir tareas, revisión o QA pendientes. |
