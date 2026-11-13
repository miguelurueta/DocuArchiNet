# DOC-120 — PruebasEvidencia

- Ticket: DOC-120
- Cambio OpenSpec: doc-120-produccion-importacion-carga-carpeta-verificacion
- Clasificacion: cross_cutting

## Corte vigente2026-11-13

Avance22/26. Corrección DOC120-UI06: carga inicial con indicador neutro accesible en el menú, sin franja amarilla ni desplazamiento41px; errores y confirmación sin acuse siguen visibles. Cambian JS/CSS y las URLs ASPX de esos recursos. Regresión focal41/41 en Chromium/cliente local con respuestas sintéticas (28 cliente,9 navegación,4 degradación); sin IIS autenticado ni cambios de negocio. Huellas nuevas: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-apertura-20261113.json. Las afirmaciones15/15/sin delta siguientes corresponden al corte histórico2026-11-12. Se conservan las cuatro tareas abiertas y los límites de capacidad/aceptación.

## Antecedente2026-11-12

## Evidencia requerida

Corte: consolidación local21/25, sin aceptación integral. Índices: Validacion/matriz-verificacion.md y resultados.md. Manifiesto: doc120-consolidacion-20261112.json, con32 casos,25 fuentes originales,11 componentes,15/15 huellas UI coincidentes,114 entradas del arnés y huellas de logs. La inspección inicial doc120-inspeccion-refinamiento-20261112.json se conserva como corte anterior.

Plan local aprobado sobre73106588 y opsxj:refine -- DOC-120 --sync PASS15/15. El mapa activo tiene25 tareas (24 originales más4.4 por reproducción del auxiliar). UI/contratos99/33 de DOC119 se reutilizan por fuentes iguales; no son ejecuciones DOC120.

Regresión nueva:5/5 inicial (2 wrappers Framework/DDL y3 políticas de degradación); reproducción del caso nuevo1fallo por botón oculto antes del arreglo; después4/4 (3 políticas repetidas y1 Chromium nuevo). Son6 Node únicos, no9 ni los casos internos Framework sumados. Logs locales excluidos: .tmp/doc120-backend-degradation-local.log, doc120-degradation-before-fix.log y doc120-degradation-after-fix.log. No se repite Framework después del cambio Node porque sus entradas permanecen intactas.

Se resuelven contradicciones de cortes DOC116/117/118 y estado global/caso de SEG01; no se reescriben JSON primarios. El cierre técnico de un caso no borra incertidumbres41/53 ni convierte laboratorio en runtime ordinario.

## QA/E2E WebForms

Ninguna nueva autenticación, HTTP de aplicación, SELECT real, recepción, creación, almacenamiento, DDL, permiso, fallo real o limpieza en DOC120. La prueba Chromium devuelve503 sintético local. El fichero DLL comparado es un artefacto de disco, no prueba de ensamblado cargado en IIS.

Abiertas5.2 (autorización real),6.4 (aceptación explícita de límites),6.5 (gobierno final) y6.6 (entrega final).5.3 consta NO_EJECUTADO con motivo/responsable/impacto, no como medición aprobada. INT03 sin contrato/autorización no concede garantía de capacidad. SII/limpieza y UI definitiva autenticada conservan su límite; no hay archivo ni Jira Listo por inferencia.
