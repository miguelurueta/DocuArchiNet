# DOC-122 — Autorización, conexión y plantillas

Implementación de fase02 de7, perfil legacy-webforms-vb, .NET Framework4.6.1/MySQL5.1.37. Contrato previo: [DOC121](01-contratos-configuracion-doc121.md). No carga archivos ni ejecuta adecuación de esquema. EGRESOS permanece intacto.

## Configuración protegida del servidor
Un administrador configura estos valores localmente, fuera de Git y sin enviar contraseñas al chat:

| Sección/clave | Regla |
|---|---|
| appSettings / ImportacionExcel.Destinos | Array JSON de objetos empresaId (entero), conexionId (ID_XMLCONEXION), nombre descriptivo, connectionStringName. Máximo100 vínculos, sin duplicar empresa+ID. |
| connectionStrings / nombre referenciado | Cuenta dedicada MySQL de DA; nunca root ni la cuenta DA histórica de sesión. Destino exacto de sesión DA_ y catálogo histórico. Puerto3306, como el resolver actual de sesión; otros puertos requieren extender ese contrato antes de habilitar. |
| appSettings / ImportacionExcel.MaxFilasDatos | Entero positivo, default10000 si falta. |
| appSettings / ImportacionExcel.MaxArchivoMB | Entero positivo, default10 si falta, MB decimal1000000 bytes. Rechaza desbordamiento. |

Ejemplo de vínculo sin datos de ambiente ni secretos:
```json
[{"empresaId":2,"conexionId":7,"nombre":"Destino autorizado","connectionStringName":"ImportacionExcelDA"}]
```

Ambas secciones appSettings y connectionStrings deben usar ASP.NET Protected Configuration, con el proveedor y ACL apropiados para la identidad del sitio. Se descifra únicamente en memoria del servidor. El nombre de conexión es configuración administrativa; el navegador solo envía ConexionId. Ningún archivo Web.config ni secreto del sitio fue cambiado durante esta entrega. No hay fallback a las credenciales de imp01_xmlconexion.

El permiso RA ADMINISTRAR_CARGAS_IMPORTACION=1 es independiente del de perfiles. Consultar usuario activo y empresa vigente en RA en cada operación; el indicador GA_ADMINISTRAR_CARGAS_IMPORTACION se reinicia a0 al cargar permisos y sirve solo para presentación.

## Operaciones ASMX
URL relativa webservice/WebServiceImportacionExcel.asmx/<operación>, POST application/json; respuesta bajo d.

| Operación | Solicitud | Resultado |
|---|---|---|
| AbrirCargaExcel | {} | Referencias de conexión, límites efectivos y token efímero. |
| ListarPlantillas | solicitud: ConexionId, Termino, Pagina, TamanoPagina | Total y resumen de plantillas imp_02_ de esa conexión; página1..1000000, tamaño1..100. |
| ConsultarPlantilla | solicitud: ConexionId, PlantillaId | Cabecera física, campos, Modo, Formato y Huella. |
| GuardarPlantilla | solicitud: ConexionId, RevisionEsperada, HuellaEsperada, Plantilla | Plantilla persistida con nueva revisión y huella. |

Tras abrir, usar X-Importacion-Excel-CSRF. Token ligado a sesión/usuario/empresa y distinto del de perfiles. Origen declarado debe coincidir; transporte limitado a262144 bytes, sin caché. Todas las respuestas son tipadas; no incluyen servidor, base, usuario ni cadena de conexión. Los errores usan código/mensaje controlados y operacionId sin detalles del proveedor.

Alta: ID_PLANTILLAIMP=0, RevisionEsperada=0, HuellaEsperada vacía; todos los ID_CAMPOS_PLANTILLA=0. Edición exige revisión y huella de la consulta previa, junto con mapeos completos y sus IDs. Se ignora la ruta del navegador: el alta genera web/<guid>.xls[x] y la edición conserva la ruta almacenada. No se resuelve ninguna ruta física en esta fase. El almacenamiento privado y recepción corresponden a fase03.

Formato XLS→8.0 xls y XLSX→12.0 xlsx. Banderas ELIMINA_TABLA/ELIMINA_INSERTA_RECORD/INSERTA_RECORD: crear/agregar0/0/1; reemplazar0/1/0. Como crear y agregar comparten flags, ConsultarPlantilla distingue ambos mediante existencia de tabla. Flags diferentes producen CONFIGURACION_INVALIDA sin reescritura. Clave NULL histórica es consultable, pero guardar exige una CAMPO_UNICO exacta dentro de los destinos mapeados.

## Persistencia y compatibilidad
Guardar exige ambos catálogos InnoDB. SELECT FOR UPDATE bloquea cabecera y campos; compara revisión y huella completa antes de actualizar. Cabecera, inserción/actualización/eliminación de mapeos y revisión se confirman juntas. Los IDs de campos conservados permanecen iguales. Errores de escritura hacen rollback; no hay reintento automático. GUARDADO_NO_COMPLETADO requiere consultar antes de repetir, pues una pérdida de respuesta no demuestra rollback.

La huella v1 incluye todas las columnas históricas, las aditivas, IDs de campos y transformaciones con NULL explícitos. Canonización ordinal, mapeos ordenados por destino/fuente, escape ASCII minúsculo y SHA256 sobre UTF8 sin BOM. Vector reproducible tests/Fixtures/ProduccionDocumental/doc122-snapshot-v1.json. No depende de JsonConvert.DefaultSettings. Preparación/confirmación futuras deben revalidar configuración y esquema: un escritor antiguo que actúe después del commit no respeta este protocolo.

Destino existente: columnas físicas y motor consultados en information_schema; preservar tipo y longitud reales, incluida ampliación de BENEFICIARIO. Rechazar columnas ajenas, auto_increment mapeado y columnas obligatorias sin mapeo/default. Crear admite destino ausente y tipos seguros CHAR/VARCHAR, enteros, DECIMAL/NUMERIC y DATE; no crea tabla en esta fase. Guardar no instala índices UNIQUE; esa adecuación sigue el contrato SQL manual antes de cargar. No modifica consumidores SII ni clases legacy.

## Instalación y comprobación
Conservar SQL00–09 ya entregados para DOC121. Nuevo [10-verificacion-doc122.sql](../Sql/10-verificacion-doc122.sql) contiene solo SELECT para RA/DA y privilegios. No requiere aplicar DDL nuevo por esta fase. Si aparecen diferencias, generar un archivo incremental manual; no reparar desde tests ni endpoints.

Cuenta dedicada, concesiones efectivas, sesión RA/DA y despliegue todavía no acreditados en un recorrido real DOC122. SQL05 es una plantilla de concesiones para revisión/ejecución manual del administrador, no prueba de que la cuenta exista. La evidencia física anterior de CONTRATOS no certifica enrutamiento del sitio ni concede permiso de pruebas sobre esa plantilla.

## E2E preparado
Reutiliza tools/e2e, createWorkflowLoginPageSession, safePost y configuración Playwright existentes. Sin trazas, vídeo, screenshots ni reintentos. Manifest local excluido de Git: issue DOC-122, disposable=true, baseUrl, conexionId, plantillaId, revisionEsperada, huellaEsperada y allowedModes [read] o [read,write].

Configuración efímera: DOC122_E2E_AUTHORIZED=YES tras autorización explícita; DOC122_E2E_SCOPE_PATH, DOC122_E2E_BASE_URL, DOC122_E2E_MODULE, DOC122_E2E_USER/PASSWORD y DOC122_E2E_RESTRICTED_USER/PASSWORD. No versionar ni imprimir valores. Antes de autenticar, leer AGENTS.md y el runbook E2E. La autorización debe identificar ambiente, cuentas y plantilla existente descartable; el permiso debe estar ya instalado/configurado manualmente.

@doc122-read abre sesión autorizada y restringida, verifica listado/detalle y rechazo CSRF; huella/revisión sin cambios. @doc122-write realiza tres solicitudes GuardarPlantilla sobre la plantilla autorizada: un roundtrip sin cambiar valores y dos peticiones con la misma revisión; resultado esperado dos guardados exitosos y un conflicto. Revisión final inicial+2, ruta/mapeos iguales. Usa la misma sesión ASP.NET, por lo que acredita conflicto optimista de solicitudes; no prueba paralelismo físico de dos escritores MySQL. No crea, limpia, restaura ni siembra datos por SQL. Una ventana real debe complementar con controles externos solo SELECT de catálogos/destino/auditoría y dejar evidencia saneada.

## Estado de validación
Compilación aislada y suite nativa sintética en Validacion/doc122-validacion-local-20261121.md. Descubrimiento E2E no equivale a ejecución. La tarea de integración real continúa abierta y el cambio no está listo para cierre.
