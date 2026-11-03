# DOC-112 — Revisión de las diferencias documentales

Fecha: 2026-11-02. Revisión técnica por Codex sobre archivos locales, por instrucción del usuario de continuar el plan reducido. Base integrada de referencia: main `40c5f55e`; la carpeta de trabajo conserva su estado local y no se publica ni despliega esta revisión. La aprobación editorial final del resultado no se infiere de la autorización para trabajar.

## Alcance y resultado

Se aplicaron las doce correcciones de DOC-112 a los cinco documentos comunes. Se reutilizaron implementación y evidencia de DOC-110/111; no se generaron funcionalidades, esquemas o pruebas E2E nuevas. La revisión de diferencias no identificó una necesidad de cambiar código para describir los contratos observados. No es una auditoría exhaustiva de seguridad ni una certificación de otros ambientes.

| Tarea / origen | Diferencia corregida | Comprobación documental |
|---|---|---|
| 1.1 / D-01, RQ-01 | Inventario trataba archivos existentes como nuevos propuestos. | [Inventario](../Diseno/design.md): 72 rutas exactas comprobadas; propietarios conservados; estados reales/locales remiten a matriz final. Ruta de prueba cliente corregida al archivo existente. |
| 1.2 / D-02, RQ-02 | Modelo citaba SQL 01 sin secuencia incremental y omitía detalles físicos. | [Modelo, sección 6](../Diseno/01-modelo-datos.md): SQL 03 más activación 04, NULL físico de lote.ambito_integridad e índices de versión/recurso/intento; 126 nombres de columnas cotejados en nueve entidades. No se aplicó SQL. |
| 1.3 / D-03, RQ-03 | Confirmación conceptual sin nombres wire ni salida exacta. | [Contratos, sección 5](../Diseno/02-contratos-operacion.md): ejemplo con planId/revisionEsperada/planHashEsperado/claveConfirmacion; resultado sin mapas inexistentes; recuperación por consulta y correlación paginada. |
| 1.4 / D-04, RQ-04 | Transporte/sobre/errores se presentaban como propuesta. | Contratos sección 1: RespuestaPerfilImportacion, incidencias tipadas, guard operativo separado, JSON/multipart y mapeo exacto de EstadoHttp. Sin valores de tokens o sesión. |
| 1.5 / D-05, RQ-05 | Snapshot/diferidos repartidos entre cortes. | Contratos sección 4: referencias únicas a 05/06, PerfilImportacionJson y .Valores; 262144 caracteres/profundidad 16 no confundidos con cuotas de upload; checkpoint vacío fijado no se recalcula. |
| 1.6 / D-06, RQ-06 | Operaciones conceptuales distintas de la fachada. | Contratos sección 2: doce firmas ASMX, DTO y 39 propiedades directas de solicitud cotejadas, más loteId heredado; handler separado. Detención no recibe clave adicional; no hay ListarLotes ni endpoint de adaptador. |
| 1.7 / D-07, RQ-07 | Coordinación/agregación/reconciliación aún figuraban sin implementar. | [Estados, sección 2](../Diseno/03-estados-recuperacion.md): corte histórico delimitado y parciales vigentes; tabla cerrada preservada íntegra. |
| 1.8 / D-08, RQ-08 | Barreras sin enlace al resultado posterior. | Estados sección 3: evidencia por EXPEDIENTE, LOTE y PARCIAL, con límites explícitos de driver/UI y carreras locales. |
| 1.9 / D-09, RQ-09 | Almacenamiento seguía como candidato no aprobado. | Estados sección 8: adaptadores caracterizados, copia consumible/original retenido, nombre/índices, Archivo Gestión por nombre y límites de incertidumbre; almacenamiento y SII intactos. |
| 1.10 / D-10, RQ-10 | Reconciliación conceptual por recurso/motivo podía interpretarse como ejecución. | Contratos sección 9: loteId/intentoId/revisionEsperada, verificador sin escritor y cierre sin inicio; recuperación del lote 2 enlazada con su alcance original. |
| 1.11 / D-11, RQ-11 | Pendientes técnicos históricos bloqueaban capacidades ya aceptadas. | [Decisiones](../Diseno/04-decisiones-y-pendientes.md): estados P02/P03/P06/P07/P08/P09 delimitados; P04/P05 siguen abiertos con responsable por rol y efecto. Decisiones del usuario sin cambios. |
| 1.12 / D-12, RQ-12 | Faltaba revisión del delta documental. | Este acta, validación estricta, enlaces y conservación de fuentes; ninguna prueba real nueva atribuida a DOC-112. |

## Recorrido de coherencia en papel

La revisión siguiente confronta texto, DTO y evidencia heredada; no envía solicitudes ni reproduce las aceptaciones REC.

| Caso | Secuencia y resultado descritos por el contrato | Propietario / evidencia |
|---|---|---|
| Dos raíces confirmadas; primera recibida, segunda con contenido pendiente; EXPEDIENTE | La barrera admite crear la primera raíz; la segunda conserva su bloqueo y todos sus elementos. Cada fase CREAR_EXPEDIENTE → MATERIALIZAR_VALORES → ALMACENAR requiere petición explícita y revisión actual. | 07-A persiste, 06 prepara, 07-B confirma/recibe y 10 coordina; estados sección 3 y REC EXPEDIENTE. |
| Mismo inventario incompleto; LOTE | La barrera global no permite crear ninguna raíz. En preflight un error impeditivo global tampoco se convierte en confirmación válida. Consultar no avanza ni cambia el inventario. | Mismo coordinador/repositorio; REC LOTE. |
| Dos PDF elegibles y una entrada .tif rechazada; PARCIAL/NO_APLICA | Se conservan tres originales lógicos, avanzan los dos elegibles y el rechazado no se recibe como PDF ni desaparece de los totales. No se crea una raíz vacía sin elegibles. | 06/07/10; REC PARCIAL. |
| ID futuro y checkpoint efectivo vacío | PREFLIGHT conserva dependencia tipada sin crear; TRAS_EXPEDIENTE resuelve desde identidad comprobada. Un checkpoint fijado, incluso vacío, se reutiliza sin consultar otra vez proveedor. | Resolvedor DOC-110 y coordinador .Valores; evidencia local/real delimitada en matriz RQ-04. |
| Confirmación repetida o respuesta perdida | Una repetición compatible conserva lote/resultado; clave/payload incompatible o revisión obsoleta produce conflicto según la ruta. Si se desconoce el resultado se consulta planId antes de repetir; las identidades por cliente se recuperan paginadas. | Repositorio 07-A y servicio 07-B; contratos sección 5 y matriz RQ-05. |
| Caída después de un posible efecto y reserva nunca iniciada | Lo primero conserva incertidumbre hasta prueba válida, sin replay por timeout. Lo segundo admite solo cierre ALMACENAR sin inicio bajo el bloqueo común, sin liberar otra reclamación ni invocar writer. | Coordinación/verificador 10; recuperación real del lote 2 y carreras locales, matriz RQ-09. |
| Perfil retirado o permiso perdido | Snapshot confirmado no se sustituye por otro borrador/publicación; continuidad conserva facultades vigentes. Retirada para nuevas preparaciones y detención son decisiones diferentes. | Contrato 06 y autoridad por fase existente. |

## Comprobaciones y evidencia

- `openspec.cmd validate doc-112-produccion-importacion-carga-carpeta-contrato --strict`: PASS.
- Comprobación documental local: 72 rutas de inventario, 21 enlaces relativos de los cinco documentos, doce operaciones y 39 propiedades directas de solicitudes, 126 nombres de columnas en nueve entidades: PASS. Los nombres de columnas no sustituyen la revisión de tipos/nulabilidad/índices de la tabla de diferencias del modelo.
- Tabla cerrada de transiciones y sección de decisiones confirmadas del usuario: iguales a las copias previas de esta corrección.
- 112 archivos de código, DTO/modelos, configuración y SQL del ámbito protegido: hashes sin cambios durante DOC-112.
- Auditoría OPSXJ: trazabilidad y estructura correctas; la compuerta editorial permanece abierta hasta revisión del responsable. No se cambia a approved ni se simula aceptación.
- E2E, login, consultas autenticadas, carga, DDL y MSBuild: NO_APLICA / no ejecutados en DOC-112. No se consultó ningún valor privado del ambiente.

Las aceptaciones reales proceden de la [matriz final DOC-111](doc111-matriz-final-20261102.md): [EXPEDIENTE](doc111-rec-expediente-pass-20261101.md), [LOTE](doc111-rec-lote-pass-20261102.md), [PARCIAL](doc111-rec-parcial-pass-20261102.md), [degradación/carga individual](doc111-degradacion-individual-pass-20261102.md) y [verificación del lote 2](doc111-lote2-verificacion-pass-20261102.md). Cada reporte conserva sus fechas, recursos y huellas. Los REC combinan operaciones del driver y reapertura UI; no equivalen a selección manual completa en todos los navegadores. Revocación real, limpieza productiva y SII real no se acreditan por esos resultados.

## Condiciones que permanecen abiertas

P04: operación/usuario define volumen, tamaños objetivo, navegadores y presupuestos antes de anunciar garantías productivas. P05: operación define retención/facultad antes de habilitar borrado; capacidad interna de limpieza no implica endpoint/job activo y los recursos aprobados siguen conservados. La ausencia de correlación inequívoca mantiene el resultado incierto y prohíbe replay del escritor. No se reabre el aislamiento XML descartado ni se amplía PDF a TIFF.

La revisión técnica de las correcciones queda registrada. La aprobación editorial final, publicación, archivo OpenSpec y cierre Jira son pasos posteriores; no se declaran realizados por este acta.

Verificación posterior de entrega: [completitud, corrección y coherencia](doc112-verificacion-final-20261102.md). Se revisaron 12 requisitos y 24 escenarios documentales, se completó el dossier OPSXJ como índice de fuentes y se conservó la separación entre verificación técnica y aceptación editorial.

## Aceptación editorial posterior

El 2026-11-02 el usuario indicó «aprobado» ante la solicitud de aprobar la entrega documental para publicación y cierre. El estado de revisión abierta mencionado arriba pertenece al corte previo a esa respuesta. Aprobación vinculada al reporte revisado SHA256 `a7ec370f0a8a71eaef4aef1b59654c8311d80423f09565999f22743916208db3`. Este registro autoriza continuar el flujo de publicación/cierre; no afirma que esos pasos ya hayan ocurrido ni agrega pruebas funcionales.
