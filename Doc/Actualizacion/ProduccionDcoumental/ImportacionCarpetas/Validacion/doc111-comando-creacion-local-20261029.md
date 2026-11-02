# DOC-111: composición de comando de creación, 2026-10-29

Avance parcial de 9.3, D-07/RQ-07. **95/130 completas, sin incremento**: el comando está implementado y probado localmente; todavía no se invoca Registrar_Expediente_Conservacion, no se implementa el adaptador completo y no se certifica 9.4.

El nuevo componente interno `Infrastructure/ProduccionDocumental/ImportacionCarpetas/ComandoCreacionExpedienteImportacion.vb` captura usuario/empresa/nivel/ruta/sello y snapshot confirmados, IDs de lote/raíz/intento, revisión, token/huella y valores resueltos. Su constructor es privado; Preparar valida el contexto y las correspondencias antes de devolver un objeto inmutable por copia. Se incluye en el vbproj y el arnés existente, sin otro repositorio, endpoint o configuración.

Reutiliza ValidadorPerfilImportacion, el codec/huellas y la conversión del resolvedor existente. El nuevo helper interno ValidarValorResuelto comprueba la representación canónica sin consultar fuentes/proveedores: una fecha ya normalizada no se reinterpreta con el formato original de carpeta. La configuración se valida en copia y se rechaza si esa validación exigiría normalizar el snapshot. Los valores conservan procedencia y no se recalculan.

Guardas: contexto cambiado o sin facultad; lote detenido/no EN_CURSO; raíz ajena/no CREANDO/con identidad o valores efectivos; intento ajeno/repetido/de otra fase/con archivo/resultado/finalización; token/huella inválidos; perfil/reglas alterados; valores faltantes/duplicados, diferidos, tipo/fuente incompatibles o datos no canónicos. La fecha final debe ser coherente y responsable, si se configura, una referencia canónica. Los campos opcionales no configurados permanecen ausentes.

Validación PASS:

- `node --test tests/doc110-perfil-importacion-behavior.test.cjs`: arnés Framework y familias previas. Casos nuevos de guardas, fechas canónicas con formato de entrada distinto, ceros iniciales, nombre Unicode con apóstrofo, opcionales ausentes y ausencia de mutaciones. Cambiar los objetos de entrada o las copias devueltas no altera el comando capturado.
- MSBuild Debug completo, salida/intermedios `.tmp/doc111-comando-creacion-build/{bin,obj}`, log `.tmp/doc111-comando-creacion-build.log`, código 0; sin despliegue ni sustitución de DLL del sitio.

Pendiente antes de invocar: componer y comprobar nombres de catálogo/gabinete y responsable que exige legacy, límites Integer de esa firma, compatibilidad de texto/SQL y conexión RA, autoridad actual, barrera y correlación/verificación del resultado. EsNuevaReserva es un marcador transitorio, no fencing ni prueba de una reclamación durable actual. La composición no acredita XML, persistencia o causalidad de efectos. El comando conserva nombres exactos; no promete que legacy acepte arbitrariamente sus caracteres.

Sin consultas SQL/HTTP/E2E reales, creación, almacenamiento, limpieza, cambios de configuración o despliegue en este corte. ClassGaExpediente, ClassAlmacenamiento, ClassNeodynamic y conect permanecen intactos; no se fabrican tareas Workflow/autorregistro/SII.
