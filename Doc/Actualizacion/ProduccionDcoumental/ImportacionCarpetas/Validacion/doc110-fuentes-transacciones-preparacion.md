# DOC-110: aceptación de duplicados y concurrencia de fuentes

15.5/15.14 completas con la aceptación autenticada y la matriz acumulada descritas a continuación. Instalación de empresa única: prueba entre empresas NO_APLICA conforme a decisión del usuario. Las secciones de preparación conservan el historial previo; sus estados pendientes quedan superados únicamente por la evidencia explícita de aceptación.

## Aceptación autenticada

Resultado aportado por el usuario: **1 passed (35.9 s)**, caso 34.6 s. Reporte local saneado doc110-source-transactions-result.json: PASS, sourceTable imp_02_facturas, sourceId 4, finalState INACTIVE, checks 2, inicio 2026-10-27T14:36:53.732Z y fin 2026-10-27T14:37:10.228Z. Este intervalo corresponde al helper, no a toda la prueba/login.

- SOURCE_INDEPENDENT_ASPNET_SESSIONS: dos sesiones distintas de la cuenta autorizada, comprobadas sin exponer cookies.
- SOURCE_FACTURAS_ABSENT_AND_METADATA_VERIFIED: ausencia de autorización anterior, incluyendo inactivas, y metadatos de NIT/NUMERO compatibles antes del alta UI.
- SOURCE_DUPLICATE_REJECTED_SINGLE_IDENTITY_HISTORY_UNCHANGED: alta duplicada 409 sin otra fuente; documento e historial idénticos después del rechazo y listado acotado con una sola identidad.
- SOURCE_CONCURRENT_EDIT_ONE_WINNER_COLUMNS_AND_HISTORY_ATOMIC_INACTIVE: dos ediciones con revisión 1 producen un ganador 200 y un conflicto 409; revisión final 2, solo las columnas del ganador y un registro EDITAR adicional con snapshot íntegro. Ambas sesiones leen el mismo estado. La revisión perdedora repetida explícitamente se rechaza sin cambios.
- SOURCE_FACTURAS_FINALLY_INACTIVE_WITHOUT_ACTIVATION: fuente final inactiva, sin ejecutar ActivarFuente en este modo.

No se ejecutó DDL ni se consultaron filas de negocio DA; el modo solo lee metadatos y crea/modifica la nueva autorización RA de facturas. Créditos y egresos no se modificaron. No se crean proveedores o perfiles.

## Cierre por criterio y límites

| Tarea/criterio | Evidencia |
|---|---|
| 15.5 alta/lectura/edición, revisión y columnas | Alta UI y carrera reales anteriores; dos sesiones comparten revisión/columnas/historial exactos. |
| 15.5 duplicados y conflicto sin columnas parciales | Alta duplicada no genera otra identidad ni auditoría; edición perdedora y revisión obsoleta posterior no alteran el ganador. |
| 15.14 sin facultad | Arnés productivo deniega las siete acciones antes de repositorio/metadatos; guard HTTP de fuentes previo acredita 17 rechazos con documento/historial intactos. |
| 15.14 revisión concurrente con ganador único | Nueva carrera real de facturas acreditada arriba. |
| 15.14 política ausente o RA no disponible | Pruebas locales del adaptador productivo con callback de política vacío/fallido, sin resolver origen ni consultar datos. Revisión de composición sin fallback. No es una caída real de RA. |
| 15.14 revocación y publicaciones inmutables | Ciclo real previo de créditos/proveedor/perfil (1 passed, 23.8 s) y diagnóstico de solo lectura posterior (17.4 s); revocación deniega uso y conserva consumidores/historial. |
| Aceptación real entre empresas | NO_APLICA por decisión de instalación de empresa única. |

Referencias: [guard de fuentes](doc110-guard-fuentes-20261027.md), [servicio y conflictos locales](doc110-fuentes-conflictos-servicio-20261027.md), [política RA](doc110-politica-ra-consumidores-20261027.md), [ciclo completo](doc110-fuentes-aceptacion-preparada-20261026.md) y [empresa única](../Exploracion/criterio-empresa-unica-doc110.md).

Se completa el criterio de conflicto sin cambios parciales mediante la carrera y el rechazo reales. No se atribuye a esta corrida una inyección de fallo SQL entre DELETE/INSERT ni una caída de RA; esos escenarios no se ejecutaron. No se prueban todas las intercalaciones posibles. No se reejecuta el modo sobre la fuente 4: ahora existe y el preflight debe detenerlo sin cambios.

## Verificador preparado

`tools/e2e/tests/support/doc110-source-transactions.cjs` recibe dos adaptadores de petición y el documento de un fixture nuevo creado por un runner autorizado. No crea ese fixture ni autentica por sí mismo. Requiere fuente DA INACTIVA, revisión 1, aislamiento CONEXION y dos columnas distintas; rechaza expresamente créditos y egresos. No activa fuentes ni crea proveedores/perfiles.

Comprueba identidad e historial inicial desde ambas sesiones. Intenta alta de la misma identidad sin ID/revisión y exige 409 CONFLICTO_REVISION; relee documento/historial intactos y recorre listado acotado para exigir una sola identidad, no solo una respuesta de error.

Después envía dos ediciones con revisión idéntica y selecciones de columnas distintas. Espera ambas respuestas aun si una falla por transporte. Exige un 200 y un 409, una sola revisión adicional, exactamente las columnas del ganador y un único registro EDITAR con snapshot completo; ambas sesiones deben leer el mismo documento e historial. Un intento posterior con revisión perdedora debe rechazarse sin alterar nada. La fuente permanece inactiva.

Este intento posterior de revisión obsoleta es una comprobación explícita tras resultados confirmados. Ante fallo de transporte se informa OUTCOME_UNCONFIRMED y no se repite la escritura. No se promete rollback de un commit cuyo resultado no se recibió.

## Pruebas locales

```powershell
node --test tools/e2e/tests/doc110-source-transactions.test.cjs
```

**5 PASS**, transporte simulado. Casos positivos con ganador en primera o segunda sesión; detección de duplicado insertado, doble éxito, columnas cambiadas por el perdedor e historial extra; transporte incierto sin reintento; rechazo de fuentes previas/activas o revisión reutilizada antes de llamar al adaptador.

Estos casos verifican el detector, no el motor MySQL. El marcador de columnas/historial íntegros en una carrera no equivale a inyectar un fallo entre DELETE/INSERT y probar rollback.

## Datos pendientes para integrar y ejecutar

Primer intento autenticado aportado por el usuario: FALLÓ antes del recorrido de fuentes, caso 15.7 s, con DOC110_SESSION_REQUIRED al comprobar ListarPerfiles (401). No se alcanzó preflight, alta ni edición de facturas. La cuenta mostrada fue LUZ.GUILERA, distinta de LUZ.AGUILERA usada en las corridas autorizadas previas; se señala la diferencia para la repetición, sin afirmar que el servidor diagnosticó esa causa. No se cambian login, pruebas ni tiempos de espera por este resultado. Pruebas transaccionales reales siguen NO_EJECUTADO.

Actualización: el usuario identifica imp_02_facturas mediante DDL (NUMERO/NIT/BENEFICIARIO/FECHA TEXTO(250), PK IMP_02_ID_CLAVE). No se ejecuta ese DDL. Se integra el modo exclusivo --source-transactions del runner existente para DA imp_02_facturas, seleccionando solo NIT/NUMERO. Comprueba ausencia incluyendo inactivas y tipos reales antes del alta UI; si falta la tabla o ya hay autorización, detiene la prueba sin reutilizarla. La existencia física no se infiere del DDL: debe confirmarse por los metadatos.

El adaptador abre dos sesiones de la cuenta autorizada y compara únicamente un booleano de independencia ASP.NET, sin imprimir cookies/tokens. Verifica acceso desde la segunda sesión antes de crear. El alta queda INACTIVA y ninguna operación del modo la activa. No crea proveedores/perfiles ni consulta filas DA. Guarda reporte saneado separado doc110-source-transactions-result.json con ID de fuente apenas se confirma el alta; finalmente relee estado inactivo. Un fallo de transporte no provoca reintento ni revocación automática de una fuente ajena. Si se pierde la confirmación del alta, un nuevo intento quedará detenido por la comprobación de identidad existente y requiere diagnóstico, sin crear otra.

Verificación integrada local: 10 Node PASS (8 del verificador/preflight y 2 de transporte seguro); 2 Chromium PASS con UI productiva y red simulada. Chromium acredita tabla en segunda página, alta única inactiva con NIT/NUMERO y ausencia de alta si la tabla no se descubre. La prueba negativa detectó una espera por texto de catálogo no vacío; el driver ahora acepta el estado vacío y termina con NOT_DISCOVERED. Sin cambios productivos.

```powershell
npm.cmd --prefix tools/e2e run test:doc110:session -- --source-transactions --headed
```

Corrida nueva preparada en ambiente/cuenta ya autorizados, dentro de la instrucción de continuar y del fixture aportado por el usuario. Credenciales efímeras solo en consola. HTTP nuevo NO_EJECUTADO; rollback provocado sigue pendiente. Las notas de falta de tabla/integración inferiores son el estado anterior a este avance.

Se necesita identificar una tabla física descartable existente con prefijo imp_02, aún no registrada como fuente. El usuario debe indicar cuál está disponible; no se inventa una tabla ni se crea DDL. Una vez identificada, el runner existente debe verificar ausencia de autorización incluyendo inactivas, contrastar columnas, crear la autorización desde UI y comprobar dos sesiones ASP.NET distintas antes de usar el verificador. No debe reutilizar la fuente revocada ni tratar dos llamadas de una sesión como carrera real.

El alta/edición de esa autorización nueva requiere quedar dentro del ambiente/cuenta y fixture expresamente autorizados. No se ejecuta todavía. La prueba de rollback ante fallo transaccional requiere además un entorno/fixture controlado; no se retiran tablas, se interrumpe RA ni se alteran permisos para provocarlo. Estado HTTP: **NO_EJECUTADO**. No se cierran 15.5/15.14 por esta preparación.
