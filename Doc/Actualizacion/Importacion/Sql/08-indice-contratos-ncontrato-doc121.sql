-- DOC-121 | SOLO imp_02_contratos.NCONTRATO | MySQL 5.1, InnoDB.
-- APLICADO Y VERIFICADO 2026-11-21 por el agente tras la instruccion
-- expresa del usuario: "aplica por mi". No repetir en esa base.
-- Se conserva el ALTER entregado; el estado anterior era ejecucion manual.
-- Evidencia: ../Validacion/doc121-contratos-indice-aplicado-20261121.md
-- Aplicar UNA VEZ, despues de revisar 07 en la DA correcta.
-- Base inspeccionada el 2026-11-21: 625 filas, cero NULL/vacios/duplicados.
-- Esa lectura no garantiza que los datos no cambien despues.
-- Precondiciones vigentes en el momento de ejecutar:
-- * NCONTRATO VARCHAR(250) latin1/latin1_swedish_ci, nullable, DEFAULT NULL,
--   comentario vacio, sin EXTRA; motor InnoDB. Preservar longitud/collation.
-- * Cero NULL/vacios/duplicados bajo la igualdad MySQL de esa columna.
-- * Sin PK/UNIQUE equivalente completo ni indice con el nombre propuesto.
-- * Confirmar identidad unica global de NCONTRATO para este destino.
--   Si la fuente se comparte por empresa, generar otro archivo compuesto.
-- * Coordinar la ventana con los escritores; el ALTER puede bloquear.
-- Si algun punto difiere, detener y preparar otro archivo especifico.
-- No asigna CAMPO_UNICO a plantillas, no registra fuentes/proveedores,
-- no modifica EGRESOS y no borra o normaliza datos.
ALTER TABLE imp_02_contratos
  MODIFY COLUMN NCONTRATO VARCHAR(250)
    CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  ADD UNIQUE INDEX ux_doc121_NCONTRATO (NCONTRATO);
-- Comprobacion posterior: archivo 09. No repetir si ya esta aplicado.
