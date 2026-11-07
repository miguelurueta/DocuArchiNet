# Coincidencias y disponibilidad — decisiones aprobadas

El usuario aprobó estas dos políticas el 2026-10-20. Esta aprobación no equivale a aprobar el refinamiento completo ni ejecutar implementación, DDL o pruebas reales.

## Disponibilidad del perfil

Desactivar impide nuevas selecciones y nuevas confirmaciones de importación. Los lotes ya confirmados conservan su snapshot y pueden continuar; no se suspenden por desactivación del perfil. Publicar otra versión tampoco los cambia. Para detener un lote se usa su acción explícita autorizada, en un punto seguro, conservando efectos confirmados.

La pérdida de autorización sí impide nuevos efectos afectados. No confundir perfil inactivo con permisos revocados. Consulta y reconciliación exigen autorización vigente; ninguna rama concede acceso por tener un lote anterior.

| Situación | Comportamiento |
|---|---|
| Perfil inactivo, plan aún no confirmado | Rechazar confirmación; no crear lote ejecutable. |
| Perfil desactivado después de confirmar | Lote sigue con snapshot original y permisos vigentes. |
| Confirmación y desactivación concurrentes | Ordenar ambas mediante control transaccional de la cabecera: si desactivar se confirma primero, rechazar nueva confirmación; si el lote se confirma primero, se considera previo. |
| Repetición de confirmación ya persistida | Recuperar el mismo lote autorizado; no tratar el reintento como confirmación nueva ni crear otro lote. |
| Detención explícita o pérdida de permiso | Detener nuevos efectos en punto seguro y conservar resultados; no prometer rollback global. |
| Reactivación | Permitir nuevas selecciones/confirmaciones tras validaciones vigentes; no sustituir versiones ni reanudar automáticamente lotes detenidos. |

DOC-110 administra disponibilidad e historial. Preparación/confirmación y coordinador implementan las consecuencias de ejecución en sus entregas; no añadir control de lotes al servicio del perfil.

## Coincidencias de expediente

La versión del perfil conserva una política explícita: `BLOQUEAR` por defecto o `REUTILIZAR_VALIDADO`. El servicio aplica el default al crear configuración nueva, lo persiste y lo muestra; publicación exige valor reconocido, no ausencia ambigua. El plan/lote guarda la política efectiva en su snapshot. No cambia por editar/publicar otro perfil.

| Resultado de búsqueda autorizada | BLOQUEAR | REUTILIZAR_VALIDADO |
|---|---|---|
| Ningún expediente coincidente | Proponer creación conforme a datos y permisos válidos | Igual |
| Un expediente coincidente | Conflicto; no crear duplicado ni incorporar allí | Reutilizar solo si cumple las condiciones de abajo |
| Más de un candidato | Conflicto, sin elegir primero | Conflicto, sin elegir primero |
| Resultado incierto de creación anterior | Reconciliar antes de crear/reintentar | Igual; no resolver incertidumbre mediante coincidencia aproximada |

Condiciones acumulativas de reutilización:

1. Coincidencia única según identidad documental caracterizada; nunca solo nombre de carpeta/expediente.
2. Empresa, estructura destino y clasificación compatibles.
3. Estado que permita incorporación y permisos vigentes.
4. Identidad completa y relación de destino verificadas, sin modificar datos existentes para hacerlos coincidir.

El lector legacy `Verfica_Existencia_Codigo_Unico_Expediente` consulta código, empresa, volumen y área; no demuestra por sí solo identidad suficiente, unicidad física ni autorización de reutilización. El adaptador 08 debe caracterizar la identidad real y su comparación/collation antes de habilitar reutilización operativa. No convertir una consulta previa en garantía contra carreras con otros escritores.

DOC-110 guarda y valida la opción configurada, sin ejecutar búsquedas ni creación durante publicación. Que una opción pueda configurarse no acredita que el adaptador operativo esté implementado. Preparación/ejecución rechazan capacidades no disponibles, sin degradar silenciosamente a otra política.

Reintentar una carpeta del mismo lote recupera primero su relación confirmada. No volver a aplicar BLOQUEAR contra el expediente creado por ese mismo intento como si fuese una nueva importación.

Reutilizar expediente no omite archivos, no concede sobrescritura documental y no resuelve coincidencias de documentos. Las políticas de documentos y la exigencia COMPLETO_OBLIGATORIO/PARCIAL_PERMITIDO permanecen independientes. En modo completo, un conflicto impide continuar conforme a la barrera EXPEDIENTE/LOTE seleccionada; no se transforma en omisión.

## Pruebas exigidas por entrega

- Perfil: default BLOQUEAR visible/persistido; ambas opciones válidas; valor desconocido impide publicar; versiones anteriores inmutables.
- Confirmación: perfil inactivo rechaza lote nuevo; reintento recupera lote ya confirmado; carrera confirmar/desactivar respeta orden confirmado.
- Adaptador: cero/uno/varios candidatos; empresa/nivel/clasificación/estado/permisos incompatibles; reintento propio; resultado incierto; no mutación correctiva de expediente existente.
- Coordinador: desactivar perfil no detiene lote previo; pérdida de permiso o detención explícita sí bloquean nuevos efectos; snapshot y resultados conservados.
- Integración: reutilización no reduce inventario de archivos ni declara completitud sin incorporación confirmada.

Estos son criterios de pruebas futuras. No se ejecutaron como parte de esta aprobación documental.

## Implementación preventiva DOC-111, 2026-10-28

El lector parametrizado del repositorio operativo consulta candidatos por `CODIGO_UNICO`, `ID_EMPRESA_EXPEDIENTE` y `CODIGO_AREA_TRD` (máximo dos filas). El área corresponde a `AreaRef`: el catálogo operativo usa `areas_depart_radicacion.Codigo_Area`, la misma referencia que resuelve Conservación antes de escribir CODIGO_AREA_TRD. No usa el nombre de carpeta como identidad.

La consulta abarca **todos los volúmenes**. Es una restricción conservadora del recorrido: no se inventa un volumen implícito para reproducir un lector legacy cuyo filtro no coincide con el INSERT inspeccionado. Puede exigir revisión para códigos que otra operación permitiría en volúmenes distintos. Tampoco incluye un filtro por nivel o clasificación que pudiera ocultar un conflicto existente. El mensaje no devuelve datos o IDs del candidato.

Antes de la búsqueda se comprueban tipos de columnas y charset/collation de CODIGO_UNICO. La conexión de esta lectura usa UTF-8; se exige conversión de ida/vuelta sin pérdida. En este corte se admiten charsets `latin1` y `utf8`, compatibles con MySQL 5.1; otro origen deja la capacidad indisponible. Los códigos se comparan dentro del inventario mediante expresiones CONVERT/COLLATE con identificadores obtenidos de metadatos y validados; todos los valores usan parámetros. Las correlaciones de raíz usan ascii_bin para conservar distinción de mayúsculas. La comparación, incluidos acentos, mayúsculas y espacios finales, pertenece al motor; las pruebas locales no certifican su resultado en un servidor real.

Con cero candidatos puede proponerse creación bajo cualquiera de las dos políticas. BLOQUEAR rechaza uno o varios candidatos; REUTILIZAR_VALIDADO con un candidato devuelve `REUTILIZACION_NO_DISPONIBLE` y con varios devuelve `COINCIDENCIA_AMBIGUA`. No se ejecuta una reutilización ni se cambia de política silenciosamente. Completar reutilización exige verificar identidad/relación/clasificación/estado/permisos y conservar esa identidad en el plan antes de habilitarla.

Una comprobación fallida no equivale a cero candidatos. El servicio deja incidencia global no confirmable si faltan metadatos o lectura; conserva incidencias y raíces originales. La consulta no reserva identidad, no crea expedientes y no garantiza ausencia de carreras con escritores externos. Confirmar reconsulta para las unidades que el plan permite ejecutar; el adaptador deberá revalidar antes del efecto.

Evidencia de este corte: `Validacion/doc111-plan-confirmacion-20261028.md`. Consultas y composición verificadas con dobles locales explícitos; comparación física, controles reales y reutilización completa pendientes.
