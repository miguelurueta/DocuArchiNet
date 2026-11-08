# Ampliación autorizada para recuperar el lote24

El usuario confirmó con «continua» la propuesta de9 aperturas/1500 HTTP acumulados (1425/75), conservando recursos y consumos. La petición HEAD de disponibilidad pasó HTTP200 y elevó el acumulado de995 a996; siguen40 SELECT y7 aperturas. El worker31784 tiene observador externo ATTACHED con sourceBound=true. La consola local enmascarada está abierta, pendiente de captura; este corte no registra otro login ni solicitudes documentales. La recuperación del lote24 y la calificación Freeze/Thaw siguen pendientes. Regresión177/177; progreso16/27. Evidencia de disponibilidad: `doc118-prepared-recovery-readiness-20261108.json`.

## Cortes previos

# Corte actual — lote24 conservado, antes del primer almacenamiento

La autorización1200 HTTP/7 aperturas fue aplicada tras «confirmado». PREFLIGHT y PREPARE02 pasaron; RUN02 se detuvo antes de despachar ALMACENAR. Lote24:dos PDFs recibidos/materializados y expedientes670/671;0 documentos almacenados solicitados por esta corrida. Consumo995 HTTP/40 SELECT/7 aperturas, sin capacidad de otra apertura. Finally y limpieza del observador confirmados. ARM/RESUME/CLOSE posterior pasó sin escritor ni efectos; Freeze/Thaw y progreso del peer aún no calificados. Progreso16/27.

Detalle y límites: [preparación y recuperación](doc118-preparacion-recuperacion-20261108.md), [corte saneado](doc118-prepared-stop-20261108.json), [ARM externo](doc118-observer-arm-20261108.json). Regresión local177/177. Recuperación implementada por lecturas del lote existente; propuesta9 aperturas/1500 HTTP pendiente, sin aplicar.

## Cortes anteriores con su alcance original

# DOC-118 — preflight autenticado y bloqueo del observador

El preflight de dos sesiones pasó con publicación38/gabinete10 y17 SELECT. Consumo acumulado699 HTTP C02,0 C03 y5 aperturas;0 fases documentales y0 efectos. El worker26244 dejó de estar activo por causa no determinada. La consola terminó sin reconocer la orden CLOSE ni el finally; no quedan consola/shell ni hijos directos detectados. Las instancias privadas de depuración ya no están presentes y los dos IDE originales quedaron sin depuración activa, con12 breakpoints del usuario conservados. La observación interna sigue SIN_CALIFICAR. Evidencia: doc118-authenticated-preflight-20261107.json.

## Autorización y consumo

La ampliación1000 HTTP acumulados (925/75) y5 aperturas fue autorizada por el usuario y aplicada conservando425 HTTP/3 aperturas anteriores. La pareja nueva autenticó sin retries y consultó lote23 desde contextos independientes. El preflight comprobó publicación, gabinete, propietario y obtuvo baselines de lotes18/21 mediante17 SELECT de control. No se realizó una comparación posterior ni hubo solicitudes mutantes a esos lotes.

| Concepto | Consumido | Disponible |
| --- | --- | --- |
| HTTP C02 |699 de925;3 ConsultarLote acumuladas |226 |
| HTTP C03 |0 de75 |75 |
| HTTP global |699 de1000 |301 |
| Aperturas/intentos asignados |5 de5 |0 |
| SELECT de control |17 de240 |223 |
| Planes/lotes/raíces/PDFs/expedientes/documentos nuevos |0 | Límites originales |
| Detenciones y congelaciones |0 | Sin ejecutar |

La política de omisión evitó223 subrecursos antes de despacho. No son reembolsos de los425 HTTP anteriores ni evidencia de QA visual. La pareja consumió272 HTTP nuevos. Los archivos de entrada siguen locales e ignorados y no se conservaron datos de la captura en los informes.

## Observador externo

La lectura CodeView/MSF confirma GUID y edad coincidentes entre DLL/PDB ordinarios en disco, conservando sus hashes iniciales. Se corrigió el lector local para el mapa de directorio que abarca varios bloques de512 bytes. Esto no verifica el módulo cargado.

El IDE del repositorio tenía12 breakpoints habilitados. Se usaron instancias privadas vacías para no adoptar esa configuración. Una instancia adjuntó al worker26244 y resolvió un breakpoint False dentro del cuerpo ordinario; se cerró antes de efectos. Otra inicialización falló en BIND_SOURCE y no reconoció su limpieza; la inspección posterior no encontró la instancia privada ni depuración en los IDE originales. La última inicialización cerró en FIND_TARGET_PROCESS; la lectura de IIS confirmó que ya no había worker. No se determinó la causa de su salida; no atribuirla al idle, al depurador ni a un reciclado sin evidencia.

No hubo ARM documental, solicitud ALMACENAR ni Freeze/Thaw. El controlador y el adaptador permanecen candidatos: compilar, adjuntar y resolver un breakpoint no acreditan que otro cliente responda mientras un hilo está retenido. No se modifica almacenamiento compartido, SII, fuente VB, DLL/PDB, configuración o permisos de IIS.

## Salida de consola y aceptación

El último estado del proceso es PREFLIGHT_PASS. Se envió CLOSE con secuencia2, que quedó sin consumir; el proceso y su shell ya no existían al verificarlos y no se observaron hijos directos. El informe no afirma cierre protocolario de contextos ni borrado explícito por finally. La ausencia de la consola y del worker obliga a revisar una nueva ejecución; los contadores no se reinician y ya no quedan aperturas autorizadas.

Progreso16/27. COORD02/03 específicos permanecen NO_EJECUTADOS. En ese corte la CLI solo implementaba PREFLIGHT; la continuación local posterior implementa las siete fases. Regresión local:130 pruebas (12 preflight,31 enlace,16 coordinación,63 drivers,4 símbolos,4 adaptador externo), separada de la evidencia real. El controlador C# compila contra el SDK instalado; no tiene calificación de retención de hilo en runtime.


## Continuación local y comprobación del observador

Las siete fases del ejecutor están implementadas y verificadas localmente. El observador real pasó disponibilidad, breakpoint de fuente, INSPECT y CLOSE con ack de limpieza y salida0 en sesión supervisada. No se ejecutaron ARM/Freeze/Thaw, fases documentales ni nuevas sesiones autenticadas. Consumo acumulado699 HTTP C02,0 C03,17 SELECT y5 aperturas;0 aperturas disponibles. Progreso16/27: COORD02/03 siguen NO_EJECUTADOS para aceptación.

El detalle de la implementación está en doc118-ejecutor-fases-local-20261108.md. La consola previa sigue sin ack de finally; el controlador nuevo sí reconoció su cierre tras corregir el guardado atómico. La disponibilidad se comprobó sin autenticación. La calificación de retención y las ventanas de negocio siguen pendientes. En ese corte la propuesta de1200 HTTP/7 aperturas no estaba aplicada; la sección inicial registra su autorización y consumo posteriores.
