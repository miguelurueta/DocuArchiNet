# DOC-122 — Autorización, conexión y plantillas

Implementación de fase02 de7, perfil legacy-webforms-vb, .NET Framework4.6.1/MySQL5.1.37. Contrato previo: [DOC121](01-contratos-configuracion-doc121.md). No carga archivos ni ejecuta adecuación de esquema. EGRESOS permanece intacto.

## Conexión del módulo existente
Ajuste solicitado por el usuario: usar los recursos de conexión del repositorio. Sustituye la implementación inicial que exigía otra cuenta y connectionStrings cifrado para Excel.

El recorrido es gestor_modulos → GestorModuleSesion.AsignaAtributosConexionModuloDocuarchi → sesión DA_ → ModuleSessionConnectionStringResolver.Resolve(http,"DA_"). RA sigue resolviéndose por RA_ y comprobando permiso/usuario/empresa en cada operación. No se modificó el resolver ni el cargador de sesión compartidos.

ImportacionExcelConexionAdapter exige DA_ID_EMPRESA igual al contexto RA y DA_ID_MODULO positivo. Usa la cuenta, destino y pool del módulo ya cargados en servidor. Consulta imp01_xmlconexion solo para obtener ID/nombre y comprobar servidor/base/proveedor; nunca selecciona sus campos de usuario/contraseña. El navegador recibe referencias/nombres, sin credenciales o destinos elegibles libremente.

Referencias elegibles: filas de imp01_xmlconexion que coinciden exactamente con la DA del módulo; hasta100. La comparación de base es ordinal para impedir equivalencias de collation. No hay mapa paralelo de conexiones por empresa. La fábrica del repositorio vuelve a comparar empresa, módulo y conexión antes de cada apertura; un cambio invalida el contexto capturado.

Esta opción usa la cuenta del módulo y no garantiza una separación física de privilegios para Excel. No rechaza esa cuenta por su nombre ni exige que sea diferente de sí misma. No eleva ni revoca privilegios del módulo; se siguen comprobando sus facultades efectivas antes del recorrido real.

No se requiere ImportacionExcel.Destinos, una connectionStrings nueva ni cifrar appSettings/connectionStrings para habilitar Excel. No se cambió Web.config, gestor_modulos, credenciales o privilegios durante este ajuste. Los límites siguen siendo ajustes de servidor opcionales:

| Clave appSettings | Regla |
|---|---|
| ImportacionExcel.MaxFilasDatos | Entero positivo, default10000 si falta. |
| ImportacionExcel.MaxArchivoMB | Entero positivo, default10 si falta; MB decimal1000000 bytes. Rechaza desbordamiento. |

El permiso ADMINISTRAR_CARGAS_IMPORTACION=1 sigue siendo independiente del de perfiles. Usuario activo y empresa se consultan en RA en cada operación; el indicador de sesión solo controla presentación.

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

La E2E real del2026-11-22 acreditó el despliegue, el enrutamiento DA y las facultades de lectura/guardado de plantillas de la cuenta del módulo. No acredita los privilegios de DDL de fases posteriores ni mínimo privilegio. SQL05 se conserva como antecedente de la cuenta separada inicialmente prevista; no es requisito de instalación de este ajuste ni debe aplicarse para modificar la cuenta compartida automáticamente. La evidencia física anterior de CONTRATOS no certifica enrutamiento del sitio ni concede permiso de pruebas sobre esa plantilla.

## E2E ejecutado y reutilizable
Ejecutar desde el runner existente de perfiles:

- Lectura: npm.cmd --prefix tools/e2e run test:doc110:session -- --excel-read
- Guardado/concurrencia: npm.cmd --prefix tools/e2e run test:doc110:session -- --excel-write

Ambos modos reutilizan profiles/doc88-production-document-upload.local.runtime.json para URL, módulo y HTTPS, igual que la suite de perfiles/carpetas. Reutilizan DOC110_E2E_USER/PASSWORD si están cargados en memoria; lectura admite DOC110_E2E_SECOND_USER/SECOND_PASSWORD como cuenta restringida. Las variables DOC122 explícitas tienen prioridad para cuentas. No hay que volver a suministrar URL o módulo. Si faltan cuentas, la consola existente las recoge de forma enmascarada y efímera.

Se conserva el manifiesto específico DOC122_E2E_SCOPE_PATH para identificar la plantilla Excel descartable y su revisión/huella; el perfil de ambiente y el ID de perfil de importación de carpetas no sustituyen ese vínculo. El runner rechaza modos mixtos, diferencias de ambiente y escritura fuera del alcance. Una autorización antigua DOC110 no habilita la prueba DOC122; admite la autorización específica ya presente o recoge la confirmación en la consola existente. La adaptación pasó6 verificaciones locales y la E2E real posterior pasó2/2; evidencia en Validacion/doc122-e2e-real-20261122.md/json.

Reutiliza tools/e2e, createWorkflowLoginPageSession, safePost y configuración Playwright existentes. Sin trazas, vídeo, screenshots ni reintentos. Manifest local excluido de Git: issue DOC-122, disposable=true, baseUrl, conexionId, plantillaId, revisionEsperada, huellaEsperada y allowedModes [read] o [read,write].

Configuración efímera: DOC122_E2E_AUTHORIZED=YES tras autorización explícita; DOC122_E2E_SCOPE_PATH, DOC122_E2E_BASE_URL, DOC122_E2E_MODULE, DOC122_E2E_USER/PASSWORD y DOC122_E2E_RESTRICTED_USER/PASSWORD. No versionar ni imprimir valores. Antes de autenticar, leer AGENTS.md y el runbook E2E. La autorización debe identificar ambiente, cuentas y plantilla existente descartable; el permiso debe estar ya instalado/configurado manualmente. El usuario confirmó disponer de un usuario con permiso; sus credenciales se suministran únicamente por el mecanismo efímero existente.

@doc122-read abre sesión autorizada y restringida, verifica listado/detalle y rechazo CSRF; huella/revisión sin cambios. @doc122-write realiza tres solicitudes GuardarPlantilla sobre la plantilla autorizada: un roundtrip sin cambiar valores y dos peticiones con la misma revisión; resultado esperado dos guardados exitosos y un conflicto. Revisión final inicial+2, ruta/mapeos iguales. Usa la misma sesión ASP.NET, por lo que acredita conflicto optimista de solicitudes; no prueba paralelismo físico de dos escritores MySQL. No crea, limpia, restaura ni siembra datos por SQL. La ventana real ejecutada creó mediante el servicio un fixture inactivo en modo CREAR y dos mapeos, sin crear tabla. Añadió23 controles exclusivamente SELECT y verificó huellas completas de EGRESOS, CONTRATOS y demás plantillas/campos. El recorrido de guardado admite también CREAR. No certifica auditoría ajena a esos controles.

## Estado de validación
Compilación aislada y suite nativa sintética vigentes en Validacion/doc122-conexion-reutilizada-20261121.md/json, con246 controles. El informe doc122-validacion-local-20261121.md/json conserva el corte histórico. La aceptación real está en Validacion/doc122-e2e-real-20261122.md/json:2/2 tests,14 llamadas de servicio,23 controles SELECT y preservación de datos previos. DOC12214/14 tareas completas. La revisión y archivo del flujo se realizan por separado.

## Cierre técnico2026-11-22
DOC122 quedó archivado, integrado mediante PR134 y confirmado Listo en Jira. CI37/37, OpenSpec93/93, gobierno72/72 y E2E real2/2. Registro vigente en ../Validacion/doc122-cierre-20261122.md/json. El merge conserva el árbol exacto revisado; el registro administrativo posterior no cambia código ni especificaciones.
