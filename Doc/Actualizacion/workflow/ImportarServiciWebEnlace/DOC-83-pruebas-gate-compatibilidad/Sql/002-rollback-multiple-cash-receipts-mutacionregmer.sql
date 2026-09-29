-- Rollback DOC-83: restaura la cardinalidad observada antes del refinamiento.
UPDATE ra_dig_tipos_docum_lista_chequeo
SET UNICO = 1
WHERE ID_TIPO_DOCUMENTAL_CHEQUEO = 265
  AND tipo_doc_entrante_id_Tipo_Doc_Entrante = 290
  AND tipo_doc_series_Id_Tipo_Doc_Series = 186
  AND OBLIGATORIO = 1
  AND UNICO = 0;

SELECT ID_TIPO_DOCUMENTAL_CHEQUEO,
       tipo_doc_entrante_id_Tipo_Doc_Entrante,
       tipo_doc_series_Id_Tipo_Doc_Series,
       OBLIGATORIO,
       UNICO
FROM ra_dig_tipos_docum_lista_chequeo
WHERE ID_TIPO_DOCUMENTAL_CHEQUEO = 265
  AND tipo_doc_entrante_id_Tipo_Doc_Entrante = 290;
