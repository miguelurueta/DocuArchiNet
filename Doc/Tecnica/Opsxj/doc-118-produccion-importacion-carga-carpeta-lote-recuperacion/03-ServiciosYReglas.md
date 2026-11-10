# DOC-118 — coordinación y recuperación

## Captura revisada lanzada — 2026-11-10

El usuario pidió «lanza» después de la parada sin login o efectos. Se preservó el intento anterior y se abrió la consola en el escritorio normal con observador2 enlazado;al registrar espera APPLICATION_USER. Ambas HEAD se conservan dentro del máximo:acumulado1580 HTTP/172 SELECT/11 sesiones.269/269 Node y32/38 tareas;sin nuevo PASS de integración. Lote25 confirmado/protegido y archivo anterior intacto. Fuente:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-captura-revisada-20261110.json`.

## Lectura durable resuelta;captura de integración por revisar

Lote25:documento16235/inventario23339 físicamente confirmados y archivoB sin efecto, mediante25 SELECT nuevos y sin replay. La consola se hizo visible fuera del escritorio aislado. La ventana nueva consumió solo1 HEAD;se detuvo antes de login/fases, con SDK/secretos cerrados. Acumulado1579 HTTP/172 SELECT/11 sesiones.268/268 Node candidatos;32/38 tareas. Falta aclarar la captura de cuatro entradas antes de otra ejecución. Fuente:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-integracion-continuacion-20261110.json`.

## Continuación técnica activa — 2026-11-10

Jira En curso y mismo cambio OpenSpec reactivado, conservando el archivo administrativo intacto. Se acreditan5.2 por resultados/procedencia explícitos y5.4 por sincronización comprobada;5.3 pasó OpenSpec estricto/refinement;32/38, con seis comprobaciones reales pendientes. La auditoría SELECT del lote25 se retoma con contadores intactos y sin replay; requiere dos entradas enmascaradas.22/22 pruebas focales; almacenamiento/SII/binarios intactos. Fuente:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-continuacion-tareas-20261110.json`.

## Cierre administrativo con salvedades — 2026-11-10

Jira confirmó el cierre administrativo en Listo (comentario24490), aceptado por el usuario conservando las nueve tareas pendientes. 29/38;260/260 Node históricos y observador4.1 calificado, sin aceptación completa de COORD02/03 ni QA/E2E. Lote25 retenido con almacenamiento UNKNOWN, sin replay ni limpieza. Cambio archivado en `openspec/changes/archive/2026-11-10-doc-118-produccion-importacion-carga-carpeta-lote-recuperacion`; sin sincronizar specs ni fusionar la rama. Las compuertas ordinarias OPSXJ siguen sin aprobarse; no se presenta como cierre técnico completo. Fuente:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-cierre-administrativo-20261110.json`.

## Corte local de pérdida de acuses V2

Completada6.11/D-18/RQ-18: decorador de pruebas de pérdida de un acuse nativo válido y conductor acotado Q1/Q2/Q3, fuera del proyecto productivo.247/247 Node (42 comportamiento/cliente/driver/conductor y205 herramientas);157 casos V2 internos del harness, incluidos sin sumarlos a Node. Progreso28/38, mismas10 tareas reales/finales pendientes. P3 preparado y probado como paquete local; enlace/lector del host de pruebas aún sin instalar/calificar. P4/P5 abiertos. Sin modificar almacenamiento compartido/InvocarAlmacenamiento/SII ni ejecutar HTTP/SQL/login reales. Fuente:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-acuses-v2-local-20261109.json`.

- Ticket: DOC-118
- Cambio OpenSpec: doc-118-produccion-importacion-carga-carpeta-lote-recuperacion
- Clasificacion: cross_cutting
- Estado: enlace/driver V2 implementados localmente;27/37 tareas, aceptación real y calificación del runtime abiertas.

## Servicios y reglas

Corte6.9/6.10:la composición documental candidata crea un enlace de la interfaz existente que reautoriza, lee revisión/contexto/snapshot desde servidor y llama AlmacenarV2 con CrearPersistenciaDocumentoV2. No hay fallback V1; el coordinador conserva historial antes de crear un escritor. Se preservan método compartido, InvocarAlmacenamiento y adaptador V1. El driver admite la operación de recuperación con contrato cerrado; no instala hooks de fallo o ejecuta el runtime.235 Node y145 casos V2 internos; P3/P4/P5 siguen abiertos.

### Corte anterior RV-08

RV-08 consolida los contratos D-11..D-16:inicio/gate/retorno V2, retención previa, mismo repositorio/transacción, recuperación sin escritor y entrada HTTP cerrada. La rama negativa exige PREPARADO sin invocación y expediente sin huellas; la positiva verifica IDs observados completos. Guardia y comparación de vista hasta commit/rollback; historial autorizado no escribe.133 casos V2 internos y matriz21 condiciones de RV-07, sin calificarlos como MySQL real. La composición habitual sigue V1; no se integra ni despliega en esta revisión.

AvanzarLote despacha una fase por recurso; ReservarIntentoDocumental aplica exclusión por lote con transacción y revisión. HayReclamacionDocumental bloquea reservas RESERVADO/EN_CURSO/INCIERTO y marcadores huérfanos; no ofrece takeover por fecha. AlmacenarArchivo reutiliza historial de la misma solicitud antes de una reserva nueva.

SolicitarDetencion conserva llamadas activas y efectos. RegistrarResultadoDocumento llama a ConsolidarEnTransaccion en su transacción exterior: no se omite ese cierre por leer solo el helper de persistencia. Consolidar conserva el inventario original; actividad e incertidumbre no equivalen a detención completada. Si el último efecto requerido queda positivo puede resultar COMPLETADO con la señal conservada.

CerrarReservaSinInicio exige reserva documental probada sin inicio, vista exacta y ausencia de detención; su cierre RECHAZADO no acredita un documento. ReconciliarRecurso verifica INCIERTO con evidencia conocida sin escritor. EN_CURSO por fecha, cierre con señal de detención o identidad insuficiente no habilitan reparación. No se modifica servicio alguno en esta entrega.
