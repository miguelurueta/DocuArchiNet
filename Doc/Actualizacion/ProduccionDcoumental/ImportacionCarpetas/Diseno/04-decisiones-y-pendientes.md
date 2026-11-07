# Diseño 04 — Decisiones, pendientes y secuencia

Estado vigente, revisión DOC-112 del 2026-11-02: implementación y aceptaciones DOC-110/111 disponibles; [matriz final](../Validacion/doc111-matriz-final-20261102.md). Se distinguen condiciones técnicas históricas resueltas en ese alcance de las restricciones operativas aún abiertas. Ambos ámbitos fueron aprobados y ejercidos. La revisión no aplica SQL, cambia permisos ni ejecuta pruebas reales. Las decisiones del usuario de sección 1 se conservan.

## 1. Decisiones confirmadas del usuario

Confirmación de contexto: los permisos de Producción se administran en otra aplicación fuera del repositorio. DOC-110 entrega el campo y su consumo autorizado; la asignación pertenece a esa aplicación. Contrato y aceptación conjunta registrados en OpenSpec DOC-110, sin nuevo administrador local.

| ID | Decisión | Consecuencia |
|---|---|---|
| D01 | SII no se modifica. | Adaptadores nuevos y tablas propias; prohibida generalización del recorrido SII. |
| D02 | Datos de clasificación desde tabla interna. | Perfil independiente de servicios externos y de identidad autorregistro. |
| D03 | Índices pueden proceder del nombre del archivo, de forma configurable. | Resolvedor declarativo versionado, sin formato de orden de trabajo fijo. |
| D04 | Producción ya tiene navegación/creación de estructuras. | Abrir ventana desde nivel seleccionado, destino solo lectura y sin crear otro árbol. |
| D05 | Cada carpeta debe producir expediente en la estructura específica. | Vincular expediente al nivel de apertura y verificar relación; no confundir nivel con gabinete. |
| D06 | El equipo aplica tablas/cambios. | Scripts MySQL 5.1 entregados para ejecución manual; sin migraciones al iniciar. |
| D07 | Todos los archivos deben pasar; debe ser configurable. | Completo predeterminado, parcial explícito y ambos ámbitos aprobados: EXPEDIENTE o LOTE seleccionables al publicar. Política fijada en el lote. |
| D08 | Base gráfica del flujo corregido aceptada. | Reutilizar interacción del prototipo, sustituir simulaciones; aceptación visual no prueba servicios. |

## 2. Decisiones técnicas y procedencia

Las propuestas técnicas originales se contrastan aquí con la implementación integrada. No se atribuyen como instrucciones históricas del usuario; el estado vigente y sus restricciones se apoyan en código y evidencia identificada.

| ID | Propuesta | Motivo / límite |
|---|---|---|
| T01 | Nueve entidades principales de perfil y operación, sin fijar cantidad global. | Tres de configuración y seis operativas; tablas de proveedor/fuente, mapas y respaldos de DOC-110 se referencian sin duplicar catálogos. FK propias compatibles, sin modificar SII. |
| T02 | INT UNSIGNED AUTO_INCREMENT como PK nueva, revisiones CAS y claves únicas explícitas. | Generación por MySQL y referencias INT UNSIGNED; transporte decimal canónico como String. No garantiza exactamente una vez en legacy. |
| T03 | Documentos JSON escapados ASCII en LONGTEXT con esquemas cerrados. | MySQL 5.1 sin JSON nativo; conservar snapshots/Unicode sin duplicar catálogos. Límites/validadores son obligatorios. |
| T04 | Referencias legacy lógicas canónicas implementadas. | Contexto servidor y verificadores por IDs/relaciones; la evidencia del ambiente de pruebas no garantiza igualdad de esquema/collation en otros ambientes. |
| T05 | Preparación completa por request acotado inicialmente. | Evitar protocolo de fragmentos incompleto. Volumen objetivo podría exigir extensión contractual antes de implementar, no un aumento global improvisado. |
| T06 | Plan inmutable por preparación; cambio de contenido crea otro plan. | Revisión clara y confirmación única por plan; simplifica auditoría y evita mutar un lote. |
| T07 | ASMX de control y ASHX de recepción implementados. | WebServiceImportacionCarpetas e ImportacionCarpetasUpload; fachada/DTO vigentes en 02, adaptadores internos sin endpoints. |
| T08 | Ejecución acotada impulsada por peticiones; worker separado opcional. | Compatibilidad inicial con operación web; no prometer continuidad con navegador cerrado. |
| T09 | Staging retenido más copia de trabajo consumible. | Legacy observado borra archivos; recuperación no puede depender del único archivo que consume. |
| T10 | Fuentes diferidas para IDs/consecutivos del expediente. | No crear expedientes durante preview ni inventar valores. |
| T11 — RESUELTO | Desactivar bloquea nuevas selecciones/confirmaciones; lotes confirmados continúan con snapshot y permisos vigentes. | Aprobado 2026-10-20. Detención explícita y pérdida de permiso se tratan separadamente; ver contrato 06. |

## 3. Pendientes y condiciones de salida

| ID | Pendiente | Quién / entrega | Qué bloquea | Qué puede avanzar |
|---|---|---|---|---|
| P01 — RESUELTO | El usuario aprobó soportar ambos ámbitos configurables en el perfil. | Decisión del usuario; contratos 02/04/06/10 | Ya no bloquea el diseño de la barrera. | Ambas ramas implementadas y ejercidas; mantener elección explícita al publicar en modo completo. |
| P02 — IMPLEMENTADO, alcance delimitado | Contexto RA, identidad/empresa/nivel y separación administrar/importar resueltos en adaptadores y guards. | DOC-110/111; administración externa conserva asignación de permisos. | No bloquea repetir diseño; otro ambiente requiere sus permisos/esquema. | Reutilizar contexto y contratos; revocación real no ejercida en los REC, cobertura local en matriz RQ-02. |
| P03 — CARACTERIZADO, límites vigentes | Fuentes 05/06 y adaptador 08 implementados; creación/relación comprobadas en REC. | Perfil 02, adaptador 08; matriz RQ-04/07. | Una configuración o modo de reutilización no ejercido requiere su propia evidencia; no inferir aceptación universal. | Conservar reglas y pruebas existentes, sin rediseñar la identidad ni repetir la decisión de coincidencias. |
| P04 — ABIERTO OPERATIVO | Volumen/tamaños objetivo, matriz de navegadores y presupuestos de tiempo sin acreditación productiva completa. | Operación/usuario; contratos 04 y validación 12. | Capacidad anunciada, cuotas definitivas y eventual extensión de fragmentación. | Límites configurables de perfil/transporte/JSON/sesión y errores vigentes; no asumir 500 archivos/25 MB ni todos los navegadores. |
| P05 — ABIERTO OPERATIVO | Retención productiva de originales/intentos/historial y habilitación de mantenimiento. | Operación; capacidad interna 11. | Activar borrado automático o limpieza real sin política/facultad válidas. | Consulta/diagnóstico en seco; limpieza interna probada localmente permanece deshabilitada, sin endpoint/job. Recursos DOC-111 conservados. |
| P06 — CARACTERIZADO, alcance aceptado | Contexto temporal, original retenido, copia consumible, nombre/índices y Archivo Gestión caracterizados; efectos reales en REC. | Adaptadores 08/09; matriz RQ-07. | No bloquea la integración PDF aceptada; nuevos formatos/escritores requieren caracterización. | Reutilizar adaptadores; función legacy/SII intactos. Exclusión XML adicional descartada por usuario, sin reabrirla. |
| P07 — RECUPERACIÓN LIMITADA EXPLÍCITA | Verificador de evidencia y cierre sin inicio implementados; recuperación real lote 2 e invariantes/carreras locales. | Coordinador 10; matriz RQ-09. | Replay automático de efectos inciertos sin prueba inequívoca sigue prohibido. | Consultar, reconciliar con evidencia válida o conservar INCIERTO; no inventar correlación externa ni usar antigüedad. |
| P08 — IMPLEMENTADO Y COMPROBADO POR CAPACIDAD | ASMX/ASHX, CSRF, sesión y degradación con carga individual preservada. | Entradas 06/07/10/11 y evidencia 12. | No certifica toda combinación IIS/permiso/navegador en otro ambiente. | Reutilizar mapeo real de EstadoHttp y matriz final; distinguir autenticación/operaciones reales de casos locales. |
| P09 — EJERCIDO EN AMBIENTE AUTORIZADO | SQL manual, cuentas y recursos de prueba autorizados; tres modos REC y degradación/carga individual registrados. | Equipo del usuario; matriz final DOC-111. | Otra ejecución requiere su autorización aplicable; la revisión DOC-112 no la necesita ni la realiza. | Reutilizar reportes saneados, fechas/huellas y alcance; no almacenar credenciales ni declarar otro ambiente habilitado. |

P01 se cerró por aprobación explícita. Los demás estados anteriores se actualizan por evidencia, no por esa aprobación: implementación/aceptaciones delimitadas en DOC-111; P04/P05 siguen abiertos con responsables y efectos concretos. La revisión editorial DOC-112 no sustituye aceptación productiva universal.

## 4. Evidencia inicial del repositorio — antecedente histórico

| Evidencia | Conclusión permitida | Conclusión no demostrada |
|---|---|---|
| Metadatos guardados: MySQL 5.1.37, collations mixtas, claves de gabinete/campo/expediente. | Diseñar con compatibilidad 5.1 y referencias reales. | Todos los ambientes usan el mismo esquema o tablas nuevas ya existen. |
| `WebServiceProducion.asmx.vb`: ScriptService/WebMethod con sesión. | ASMX es compatible con el recorrido actual. | Cualquier nueva mutación ya está protegida o autorizada. |
| `Registrar_Expediente_Conservacion`: recibe nivel y produce relaciones/efectos. | Candidato de creación mediante adaptador. | Idempotencia externa y recuperación de crash garantizadas. |
| `AlmacenamientoDocumentoProduccionDocumental`: campos índice explícitos, Session, File.Name y eliminación de archivos. | Reutilización requiere aislamiento y copias. | Método listo para worker o seguro con staging único/opaco. |
| Pruebas DOC-88: 14 aprobadas en auditoría anterior. | Línea base estática del código actual. | Integración futura sin regresiones. No se repitieron en esta entrega de diseño sin cambios de código. |

## 5. Secuencia técnica de las entregas ya integradas

La lista conserva el orden de responsabilidad original, no un backlog por ejecutar otra vez. 07-A → 06 → 07-B ya está integrado; 08/09/10/11/12 tienen evidencia en la matriz final. P04/P05 conservan sus límites operativos y 13 sigue opcional.

1. Aplicar P01 resuelto —ambos ámbitos configurables— y revisar las demás decisiones de contrato que condicionen publicación/ejecución. No repetir exploración del módulo desde cero.
2. Implementar 02 conforme a modelo de perfil/reglas; entregar SQL 01 manual y resolvedor puro probado.
3. 07-A: cerrar mapeos legacy, DDL/repositorios de preparación y operación; entregar SQL 02, sin recepción todavía.
4. 06: apertura autorizada y plan con reglas/valores diferidos/elegibilidad. 05 puede construir interfaz contra esos contratos.
5. 07-B: confirmación idempotente y recepción con staging retenido.
6. 08/09: caracterizar y aceptar adaptadores antes de habilitar efectos; resultado incierto explícito cuando no exista evidencia.
7. 10: coordinación y barrera de integridad según decisión; pruebas de fallos y reclamaciones.
8. Integración final 05/11: consultas, reanudación, árbol, CSV y mantenimiento con política.
9. 12: compilación, contratos y pruebas reales autorizadas. Ausencia de tablas nuevas no rompe carga individual ni Producción.
10. 13 solo si se solicita esa evolución y hay host/adaptadores adecuados; no incluido por terminar lo anterior.

## 6. Criterios de revisión del diseño

Este diseño documenta nombres/propietarios/tipos/contratos comunes implementados. La instalación en otro ambiente exige sus controles de tamaños/límites/collations y procedimientos manuales; P02 no sigue siendo un bloqueo general de la implementación aceptada. El conjunto no modifica catálogos ni esquemas SII y no propone refactorizar el almacenamiento compartido.

Revisar los cuatro documentos en conjunto: el modelo de datos no define una política distinta de estados; las acciones disponibles se derivan del servicio, no de la UI. Toda modificación de contrato actualiza su consumidor y matriz de pruebas. Una decisión de negocio pendiente queda marcada y no se resuelve mediante código silencioso.
