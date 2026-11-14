# DOC-120 — ContratosIntegracion

## Diagnóstico detenido y corrección del reporte — 2026-11-14

Ventana autorizada64c70664, sobre3010eef7: lote39/carpeta60, STOPPED en el primer intento contabilizado,0 HTTP/0 sesiones y credenciales eliminadas. No hay archivo de resultados; el contador anterior no distingue apertura fallida de SQL ejecutado. La causa del lote y la existencia del expediente siguen desconocidas. El ejecutor descartaba las categorías seguras DOC111_SELECT del puente: corrección local conserva solo la lista cerrada y agrega lectura actual/intentos/respuestas concluidas. **12/12 pruebas locales**, sin consultar datos de negocio ni repetir la corrida real. Evidencia: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-diagnostico-reporte-20261114.json`. El corte79/79 de UI permanece histórico; esta corrección solo cambia el auxiliar. Avance43/48;3.27 sigue abierta. Otra ventana de hasta5 SELECT exige instrucción expresa; se conserva1 intento previo (máximo6 acumulado), sin HTTP ni efectos. Los cortes siguientes conservan su alcance histórico.


## Flujo final tras creación sin confirmación — corte2026-11-14

Video194059: lote39/plan57, carpeta60 y archivo86. La recepción se muestra confirmada, pero la creación deja la carpeta CREANDO y el lote EN_CURSO durante las consultas de la grabación. La columna Expediente pendiente **no demuestra ausencia del expediente real**; no se conoce su resultado sin diagnóstico.

Corrección local3.24/3.25/3.26/3.28: contexto de operación conservado, aviso fijo con referencias, recurso bloqueante priorizado y **Revisar bloqueo** como acción principal local tras una consulta sin confirmación. Actualizar no reanuda; CREANDO no permite reconciliar por timeout. Una verificación válida sin resolución mantiene el bloqueo con explicación. Creación/índices/almacenamiento y fin solo por confirmaciones, sin encadenamiento. **79/79 Node locales**, incluidos2 wrappers Framework/DDL; compilación0 errores/1 advertencia. Evidencia: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-flujo-final-20261114.json.

**3.27 pendiente**: diagnóstico preparado de5 SELECT,0 HTTP,0 efectos; requiere autorización y credenciales efímeras. No ejecutado ni recuperado el lote39; causa interna todavía no comprobada. Avance**43/48**; pendientes3.27,5.2,6.4,6.5,6.6. La UI local aprobada no concede aceptación integral ni cierra este resultado real. Los cortes siguientes permanecen históricos.


## Fidelidad a la maqueta — corte vigente2026-11-13

Corrección solicitada tras2642e604: la reorganización anterior no igualaba el aspecto ni mantenía accesos de consulta dentro de todas las etapas. Tareas3.19–3.23 completadas: barra Consultar importación / Consultar último lote y pie siempre visibles, diseño de controles trasladado de la maqueta, selección de recurso sin efectos y consulta secundaria con preparación conservada. Un idCliente coincidente de otro lote no habilita Files del inventario original. Las operaciones mantienen sus guardas y validaciones.

**116 propiedades visuales contrastadas** y capturas390/768/1280px con CSS real, Bootstrap y servicios sintéticos; **71/71 pruebas Node**, incluidos2 wrappers Framework/DDL (no se suman casos internos), compilación Debug incremental0 errores/1 advertencia. Evidencia: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-fidelidad-maqueta-20261113.json. Avance**39/43**, con5.2,6.4,6.5,6.6 abiertas. No hay nueva sesión IIS autenticada ni aceptación integral. El marco modal y los controles operativos adicionales se conservan; la maqueta sigue siendo una página independiente con datos de ejemplo. Los cortes siguientes son históricos.


## Asistente guiado — corte vigente2026-11-13

Tareas3.10–3.18 completadas: **Seleccionar → Revisar → Seguimiento** sobre el asistente real. Perfil/carpetas juntos; una acción existente destacada con recurso, sin duplicación ni fases automáticas. Configuración/filtros desplegables con borradores y foco; consultas por referencia/último lote/plan pendiente, páginas independientes y CSV conservados. Actualizar estado muestra fecha/contador al completar lecturas válidas; envío muestra cantidad elegible de la página y archivo actual/total de cola, diferenciando transporte, recepción y almacenamiento. C01/C02/C03 de la maqueta corregidas; auditoría anterior y sus límites se conservan como antecedentes.

**Validación local:68/68 Node**, incluidos2 wrappers Framework/DDL cuyos casos internos no se suman; Chromium con servicios sintéticos y compilación Debug incremental0 errores/1 advertencia. Evidencia: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-asistente-guiado-20261113.json. Avance**34/38**; siguen abiertas5.2,6.4,6.5 y6.6. Sin ejecución IIS autenticada nueva ni aceptación integral. Los cortes siguientes mantienen su SHA, conteos y ámbito histórico.


## Corte de recepción — DOC120-REC01,2026-11-13

Tarea3.9: disponibilidad de creación por consulta, reutilizando la barrera de recepción y el ámbito completo, con dos SELECT de metadatos agrupados por página y lectura del plan. Se añade el booleano público recepcionListaParaCrear (false por defecto), sin endpoint ni esquema nuevo; pista interna excluida de JSON persistido. Cliente exige true, explica la recepción y acerca el envío. Sin cambios de creación/almacenamiento/SII/perfil Egresos ni efectos sobre lote31. Regresión local64/64 Node (incluye2 wrappers Framework/DDL) y compilación0 errores. Evidencia: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-recepcion-creacion-20261113.json. Avance25/29; cuatro tareas de autorización/aceptación/gobierno/entrega abiertas. Sin nueva prueba IIS autenticada; los apartados siguientes son cortes históricos y sus afirmaciones sin delta/DTO no describen esta corrección.

- Ticket: DOC-120
- Cambio OpenSpec: doc-120-produccion-importacion-carga-carpeta-verificacion
- Clasificacion: cross_cutting

## Contratos e integraciones

Sin DTO, endpoint, ASMX/ASHX, esquema o coordinador nuevos. Los índices relacionan requisito/caso/componente/prueba/código/ambiente/evidencia/resultado/vigencia/límite. Los primarios originales permanecen intactos, incluidos STOPPED, NO_ACREDITADO y estados de laboratorio.

El módulo interno E2E de degradación extrae y exporta checkUnavailableUI para regresión aislada; el runner usa la misma función y ahora abre el menú moderno antes del click. No es un endpoint de aplicación ni habilita fallos/producto. Controla únicamente503 y aviso existente; no amplía perfil, presupuesto o efectos autorizados.

INT-03 tiene contrato con campos/decisiones explícitos en resultados.md; faltan objetivo/umbrales/corpus/ambiente/recursos/presupuesto y autorización propia. La solicitud al responsable no equivale a aceptación ni ejecución.
