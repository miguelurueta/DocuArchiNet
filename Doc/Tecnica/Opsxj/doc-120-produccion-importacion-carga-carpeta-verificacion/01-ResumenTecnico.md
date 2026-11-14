# DOC-120 — ResumenTecnico

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

## Barra superior — implementación3.8,2026-11-13

Avance24/28. DOC120-UI08: barra y búsqueda adaptables con acciones/IDs originales, sin cambios de rutas de negocio; baseline funcional36 acciones y regresión60/60 locales con dobles de negocio/postback. Altura ajustada al desplegar, foco y montaje dinámico conservados. CSS/JS aislados, versionados e incluidos como Content; prueba de10 combinaciones de ancho/texto y desplegables largos. Evidencia: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-barra-20261113.json. Sin IIS autenticado ni efectos reales; siguen cuatro tareas abiertas. Los cortes siguientes son antecedentes.

## Menú — corte posterior2026-11-13

DOC120-UI07: iconos de cabecera/opciones y colores coherentes con Archivos, preservando permisos, referencias y carga sin desplazamiento.55/55 pruebas locales con CSS/fuente reales y servicios sintéticos; sin IIS autenticado. Avance23/27, sin aceptación integral. Evidencia: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-menu-20261113.json. Los cortes siguientes son antecedentes.

## Corte vigente2026-11-13

Avance22/26. Corrección DOC120-UI06: carga inicial con indicador neutro accesible en el menú, sin franja amarilla ni desplazamiento41px; errores y confirmación sin acuse siguen visibles. Cambian JS/CSS y las URLs ASPX de esos recursos. Regresión focal41/41 en Chromium/cliente local con respuestas sintéticas (28 cliente,9 navegación,4 degradación); sin IIS autenticado ni cambios de negocio. Huellas nuevas: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-apertura-20261113.json. Las afirmaciones15/15/sin delta siguientes corresponden al corte histórico2026-11-12. Se conservan las cuatro tareas abiertas y los límites de capacidad/aceptación.

## Antecedente2026-11-12

## Objetivo

Consolidar INT-01/02/03 sobre la interfaz entregada por DOC-119 (main8c0773d4), con resultados trazables por componente y alcance real/local/estático. La continuación sobre73106588 aprueba trabajo local.

## Alcance y compatibilidad

Avance21/25: índices matriz-verificacion.md y resultados.md creados,32 casos clasificados,25 primarios y dependencias de11 componentes;15/15 huellas UI y114 entradas del arnés fijadas. Las pruebas UI99/33 históricas se reutilizan por igualdad de huellas/diff. Regresión actual Framework/DDL y políticas/UI:6 Node únicos entre dos cortes, sin sumar casos internos o políticas repetidas.

Única corrección de código: auxiliar E2E de degradación abre el menú moderno; reproducción previa y regresión local. Sin cambios de producto, SII, almacenamiento, esquema, configuración, permisos o limpieza. INT-03 sigue sin definición/autorización y la aceptación final de límites permanece abierta; no se solicita archivo/cierre.
