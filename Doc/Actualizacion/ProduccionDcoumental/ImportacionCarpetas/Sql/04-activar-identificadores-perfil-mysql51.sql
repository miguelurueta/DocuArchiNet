ALTER TABLE ra_integracion_perfil_regla DROP FOREIGN KEY fk_ra_integracion_regla_version;
ALTER TABLE ra_integracion_perfil_version DROP FOREIGN KEY fk_ra_integracion_version_perfil;

RENAME TABLE
  ra_integracion_perfil TO ra_integracion_perfil_guid_respaldo,
  ra_integracion_perfil_version TO ra_integracion_perfil_version_guid_respaldo,
  ra_integracion_perfil_regla TO ra_integracion_perfil_regla_guid_respaldo,
  ra_integracion_perfil_ai TO ra_integracion_perfil,
  ra_integracion_perfil_version_ai TO ra_integracion_perfil_version,
  ra_integracion_perfil_regla_ai TO ra_integracion_perfil_regla;

ALTER TABLE ra_integracion_perfil_version ADD CONSTRAINT fk_ra_integracion_version_perfil FOREIGN KEY (perfil_id) REFERENCES ra_integracion_perfil (perfil_id) ON DELETE RESTRICT ON UPDATE RESTRICT;
ALTER TABLE ra_integracion_perfil_regla ADD CONSTRAINT fk_ra_integracion_regla_version FOREIGN KEY (version_id) REFERENCES ra_integracion_perfil_version (version_id) ON DELETE RESTRICT ON UPDATE RESTRICT;

ALTER TABLE ra_integracion_perfil_version_guid_respaldo ADD CONSTRAINT fk_ra_version_perfil_respaldo FOREIGN KEY (perfil_id) REFERENCES ra_integracion_perfil_guid_respaldo (perfil_id) ON DELETE RESTRICT ON UPDATE RESTRICT;
ALTER TABLE ra_integracion_perfil_regla_guid_respaldo ADD CONSTRAINT fk_ra_regla_version_respaldo FOREIGN KEY (version_id) REFERENCES ra_integracion_perfil_version_guid_respaldo (version_id) ON DELETE RESTRICT ON UPDATE RESTRICT;
