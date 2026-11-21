-- DOC-121 | DESTINO DA | EJECUCION MANUAL DEL USUARIO | UNA SOLA VEZ
-- Precondicion: 00 confirma imp01_plantillaimp y ausencia de AMBAS columnas.
-- Estado parcial: preparar otro archivo especifico, no reaplicar este ALTER.
-- Agregar al final preserva el orden de las columnas historicas.
-- No asignar automaticamente CAMPO_UNICO a plantillas existentes.
ALTER TABLE imp01_plantillaimp
  ADD COLUMN CAMPO_UNICO VARCHAR(64) CHARACTER SET ascii COLLATE ascii_bin NULL,
  ADD COLUMN REVISION_CARGA INT UNSIGNED NOT NULL DEFAULT 1;
