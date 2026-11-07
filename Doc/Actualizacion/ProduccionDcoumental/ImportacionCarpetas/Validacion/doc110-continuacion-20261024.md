# DOC-110 — Continuación de implementación y verificación

Fecha: 2026-10-24. Estado: cambio activo, sin despliegue.

## Tarea 14.33 completada

Conversión explícita de una propiedad TEXTO de proveedor a índice FECHA. La regla admite `yyyy-MM-dd` o `dd/MM/yyyy H:mm:ss`; el usuario selecciona el formato en la regla del perfil. El adaptador conserva el texto de origen. El coordinador interpreta con cultura invariante, valida fecha/hora y produce `yyyy-MM-dd` antes de validar el índice del gabinete. Las fechas nativas del proveedor conservan formato canónico. No se cambian propiedades ni versiones publicadas del proveedor.

Ejemplo verificado: `23/01/2006 0:00:00` → `2006-01-23`. Fechas imposibles, horas fuera de rango, entradas vacías o con formato diferente se rechazan. No hay autodetección regional ni modificación de `imp_02_egresos`.

Archivos: ReglasProveedorIndices.vb, CoordinadorProveedorIndices.vb, ResolvedorReglasPerfilImportacion.vb y PerfilImportacionEditor.js. No hay SQL nuevo.

## Resultados de esta corrida

| Verificación | Resultado | Alcance |
|---|---|---|
| Arnés .NET y editores/API DOC-110 | 24 PASS | Componentes y dobles; incluye fechas en es-CO/en-US y huella histórica |
| Playwright guiado, fuentes, menú, disposición y autocomplete | 8 PASS | Navegador real con transporte simulado; formato guardado/reabierto |
| DOC-88 y importar-servicio-web-modern | 33 PASS | Comprobaciones locales de políticas/contratos; no almacenamiento real |
| MSBuild aislado | PASS | Salida en .tmp/doc110-amp02-build, sin copiar DLL a bin |
| Diff en rutas de SII consultadas | Sin modificaciones rastreadas | No equivale a una prueba de integración SII |

La prueba `doc110-perfil-importacion-session.spec.cjs` ahora abre `#doc110-navigation-toggle` antes de seleccionar perfiles. Se preparó el cambio, pero no se ejecutó una sesión autenticada en esta corrida.

## Pendientes que no se cierran con esta evidencia

Persistencia/concurrencia real, aislamiento entre cuentas, revocación por aplicación externa, ambiente sin esquema, migración MySQL 5.1 y aceptación HTTP de fuentes/proveedores/herencia siguen pendientes. No se cambian sus casillas por haber código o dobles. No se archiva ni se solicita cierre de Jira.

Próximo recorrido autenticado preparado: `npm.cmd --prefix tools/e2e run test:doc110:session -- --publish --headed`, con credenciales efímeras en la consola existente y ambiente autorizado. Crea y desactiva un perfil descartable, no importa documentos. Este recorrido no cubre por sí solo todas las tareas de fuentes/proveedores ni concurrencia entre sesiones.

## Correccion de menu tras corrida autenticada

Bootstrap 4.0.0 estaba incluido dos veces en el ASPX. La prueba anterior cargaba un solo bundle. Se ajusto la prueba para leer las inclusiones reales: reprodujo el boton oculto con dos copias. Se retiro la primera inclusion, conservando la posterior a Popper. Cuatro pruebas locales PASS (dos navegacion y dos edicion guiada). Transporte simulado; aceptacion autenticada pendiente. Tarea 15.27. Sin SQL ni despliegue de DLL.

## Aceptación autenticada posterior — resultado aportado por el usuario

El usuario confirmó la repetición del recorrido de publicación en IIS: `doc110-perfil-importacion-session.spec.cjs`, **1 passed (24.3s)**; duración del caso 22.6s. Esta evidencia posterior supera el pendiente de repetición del menú indicado arriba. No representa una nueva ejecución realizada por el agente.

Marcadores aprobados: autocomplete por código/nombre, selección y limpieza; catálogos autorizados RA/DA; identidades incrementales; configuración y guardado desde UI; ensayo de reglas y valores índice; publicación EXPEDIENTE; rechazo de edición publicada; segunda versión publicada LOTE; inmutabilidad de la primera publicación; dos versiones en historial; desactivación del perfil descartable.

Queda acreditada la apertura del menú y este ciclo de persistencia/publicación del perfil. No acredita importación de archivos, integración de fuentes/proveedores/herencia, concurrencia entre sesiones ni aislamiento entre cuentas. La tarea 10.18 mantiene pendientes los escenarios del recorrido de borrador sobre el esquema incremental (patrones y publicación incompleta); no se cierra por el recorrido de publicación únicamente.

## Cierre de 10.18 — recorrido de borrador posterior

El usuario aportó el resultado del recorrido autenticado `--draft --headed`: **1 passed (18.1s)**, duración del caso 17.0s. Aprobó autocomplete, identidades incrementales, creación por UI, guardado y lectura persistida, rechazo de revisión obsoleta, dos patrones conservando ceros iniciales, rechazo de publicación incompleta, una única escritura ganadora entre solicitudes con la misma revisión, historial y desactivación del perfil descartable.

Esta corrida, junto con la publicación aprobada en 24.3s, completa los escenarios de 10.18 y supera su pendiente indicado en la sección anterior. Se marca la tarea completada con evidencia aportada por el usuario, sin atribuir una nueva ejecución al agente.

La carrera utiliza un mismo contexto autenticado: acredita control de revisión, pero no concurrencia entre sesiones independientes ni aislamiento entre cuentas. 10.19 permanece pendiente. No se acredita carga de carpetas, almacenamiento documental ni integración completa de fuentes/proveedores. El aviso NO_COLOR/FORCE_COLOR no impidió la ejecución.

## Preparacion de concurrencia independiente (10.19)

Comando: npm.cmd --prefix tools/e2e run test:doc110:session -- --concurrent --headed. Reutiliza login y prueba de borrador; crea dos contextos autenticados de LUZ.AGUILERA, verifica sesiones ASP.NET diferentes en memoria, usa CSRF propio y consulta resultado persistido desde ambas sesiones. Evidencia JSON distingue SINGLE_SESSION de INDEPENDENT_SESSIONS. No acredita aislamiento entre empresas ni numeracion concurrente de versiones. Pendiente corrida real; no se marca tarea completa.

## Concurrencia independiente — corrida confirmada

Resultado aportado por el usuario: **1 passed (33.4s)**; duración del caso 32.2s. Aprobados `INDEPENDENT_ASPNET_SESSIONS`, `CONCURRENT_REVISION_ONE_WINNER` e `INDEPENDENT_SESSIONS_ONE_WINNER_AND_SHARED_RESULT`. Dos sesiones distintas de la misma cuenta enviaron escrituras con la misma revisión: una recibió 200 y la otra 409; ambas consultaron la revisión y el nombre persistidos del ganador. También pasaron los controles del recorrido de borrador y la desactivación del perfil descartable.

Supera el pendiente de ejecución real del apartado anterior. Acredita el control de revisión entre sesiones independientes para este escenario; no demuestra por sí solo solapamiento interno de transacciones. No prueba aislamiento entre cuentas/empresas ni creación concurrente de versiones sin duplicar numeración. La tarea 10.19 permanece abierta por esos escenarios. No se registran credenciales, cookies ni datos de negocio.

## Preparacion de numeracion concurrente

Runner existente: --concurrent-publish --headed. Reutiliza ambas sesiones independientes y ciclo de publicacion; sustituye la creacion secuencial de segunda version por dos solicitudes con la misma revision. Exige una respuesta 200 y una 409 CONFLICTO_REVISION, historial con solo dos versiones y numeros/IDs unicos, incremento de revision unico y lectura desde segunda sesion. Continua con publicacion LOTE, inmutabilidad y desactivacion. Sintaxis de tres archivos y tres pruebas de soporte PASS. Pendiente aceptacion IIS; no se cierra 10.19.

## Numeración concurrente — corrida confirmada

Resultado aportado por el usuario: **1 passed (38.9s)**; duración del caso 37.6s. Aprobados `INDEPENDENT_ASPNET_SESSIONS` y `CONCURRENT_VERSION_ONE_WINNER_NO_DUPLICATE_NUMBER`: dos solicitudes de CrearVersion desde sesiones independientes produjeron una respuesta 200 y una 409, una sola nueva versión, incremento único de revisión y números e identificadores distintos en el historial. La segunda sesión pudo leer la versión ganadora.

También pasaron publicación EXPEDIENTE, rechazo de edición publicada, publicación de la segunda versión LOTE, inmutabilidad de la primera, historial con dos versiones publicadas y desactivación del perfil descartable. Supera el pendiente de aceptación de numeración indicado arriba. No atribuye al agente una nueva ejecución ni demuestra por sí sola solapamiento interno de transacciones.

La tarea 10.19 mantiene pendiente el aislamiento entre cuentas y ámbitos autorizados. Dos sesiones de LUZ.AGUILERA no sustituyen ese escenario: el acceso al perfil depende de empresa y permiso, no de propiedad individual del creador.

## Preparacion de acceso con segunda cuenta

Runner --isolation --headed: segunda cuenta capturada por TTY, login existente, solo lecturas. Usa un perfil visible a primera cuenta y exige lista filtrada vacia o denegacion 403; detalle e historial deben responder ACCESO_DENEGADO sin datos. Fallos de login/CSRF no se confunden con autorizacion correcta. Sintaxis y tres pruebas de soporte PASS; recorrido real no ejecutado. Falta cuenta expresamente autorizada. Una cuenta sin permiso acredita denegacion administrativa, no separacion entre empresas; otra cuenta de misma empresa con permiso no debe rechazarse por ser distinta del creador. Consulta auxiliar de inventario no ejecutada por politica de firma PowerShell; no hay evidencia nueva de BD. 10.19 abierta.

## Acceso con segunda cuenta — corrida confirmada (2026-10-25)

El usuario autorizó CAROLINA.CRUZ como segunda cuenta y aportó el resultado del recorrido de solo lectura con LUZ.AGUILERA: **1 passed (34.6s)**, duración del caso 33.1s. Pasaron `SECOND_ACCOUNT_DENIED_LISTARPERFILES`, `SECOND_ACCOUNT_DENIED_OBTENERPERFIL`, `SECOND_ACCOUNT_DENIED_CONSULTARHISTORIAL` y el autocomplete de la cuenta principal.

Supera el pendiente de cuenta y ejecución del apartado anterior. Confirma que la segunda cuenta no obtuvo el perfil mediante lista filtrada, detalle ni historial. El contrato de prueba rechaza 401 y errores CSRF como evidencia de aislamiento; exige lista vacía o denegación, y 403 ACCESO_DENEGADO sin datos para detalle e historial. No se modificaron permisos ni perfiles mediante este recorrido.

La evidencia no prueba separación entre empresas ni denegación de todas las operaciones de escritura. El ambiente descrito tiene una empresa; no se crea otra para simular aceptación. 10.19 mantiene pendiente la separación entre ámbitos empresariales. La ejecución fue realizada en la consola del usuario; no se registran contraseñas, cookies ni referencias de negocio.

## Restriccion de fuentes - preparacion 2026-10-25

Comando: npm.cmd --prefix tools/e2e run test:doc110:session -- --source-scope --headed. Usa sesion autorizada y CSRF existente; solo consulta metadatos RA/DA. Verifica paginacion sin duplicados, prefijo literal imp_02, rechazo 403 ACCESO_DENEGADO de tres nombres fuera del prefijo por conexion y estructura de seis columnas TEXTO(250) de imp_02_egresos. No consulta NIT/beneficiarios ni otras filas. La configuracion local no tiene excepciones; no se habilita ninguna. Sintaxis de tres archivos y tres pruebas de soporte PASS; aceptacion real pendiente. No acredita todavia denegacion de un proveedor previamente guardado fuera de alcance; 15.19 sigue abierta.

## Restricción de fuentes — corrida confirmada (2026-10-25)

Resultado aportado por el usuario: **1 passed (18.7s)**, duración del caso 17.5s. Aprobados `SOURCE_PREFIX_PAGINATION_RA`, `SOURCE_PREFIX_PAGINATION_DA`, `SOURCE_OUTSIDE_LITERAL_PREFIX_DENIED_RA`, `SOURCE_OUTSIDE_LITERAL_PREFIX_DENIED_DA`, `SOURCE_EGRESOS_METADATA_ONLY` y el autocomplete.

Se verificaron los listados disponibles en ambas conexiones, sin nombres fuera del prefijo literal ni duplicados. Las solicitudes de columnas para los tres nombres de prueba fuera del prefijo fueron rechazadas con 403 ACCESO_DENEGADO por conexión. Se confirmaron las seis columnas TEXTO de longitud 250 de DA imp_02_egresos, sin consultar registros de negocio. El recorrido soporta paginación, pero la salida no demuestra que existieran suficientes tablas para recorrer varias páginas.

Esta evidencia supera el pendiente de ejecución real del apartado anterior. No valida excepciones configuradas ni el rechazo de un proveedor previamente guardado fuera de alcance. 15.19 permanece abierta únicamente por ese escenario restante de su alcance. No hubo cambios de permisos, configuración o datos mediante este recorrido.

## Proveedor previo fuera de alcance - preparacion 2026-10-25

Runner --provider-scope --headed consulta versiones persistidas paginadas. Si encuentra una tabla fuera de imp_02, lee el documento, invoca ValidarProveedor exigiendo 403 ACCESO_DENEGADO y vuelve a leer para comprobar que no cambio. No invoca ensayo, no consulta filas de origen ni modifica autorizaciones. Ambiente sin excepciones. Si no existe candidato, registra NOT_RUN y Playwright skipped. Seis pruebas locales de soporte PASS (transporte simulado); pendiente evidencia autenticada. Este recorrido verifica validacion administrativa, no todos los consumidores del proveedor.

## Proveedor previo — escenario omitido en IIS (2026-10-25)

Resultado aportado por el usuario: `PREEXISTING_PROVIDER_OUTSIDE_SCOPE_NOT_FOUND`, **1 skipped**. ListarAdministracionProveedores respondió en 176 ms; no se encontró una versión candidata fuera del prefijo entre las visibles a la cuenta autorizada. No demuestra ausencia global en otros ámbitos.

El descubrimiento sí se ejecutó; la comprobación de rechazo del proveedor quedó NOT_RUN por falta de un caso disponible. No hubo validación de un candidato, consultas a sus datos ni mutaciones. No se considera fallo del producto ni aprobación del escenario. Se conserva el PASS anterior de restricción de metadatos RA/DA; 15.19 permanece parcialmente pendiente. Repetir este escenario solo cuando exista un caso controlado en un entorno de prueba autorizado, sin ampliar permisos ni crear configuraciones inseguras en el ambiente actual.

## Herencia de clasificacion - preparacion 2026-10-25

Comando --inheritance --headed, con runner y ciclo de publicacion existentes. Configura por UI una regla de indice TEXTO heredado SERIE_ID (requiere indice compatible descubierto), guarda y relee. Prueba servidor contra SERIE_ID enviado por cliente, Diferido cuando falta clasificacion, VALOR_REQUERIDO cuando subserie obligatoria esta ausente y ausencia de cambios persistidos tras los ensayos. Luego publica dos versiones y desactiva el perfil descartable. Solo propuesta del perfil, no expediente confirmado. Sintaxis y tres pruebas de soporte PASS; aceptacion IIS pendiente. No requiere SQL ni modifica datos de catalogos.

## Diagnostico del fallo de herencia - 2026-10-25

Corrida aportada: guardado UI y lectura de regla aprobados; primer EnsayarReglas rechazo 400 CONFIGURACION_INVALIDA sin incidencias. PerfilImportacionJson.ValidarForma permite solo las seis claves legacy del diccionario Expediente; SERIE_ID y SUBSERIE_ID enviados por la prueba se rechazan antes de ServicioPerfilImportacion. Es un error en la expectativa E2E, no evidencia de fallo de la herencia. Se separo caso negativo (400 esperado) del ensayo valido con Expediente vacio. El servicio obtiene clasificacion desde catalogos autorizados. Arnes .NET PASS con cobertura explicita de rechazo de las seis claves nuevas por JSON y aceptacion de diccionario vacio; no cambios de contrato, SQL o DLL. La corrida fallida no alcanzo publicacion ni desactivacion del perfil descartable creado; no se afirma limpieza completada. Pendiente repeticion autenticada.

## Herencia — aceptación confirmada y cierre de 16.7 (2026-10-25)

Resultado aportado por el usuario: **1 passed (21.3s)**, duración del caso 20.3s. Se verificaron guardado de regla heredada desde UI y lectura persistida, rechazo HTTP de clasificación enviada por el cliente, resolución de SERIE_ID desde el servidor con procedencia PROPUESTA_PERFIL, clasificación pendiente y rechazo de subserie obligatoria ausente. Los ensayos no alteraron el perfil guardado.

El recorrido también aprobó publicación EXPEDIENTE, rechazo de edición publicada, segunda versión LOTE, inmutabilidad de la primera, historial con dos publicaciones y desactivación del perfil descartable de esta corrida. No acredita desactivación del perfil creado por la corrida fallida anterior.

La evidencia autenticada complementa las pruebas guiadas de edición/reapertura con transporte simulado y el arnés .NET. Se completa 16.7 en el alcance de configuración y ensayo del perfil. Supera los pendientes de aceptación de esta tarea indicados anteriormente. La herencia desde el expediente efectivamente creado o reutilizado, y la incorporación de documentos a sus índices, requieren el flujo operativo de importación; no se atribuyen a esta aceptación. No se crearon expedientes ni se almacenaron documentos.

## Politica de coincidencias - preparacion 2026-10-25

Runner --matching --headed reutiliza ciclo de publicacion con perfil descartable. Comprueba BLOQUEAR predeterminado y recarga, seleccion UI REUTILIZAR_VALIDADO y persistencia; politica desconocida rechazada al guardar con POLITICA_INVALIDA y configuracion inyectada en PublicarVersion rechazada por contrato cerrado. Lectura posterior exige BORRADOR y documento igual. Publica, cambia politica a BLOQUEAR en segunda version y verifica primera publicacion inmutable; desactiva al completar. Tres pruebas locales PASS (arnes .NET y transporte), sintaxis correcta. Pendiente IIS; no se marca 7.7 completa ni se ejecuta coincidencia documental real.

## Politica de coincidencias - aceptacion y cierre de 7.7 (2026-10-25)

Resultado aportado por el usuario: 1 passed (21.4s), caso 20.1s. Aprobados MATCHING_DEFAULT_BLOCK_PERSISTED, MATCHING_EXPLICIT_REUSE_UI_PERSISTED, MATCHING_INVALID_REJECTED_WITHOUT_PERSISTENCE_OR_PUBLICATION y MATCHING_POLICY_VERSIONED_WITHOUT_CHANGING_PRIOR_PUBLICATION. BLOQUEAR se conserva inicialmente; REUTILIZAR_VALIDADO se selecciona por UI y persiste. Valor desconocido rechazado al guardar y configuracion inyectada rechazada al publicar, sin alterar el documento guardado. La segunda version cambia a BLOQUEAR sin modificar la primera publicada con REUTILIZAR_VALIDADO.

Tambien aprobados ensayo, publicacion EXPEDIENTE/LOTE, rechazo de edicion publicada, historial e inmutabilidad; perfil descartable desactivado. Se completa 7.7 en alcance de configuracion y persistencia, superando el pendiente de la seccion anterior. No acredita busqueda de coincidencias documentales, reutilizacion de un expediente real ni importacion de archivos.

## Proteccion HTTP - preparacion 2026-10-25

Runner --guards --headed reutiliza borrador descartable y dos sesiones de LUZ.AGUILERA. Intenta guardar cambio sintético sin token, con token de otra sesion y desde contexto anonimo con token conocido. Exige respectivamente 403 SOLICITUD_NO_VERIFICADA, 403 y 401 SESION_REQUERIDA, sin datos; relee perfil tras cada intento y compara documento completo/revision. Luego continua recorrido de borrador y desactiva al completar. Tres pruebas locales PASS; sintaxis correcta, pendiente IIS. No prueba expiracion por tiempo ni revocacion externa ni indisponibilidad 503; 7.4/7.8 abiertas.

## Protección HTTP — corrida confirmada (2026-10-25)

Resultado aportado por el usuario: **1 passed (34.3s)**, duración del caso 33.3s. Aprobados `MISSING_CSRF_REJECTED_NO_CHANGE`, `OTHER_SESSION_CSRF_REJECTED_NO_CHANGE` y `UNAUTHENTICATED_WRITE_REJECTED_NO_CHANGE`. Los intentos sobre GuardarBorrador recibieron los códigos esperados (403 SOLICITUD_NO_VERIFICADA, 403 SOLICITUD_NO_VERIFICADA y 401 SESION_REQUERIDA); la lectura autenticada posterior a cada intento confirmó documento y revisión inalterados.

También pasaron identificación de sesiones independientes, persistencia incremental, revisión obsoleta, patrones con ceros iniciales, rechazo de publicación incompleta, una única escritura ganadora y resultado compartido entre sesiones, historial y desactivación del perfil de esta corrida. Supera el pendiente de ejecución IIS del apartado anterior.

La evidencia cubre estos intentos sobre GuardarBorrador; no se generaliza a todos los endpoints. El contexto sin login no prueba expiración temporal de una sesión previamente autenticada. Revocación externa, separación empresarial y demás escenarios de 7.4/7.8 siguen pendientes según sus criterios. No se almacenan credenciales, cookies ni tokens en esta evidencia.

## Multiples pestañas - preparacion 2026-10-25

Runner --tabs --headed abre dos paginas en el mismo BrowserContext autenticado. Exige que abrir otra pagina preserve el token anterior; guarda por HTTP con token original y comprueba un incremento de revision; consulta con token capturado desde segunda pestaña y compara documento. Escape debe cerrar dialogo y devolver foco al menu. Continúa borrador y desactivacion. Sintaxis y 13 pruebas de soporte PASS; pendiente corrida real. La escritura es por APIRequestContext compartido, no click duplicado en UI; no acredita ausencia general de handlers duplicados ni ciclo UpdatePanel. 7.8/6.8 permanecen abiertas.

## Múltiples pestañas — corrida confirmada (2026-10-25)

Resultado aportado por el usuario: **1 passed (20.0s)**, duración del caso 19.0s. Aprobados `SECOND_TAB_PRESERVES_SESSION_CSRF`, `FIRST_TAB_TOKEN_WRITES_ONCE_SECOND_TAB_TOKEN_READS_RESULT` y `SECOND_TAB_ESCAPE_RESTORES_MENU_FOCUS`. Abrir la segunda pestaña conservó el token de la sesión; el guardado con el token original incrementó una sola vez la revisión y la lectura con el token observado en la segunda pestaña devolvió el mismo documento. Escape cerró el diálogo y devolvió el foco al menú.

También pasaron revisión obsoleta, patrones con ceros iniciales, publicación incompleta rechazada, historial y desactivación del perfil descartable. La comprobación CONCURRENT_REVISION_ONE_WINNER de este modo usa una misma sesión; no constituye otra evidencia de concurrencia entre sesiones independientes.

Supera el pendiente de ejecución IIS de este recorrido. No prueba ciclo UpdatePanel, ausencia general de manejadores duplicados, expiración temporal ni revocación externa. 6.8 y 7.8 permanecen abiertas por sus demás criterios. No se registraron secretos ni se crearon expedientes o documentos.

## UpdatePanel - preparacion 2026-10-25

Runner --updatepanel --headed usa Button_actualiza real dentro de UpdatePanel_botones_carpeta. Manejador inspeccionado: sale sin expediente seleccionado o consulta listado y ajusta orden en sesion; no crea/modifica documentos. Dos POST parciales Delta=true con endRequest sin error; compara identidad de host, jQuery y plugin dropdown. Reabre editor con Enter, exige una sola solicitud ListarPerfiles y ningun postback por apertura; Escape devuelve foco. No simula evento load ni crea perfiles. Sintaxis y soporte login PASS, pendiente corrida IIS; no acredita todos los paneles ni cualquier flujo legacy.

## Correccion de foco antes de UpdatePanel (2026-10-25)

La corrida fallo antes del primer postback: tras Limpiar autocomplete, el listado reemplaza el boton enfocado y el foco queda fuera del dialogo; Escape no llega a su manejador. Reproducido con navegador local usando keyboard.press global y verificacion de foco (fallo previo). PerfilImportacionEditor.list ahora conserva intencion de foco y lo restaura al buscador si el reemplazo lo deja en body. No roba foco a otro control. Referencia JS versionada en ASPX. Tres pruebas Chromium PASS: autocomplete y dos navegacion. Tarea 6.9 completada localmente; 6.8 mantiene pendiente corrida IIS. Sin SQL ni DLL.

## UpdatePanel - aceptacion y cierre de 6.8 (2026-10-25)

Resultado aportado por el usuario: 1 passed (30.1s), caso 28.5s. Aprobados TWO_REAL_UPDATEPANEL_CYCLES, UPDATEPANEL_SINGLE_MENU_AND_LIBRARY_INSTANCES y UPDATEPANEL_KEYBOARD_REOPEN_ONE_LIST_REQUEST, ademas de autocomplete. Dos POST Delta=true terminaron con endRequest sin error; host, jQuery y plugin dropdown conservaron identidad. Menu y acceso permanecieron unicos. Cada reapertura por teclado produjo una sola consulta ListarPerfiles y ningun postback del editor; Escape cerro y restauro foco.

Se completa 6.8 en el recorrido real de Button_actualiza y administracion de perfiles. Confirma la correccion de foco 6.9 en IIS y supera los pendientes anteriores de esta corrida. No se generaliza a todos los paneles o acciones legacy ni constituye auditoria completa de accesibilidad. No se crearon perfiles, expedientes ni documentos mediante este recorrido.

## Publicacion concurrente - preparacion 2026-10-25

Se amplia --concurrent-publish --headed: dos sesiones independientes publican el mismo borrador con iguales revisiones. Exige un 200 y un 409 CONFLICTO_REVISION sin datos; revisiones de perfil/version aumentan una vez, configuracion permanece igual y ambas sesiones leen documento ganador identico. Conserva verificacion de creacion concurrente, historial con dos publicaciones, primera version inmutable y desactivacion. Tres pruebas locales PASS (arnes .NET y transporte), sintaxis correcta. Pendiente aceptacion IIS de esta carrera; las corridas anteriores solo acreditaban creacion de version concurrente y publicacion secuencial. 7.3 sigue abierta hasta evidencia.

## Publicacion concurrente - aceptacion y cierre de 7.3 (2026-10-25)

Resultado aportado por el usuario: 1 passed (36.6s), caso 35.4s. Aprobados INDEPENDENT_ASPNET_SESSIONS, CONCURRENT_VERSION_ONE_WINNER_NO_DUPLICATE_NUMBER, CONCURRENT_PUBLICATION_ONE_WINNER_SINGLE_REVISION_INCREMENT y CONCURRENT_PUBLICATION_SHARED_PERSISTED_RESULT. Creacion y publicacion concurrentes produjeron un ganador y un conflicto controlado 200/409; cabecera/version incrementaron revision una sola vez y ambas sesiones leyeron el mismo documento publicado.

Tambien pasaron primera publicacion inmutable, historial con dos versiones publicadas, ambito LOTE y desactivacion del perfil descartable. Junto con la corrida de borrador entre sesiones independientes (33.4s), completa 7.3 en los escenarios implementados. Supera el pendiente de publicacion concurrente del apartado anterior. No demuestra todas las intercalaciones internas posibles ni aislamiento entre empresas; 10.19 conserva sus pendientes propios. Evidencia de ejecucion aportada por el usuario, sin secretos.

## Ciclo administrativo proveedor - preparacion 2026-10-25

Runner --provider-lifecycle --headed comprueba primero metadatos autorizados de imp_02_egresos. Crea desde UI proveedor INACTIVO, configura CONSECUTIVO como clave TEXTO y unica propiedad de salida (sin NIT/beneficiario), guarda/valida, ensaya clave aleatoria ausente y 44 ambigua conforme a evidencia previa. Si datos difieren, falla; no asume unicidad. Verifica ensayos sin mutacion/publicacion, publica, rechaza edicion publicada, clona/edita/publica segunda version y compara contrato/revision de primera, historial y disponibilidad inactiva. No activa ni revoca fuentes. Cuatro pruebas locales PASS; aceptacion IIS pendiente. Todavia no cubre alta de fuente, clave univoca, consumidor perfil ni indices esperados; 14.17/14.31 siguen abiertas.

## Proveedor administrativo - aceptacion y cierre de 14.28 (2026-10-25)

Resultado aportado por el usuario: 1 passed (20.8s), caso 19.8s. Aprobados metadatos autorizados, creacion/configuracion/guardado y validacion por UI, ensayos SIN_COINCIDENCIA y AMBIGUO, ausencia de cambios/publicacion por ensayo, publicacion UI y rechazo de edicion publicada, segunda version publicada con primera intacta e historial de dos versiones inactivas.

La evidencia acredita 14.28 en el recorrido administrativo del proveedor; supera su pendiente de aceptacion real. Se conservaron tabla y datos de origen y no se activo el proveedor descartable. No acredita consulta univoca, seleccion del proveedor en un perfil, resolucion de indices ni rechazo entre ambitos; 14.17 y 14.31 permanecen abiertas. Tampoco sustituye pruebas de concurrencia del repositorio de proveedores ni alta/revocacion de fuentes.

## Integracion proveedor-perfil - preparacion 2026-10-25

Runner --provider-profile --headed requiere CONSECUTIVO univoco, capturado oculto en TTY sin escribirlo en evidencia. Reutiliza proveedor descartable por UI, confirma consulta OK devolviendo solo CONSECUTIVO, activa temporalmente ese proveedor, crea perfil por UI y selecciona version publicada/propiedad numero_orden y clave constante. Exige lectura persistida de referencias y valor indice igual al consecutivo, luego publicacion e historial de perfil. Desactiva proveedor en finally mediante revision actual; el perfil se desactiva al completar su ciclo. No se modifica fuente ni registros de negocio. Cuatro pruebas locales PASS y sintaxis correcta; pendiente aceptacion IIS y dato univoco. No acredita aislamiento empresarial ni todo el flujo de importacion.

## Integracion proveedor-perfil - correccion del dato de prueba (2026-10-25)

Corrida aportada por el usuario: 1 failed (19.1s), consulta esperada OK devolvio AMBIGUO tras publicar la segunda version del proveedor descartable. La comprobacion ocurre antes de activar el proveedor y antes de crear el perfil: esta corrida no acredita integracion y el proveedor creado permanece inactivo. No se cambia el contrato para aceptar resultados ambiguos.

Se obtuvo mediante SELECT en DA un CONSECUTIVO con COUNT(*)=1, agrupando toda la columna antes de filtrar formato. Una segunda consulta parametrizada con WHERE CONSECUTIVO=@clave confirmo exactamente una coincidencia, conforme a la comparacion del adaptador. Solo se conserva la clave en un fixture local ignorado por git; no se registran valores de negocio ni credenciales en esta evidencia. No se modificaron fuentes ni registros de origen. Se prepara repeticion del recorrido con ese dato; 14.17 y 14.31 siguen pendientes de aceptacion.

## Integracion proveedor-perfil - aceptacion y cierre de 14.31 (2026-10-25)

Resultado aportado por el usuario: 1 passed (24.1s), caso 22.9s. Consulta univoca confirmada tras ensayos ausente y ambiguo; proveedor creado, configurado y publicado en dos versiones por UI, primera intacta. PUBLISHED_PROVIDER_SELECTED_UI_PERSISTED_AND_REAL_INDEX_RESOLVED acredita seleccion de version publicada y propiedad desde el perfil, persistencia de referencias y resolucion del indice TEXTO con el CONSECUTIVO esperado mediante EnsayarReglas.

El perfil publico versiones con ambitos EXPEDIENTE y LOTE, rechazo edicion publicada, conservo primera publicacion e historial. DISABLE_DISPOSABLE_PROFILE e INTEGRATION_DISPOSABLE_PROVIDER_DISABLED confirman desactivacion de ambos descartables; consulta final confirmo dos versiones publicadas del proveedor inactivo. No se modificaron fuentes ni registros de origen. No se guardan claves de negocio ni secretos en esta evidencia.

Se completa 14.31 y se supera el fallo anterior por dato ambiguo y el pendiente de aceptacion del recorrido positivo. 14.17 permanece abierta por rechazo de ambito no autorizado. Esta prueba acredita configuracion y ensayo de indices, no carga fisica de documentos, creacion de expedientes ni almacenamiento masivo en gabinete. Tampoco acredita aislamiento empresarial o concurrencia de proveedores.

## Autorizacion administrativa de proveedores - preparacion (2026-10-25)

Se amplia --isolation con las cuentas ya autorizadas LUZ.AGUILERA y CAROLINA.CRUZ. La primera obtiene por API un proveedor existente visible; la segunda autentica por el helper vigente y debe recibir 403 ACCESO_DENEGADO sin datos en ListarAdministracionProveedores, ObtenerAdministracionProveedor y ConsultarMetadatosProveedor. No acepta 401 ni rechazo CSRF como evidencia de autorizacion. Conserva verificaciones previas de perfiles. Solo lecturas: no ensaya consultas del proveedor, no modifica permisos, disponibilidad, fuentes ni origen.

Seis pruebas locales PASS (tres del nuevo helper, transporte seguro y reutilizacion de login); sintaxis revisada. Pendiente aceptacion IIS. Es una prueba de cuenta sin permiso, no separacion entre empresas ni proveedor persistido fuera del prefijo. 14.17 y 15.19 conservan esos limites; no se cierra ninguna tarea por esta preparacion.

## Autorizacion administrativa de proveedores - aceptacion (2026-10-25)

Resultado aportado por el usuario: 1 passed (33.1s), caso 31.9s. La cuenta administrativa obtuvo un proveedor existente; la segunda cuenta autenticada recibio 403 ACCESO_DENEGADO sin datos en ListarAdministracionProveedores, ObtenerAdministracionProveedor y ConsultarMetadatosProveedor. No se confundieron errores de CSRF o sesion ausente con rechazo de permisos. Tambien pasaron los rechazos previos de ListarPerfiles, ObtenerPerfil y ConsultarHistorial, y autocomplete de la cuenta administrativa.

La corrida fue de solo lectura, sin cambios de permisos, proveedores, fuentes ni registros de origen. Supera el pendiente IIS del apartado anterior y acredita falta de permiso administrativo con las dos cuentas autorizadas. No demuestra aislamiento entre empresas, cuenta con permiso pero fuera de ambito, revocacion durante una operacion ni proveedor persistido fuera del prefijo permitido. Por esos limites no se cierran globalmente 14.17, 14.26, 14.30 ni 15.19 con esta sola evidencia.

## GuardarProveedor - preparacion de protecciones HTTP (2026-10-25)

Runner existente --provider-guards --headed reutiliza alta visual de proveedor descartable INACTIVO. Tras guardar un contrato valido, intenta cambiar una propiedad sin CSRF y con token invalido (403 SOLICITUD_NO_VERIFICADA), desde contexto anonimo con token (401 SESION_REQUERIDA) y con revision anterior autenticada (409 CONFLICTO_REVISION). Cada rechazo exige ausencia de datos y lectura autorizada posterior identica al documento completo previo, incluidas revisiones y disponibilidad. No reintenta escrituras; cierra contexto anonimo en finally. Luego continua validacion/ensayos/publicaciones e historial habituales, sin activar proveedor ni modificar fuentes/origen.

Cinco pruebas locales PASS: helper de rechazos (incluye fallar ante documento alterado/codigo incorrecto), transporte seguro y reutilizacion de login; sintaxis correcta. Pendiente aceptacion IIS. Un contexto anonimo no prueba expiracion temporal; token invalido no es token de otra sesion; revision obsoleta no prueba carrera concurrente. No se cierran tareas 14.23/14.26/14.30 por esta preparacion.

## GuardarProveedor - aceptacion de protecciones HTTP (2026-10-25)

Resultado aportado por el usuario: 1 passed (19.3s), caso 18.0s. Aprobados PROVIDER_MISSING_CSRF_NO_CHANGE, PROVIDER_INVALID_CSRF_NO_CHANGE, PROVIDER_UNAUTHENTICATED_WRITE_NO_CHANGE y PROVIDER_STALE_REVISION_NO_CHANGE. GuardarProveedor rechazo token ausente/invalido con 403 SOLICITUD_NO_VERIFICADA, contexto sin sesion con 401 SESION_REQUERIDA y revision anterior con 409 CONFLICTO_REVISION. Cada intento fue seguido por lectura autenticada que confirmo documento completo, revisiones y disponibilidad sin cambios.

Tambien pasaron alta/configuracion visual, validacion, ensayos ausente y ambiguo, ensayos sin publicacion ni modificacion, publicacion y rechazo de edicion publicada, segunda version con primera intacta e historial de dos publicaciones INACTIVAS. No se activo el proveedor ni se modificaron fuentes o registros de origen.

Supera el pendiente IIS de la preparacion anterior. Acredita estos controles para GuardarProveedor, sin generalizar a todos los endpoints. Revision obsoleta no demuestra carrera concurrente; contexto anonimo no prueba expiracion temporal y token invalido no prueba token ajeno de otra sesion. 14.23/14.26/14.30 permanecen abiertas por sus criterios restantes, incluidos concurrencia y cobertura administrativa adicional. Evidencia aportada por el usuario, sin secretos.

## Concurrencia administrativa proveedor - preparacion (2026-10-25)

Runner --provider-concurrent --headed crea/configura/guarda proveedor descartable INACTIVO mediante UI y abre segunda sesion usando el helper de login existente. Comprueba ASP.NET_SessionId distintos mediante booleano sin imprimir cookies. Cada sesion usa su propio token observado en ListarPerfiles. Ejecuta pares concurrentes GuardarProveedor (contratos distinguibles), PublicarProveedor y CrearVersionProveedor con iguales revisiones de partida.

Cada carrera exige exactamente 200/409 CONFLICTO_REVISION sin datos de perdedor. Verifica incremento unico de revisiones, contrato ganador completo, resultado compartido leido desde ambas sesiones, numero/version nuevos sin duplicados y primera publicacion intacta. Final esperado: proveedor INACTIVO, una version PUBLICADA y otra BORRADOR; no activa ni consulta datos del origen, ni modifica fuentes. Cierra segunda sesion en finally. No demuestra todas las intercalaciones internas posibles ni concurrencia de disponibilidad.

Cuatro pruebas locales PASS: verificador de carreras con dobles que detectan doble exito, doble incremento y reescritura anterior, mas transporte seguro. Sintaxis correcta. Pendiente corrida IIS; 14.23/14.30 siguen abiertas hasta evidencia real.

## Concurrencia administrativa proveedor - aceptacion y cierre de 14.23 (2026-10-25)

Resultado aportado por el usuario: 1 passed (33.9s), caso 32.7s. PROVIDER_INDEPENDENT_ASPNET_SESSIONS confirma sesiones distintas. GuardarProveedor, PublicarProveedor y CrearVersionProveedor tuvieron exactamente un ganador 200 y un conflicto 409 CONFLICTO_REVISION por carrera. Las lecturas desde ambas sesiones coincidieron con el resultado ganador; guardado/publicacion incrementaron revisiones una sola vez.

La clonacion genero una sola version nueva con numero consecutivo sin duplicados; la primera publicacion conservo contrato, estado y revision. El historial contiene dos versiones (una PUBLICADA y otra BORRADOR), ambas del proveedor INACTIVO. No se activaron proveedores ni se modificaron fuentes o datos de origen. No se registran tokens, cookies ni valores de negocio.

Se completa 14.23 junto con las evidencias anteriores de alta visual, rechazo de edicion publicada y revision obsoleta sin cambios. Se corrige su referencia de archivo al repositorio administrativo MySqlAdministracionProveedorRepository.vb. Supera el pendiente IIS del apartado anterior. 14.30 permanece abierta para los criterios restantes de su cobertura mas amplia. Esta corrida no prueba todas las intercalaciones posibles ni concurrencia de cambios de disponibilidad, aislamiento empresarial o revocacion.

## Disponibilidad concurrente del proveedor - preparacion (2026-10-25)

Modo --provider-availability --headed reutiliza las carreras y dos sesiones del proveedor descartable creado por UI. Agrega dos solicitudes simultaneas de activacion con iguales revisiones: exige 200/409 CONFLICTO_REVISION, un incremento de cabecera y documento compartido en ambas sesiones sin cambiar version/contrato. La activacion es temporal y solo afecta al proveedor creado por esa corrida.

Finalmente relee revision actual y desactiva si esta activo; ambas sesiones verifican disponibilidad false. Espera ambas solicitudes antes de reconciliar incluso ante fallo de transporte. Despues exige incremento total de cabecera de dos unidades (activar/desactivar) y compara historial completo desde ambas sesiones con el previo: mismos IDs, numeros, estados, contratos y revisiones de version. No elimina versiones ni modifica fuentes/datos de origen. Si servidor deja de estar disponible, la prueba falla y no declara desactivacion confirmada.

Seis pruebas locales PASS: carreras, disponibilidad e historial, desactivacion tras fallo de asercion o transporte simulado posterior al commit y transporte seguro. Pendiente aceptacion IIS; 14.24/14.30 permanecen abiertas. No prueba aislamiento entre empresas, referencias de perfiles publicados ni revocacion de fuentes.

## Disponibilidad concurrente del proveedor - aceptacion (2026-10-25)

Resultado aportado por el usuario: 1 passed (33.3s), caso 32.3s. PROVIDER_CONCURRENT_ACTIVATION_ONE_WINNER_SHARED_RESULT confirma exactamente un ganador 200 y un conflicto 409, incremento unico de revision de cabecera y mismo documento activo desde dos sesiones independientes. PROVIDER_AVAILABILITY_FINALLY_DISABLED_BOTH_SESSIONS confirma desactivacion posterior con revision vigente y lectura inactiva desde ambas sesiones.

PROVIDER_AVAILABILITY_HISTORY_AND_VERSION_REFERENCES_UNCHANGED confirma historial completo preservado: mismos identificadores, numeros, estados, contratos y revisiones de version. Solo cambiaron disponibilidad y revision de cabecera, con dos incrementos totales por activar/desactivar. Tambien pasaron guardado/publicacion/clonacion concurrentes, lectura compartida y primera publicacion intacta. El proveedor descartable termino INACTIVO; no se modificaron fuentes ni registros de origen.

Supera el pendiente IIS de la preparacion anterior. Acredita concurrencia de activacion e historial en el ambito autorizado; el marcador de referencias corresponde a versiones de proveedor, no a perfiles publicados que lo referencien. 14.24 conserva pendiente esa comprobacion historica de consumidores y no se generaliza a aislamiento entre empresas. 14.30 mantiene sus criterios restantes. Evidencia de ejecucion aportada por el usuario, sin secretos.

## Referencias de consumidores tras desactivar proveedor - preparacion (2026-10-25)

Se amplia --provider-profile --headed. Tras publicar dos versiones del perfil enlazadas al proveedor y desactivar el perfil descartable, captura ambas lecturas completas y su historial mientras el proveedor sigue activo. Desactiva proveedor en finally y relee publicaciones/historial: exige igualdad completa, incluidas referencias proveedor/version y reglas. Verifica ademas que ConsultarProveedores filtrado por los IDs del proveedor inactivo devuelva lista vacia. Los snapshots y la clave permanecen en memoria, no se escriben en evidencia.

Cuatro pruebas locales PASS: verificador de referencias/historial/catalogo (detecta alteraciones) y transporte seguro; sintaxis correcta. Pendiente IIS para 14.24 y avance 14.6/14.11. Reutiliza dato local de prueba previamente verificado, pero el propio ensayo exige nuevamente resultado univoco antes de activar. No acredita rechazo de todas las operaciones consumidoras, aislamiento empresarial o importacion fisica. No modifica fuentes ni datos de origen.

## Referencias de consumidores - aceptacion y cierre de 14.24 (2026-10-25)

Resultado aportado por el usuario: 1 passed (22.7s), caso 21.3s. DISABLED_PROVIDER_PUBLISHED_PROFILE_REFERENCES_AND_HISTORY_UNCHANGED confirma igualdad completa de ambas publicaciones del perfil y su historial antes/despues de desactivar el proveedor: referencias proveedor/version, reglas, configuracion y revisiones conservadas. DISABLED_PROVIDER_EXCLUDED_FROM_CONSUMER_CATALOG confirma lista vacia al consultar opciones por los IDs del proveedor inactivo.

Tambien pasaron consulta univoca real, indice esperado, publicaciones con ambitos EXPEDIENTE y LOTE, inmutabilidad y desactivacion de perfil/proveedor descartables. No se modificaron fuentes ni registros de origen. Los snapshots y la clave de consulta no se guardan en esta evidencia.

Se completa 14.24 junto con la corrida --provider-availability (33.3s), que acredita concurrencia de disponibilidad e historial del proveedor. Supera pendientes anteriores de referencias historicas desde consumidores y de esta corrida IIS. No acredita aislamiento entre empresas, rechazo de todas las operaciones consumidoras ante proveedor inactivo ni importacion fisica. 14.6/14.11 conservan sus criterios restantes; no se cierran por inferencia. Evidencia real aportada por el usuario.

## Proveedor activo con version BORRADOR - preparacion (2026-10-25)

--provider-profile ampliado: tras publicar dos versiones y activar temporalmente SOLO proveedor descartable, clona una tercera version BORRADOR dentro del try/finally de desactivacion. El perfil se configura por UI con la segunda version PUBLICADA. Luego ConsultarProveedores filtrado por ID del borrador debe devolver vacio; GuardarBorrador y EnsayarReglas reciben directamente ese ID en una copia de las reglas y deben responder 403 ACCESO_DENEGADO sin datos. Cada intento exige perfil completo inalterado; al final se confirma tambien documento del proveedor borrador sin cambios.

Continua ensayo/publicacion positivos con la version publicada, conserva referencias historicas y desactiva descartables. Historial final esperado del proveedor cambia a tres versiones: dos PUBLICADAS y una BORRADOR, proveedor INACTIVO. No se modifican fuentes ni registros de origen. Seis pruebas locales PASS incluyendo deteccion de catalogo erroneo, rechazo por motivo ajeno y perfil alterado; sintaxis correcta. Pendiente IIS. 14.29/14.11 permanecen abiertas: no acredita aun rechazo en todos los puntos de publicacion/seleccion, ni aislamiento empresarial.

## Proveedor activo con version BORRADOR - aceptacion parcial (2026-10-25)

Resultado aportado por el usuario: 1 passed (22.9s), caso 21.7s. ACTIVE_PROVIDER_DRAFT_EXCLUDED_FROM_CONSUMER_CATALOG confirma exclusion de la version BORRADOR aunque el proveedor este activo. GuardarBorrador y EnsayarReglas con su ID directo devolvieron 403 ACCESO_DENEGADO sin datos; lecturas posteriores confirmaron perfil completo intacto. REJECTED_CONSUMER_REQUESTS_KEEP_PROVIDER_DRAFT_UNCHANGED confirma proveedor borrador sin cambios.

El recorrido positivo continuo con la version publicada: indice real correcto, dos publicaciones de perfil, primera inmutable y referencias/historial conservados tras desactivar proveedor. Perfil y proveedor descartables terminaron INACTIVOS. Historial del proveedor: dos versiones PUBLICADAS y una BORRADOR. No se modificaron fuentes ni registros de origen.

Supera el pendiente IIS del apartado anterior. Avance de 14.29/14.11: catalogo, guardado y ensayo verificados; permanecen pendientes sus criterios restantes de publicacion/seleccion y referencias incompatibles/no autorizadas. No se infiere que todos los endpoints esten cubiertos ni se acredita aislamiento empresarial. Evidencia de ejecucion aportada por el usuario, sin secretos.
