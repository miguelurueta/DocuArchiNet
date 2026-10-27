# Diseño 04 — Decisiones, pendientes y secuencia

Estado actualizado: diseño y contrato de fuentes documentados; **decisión de ámbitos cerrada por aprobación explícita del usuario**: ambos configurables en el perfil. SQL 01 generado como archivo revisable, no ejecutado. No se modificaron servicios, aplicación, prototipo o SII. P02 tiene ámbito RA y política de permisos definidos en DOC-110; asignación administrativa confirmada en aplicación externa; aceptación conjunta antes de habilitación. Las políticas de coincidencias y disponibilidad fueron aprobadas el 2026-10-20 y se desarrollan en `06-coincidencias-disponibilidad.md`; P03 conserva caracterización técnica del adaptador.

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

## 2. Propuestas técnicas de este diseño

Estas decisiones de arquitectura son propuestas revisables; no se atribuyen como instrucciones previas del usuario.

| ID | Propuesta | Motivo / límite |
|---|---|---|
| T01 | Nueve tablas nuevas con FK solo entre entidades propias. | Tres de configuración y seis de preparación/operación; catálogos legacy referenciados/verificados sin alterar índices ni SII. |
| T02 | INT UNSIGNED AUTO_INCREMENT como PK nueva, revisiones CAS y claves únicas explícitas. | Generación por MySQL y referencias INT UNSIGNED; transporte decimal canónico como String. No garantiza exactamente una vez en legacy. |
| T03 | Documentos JSON escapados ASCII en LONGTEXT con esquemas cerrados. | MySQL 5.1 sin JSON nativo; conservar snapshots/Unicode sin duplicar catálogos. Límites/validadores son obligatorios. |
| T04 | Referencias legacy lógicas canónicas. | Esquemas y claves mixtos; ID de expediente solo no representa toda su identidad física. Mapeo real pendiente antes de DDL. |
| T05 | Preparación completa por request acotado inicialmente. | Evitar protocolo de fragmentos incompleto. Volumen objetivo podría exigir extensión contractual antes de implementar, no un aumento global improvisado. |
| T06 | Plan inmutable por preparación; cambio de contenido crea otro plan. | Revisión clara y confirmación única por plan; simplifica auditoría y evita mutar un lote. |
| T07 | ASMX propio de control y ASHX propio de recepción. | Adaptación a stack existente sin agregar lógica de lote al handler compartido. Nombres finales se ajustan a convenciones del proyecto. |
| T08 | Ejecución acotada impulsada por peticiones; worker separado opcional. | Compatibilidad inicial con operación web; no prometer continuidad con navegador cerrado. |
| T09 | Staging retenido más copia de trabajo consumible. | Legacy observado borra archivos; recuperación no puede depender del único archivo que consume. |
| T10 | Fuentes diferidas para IDs/consecutivos del expediente. | No crear expedientes durante preview ni inventar valores. |
| T11 — RESUELTO | Desactivar bloquea nuevas selecciones/confirmaciones; lotes confirmados continúan con snapshot y permisos vigentes. | Aprobado 2026-10-20. Detención explícita y pérdida de permiso se tratan separadamente; ver contrato 06. |

## 3. Pendientes y condiciones de salida

| ID | Pendiente | Quién / entrega | Qué bloquea | Qué puede avanzar |
|---|---|---|---|---|
| P01 — RESUELTO | El usuario aprobó soportar ambos ámbitos configurables en el perfil. | Decisión del usuario; contratos 02/04/06/10 | Ya no bloquea el diseño de la barrera. | Implementar y probar ambas ramas; elegir explícitamente al publicar en modo completo. |
| P02 | Mapeo efectivo de identidad/ámbito/nivel y permisos de administración/importación. | Investigación 02/06/07-A | SQL/contratos finales de referencias y autorización. | Interfaces y pruebas con identidades sintéticas bien señaladas. |
| P03 — PARCIAL | Fuentes documentadas en 05; BLOQUEAR/default y REUTILIZAR_VALIDADO aprobados en 06. Caracterizar identidad real/comparación y compatibilidad del adaptador. | Perfil 02 y adaptador 08 | Habilitar reutilización/creación operativa sin caracterización. | Persistir/validar opciones del perfil y redactar pruebas; no inventar identidad. |
| P04 | Volumen y tamaños objetivo, límites de preparación/recepción, navegadores y tiempos de operación. | Usuario/operación; 04/07/12 | Capacidad anunciada, cuotas definitivas y necesidad de fragmentación. | Implementar límites configurables y errores de rechazo sin declarar capacidades ficticias. |
| P05 | Retención de staging, intentos e historial y mecanismo de mantenimiento. | Operación; 11 | Borrado automático. | Consulta, listado en seco y limpieza solo bajo política configurada. |
| P06 | Aislamiento de sesión, copias temporales, nombre original y concurrencia externa. | Caracterización 08/09 | Declarar reusable adaptador legacy y ejecutar efectos. | Persistencia, interfaz y contratos sin éxito simulado. |
| P07 | Correlación fiable para crash posterior a creación/almacenamiento. | 08/09/10 | Reintento automático de efectos inciertos. | Estados inciertos, consulta y revisión autorizada con evidencia. |
| P08 | Comportamiento de ASMX/ASHX con CSRF, errores, expiración y despliegue sin esquema. | 05/06/07/12 | Integración productiva segura. | Diseño y pruebas aisladas. |
| P09 | Entorno/cuentas autorizados y aplicación manual de SQL. | Equipo del usuario | Integración real, E2E y carga. | Archivos SQL revisables, tests locales sin autenticación. |

P01 se cerró por aprobación explícita, no por silencio. Se mantienen las demás condiciones técnicas y operativas: esta aprobación no acredita tablas, código ni pruebas integradas.

## 4. Evidencia del repositorio contrastada

| Evidencia | Conclusión permitida | Conclusión no demostrada |
|---|---|---|
| Metadatos guardados: MySQL 5.1.37, collations mixtas, claves de gabinete/campo/expediente. | Diseñar con compatibilidad 5.1 y referencias reales. | Todos los ambientes usan el mismo esquema o tablas nuevas ya existen. |
| `WebServiceProducion.asmx.vb`: ScriptService/WebMethod con sesión. | ASMX es compatible con el recorrido actual. | Cualquier nueva mutación ya está protegida o autorizada. |
| `Registrar_Expediente_Conservacion`: recibe nivel y produce relaciones/efectos. | Candidato de creación mediante adaptador. | Idempotencia externa y recuperación de crash garantizadas. |
| `AlmacenamientoDocumentoProduccionDocumental`: campos índice explícitos, Session, File.Name y eliminación de archivos. | Reutilización requiere aislamiento y copias. | Método listo para worker o seguro con staging único/opaco. |
| Pruebas DOC-88: 14 aprobadas en auditoría anterior. | Línea base estática del código actual. | Integración futura sin regresiones. No se repitieron en esta entrega de diseño sin cambios de código. |

## 5. Secuencia concreta

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

Este diseño permite trabajar con nombres/propietarios/tipos/contratos comunes, pero no debe convertirse directamente en DDL sin cerrar P02 y comprobar tamaños/límites/collations del despliegue. El conjunto no modifica catálogos ni esquemas SII y no propone refactorizar el almacenamiento compartido.

Revisar los cuatro documentos en conjunto: el modelo de datos no define una política distinta de estados; las acciones disponibles se derivan del servicio, no de la UI. Toda modificación de contrato actualiza su consumidor y matriz de pruebas. Una decisión de negocio pendiente queda marcada y no se resuelve mediante código silencioso.
