# DOC-112 — Contraste y reducción de alcance

Fecha: 2026-11-02. Contraste previo de archivos locales, sin conexiones ni ejecución del producto. Esta matriz conserva las diferencias que justificaron el plan reducido. Las doce correcciones se aplicaron posteriormente por instrucción del usuario; el [acta de revisión](../../../../Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc112-contratos-revision.md) registra el resultado y sus límites. «Corrección pendiente» en cada entrada identifica el trabajo definido en este contraste previo, no una capacidad funcional todavía inexistente.

## Criterio arquitectónico

Se reutiliza la implementación de DOC-110/111 y la evidencia final con su alcance original. Una propuesta histórica no equivale a una brecha funcional actual. DOC-112 corrige contradicciones y referencias documentales; no abre desarrollo por inferencia. Los doce D/RQ permanecen como criterios de aceptación, cubiertos mediante reutilización más diferencias concretas.

La comprobación física se limita a scripts/repositorios versionados. No certifica otros ambientes. En el cotejo de contratos se han confirmado las firmas públicas y los campos de confirmación; el cotejo campo a campo restante es parte de las tareas, no una auditoría ya completada.

## G-01 — D-01 / RQ-01

- **Reutilizar:** Modelos, DTO, servicios, repositorio y adaptadores ya existen; la matriz final acredita tres modos REC.
- **Diferencia observada:** Diseno/design.md: encabezado y filas Nuevo propuesto; mantiene main 6313f91f y aceptación real pendiente.
- **Fuente de contraste:** InventarioImportacion.js; ImportacionCarpetasDtos.vb; doc111-matriz-final-20261102.md.
- **Tareas anteriores absorbidas:** 1.1, 1.3.
- **Corrección pendiente:** 1.1; Corregir el inventario que todavía clasifica rutas implementadas como nuevas propuestas.

## G-02 — D-02 / RQ-02

- **Reutilizar:** El diccionario ya describe las nueve entidades principales, tipos, claves y relaciones; SQL 02 y migración 03 están versionados.
- **Diferencia observada:** Diseno/01: sección 2 solo remite SQL 01 sin migración 03; sección 5 aún presenta persistencia y ejecución como pendientes.
- **Fuente de contraste:** Sql/02-carga-carpetas-mysql51.sql; Sql/03-migrar-identificadores-perfil-mysql51.sql; MySqlPerfilImportacionRepository.vb.
- **Tareas anteriores absorbidas:** 2.1–2.10.
- **Corrección pendiente:** 1.2; Corregir exclusivamente las diferencias del diccionario frente a SQL 02 y migración 03.

## G-03 — D-03 / RQ-03

- **Reutilizar:** Identidad estable, SHA-256, comparación e idempotencia están implementados y documentados en preparación/confirmación.
- **Diferencia observada:** Diseno/02: tabla conceptual habla de revisión/huella genéricas; se debe vincular el ejemplo wire al DTO exacto.
- **Fuente de contraste:** DTOs/ProduccionDocumental/ImportacionCarpetas/ImportacionCarpetasDtos.vb: ConfirmarPlanImportacionSolicitud; CanonizadorManifiestoImportacion.vb.
- **Tareas anteriores absorbidas:** 3.1–3.3, 6.3.
- **Corrección pendiente:** 1.3; Alinear el ejemplo de confirmación y repetición con los nombres exactos del DTO vigente.

## G-04 — D-04 / RQ-04

- **Reutilizar:** Separación administración/importación, identidad servidor y guards JSON/multipart ya implementados; evidencia real y local delimitada.
- **Diferencia observada:** Diseno/02: apertura propone ASMX/ASHX con nombres por fijar y un sobre común ilustrativo, aunque ya existe fachada tipada.
- **Fuente de contraste:** WebServiceImportacionCarpetas.asmx.vb; ImportacionCarpetasRequestGuard.vb; PerfilImportacionDtos.vb; doc111-matriz-final-20261102.md RQ-02.
- **Tareas anteriores absorbidas:** 4.1–4.2.
- **Corrección pendiente:** 1.4; Sustituir el sobre y la descripción de transporte propuestos por el contrato público existente.

## G-05 — D-05 / RQ-05

- **Reutilizar:** Fuentes y etapas ya se documentan en 05/06 y en los cortes del coordinador; DOC-111 verificó índices efectivos.
- **Diferencia observada:** La información de valores/configuración está repartida entre diseño inicial, contratos 05/06 y cortes locales; falta una referencia actual que identifique cuál aplica.
- **Fuente de contraste:** Diseno/05-contrato-fuentes-expediente.md; Diseno/06-coincidencias-disponibilidad.md; CoordinadorImportacionCarpetas.Valores.vb; PerfilImportacionJson.vb.
- **Tareas anteriores absorbidas:** 5.1–5.3, 10.2.
- **Corrección pendiente:** 1.5; Unificar las referencias vigentes de snapshot y valores diferidos sin repetir su diseño.

## G-06 — D-06 / RQ-06

- **Reutilizar:** La fachada publica doce operaciones operativas; recepción usa handler propio y los adaptadores permanecen internos.
- **Diferencia observada:** Diseno/02 sección 2 mantiene nombres conceptuales, omite ConsultarConfirmacionLote/CerrarReservaSinInicio y la entrada de reconciliación no coincide con el DTO.
- **Fuente de contraste:** webservice/WebServiceImportacionCarpetas.asmx.vb; generic_control/ImportacionCarpetasUpload.ashx.vb; ImportacionCarpetasDtos.vb.
- **Tareas anteriores absorbidas:** 1.2, 6.1–6.2, 6.4–6.11.
- **Corrección pendiente:** 1.6; Reemplazar el catálogo conceptual de operaciones por sus equivalencias ASMX y ASHX existentes.

## G-07 — D-07 / RQ-07

- **Reutilizar:** Ya hay seis dimensiones, tabla cerrada de transiciones y consolidación implementada; no hace falta definir otros estados.
- **Diferencia observada:** Diseno/03 sección 2 todavía declara que reclamación, ejecución, agregación y reconciliación no están implementadas.
- **Fuente de contraste:** CoordinadorImportacionCarpetas.vb; CoordinadorImportacionCarpetas.Consolidacion.vb; doc111-matriz-final-20261102.md RQ-08/09.
- **Tareas anteriores absorbidas:** 7.1–7.2.
- **Corrección pendiente:** 1.7; Resolver las afirmaciones históricas contradictorias sobre coordinación y completitud.

## G-08 — D-08 / RQ-08

- **Reutilizar:** Las tres políticas y exclusión ya están implementadas; aceptaciones reales sesiones 18, 22 y 25.
- **Diferencia observada:** Diseno/03 conserva estado de diseño propuesto y cortes anteriores a las aceptaciones; falta enlazar el resultado posterior a cada barrera.
- **Fuente de contraste:** CoordinadorImportacionCarpetas.Barreras.vb; Validacion/doc111-rec-expediente-pass-20261101.md; doc111-rec-lote-pass-20261102.md; doc111-rec-parcial-pass-20261102.md.
- **Tareas anteriores absorbidas:** 7.3–7.6, 10.1.
- **Corrección pendiente:** 1.8; Vincular las barreras existentes con sus tres aceptaciones y límites de evidencia.

## G-09 — D-09 / RQ-09

- **Reutilizar:** Creación y almacenamiento/incorporación ya ejercidos en los REC; función legacy conservada.
- **Diferencia observada:** Diseno/03 sección 8 declara almacenamiento candidato no aprobado y posible bloqueo de integración, anterior a las aceptaciones reales.
- **Fuente de contraste:** AdaptadorExpedienteImportacion.Legacy.vb; AdaptadorDocumentoImportacion.vb; ComandoAlmacenamientoCarpetas.vb; doc111-matriz-final-20261102.md RQ-07.
- **Tareas anteriores absorbidas:** 8.1–8.2.
- **Corrección pendiente:** 1.9; Actualizar la condición de aceptación de adaptadores ya caracterizados.

## G-10 — D-10 / RQ-10

- **Reutilizar:** Matriz de fallos, reconciliación y cierre sin inicio ya existen; recuperación real del lote 2 y carreras locales documentadas.
- **Diferencia observada:** Diseno/02 tabla inicial usa recursoId/motivo y permite completar fase pendiente bajo autorización sin delimitar el contrato verificador; cortes posteriores sí lo precisan.
- **Fuente de contraste:** ReconciliarRecursoImportacionSolicitud; CoordinadorImportacionCarpetas.Reconciliacion.vb; MySqlImportacionCarpetasRepository.Reconciliacion.vb; doc111-matriz-final-20261102.md RQ-09.
- **Tareas anteriores absorbidas:** 8.3–8.4, 10.3–10.4.
- **Corrección pendiente:** 1.10; Corregir el alcance de reconciliación pública y enlazar la recuperación realmente ejercida.

## G-11 — D-11 / RQ-11

- **Reutilizar:** Disponibilidad/snapshot, límites configurables y limpieza interna están implementados; volumen, navegadores y retención productiva siguen sin acreditación completa.
- **Diferencia observada:** Diseno/04 conserva P02/P06/P08/P09 como impedimentos generales, T01 fija nueve tablas globales y secuencia describe implementación todavía futura.
- **Fuente de contraste:** Diseno/04 secciones 2/3/5; Diseno/06-coincidencias-disponibilidad.md; LimitesImportacionCarpetas.vb; ServicioConsultaImportacion.Limpieza.vb.
- **Tareas anteriores absorbidas:** 9.1–9.3.
- **Corrección pendiente:** 1.11; Reclasificar los pendientes históricos frente a los límites operativos que siguen abiertos.

## G-12 — D-12 / RQ-12

- **Reutilizar:** La matriz final DOC-111 ya reúne dos raíces/tres modos, valores, fallos y huellas; sus recorridos no necesitan reproducirse para redactar el contrato.
- **Diferencia observada:** Falta acta DOC-112 que confirme solo las diferencias corregidas y mantenga la procedencia de evidencia; se reemplazan siete recorridos/revisiones separados por una revisión de delta.
- **Fuente de contraste:** Validacion/doc111-matriz-final-20261102.md; openspec/changes/doc-112-produccion-importacion-carga-carpeta-contrato/tasks.md.
- **Tareas anteriores absorbidas:** 10.5–10.7.
- **Corrección pendiente:** 1.12; Registrar la revisión de las diferencias documentales y su trazabilidad final.

## Tratamiento de lo ya resuelto y de las restricciones

- Las definiciones coincidentes de claves/tipos, autoridad, fuentes diferidas, transiciones, barreras y fallos se conservan y enlazan; no se generan copias paralelas.
- Los REC aprobados de EXPEDIENTE, LOTE y PARCIAL, recuperación del lote 2 y degradación/carga individual proceden de DOC-111. No son ejecución de DOC-112.
- La cobertura real no incluye todo el recorrido manual de selección de carpetas, revocación real de permisos, SII real o limpieza productiva; la matriz final distingue estos límites. No se abre por ello una campaña E2E en este ticket documental.
- Volumen/navegadores/presupuestos y retención operativa conservan responsables por rol y consecuencias concretas. Su falta no obliga a rehacer componentes ya aceptados ni autoriza inventar garantías.
- Almacenamiento legacy y SII permanecen intactos; no se reabre el aislamiento XML descartado.

## Correspondencia completa

Las 52 tareas se distribuyen sin pendientes ocultos: 1.1/1.3→1.1; 1.2→1.6; 2.1–2.10→1.2; 3.1–3.3/6.3→1.3; 4.1–4.2→1.4; 5.1–5.3/10.2→1.5; 6.1–6.2/6.4–6.11→1.6; 7.1–7.2→1.7; 7.3–7.6/10.1→1.8; 8.1–8.2→1.9; 8.3–8.4/10.3–10.4→1.10; 9.1–9.3→1.11; 10.5–10.7→1.12. La absorción combina referencia a lo resuelto y corrección puntual; no acredita ejecución del nuevo checkbox.
