-- DOC-67 - Seguimiento manual de la E2E ESAL.
-- Tarea: 220572. Radicado: S002194198. Gabinete: ESAL.
-- Compatible con MySQL 5.1. Todas las sentencias son exclusivamente SELECT.
-- No ejecutar como un único resultset: cada bloque produce una evidencia distinta.

-- 1. Cabecera moderna más reciente de la tarea.
SELECT i.intent_id, i.task_id, i.procedure_id, i.provider_id, i.radicado,
       i.status, i.version_token, i.created_utc, i.updated_utc
FROM workflowdocument.workflow_import_intent i
WHERE i.task_id = 220572
ORDER BY i.created_utc DESC
LIMIT 1;

-- 2. Requisitos del preflight.
SELECT r.intent_id, r.requirement_code, r.is_satisfied, r.visible_message
FROM workflowdocument.workflow_import_intent_requirement r
WHERE r.intent_id = (
  SELECT i.intent_id
  FROM workflowdocument.workflow_import_intent i
  WHERE i.task_id = 220572
  ORDER BY i.created_utc DESC
  LIMIT 1
)
ORDER BY r.requirement_code;

-- 3. Inscripciones y expediente lógico asignado.
SELECT s.intent_id, s.inscription_key, s.inscription_ordinal, s.book_code,
       s.registry_number, s.matricula, s.normalized_matricula,
       s.cabinet_name, s.expedient_id, s.expedient_role,
       s.expedient_status, s.cache_status, s.created_utc, s.updated_utc
FROM workflowdocument.workflow_import_inscription s
WHERE s.intent_id = (
  SELECT i.intent_id
  FROM workflowdocument.workflow_import_intent i
  WHERE i.task_id = 220572
  ORDER BY i.created_utc DESC
  LIMIT 1
)
ORDER BY s.inscription_ordinal;

-- 4. Item SII seleccionado y estados agregados por efecto.
SELECT x.intent_id, x.client_item_id, x.inscription_key, x.provider_id,
       x.external_key, x.target_task_id, x.document_type_id,
       x.document_type_name, x.status, x.document_id, x.expedient_id,
       x.storage_status, x.relation_status, x.index_status, x.cache_status,
       x.persistence_known, x.retryable, x.error_code, x.visible_message,
       x.correlation_id
FROM workflowdocument.workflow_import_intent_item x
WHERE x.intent_id = (
  SELECT i.intent_id
  FROM workflowdocument.workflow_import_intent i
  WHERE i.task_id = 220572
  ORDER BY i.created_utc DESC
  LIMIT 1
)
ORDER BY x.client_item_id;

-- 5. Historial de transiciones de la saga.
SELECT t.transition_id, t.intent_id, t.client_item_id, t.previous_status,
       t.new_status, t.occurred_utc, t.result_code, t.correlation_id
FROM workflowdocument.workflow_import_intent_transition t
WHERE t.intent_id = (
  SELECT i.intent_id
  FROM workflowdocument.workflow_import_intent i
  WHERE i.task_id = 220572
  ORDER BY i.created_utc DESC
  LIMIT 1
)
ORDER BY t.transition_id;

-- 6. Universo relacionado descubierto por ENLASE y cada postcondición.
SELECT d.related_document_id, d.intent_id, d.task_id, d.image_id,
       d.cabinet_name, d.sii_radicado, d.document_type_id,
       d.inscription_key, d.expected_expedient_id, d.discovery_status,
       d.destination_status, d.relation_status, d.cache_status,
       d.cabinet_index_status, d.electronic_index_status,
       d.xml_index_status, d.reconciliation_status, d.retryable,
       d.error_code, d.created_utc, d.updated_utc
FROM workflowdocument.workflow_import_related_document d
WHERE d.intent_id = (
  SELECT i.intent_id
  FROM workflowdocument.workflow_import_intent i
  WHERE i.task_id = 220572
  ORDER BY i.created_utc DESC
  LIMIT 1
)
  AND d.task_id = 220572
ORDER BY d.image_id;

-- 7. Cruce del universo relacionado con la caché documental verificada.
SELECT d.image_id, d.cabinet_name, d.sii_radicado,
       d.expected_expedient_id AS planned_expedient_id,
       d.relation_status AS planned_relation_status,
       c.document_link_cache_id,
       c.expected_expedient_id AS cached_expedient_id,
       c.relation_status AS cached_relation_status,
       c.created_utc AS cache_created_utc,
       c.verified_utc
FROM workflowdocument.workflow_import_related_document d
LEFT JOIN workflowdocument.workflow_import_document_link_cache c
  ON c.task_id = d.task_id
 AND c.image_id = d.image_id
 AND c.cabinet_name = d.cabinet_name
WHERE d.intent_id = (
  SELECT i.intent_id
  FROM workflowdocument.workflow_import_intent i
  WHERE i.task_id = 220572
  ORDER BY i.created_utc DESC
  LIMIT 1
)
  AND d.task_id = 220572
ORDER BY d.image_id;

-- 8. Caché física de expediente por identidad normalizada y gabinete.
SELECT c.id_ra_sii_cache_exepediente, c.RadicadoSII, c.CodigoBarras,
       c.NombreGabinete, c.Matricula, c.IdExpediente,
       c.EstadoVinculaDocumento, c.FechaRegistroCache, c.EstadoPadre
FROM docuarchi.ra_sii_cache_exepediente c
WHERE c.NombreGabinete = 'ESAL'
  AND c.Matricula IN (
    SELECT s.normalized_matricula
    FROM workflowdocument.workflow_import_inscription s
    WHERE s.intent_id = (
      SELECT i.intent_id
      FROM workflowdocument.workflow_import_intent i
      WHERE i.task_id = 220572
      ORDER BY i.created_utc DESC
      LIMIT 1
    )
  )
ORDER BY c.id_ra_sii_cache_exepediente;

-- 9. Relación del radicado actual con el expediente físico reutilizado/creado.
SELECT r.expediente_archivo_ID_EXPEDIENTE, r.RadicadoExterno, r.FechaRegistro
FROM docuarchi.ra_relacion_radicado_externo_expediente r
WHERE r.RadicadoExterno = 'S002194198'
   OR r.expediente_archivo_ID_EXPEDIENTE IN (
     SELECT s.expedient_id
     FROM workflowdocument.workflow_import_inscription s
     WHERE s.intent_id = (
       SELECT i.intent_id
       FROM workflowdocument.workflow_import_intent i
       WHERE i.task_id = 220572
       ORDER BY i.created_utc DESC
       LIMIT 1
     )
   )
ORDER BY r.expediente_archivo_ID_EXPEDIENTE, r.FechaRegistro;

-- 10. Cabecera del expediente físico.
SELECT e.*
FROM docuarchi.expediente_archivo e
WHERE e.ID_EXPEDIENTE IN (
  SELECT s.expedient_id
  FROM workflowdocument.workflow_import_inscription s
  WHERE s.intent_id = (
    SELECT i.intent_id
    FROM workflowdocument.workflow_import_intent i
    WHERE i.task_id = 220572
    ORDER BY i.created_utc DESC
    LIMIT 1
  )
);

-- 11. Documentos físicos del gabinete descubiertos por ENLASE.
-- Este bloque es específico para el gabinete ESAL del caso 220572.
SELECT g.*
FROM docuarchi.ESAL g
WHERE g.ENLASE = 'S002194198'
   OR g.ID IN (
     SELECT d.image_id
     FROM workflowdocument.workflow_import_related_document d
     WHERE d.intent_id = (
       SELECT i.intent_id
       FROM workflowdocument.workflow_import_intent i
       WHERE i.task_id = 220572
       ORDER BY i.created_utc DESC
       LIMIT 1
     )
   )
ORDER BY g.ID;

-- 12. Producción documental de los documentos relacionados.
SELECT p.*
FROM docuarchi.registro_producion_documental p
WHERE p.ID_DOCUMENTO_DOCUARCHI_ALMACEN IN (
  SELECT d.image_id
  FROM workflowdocument.workflow_import_related_document d
  WHERE d.intent_id = (
    SELECT i.intent_id
    FROM workflowdocument.workflow_import_intent i
    WHERE i.task_id = 220572
    ORDER BY i.created_utc DESC
    LIMIT 1
  )
)
ORDER BY p.ID_DOCUMENTO_DOCUARCHI_ALMACEN;

-- 13. Auditoría física de registro Workflow.
SELECT l.*
FROM docuarchi.logdocuarchi l
WHERE l.id_tran IN (
  SELECT d.image_id
  FROM workflowdocument.workflow_import_related_document d
  WHERE d.intent_id = (
    SELECT i.intent_id
    FROM workflowdocument.workflow_import_intent i
    WHERE i.task_id = 220572
    ORDER BY i.created_utc DESC
    LIMIT 1
  )
)
  AND l.MODULO_REGISTRO = 'WORKFLOW'
ORDER BY l.id_tran;

-- 14. Configuración de creación de expediente del trámite efectivo.
SELECT t.id_Tipo_Doc_Entrante, t.nombre_gabinete_workflow,
       t.ra_auto_registro_expediente_id_auto_registro,
       t.util_Estado_Crea_ExpedienteSII,
       t.util_Estado_Multiple_expedienteSII
FROM docuarchi.tipo_doc_entrante t
WHERE t.id_Tipo_Doc_Entrante = (
  SELECT i.procedure_id
  FROM workflowdocument.workflow_import_intent i
  WHERE i.task_id = 220572
  ORDER BY i.created_utc DESC
  LIMIT 1
);

-- 15. Campos configurados como identidad única del expediente.
SELECT f.campo_expediente, f.estado_obligatorio, f.estado_unico
FROM docuarchi.ra_auto_campo_unico_expediente f
WHERE f.ra_auto_registro_expediente_id_auto_registro = (
  SELECT t.ra_auto_registro_expediente_id_auto_registro
  FROM docuarchi.tipo_doc_entrante t
  WHERE t.id_Tipo_Doc_Entrante = (
    SELECT i.procedure_id
    FROM workflowdocument.workflow_import_intent i
    WHERE i.task_id = 220572
    ORDER BY i.created_utc DESC
    LIMIT 1
  )
)
  AND f.estado_unico = 1
ORDER BY f.campo_expediente;

-- 16. Tipología documental autorizada para el trámite y el item.
SELECT r.ID_TIPO_DOCUMENTAL_CHEQUEO,
       r.tipo_doc_entrante_id_Tipo_Doc_Entrante,
       r.tipo_doc_series_Id_Tipo_Doc_Series,
       s.Descripcion_Documento
FROM docuarchi.ra_dig_tipos_docum_lista_chequeo r
INNER JOIN docuarchi.tipo_doc_series s
  ON s.Id_Tipo_Doc_Series = r.tipo_doc_series_Id_Tipo_Doc_Series
WHERE r.tipo_doc_entrante_id_Tipo_Doc_Entrante = (
  SELECT i.procedure_id
  FROM workflowdocument.workflow_import_intent i
  WHERE i.task_id = 220572
  ORDER BY i.created_utc DESC
  LIMIT 1
)
  AND r.tipo_doc_series_Id_Tipo_Doc_Series IN (
    SELECT x.document_type_id
    FROM workflowdocument.workflow_import_intent_item x
    WHERE x.intent_id = (
      SELECT i.intent_id
      FROM workflowdocument.workflow_import_intent i
      WHERE i.task_id = 220572
      ORDER BY i.created_utc DESC
      LIMIT 1
    )
  );

-- 17. Telemetría segura de servicios externos por correlación.
SELECT a.Id_ser_intentoServicioIntegracion,
       a.Id_ser_servicioIntegracion,
       a.NombreServicioSnapshot, a.Operacion, a.Exitoso,
       a.CodigoError, a.CategoriaError, a.EstadoHttp,
       a.Reintentable, a.IntentId, a.ClientItemId,
       a.OperationId, a.CorrelationId,
       a.FechaInicioUtc, a.FechaFinUtc, a.DuracionMs
FROM docuarchi.ra_ser_intento_serviciointegracion a
WHERE a.IntentId = (
  SELECT i.intent_id
  FROM workflowdocument.workflow_import_intent i
  WHERE i.task_id = 220572
  ORDER BY i.created_utc DESC
  LIMIT 1
)
   OR a.CorrelationId IN (
     SELECT x.correlation_id
     FROM workflowdocument.workflow_import_intent_item x
     WHERE x.intent_id = (
       SELECT i.intent_id
       FROM workflowdocument.workflow_import_intent i
       WHERE i.task_id = 220572
       ORDER BY i.created_utc DESC
       LIMIT 1
     )
   )
ORDER BY a.FechaInicioUtc;

