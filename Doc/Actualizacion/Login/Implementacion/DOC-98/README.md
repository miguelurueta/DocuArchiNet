# DOC-98 — Despliegue y activación transversal de 2FA

## Resultado esperado

Este paquete prepara el despliegue reversible y la activación de segundo factor ya implementado por DOC-91 a DOC-97. No modifica lógica de aplicación, no agrega tablas o columnas y no contiene valores reales de ambientes.

La activación es una propiedad de la fila central del módulo en `gestor_modulos`. Cuando `RequiereSegundoFactor=1`, `SecondFactorProviderType=1` y `SegundoFactorTiempoExpira` está entre 1 y 10, el flujo se aplica a cualquier principal que intente autenticarse en ese módulo. No existe configuración, lista, excepción ni bypass por usuario.

Las tablas de principales solo resuelven identidad y correo durante el login:

| Tipo de módulo | Tabla existente | Identidad | Login | Correo |
|---|---|---|---|---|
| `DOCUARCHI CONTENEDOR` | `usuarios_da` | `Clave_Usuario` | `idusuario` | `correo` |
| `GESTOR DOCUMENTAL` | `remit_dest_interno` | `Id_Remit_Dest_Int` | `Login_Usuario` | `Correo_Electronico` |
| `RADICACION DOCUMENTAL` | `usuario_radicador` | `id_usuario` | `Login_usuario` | `Correo_Usuario` |
| `WORKFLOW DOCUMENTAL` | `usuario_workflow` | `idU_suario` | `login_Usuario` | `Correo_Usuario` |

Los repositorios actuales no consultan una bandera de activación de usuario. Una identidad ambigua o un correo inválido falla cerrado para ese intento, sin cambiar la activación transversal del módulo.

## Inventario

- `01-RUNBOOK-DESPLIEGUE-ACTIVACION.md`: operación, responsabilidades, puertas y orden.
- `02-MATRIZ-GO-NO-GO.md`: criterios objetivos.
- `03-ROTACION-LIMPIEZA-ROLLBACK.md`: llaves, retención y reversión.
- `04-VALIDACION-Y-EVIDENCIA.md`: comandos y estado verificable.
- `Sql/01-preflight-readonly.sql`: estructura y configuración, solo lectura.
- `Sql/02-pilot-activation.sql`: plantilla guardada para una fila piloto.
- `Sql/03-pilot-disable-rollback.sql`: restauración exacta de una fila.
- `Sql/04-observability-readonly.sql`: métricas agregadas.
- `Sql/05-principal-diagnostic-readonly.sql`: calidad agregada de principales; no habilita usuarios.
- `Sql/06-global-wave-activation.sql`: oleada explícita mediante manifiesto temporal.
- `Sql/07-terminal-cleanup-preview-readonly.sql`: vista previa de retención; no elimina.

## Límites

- Los scripts mutables nacen cerrados con `@doc98_authorized=0` y requieren autorización específica del ambiente.
- Ningún script se debe ejecutar pegado como lote ciego. El operador debe completar IDs, valores esperados y snapshots.
- DOC-98 no incorpora DDL, scheduler ni limpieza automática.
- El paquete local no demuestra que un ambiente haya sido activado. Esa afirmación requiere evidencia saneada de una ejecución autorizada.

Trazabilidad: D-01..D-10, RQ-01..RQ-10.
