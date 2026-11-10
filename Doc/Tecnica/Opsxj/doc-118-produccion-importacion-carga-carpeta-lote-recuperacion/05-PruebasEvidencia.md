# DOC-118 — coordinación y recuperación

## Cierre administrativo con salvedades — 2026-11-10

Jira confirmó el cierre administrativo en Listo (comentario24490), aceptado por el usuario conservando las nueve tareas pendientes. 29/38;260/260 Node históricos y observador4.1 calificado, sin aceptación completa de COORD02/03 ni QA/E2E. Lote25 retenido con almacenamiento UNKNOWN, sin replay ni limpieza. Cambio archivado en `openspec/changes/archive/2026-11-10-doc-118-produccion-importacion-carga-carpeta-lote-recuperacion`; sin sincronizar specs ni fusionar la rama. Las compuertas ordinarias OPSXJ siguen sin aprobarse; no se presenta como cierre técnico completo. Fuente:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-cierre-administrativo-20261110.json`.

## Ejecución consolidada: observador calificado y parada conservada

4.1 calificada en llamada real con hilo interno congelado y otra sesión atendida; 29/38 tareas. Plan41/lote25 nuevo conservado: COORD02 detenido tras SAME_COMMAND, primera petición UNKNOWN y sin OTHER_FILE/COORD03. SDK liberado/cerrado y sesiones eliminadas; sin replay. Acumulado1578 HTTP/147 SELECT/11 sesiones. Corrección del ejecutor: diagnóstico cerrado y espera original tras RESUME hasta35s. 260/260 Node; almacenamiento/SII/binarios intactos. Auditoría SELECT pendiente de dos entradas enmascaradas. Fuente:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-ejecucion-consolidada-20261109.json`.

## Fuente del observador enlazada y SDK cerrado

Continuación del observador revisada tras «continua»: fuente enlazada, worker vivo/otros hilos en ejecución, cero entradas/fallo/Freeze y desconexión/liberación SDK confirmadas. Cero HTTP/login/SELECT/efectos nuevos; una conexión aprobada consumida, ninguna restante. La HEAD anterior y su fallo se preservan. No se observó IDE nueva residual en la lectura posterior; PID propio no identificado directamente. Fuentes/instalación/SII intactos. 28/38, no completa 4.1 sin llamada activa. Fuente:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-observador-continuacion-20261109.json`.

## P4-B detenida en la comprobación HTTP

P4-B aprobada y ejecutada una vez: cliente sin estado HTTP; log IIS con 200 y 14,807 s frente a timeout 10 s. Corte antes de construir/conectar observador, sin ARM/Freeze ni efectos. Una HEAD consumida y cero restantes; sin retries. Cero login/SELECT/efectos; acumulado entre histórico y P4-B 1277 HTTP/113 SELECT/9 aperturas. Fuentes/instalación/respaldo intactos. No acredita enlace de fuente o runtime. 28/38, sin nuevas tareas/cierres. Fuente:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-runtime-smoke-20261109.json`.

## Metadatos IIS y siguiente comprobación propuesta

Inspección de metadatos IIS: un worker y módulo objetivo no observado en la vista del sistema operativo; no acredita ausencia en CLR o identidad cargada. Cero red/login/SQL/efectos y ningún cambio de producto/runner/instalación. Preparada P4-B: una HEAD y enlace de fuente mediante Initialize/Inspect/Dispose del SDK existente, sin ARM/Freeze; propuesta aún sin autorización. 28/38, ninguna tarea añadida/cerrada. Fuente:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-metadatos-runtime-20261109.json`.

## Despliegue autorizado del candidato V2

Despliegue P4-A autorizado mediante «autoriozado»: DLL/PDB ordinarios instalados y verificados por hash/CodeView, respaldo preservado y mantenimiento propio retirado en 627 ms. Fuente compartida/InvocarAlmacenamiento/SII y Web.config intactos; pool de 6 aplicaciones sin reciclar. Cero login/HTTP/SQL/efectos. La vista de solicitudes IIS devuelve 0x80004001; se usó la ventana aprobada, sin afirmar ausencia demostrada de peticiones. Módulo cargado/proveedor/MySQL/observador aún sin calificar y runner histórico mantiene pin anterior. Progreso 28/38, ninguna tarea nueva o cerrada. Fuente:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-despliegue-v2-20261109.json`.

## Candidato compilado y despliegue propuesto

Compilación completa del candidato ordinario V2:MSBuild salida0,920 fuentes estables y DLL/PDB concordantes en .tmp; binarios instalados/Web.config/almacenamiento/SII intactos. Aplicación IIS localizada por lectura elevada; raíz igual al repo principal y pool compartido por6 aplicaciones. Propuesta de despliegue limitado a dos archivos, respaldo y mantenimiento solo de la aplicación, aún SIN_AUTORIZAR/SIN_EJECUTAR.28/38 tareas, ninguna nueva/cerrada. No se repite regresión247 Node ni se ejecuta HTTP/SQL/login. Fuente:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-candidato-v2-build-20261109.json`. Alcance revisable:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-despliegue-v2-propuesto-20261109.md`.

## Corte local de pérdida de acuses V2

Completada6.11/D-18/RQ-18: decorador de pruebas de pérdida de un acuse nativo válido y conductor acotado Q1/Q2/Q3, fuera del proyecto productivo.247/247 Node (42 comportamiento/cliente/driver/conductor y205 herramientas);157 casos V2 internos del harness, incluidos sin sumarlos a Node. Progreso28/38, mismas10 tareas reales/finales pendientes. P3 preparado y probado como paquete local; enlace/lector del host de pruebas aún sin instalar/calificar. P4/P5 abiertos. Sin modificar almacenamiento compartido/InvocarAlmacenamiento/SII ni ejecutar HTTP/SQL/login reales. Fuente:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-acuses-v2-local-20261109.json`.

## Regresión de enlace/driver V2

235/235 Node:30 comportamiento/cliente/driver nuevo y205 herramientas existentes.12 casos .NET nuevos del enlace se incluyen en145 casos V2 internos, sin sumarlos a Node. Compila composición documental/dispatcher reales y prueba revisión de servidor, autoridad/contexto, retención, acuses, historial V1 y puertos obligatorios; SQL/escritor simulados.12 pruebas Node nuevas validan contratos/correlación/privacidad, readOnly y timeout/rechazo sin replay. Sin invocar la función compartida ni modificar su fuente/SII. Progreso27/37; sin calificación MySQL/IIS real. Fuente:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-enlace-v2-local-20261109.json`.

## Revisión anterior de evidencia RV-08

Se reutiliza la regresión de e68d9972/RV-07:223/223 Node y133 casos V2 internos incluidos en el arnés. RV-08 solo cambia documentación/manifiestos; se comprueban de nuevo hashes, referencias y trazabilidad, sin repetir compilaciones/pruebas ni atribuir otra corrida a los informes anteriores.16 requisitos/35 escenarios y21 condiciones mapeados.9/9 entregables locales,25/35 tareas;10 criterios reales/finales bloquean archivo. MySQL/IIS y COORD-02/03 no se aceptan por este corte. Fuente:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-revision-v2-local-20261109.json`.

## Regresión local anterior tras RV-07

223/223 Node:18 comportamiento/cliente y205 herramientas, sin sumar133 casos V2 internos del arnés. Los12 nuevos verifican intercalaciones antes/después de inicio, gate, escritor, observación y checkpoint; dos recuperadores, rollback y lector en otro proceso. Barreras explícitas con liberación acotada; dos hijos nuevos cerrados. Guardia real; escritor/SELECT/transacción simulados. Sin cambios de producto, despliegue o calificación MySQL/IIS real. Progreso24/34. Fuente:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-matriz-v2-local-20261109.json`.

## Regresión anterior tras RV-06

223/223 Node:18 comportamiento/cliente y205 herramientas.19 casos nuevos HTTP V2 se incluyen dentro del arnés, sin sumarlos a Node; siguen pasando los102 casos V2 anteriores. Compila dispatcher/composición productivos y prueba guards, cuerpo cerrado, autoridad, proyección, historial y RA/DA/HttpContext. HTTP es sintético; guardia real sobre PDF de prueba, SELECT/transacción simulados y cero escritor. Sin despliegue ni calificación MySQL real. Progreso23/33. Fuente:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-recuperacion-http-v2-local-20261109.json`.

## Regresión anterior tras RV-05

223/223 Node:18 comportamiento/cliente y205 herramientas. El harness añade22 casos de recuperación con guardia real sobre PDF sintético, SELECT/transacción simulados y cero escritor. Comprueba cierre de control, identidad confirmada, rollback, autoridad/vista exactas, historial y bloqueo V1/69. No certifica MySQL real ni habilita HTTP. Progreso22/32. Fuente:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-recuperacion-v2-local-20261109.json`.

## Regresión anterior tras RV-04

223/223 Node actuales, más ejecución enfocada del chequeo DDL ya incluido.20 casos SQL nuevos dentro del harness prueban el mismo núcleo de producción integrado con el adaptador y un doble transaccional; no certifican MySQL real. Conserva los casos previos19/21/20, sin sumarlos al contador Node. Progreso21/31. Fuente:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-persistencia-v2-local-20261108.json`.

## Regresión anterior tras RV-03

223/223 Node:18 comportamiento/cliente y205 herramientas. El harness incorpora20 casos del adaptador y conserva19 de contratos y21 del proveedor. Prueba retención anterior al inicio, respuesta perdida/alterada, una invocación, IDs parciales, concurrencia y cancelación/ThreadAbort propagados. Persistencia y escritor son dobles explícitos; no acredita MySQL o legacy real. Progreso20/30. Fuente:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-adaptador-v2-local-20261108.json`.

## Regresión anterior tras RV-02-B

223/223 Node actuales:18 comportamiento/cliente y205 herramientas. El harness añade21 casos del proveedor real y conserva19 casos RV-01, incluidos en el contador del harness.3 procesos sintéticos cerrados, exclusión frente a lectores de otro proceso, identidad, enlaces y Dispose comprobados. La producción, otro nodo/reinicio y recuperación siguen sin acreditar. Progreso19/29. Fuente:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-puerto-staging-v2-local-20261108.json`.

## Regresión anterior tras RV-01

18/18 pruebas Node del harness/cliente y205/205 de herramientas,223 disjuntas actuales. El harness compila y ejecuta19 casos nuevos V2; no se suman al contador Node. Incluyen esquema cerrado, vínculos/fases, compatibilidad V1, privacidad, datos incompletos y ausencia de efectos/capacidad. No hubo nuevas sesiones, HTTP de negocio o SELECT. Código interno sin conexión a recuperación. Progreso18/28. Fuente:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-contratos-v2-local-20261108.json`.

## Regresión anterior tras RV-02-A

205/205 pruebas en16 archivos. El probe nuevo aporta13 aserciones sobre NTFS local con6 procesos independientes y limpieza confirmada. No usa el staging real, sesiones, HTTP, SELECT o datos de negocio. Puerto del proveedor y recuperación V2 siguen pendientes; la prueba no habilita69 ni acredita identidad entre nodos. Fuente:Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-staging-lock-probe-20261108.json.

- Ticket: DOC-118
- Cambio OpenSpec: doc-118-produccion-importacion-carga-carpeta-lote-recuperacion
- Clasificacion: cross_cutting
- Estado: ejecutor de siete fases verificado localmente; disponibilidad/cierre de observador PASS, retención SIN_CALIFICAR. Progreso16/27.

## Evidencia requerida

unit: el 2026-11-07 se ejecutó node --test tests/doc110-perfil-importacion-behavior.test.cjs; 2/2 pruebas Node, cero fallos, VB/.NET Framework y DDL locales sin HTTP ni conexión. No se suman grupos internos C# al contador Node.

manual_qa: no se ejecutó QA real nuevo para DOC-118. La matriz Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-matriz-coordinacion.md distingue REC reales históricos, dobles actuales y las integraciones COORD-02/03 NO_EJECUTADAS. source-review.json registra hashes de fuentes y reportes y la base de revisión. Los resultados previos no se trasladan a los binarios actuales.

## QA/E2E WebForms

Antes de ejecutar: resolver observación compatible dentro de llamada legacy real, documentar ambiente/cuentas/recursos nuevos, puntos de fallo, límites y corte, leer AGENTS.md y el runbook, y obtener autorización específica. No se concede por este documento. Intentos41/lote18 y53/lote21 permanecen intactos. No repetir cargas acreditadas ni efectos inciertos. La revisión editorial, pruebas locales y validación OpenSpec no completan las brechas reales ni autorizan su ejecución.


## Revalidación de preparación DOC-118

El 2026-11-07 se ejecutó `node --test tests/doc110-perfil-importacion-behavior.test.cjs tests/doc111-importacion-carpetas-client.test.cjs`: 18/18 pruebas Node, cero fallos. Incluye compilación/reglas/DDL y cliente Chromium local con servicios sintéticos, sin login ni HTTP real. Se verificaron nuevamente 14 huellas de fuentes y 10 reportes históricos.

Protocolo y dependencia real: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-plan-integracion.md`. El mapa de seguridad/recuperación está en `doc118-seguridad-recuperacion.md` de la misma carpeta. La aprobación editorial y refinement PASS no acreditan observador calificado ni COORD-02/03 real.


El helper nuevo se verificó con `node --test tests/doc118-active-call.test.cjs`: 16/16 pruebas, cero fallos. Verifica rechazo de observación no correlacionada, hilo cambiado, primera petición ya terminada, timeout, respuesta perdida y fallo de liberación sin reenvíos ni exposición de errores privados. Sumadas a las 18 anteriores son 34 pruebas locales en dos ejecuciones disjuntas; no acreditan el depurador real ni efectos documentales nuevos.


## Regresión vigente y resultado real

El preflight de dos sesiones pasó con publicación38/gabinete10 y17 SELECT. Consumo acumulado697 HTTP C02,0 C03 y5 aperturas;0 fases documentales y0 efectos. El worker26244 dejó de estar activo por causa no determinada. La consola terminó sin reconocer la orden CLOSE ni el finally; no quedan consola/shell ni hijos directos detectados. Las instancias privadas de depuración ya no están presentes y los dos IDE originales quedaron sin depuración activa, con12 breakpoints del usuario conservados. La observación interna sigue SIN_CALIFICAR. Evidencia: doc118-authenticated-preflight-20261107.json.

130 pruebas locales:12 preflight,31 enlace,16 coordinación,63 drivers,4 símbolos y4 adaptador externo. Más18 previas disjuntas:148, sin sumar repeticiones. El C# compila y hubo source binding real, pero no Freeze/Thaw ni ARM. La limpieza de una instancia y el finally de la consola no tuvieron reconocimiento; conservar ambas limitaciones y su inspección posterior.


## Ejecutor local y prueba SDK de disponibilidad

Las siete fases del ejecutor están implementadas y verificadas localmente. El observador real pasó disponibilidad, breakpoint de fuente, INSPECT y CLOSE con ack de limpieza y salida0 en sesión supervisada. No se ejecutaron ARM/Freeze/Thaw, fases documentales ni nuevas sesiones autenticadas. Consumo acumulado699 HTTP C02,0 C03,17 SELECT y5 aperturas;0 aperturas disponibles. Progreso16/27: COORD02/03 siguen NO_EJECUTADOS para aceptación.

160/160 locales y178 pruebas disjuntas contando18 previas; sin sumar repeticiones. doc118-observer-readiness-20261108.json conserva HEAD699 acumulado, fuente resuelta e INSPECT/CLOSE con ack. La regresión PowerShell incluye el ArgumentException reproducido del reemplazo atómico. No se ejecutaron las ventanas documentales.


## Corte posterior a la preparación del lote24

La regresión vigente pasa177/177. El corte real acumulado es995 HTTP/40 SELECT/7 aperturas, con dos expedientes y0 solicitudes ALMACENAR; limpieza confirmada. ARM/RESUME/CLOSE pasó sin Freeze ni escritor. COORD02/03 no están aceptados; progreso16/27. Propuesta de9 aperturas/1500 HTTP pendiente. Véase `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-preparacion-recuperacion-20261108.md`.


## Ventana real y auditoría posterior

RUN02 se detuvo con fallo del depurador. Lote24/archivo40/intento69 permanece EN_CURSO, con reclamación activa y sin resultado durable; archivo41 mantiene NO_INICIADO y ausencia física verificada. No se puede afirmar cero efectos de A ni repetirlo. Acumulado1276 HTTP/95 SELECT/9 aperturas; C03 no iniciado. La candidata local sincroniza Break/Thaw/Go y conserva historial propio. Regresión187/187, SDK compilado, sin prueba documental posterior de esa corrección. Progreso16/27. Detalle:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-ventana-activa-auditoria-20261108.md`.


## Ausencia física y prueba sintética de liberación

La auditoría encontró0 registros físicos de A/B y mantuvo intacto el intento69 EN_CURSO; no se permite replay. La contención4.6 queda acreditada:17/27. La mecánica SDK Freeze/peer/Break/Thaw/Go pasó en un proceso propio con PDB y fuente sintéticos, sin IIS ni efectos documentales; legacy sigue sin calificar. Regresión192/192 y2/2 comportamiento existentes. Acumulado1276 HTTP/113 SELECT/9 aperturas. La evaluación de un diseño de recuperación EN_CURSO está propuesta, sin implementación o mutación autorizadas. Véase `doc118-auditoria-fisica-liberacion-20261108.md`.
