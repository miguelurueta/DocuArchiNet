# DOC-110: referencias de proveedor al guardar, publicar y seleccionar

Fecha: 2026-10-27. Tarea 14.11, D-02/RQ-02 y AMP-01. Se amplían pruebas del servicio productivo; no cambia código productivo ni se ejecuta una nueva sesión autenticada.

## Matriz local

`PerfilImportacionServiceBehaviorTests` ejecuta `ServicioPerfilImportacion` con el coordinador productivo y repositorios dobles. Cada condición se verifica en Guardar, Publicar y Seleccionar (uso ADMINISTRACION):

| Metadatos recibidos del repositorio | Resultado esperado |
|---|---|
| Ámbito distinto | CONFIGURACION_INVALIDA |
| Identidad de proveedor distinta | CONFIGURACION_INVALIDA |
| Identidad de versión distinta | CONFIGURACION_INVALIDA |
| Proveedor inactivo | CONFIGURACION_INVALIDA |
| Versión BORRADOR | CONFIGURACION_INVALIDA |
| Tipo de clave incompatible | CONFIGURACION_INVALIDA |
| Colección de propiedades ausente | CONFIGURACION_INVALIDA |
| Propiedad referenciada inexistente | CONFIGURACION_INVALIDA |
| Propiedad referenciada duplicada | CONFIGURACION_INVALIDA |
| Tipo de propiedad incompatible | CONFIGURACION_INVALIDA |
| Proveedor inexistente (resultado null) | CONFIGURACION_INVALIDA |
| Repositorio deniega la referencia | ACCESO_DENEGADO |

Son 36 comprobaciones de rechazo. Cada una exige igualdad del documento completo antes/después, contador de escrituras intacto y ninguna consulta adicional al proveedor de datos. Se usan estados BORRADOR para guardar/publicar y PUBLICADA para seleccionar. El repositorio doble deliberadamente devuelve metadatos incorrectos para ejercitar las defensas del coordinador; no representa una consulta real entre empresas. El repositorio MySQL normalmente deniega las referencias no disponibles con ACCESO_DENEGADO antes de devolver esos metadatos.

Guardar valida el contrato propuesto. Publicar y Seleccionar vuelven a validar la configuración persistida; una validación anterior no sustituye esta comprobación. La selección positiva previa del arnés usa la publicación almacenada y tampoco consulta datos del origen. El uso IMPORTACION permanece no disponible conforme al alcance del servicio.

## Evidencia autenticada existente

La [aceptación del ciclo completo](doc110-fuentes-aceptacion-preparada-20261026.md) registra dos versiones publicadas de proveedor y perfil, referencias elegidas desde UI y resolución real de índices. El cambio de versión conserva la primera publicación; la desactivación del proveedor y la revocación de la fuente conservan documentos e historial. Corrida aportada: **1 passed (23.8 s)**, caso 22.8 s. El [diagnóstico posterior de solo lectura](doc110-fuente-referencias-guardadas-20261027.md) conserva referencias originales con fuente revocada y cero escrituras.

La [matriz de consumidores publicados](doc110-politica-ra-consumidores-20261027.md) distingue las denegaciones HTTP reales de guardar/ensayar con proveedor BORRADOR y las defensas locales de publicación/selección. No se introduce un perfil inválido en BD para fabricar estas últimas.

## Verificación

```powershell
node --test tests/doc110-perfil-importacion-behavior.test.cjs
```

PASS: una prueba Node compila y ejecuta el arnés .NET Framework, incluida esta matriz. No es una compilación completa de WebForms ni una prueba MySQL. Se completa 14.11 con implementación, pruebas locales y evidencia autenticada existente. El aislamiento real entre ámbitos, las transacciones y demás tareas abiertas conservan sus propios criterios. Validar el contrato no garantiza disponibilidad futura de datos. Sin DDL, cambios SII, nuevos permisos ni acceso nuevo a DA.
