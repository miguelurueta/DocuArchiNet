# DOC-111: verificación estructural de expediente, 2026-10-28

HEAD `b2e93b726f221868cf2b82d43200a8e568871117`, workspace con cambios sin commit. **95/130 tareas completas**, sin incremento: esta entrega adelanta parte de 9.4 y revisa condiciones de 9.3, manteniendo ambas abiertas. Almacenamiento, creación legacy, sus helpers y SII quedan intactos.

El repositorio único incorpora el parcial Verificacion, registrado en Compile y arnés Framework. ComprobarRelacionExpedienteConocido recibe contexto e IDs conocidos de expediente/relación/nivel. Hace SELECT parametrizado con cardinalidad máxima dos, empresa/creador/nivel, nivel activo, estado electrónico y permiso actual de creación; verifica todos los componentes devueltos. No busca por nombre, no crea expedientes, no cambia recursos/eventos y no expone endpoint. Su booleano solo describe correspondencia estructural en esa lectura; no prueba causalidad del intento, clasificación completa o ausencia de efectos para reintentar.

La revisión estática de creación encontró búsquedas legacy por nombre y configuración de ruta global. El esquema histórico del 2026-10-19 sí registra unicidad de nombre de Fondo y de Área por organigrama; no se presenta como una ambigüedad demostrada en esas tablas. Organigrama no registra unicidad de nombre/empresa en esa evidencia. VOLUMEN_EXPEDIENTE figura NOT NULL sin DEFAULT, mientras el INSERT de Conservación omite el campo. El entorno/modo SQL actual debe contrastarse; no se ejecutaron consultas reales para suponer un valor o compatibilidad.

Las alternativas de volumen, autorregistro y trámite/SII inspeccionadas tienen dependencias distintas; no se fabricaron expedientes padres, imágenes, autorregistros o tareas para usarlas. Detalle y referencias en diseño 15. Los SELECT de esquema/modo SQL, índices, cardinalidad de ruta y clasificación autorizada quedan preparados en `Sql/02-aplicacion-carga-carpetas.md`, **NO_EJECUTADO**, pendientes de identificar el ambiente de pruebas. No se propone editar datos o configuración para forzar la llamada.

Validación local PASS:

- `node --test tests/doc110-perfil-importacion-behavior.test.cjs`: arnés Framework previo, ampliado con lector SQL sintético; cardinalidades 0/1/2, componentes ajenos/nulos, permiso/estado incorrectos, origen no canónico, destino distinto y referencia malformada. Se rechazan antes de SQL las entradas estructuralmente inválidas. Las familias previas siguen aprobadas.
- Build Debug completo sin despliegue en `.tmp/doc111-verificacion-expediente-build/`, log `.tmp/doc111-verificacion-expediente-build.log`, código 0.
- OpenSpec estricto y `git diff --check` PASS. Los diffs de ClassGaExpediente, ClassAlmacenamiento, ClassNeodynamic y conect están vacíos.

**NO_EJECUTADO en este corte local:** SQL/HTTP/autenticación del sistema, creación o reconciliación real, comprobación física de XML, almacenamiento/incorporación, cambios de modo SQL/datos/esquema o despliegue. No se certifica el adaptador de creación por implementar este subconjunto lector.

Actualización posterior del mismo día: se encontró y reutilizó la configuración de prueba del repositorio para ejecutar SELECT de compatibilidad. Ver `doc111-compatibilidad-creacion-select-20261028.md`. El esquema actual sí declara DEFAULT 1 para VOLUMEN_EXPEDIENTE; los pendientes de identificación de ambiente anteriores corresponden al corte local, y quedan sustituidos por ese contraste real. No se invocó creación ni almacenamiento.
