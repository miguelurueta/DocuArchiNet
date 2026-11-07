# DOC-110: instalación nueva en base aislada

El usuario autoriza explícitamente crear únicamente doc110_instalacion_prueba y realizar allí las comprobaciones, sin cambiar la base/configuración del sitio actual. Esta autorización concreta permite aplicar el paquete previamente presentado; no autoriza cambiar el catálogo de módulos ni retirar tablas existentes.

## Ejecución

Preflight sin escrituras:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools/e2e/scripts/test-doc110-clean-install.ps1
```

READY_NOT_APPLIED: base ausente, ocho instrucciones con huella del paquete aprobado y tipos del repositorio productivo resueltos en la DLL activa. Motor MySQL 5.1.37-community.

Aplicación única autorizada:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools/e2e/scripts/test-doc110-clean-install.ps1 -Apply
```

**PASS**, inicio 2026-10-27T18:49:16.7322661Z, fin 2026-10-27T18:49:19.5543084Z. Reporte saneado doc110-clean-install-result.json. Las ocho instrucciones crean/seleccionan exclusivamente la base aislada, su marca y prerrequisito mínimo, y aplican el SQL 01 original. El resto de DDL no se ejecuta antes de confirmar SELECT DATABASE() contra el nombre fijo aprobado.

No se copiaron usuarios ni datos personales de RA. La tabla mínima de prerrequisito permite verificar el ALTER del permiso; no se presenta como una copia completa del sistema legacy. No se modificaron Web.config, gestor_modulos ni el destino del sitio.

## Resultado de esquema e inserciones

| Comprobación | Resultado |
|---|---|
| Objetos del fixture | 5 tablas: marca, prerrequisito y 3 tablas del perfil |
| Estado inicial de tablas de perfil/permisos | Vacío |
| ADMINISTRAR_PERFILES_IMPORTACION | NOT NULL, default 0 |
| MySqlPerfilImportacionRepository.VerificarEsquema de DLL activa | PASS: columnas, índices, tipos e integridad exigidos por producción |
| Inserciones por Crear/Guardar del repositorio productivo | 2 perfiles, 2 versiones, 2 reglas |
| IDs asignados por BD | 1 y 2 en cada entidad, positivos y distintos |
| Regla serializada y recargada | Valor constante 000123 conservado; IDs generados en JSON recuperados |
| Desactivación del fixture | Ambos perfiles INACTIVOS mediante repositorio |
| SELECT de conteos finales | 2/2/2 |
| Versiones/reglas huérfanas | 0 |
| Conteos de perfil/versión/regla de RA original | Iguales antes/después |
| Huella de Web.config | Igual antes/después |

Esta prueba ejecuta la DLL/repositorio reales sobre MySQL; el contexto de repositorio es sintético (usuario/ámbito 1), sin conceder permisos a una cuenta real ni ejecutar login/UI en esa base. Las configuraciones de prueba quedan como BORRADOR; no se presentan como publicaciones funcionales ni como nuevos expedientes/documentos. La aceptación UI/HTTP completa tiene [su corrida específica](doc110-per01-03-consolidacion-preparacion.md).

## Identificación y preservación de migración

Paquete aplicado SHA-256: 1b46bef87631ab775a5b1be846e71fd41ba9ca2d12a695fa415319d0e42a17a8. SQL 01 incluido sin modificaciones: 002e34b873b90d0b92d822aba0385e645fbdeaba645c94633eabf4394d7c0461. DLL activa: 80F305D1BC7D1F629431FA27EC011CF04A031EB323AD8C377F1E18FCDDCE2A21.

La otra parte de 10.16 ya tiene [migración local aplicada y verificada](doc110-correccion-incrementales.md): 5 perfiles, 6 versiones, 13 reglas y 2 publicaciones previas; huellas originales verificadas, JSON transformado con correspondencias y POST_CUTOVER_READ_ONLY PASS del 2026-10-20. El resultado está registrado en doc110-migracion-incremental-aplicada.json. Las inserciones y controles posteriores se acreditaron además en [identidades y concurrencia](doc110-identidades-select-20261027.md).

Se completa 10.16 con ambas evidencias diferenciadas, sin repetir la migración ni atribuir a la instalación nueva un corte/migración reciente. La base aislada se conserva con datos sintéticos; el script rechaza una base ya existente y no la borra/recrea ni repite automáticamente DDL.

Esta base mínima no acredita 7.5: no contiene la aplicación legacy necesaria para probar carga individual operativa sin las tablas de perfiles. Tampoco proporciona el proveedor histórico fuera del prefijo requerido por 15.19. Esas tareas permanecen abiertas.
