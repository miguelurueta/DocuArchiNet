-- Aplicacion manual UNA VEZ en RA. Despues de SQL 05.
-- No asigna permisos a usuarios ni registra fuentes/proveedores.
ALTER TABLE remit_dest_interno_perfil_produccion
  ADD COLUMN ADMINISTRAR_PROVEEDORES_IMPORTACION TINYINT UNSIGNED NOT NULL DEFAULT 0;
