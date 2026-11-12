# DOC-119 — plan de aceptación para concretar y autorizar

Preparación de tarea 1.2, D-10/RQ-10. El usuario indicó «autorizado» en respuesta al plan presentado: se autorizan las ventanas de lectura SEG-01/02 descritas abajo en el ambiente local. Cuentas y recursos se concretan por el operador en la consola enmascarada; no se deducen de permisos históricos ni se guardan credenciales. La tarea no se completa hasta fijar esas referencias.

## Destino propuesto y datos que faltan

Ambiente de estas ventanas autorizadas: `https://localhost/GestionDocumental-Docuarchi.net/`. Se autentica la cuenta propietaria y se muestran hasta 30 importaciones propias por fecha UTC, destino, estado y cantidades. El operador elige una opción de la lista; no necesita conocer IDs internos. La consulta se limita a usuario/empresa y facultades actuales, sin deducirlos de permisos/sesiones históricas. La cuenta ajena autorizada con permiso de importar se captura al llegar a la prueba de privacidad. Una excepción para certificado local solo se activa si el operador elige expresamente SI en la consola; de lo contrario se valida TLS normalmente.

SEG-01 necesita lote del propietario con estados mixtos y suficientes filas para más de una página según política vigente; al menos un filtro con coincidencias y uno sin ellas. Lote5 histórico PARCIAL es una referencia candidata para revisar por SELECT si el responsable la elige; no se presupone propietario, disponibilidad o adecuación actuales. Lotes27/28 y restantes recursos protegidos se conservan; este plan no los modifica ni reejecuta.

SEG-02 necesita un lote existente con recepción por completar, archivo local correspondiente autorizado para reselección y rama original visible/cargada; además otro nivel seleccionable y caso homónimo. Si no existe recurso adecuado, su creación/carga requiere propuesta y autorización separadas. No fabricar datos, modificar estados ni repetir efectos inciertos para construir el escenario.

## Ventanas propuestas

| Ventana | Acciones autorizadas | Techo autorizado por ventana | Criterio de aceptación |
| --- | --- | --- | --- |
| SEG-01 | Dos sesiones: propietario y cuenta ajena; ConsultarLote, ConsultarRecursosLote, ExportarPaginaLote y descarga desde vista | 2 aperturas de sesión, 120 HTTP de aplicación, 80 SELECT; sin efectos documentales | Filtros/páginas/CSV coinciden, totales originales preservados, fórmulas neutralizadas, cuenta ajena sin filas/CSV y huellas de negocio invariantes |
| SEG-02 lectura/UI | Recarga, ConsultarConfirmacionLote si corresponde, consulta del lote, reselección local sin envío y actualización deduplicada de rama | 2 aperturas de sesión, 80 HTTP de aplicación, 60 SELECT; cero ConfirmarPlan/AvanzarLote/ReconciliarRecurso/upload | Destino del lote y selección conservados, referencias aisladas, homónimos identificados por IDs y ausencia de confirmación repetida |
| SEG-02 recepción, solo si resulta necesaria | Enviar únicamente el recurso pendiente y contenido fijados después de revisar lectura/UI | No concedido; requiere IDs, PDF/hash/tamaño y presupuesto de un envío acordados aparte | Verificación servidor por contenido/autoridad sin duplicación; ningún almacenamiento o efecto adicional implícito |

Los techos propuestos cuentan reintentos y fallos de sesión; al agotarlos se detiene sin reiniciar contador. La sesión genera los registros normales de login/apertura existentes; invariancia se evalúa sobre datos/auditoría de negocio de los recursos, no se promete ausencia absoluta de registros de acceso.

## Controles y reutilización de herramientas

Reutilizar sesión autenticada y puente ODBC `queryRows` existentes. Los DSN RA/DA y credenciales SELECT se capturan en consola antes del descubrimiento parametrizado; no se admiten cadenas de conexión o secretos en el perfil. Huellas antes/después de lote/raíces/archivos/intentos y relaciones documentales pertinentes; sin UPDATE/DELETE, FOR UPDATE ni funciones de espera.

`run-doc111-importacion-carpetas.cjs` predeterminado ejecuta REC con efectos. `--diagnose` solo diagnostica acceso y no acredita seguimiento; `--verify-lot` está restringido a lote COMPLETADO y no acredita filtro/CSV/negativo/recarga incompleta. Los modos de recuperación pueden cerrar reservas, reconciliar y continuar almacenamiento: quedan fuera de las ventanas de lectura.

`run-doc113-ventana-importacion.cjs` cubre los casos UI-01/02/03 de DOC-113; UI-02 prepara planes y puede confirmar si se solicita. No es un selector SEG-01/02. Antes de una corrida DOC-119 hay que revisar/adaptar el recorrido de seguimiento con las mismas utilidades y una lista cerrada de operaciones permitidas; no ejecutar otro escenario para simular aceptación.

## Detención y evidencia

Detener ante autoridad/identidad/destino discordantes, operación fuera de la lista, mutación inesperada, respuesta no interpretable, control SELECT fallido o presupuesto agotado. No reintentar automáticamente ni convertir incertidumbre en permiso de recepción/almacenamiento. No cambiar permisos para probar revocación sin autorización específica.

Conservar solo resultado saneado, SHA/hashes del cliente servido y ejecutor, ambiente, IDs autorizados, contadores, códigos y huellas. Capturar secretos por consola compartida y eliminarlos al cerrar; nunca guardarlos o imprimirlos. No borrar lotes, temporales o evidencia al terminar.

La selección concreta de importación/nivel alterno/PDF se realiza después de autenticar al propietario y consultar sus recursos existentes. Esa sesión y los SELECT de descubrimiento consumen los mismos presupuestos SEG-01/02, sin reiniciarlos; la sesión propietaria se reutiliza en SEG-01. Hasta elegir el recurso, la guardia no permite operaciones de importación. La autorización actual no permite crear recursos si no existen casos adecuados ni habilita recepción: la tercera ventana sigue sin concederse. La reorganización UI de la sección 3 permanece detrás de aceptación integrada SEG-01/02; preparar el selector o pasar pruebas locales no abre esa compuerta. SEG-03 requiere decisión propia y no habilita borrados.

## Selector preparado

`node tools/e2e/scripts/run-doc119-seguimiento.cjs --check` comprueba el código sin red, sesiones ni credenciales. La corrida usa `--run <runId-32-hex> --authorized-plan` en consola visible; cuentas, claves y ruta local del PDF se enmascaran. El selector no consume perfiles con secretos ni configura otro login.

Reutiliza openSession/consulta de diagnóstico, queryRows, createSelectControls, selectLevel y captura de evidencia existentes. La guardia bloquea cualquier ASMX fuera de la lista, todo ASHX y postbacks de negocio; en SEG-02 solo permite Button_inline_trevie con título de nivel original/alterno elegido. Cuenta todos los HTTP de aplicación permitidos, incluidos recursos estáticos y pruebas de origen servido; cuenta SELECT y sesiones antes de invocarlos y conserva consumos en `.tmp/doc119-seguimiento/<runId>/state.json`, excluido de Git. No reutilizar un runId ni reiniciar el presupuesto tras un fallo.

Recursos inadecuados producen detención sin fabricar éxito: SEG-01 necesita un filtro cuyo resultado tenga segunda página real bajo el tamaño vigente; lote5 es solo candidato y puede ser insuficiente. SEG-02 ofrece las importaciones con archivos por recibir y muestra el nombre/ruta relativa del archivo esperado; necesita además raíz confirmada, rama original cargada y archivo correspondiente. Se puede omitir con opción 0 y queda NO_EJECUTADO. Los destinos alternos se eligen por nombre entre los actualmente autorizados. Homónimos o nuevas relaciones no observados se informan como límite, no como aceptación completa. Reselección solo acredita metadatos/identidad local; bytes/hash servidor no se verifican sin una recepción aparte.

## Continuación manual solicitada — 2026-11-12

La corrida `d09ca8a7cdc547cca79fb53d37af26af` se detuvo por guardia antes del postback de login: 1 apertura de sesión, 1 HTTP de aplicación, 0 SELECT y sin casos aceptados. Un diagnóstico posterior sin credenciales consumió 1 GET de aplicación y confirmó que la primera solicitud bloqueada era la hoja opcional de Google Fonts. La guardia ahora omite esa referencia exacta mediante aborto local, sin enviar tráfico externo ni ampliar las operaciones de negocio permitidas.

El usuario solicitó expresamente «continua con la prueba real» después de la primera corrida. Esa instrucción se toma como continuación manual tras corregir el ejecutor, con una nueva ventana de los mismos topes SEG-01/02 y runId distinto. No se borra ni reinicia la corrida fallida; se conservan sus consumos y el diagnóstico, y no se configura retry automático. Los secretos anteriores fueron eliminados y deben capturarse nuevamente por consola. Este intento no autoriza recepción, carga ni limpieza.

## Corrección de acceso y presupuesto propuesto — pendiente de aprobación

La corrida `42a55db64044453c8c6cbac63109702a` se detuvo con 93 HTTP, 1 apertura de autenticación y 0 SELECT. Un diagnóstico sin credenciales reprodujo el bloqueo durante un POST XHR al iniciar la página. El cliente 2FA llama automáticamente a ObtenerEstado con JSON vacío; el ejecutor ahora contempla exclusivamente esa consulta y la página local fija de destino de login. No contempla operaciones de verificación/reenvío/cancelación ni amplía APIs de importación.

Preflight real sin credenciales sobre la guardia corregida: página HTTP 200, controles presentes, 1 consulta ObtenerEstado, 95 HTTP de aplicación, 0 aperturas de autenticación/SELECT y ninguna violación. No acredita login ni SEG-01/02.

La carga de esa página ya supera los 80 HTTP de SEG-02 y deja solo 25 de los 120 HTTP de SEG-01 para Producción Documental y la segunda cuenta. Por ello se propone **una nueva ventana manual con 600 HTTP por SEG-01 y 600 por SEG-02**, contando todos los recursos estáticos, postbacks, recargas y consultas como antes. Se conservan 80/60 SELECT y 2 aperturas de autenticación por ventana, sin concurrencia, efectos, recepción ni limpieza. Estos nuevos techos **no están autorizados ni configurados**; el código mantiene 120/80 HTTP hasta recibir aprobación expresa. La corrida fallida y sus dos diagnósticos permanecen registrados y no se reinician.
