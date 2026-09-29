-- DOC-83: mutacionregmer exige al menos un Recibo De Caja, pero admite varios.
-- Aplicar sobre el esquema de configuración que contiene el trámite 290.
UPDATE ra_dig_tipos_docum_lista_chequeo
SET UNICO = 0
WHERE ID_TIPO_DOCUMENTAL_CHEQUEO = 265
  AND tipo_doc_entrante_id_Tipo_Doc_Entrante = 290
  AND tipo_doc_series_Id_Tipo_Doc_Series = 186
  AND OBLIGATORIO = 1
  AND UNICO = 1;

SELECT ID_TIPO_DOCUMENTAL_CHEQUEO,
       tipo_doc_entrante_id_Tipo_Doc_Entrante,
       tipo_doc_series_Id_Tipo_Doc_Series,
       OBLIGATORIO,
       UNICO
FROM ra_dig_tipos_docum_lista_chequeo
WHERE ID_TIPO_DOCUMENTAL_CHEQUEO = 265
  AND tipo_doc_entrante_id_Tipo_Doc_Entrante = 290;
