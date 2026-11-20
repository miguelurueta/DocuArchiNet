-- DOC-121 | DESTINO RA | EJECUCION MANUAL DEL USUARIO | UNA SOLA VEZ
-- Precondicion: 00 confirma tabla correcta y ausencia de esta columna.
-- Si existe, verificar definicion y omitir; no repetir ni cambiar usuarios.
-- Efecto: permiso propio del cargador inicialmente deshabilitado para todos.
ALTER TABLE remit_dest_interno_perfil_produccion
  ADD COLUMN ADMINISTRAR_CARGAS_IMPORTACION TINYINT UNSIGNED NOT NULL DEFAULT 0;
