# DOC-111: contraste de clasificación persistida, 2026-10-29

Avance parcial de 9.4, D-07/RQ-07. **95/130 tareas completas, sin incremento**: faltan la composición del comando de creación, su correlación con el intento, la comprobación completa de efectos y la aceptación autorizada. No se habilitan efectos o endpoints.

Se amplió el parcial Verificacion del repositorio operativo existente. `ComprobarClasificacionExpedienteConocido` recibe contexto, identidad conocida y configuración del perfil confirmado. Reutiliza el lector de relación/empresa/creador/nivel activo/permiso/estado electrónico, agregando en el mismo SELECT fondo, instrumento, área, serie, subserie, tipo de expediente y unidad documental persistidos. La consulta conserva IDs parametrizados, LIMIT 2 y ausencia de bloqueo de escritura; no busca expedientes por etiquetas ni crea otro repositorio.

Referencias requeridas ausentes, no canónicas o malformadas se rechazan antes de consultar. Una discrepancia, NULL inesperado o cardinalidad distinta de uno devuelve False; columnas ausentes implican indisponibilidad. La subserie opcional ausente exige SQL NULL: Conservación transforma su ID cero a literal SQL null en `Gestion/ClassGaExpediente.vb:7638–7643`. No se normalizan cero o texto vacío a ausencia. Cuando el perfil exige subserie, se compara su ID exacto.

True acredita únicamente correspondencia de esos campos en la lectura. No acredita causalidad del intento, valores de código/nombre/fechas, integridad del XML, gabinete físico, relaciones actuales entre catálogos o estabilidad concurrente. La configuración esperada debe proceder del snapshot autorizado; el lector no autentica por sí mismo su procedencia. False no demuestra ausencia de efectos ni concede replay. El lector estructural anterior conserva su contrato limitado.

Validación PASS:

- `node --test tests/doc110-perfil-importacion-behavior.test.cjs`: arnés .NET Framework existente y familias previas; nuevo caso con registros SELECT sintéticos. Cubre clasificación coincidente, cambio/NULL de cada componente de alcance y clasificación, cero/dos filas, subserie ausente/inesperada, referencias inválidas, nivel distinto, permiso denegado y columna faltante.
- Build Debug completo, sin despliegue ni sustitución de DLL del sitio; salidas `.tmp/doc111-clasificacion-expediente-build/{bin,obj}`, log `.tmp/doc111-clasificacion-expediente-build.log`, código 0.

No se ejecutaron SQL/HTTP/E2E reales en este corte, creación, reconciliación, almacenamiento o cambios de datos/configuración. El contraste SELECT real del día anterior se conserva en su evidencia separada; no se presenta como prueba de este lector ampliado. Las funciones ClassGaExpediente, ClassAlmacenamiento, ClassNeodynamic y conect permanecen sin cambios.
